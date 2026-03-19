using Bai_Cuoi_Ky.Data; // Gọi không gian tên chứa ApplicationDbContext
using Bai_Cuoi_Ky.Models; // Gọi không gian tên chứa các class Models
using Microsoft.AspNetCore.Mvc; // Gọi thư viện hỗ trợ MVC (Model-View-Controller)
using Microsoft.AspNetCore.Mvc.Rendering; // ĐÃ THÊM: Thư viện hỗ trợ tạo SelectList cho Dropdown list
using Microsoft.EntityFrameworkCore; // Gọi thư viện Entity Framework Core để thao tác SQL

namespace Bai_Cuoi_Ky.Controllers // Khai báo không gian tên cho Controllers
{
    public class ProductController : Controller // Khai báo lớp ProductController kế thừa từ Controller
    {
        private readonly ApplicationDbContext _context; // Khai báo biến _context chỉ đọc để làm việc với CSDL

        public ProductController(ApplicationDbContext context) // Hàm khởi tạo nhận DbContext từ hệ thống (Dependency Injection)
        {
            _context = context; // Gán context vào biến _context để sử dụng trong các hàm bên dưới
        }

        // ===== QUẢN LÝ SẢN PHẨM =====

        public async Task<IActionResult> Index(int? categoryId = null, string search = null) // SỬA: Đổi category chuỗi thành categoryId (Khóa ngoại)
        {
            var products = _context.Products.Include(p => p.Category).AsQueryable(); // SỬA: Dùng Include để kết nối (JOIN) lấy thông tin từ bảng Category

            if (categoryId.HasValue) // Kiểm tra nếu người dùng có chọn danh mục để lọc
            {
                products = products.Where(p => p.CategoryId == categoryId.Value); // Lọc sản phẩm theo mã danh mục (CategoryId)
            }

            if (!string.IsNullOrEmpty(search)) // Kiểm tra nếu người dùng có nhập từ khóa tìm kiếm
            {
                products = products.Where(p => p.Name.Contains(search)); // Lọc sản phẩm chứa từ khóa trong tên
            }

            ViewBag.Categories = await _context.Categories.ToListAsync(); // SỬA: Lấy toàn bộ danh mục từ SQL Server để làm nút lọc
            ViewBag.CurrentCategory = categoryId; // Lưu lại ID danh mục đang chọn để hiển thị trên View
            ViewBag.Search = search; // Lưu lại từ khóa tìm kiếm để hiển thị trên View

            return View(await products.ToListAsync()); // Trả về View cùng danh sách sản phẩm đã được lọc
        }

        public async Task<IActionResult> Details(int id) // Hàm xem chi tiết sản phẩm
        {
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id); // SỬA: Include Category để hiển thị tên danh mục thật thay vì chỉ hiện ID

            if (product == null) // Nếu không tìm thấy sản phẩm
            {
                return NotFound(); // Trả về lỗi 404
            }

            return View(product); // Trả về View chi tiết sản phẩm
        }

        public IActionResult Create() // Hàm hiển thị form thêm mới sản phẩm (GET)
        {
            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name"); // SỬA: Lấy danh mục từ SQL tạo thành Dropdown (chọn Id, hiển thị Name)
            return View(); // Trả về giao diện form Create
        }

        [HttpPost] // Đánh dấu hàm này chỉ nhận phương thức POST (khi người dùng bấm Submit)
        [ValidateAntiForgeryToken] // Bảo vệ form khỏi tấn công CSRF
        public async Task<IActionResult> Create(Product product) // Hàm xử lý lưu sản phẩm mới
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu người dùng nhập có đúng chuẩn Model không
            {
                product.IsAvailable = product.Stock > 0; // Tự động set trạng thái còn hàng dựa vào tồn kho

                _context.Add(product); // Thêm đối tượng sản phẩm mới vào DbContext
                await _context.SaveChangesAsync(); // Lưu thay đổi xuống SQL Server

                TempData["Success"] = "Thêm sản phẩm thành công!"; // Hiển thị thông báo thành công
                return RedirectToAction(nameof(Index)); // Chuyển hướng về trang danh sách (Index)
            }

            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId); // SỬA: Nếu nhập lỗi, load lại Dropdown danh mục
            return View(product); // Trả lại form cùng dữ liệu người dùng đã nhập để họ sửa lỗi
        }

        public async Task<IActionResult> Edit(int id) // Hàm hiển thị form sửa sản phẩm (GET)
        {
            var product = await _context.Products.FindAsync(id); // Tìm sản phẩm theo Id trong cơ sở dữ liệu

            if (product == null) // Nếu không tìm thấy
            {
                return NotFound(); // Trả về lỗi 404
            }

            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId); // SỬA: Load Dropdown danh mục và chọn sẵn danh mục hiện tại của sản phẩm
            return View(product); // Trả về giao diện form Edit
        }

        [HttpPost] // Đánh dấu hàm xử lý POST
        [ValidateAntiForgeryToken] // Bảo mật form
        public async Task<IActionResult> Edit(int id, Product product) // Hàm xử lý cập nhật sản phẩm
        {
            if (id != product.Id) // Kiểm tra Id trên URL và Id trong form có khớp không
            {
                return BadRequest(); // Trả về lỗi 400 nếu sai lệch (chống hack)
            }

            if (ModelState.IsValid) // Nếu dữ liệu hợp lệ
            {
                try // Bắt đầu khối Try-Catch để bắt lỗi
                {
                    product.IsAvailable = product.Stock > 0; // Cập nhật lại trạng thái còn hàng

                    _context.Update(product); // Đánh dấu đối tượng này đã bị thay đổi
                    await _context.SaveChangesAsync(); // Cập nhật xuống SQL Server
                }
                catch (DbUpdateConcurrencyException) // Bắt lỗi xung đột dữ liệu
                {
                    if (!ProductExists(product.Id)) // Kiểm tra xem sản phẩm còn tồn tại không
                    {
                        return NotFound(); // Báo 404 nếu không tồn tại
                    }
                    else // Nếu lỗi khác
                    {
                        throw; // Ném lỗi ra ngoài
                    }
                }

                TempData["Success"] = "Cập nhật sản phẩm thành công!"; // Báo thành công
                return RedirectToAction(nameof(Index)); // Quay về trang Index
            }

            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId); // SỬA: Nếu lỗi, nạp lại Dropdown Category
            return View(product); // Trả lại giao diện kèm thông báo lỗi
        }

        public async Task<IActionResult> Delete(int id) // Hàm hiển thị trang xác nhận xóa sản phẩm (GET)
        {
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id); // Tìm sản phẩm và kèm theo tên danh mục

            if (product == null) // Nếu không có sản phẩm
            {
                return NotFound(); // Báo 404
            }

            return View(product); // Trả về trang xác nhận xóa
        }

        [HttpPost, ActionName("Delete")] // Đánh dấu hàm POST nhưng nhận tên Action là "Delete"
        [ValidateAntiForgeryToken] // Bảo vệ form
        public async Task<IActionResult> DeleteConfirmed(int id) // Hàm thực thi xóa sản phẩm khỏi SQL
        {
            var product = await _context.Products.FindAsync(id); // Tìm sản phẩm trong DB

            if (product != null) // Nếu tìm thấy
            {
                _context.Products.Remove(product); // Xóa khỏi DbContext
                await _context.SaveChangesAsync(); // Xóa trong SQL Server
                TempData["Success"] = "Xóa sản phẩm thành công!"; // Báo thành công
            }

            return RedirectToAction(nameof(Index)); // Quay về trang danh sách
        }

        private bool ProductExists(int id) // Hàm phụ trợ kiểm tra sản phẩm
        {
            return _context.Products.Any(e => e.Id == id); // Trả về true nếu sản phẩm tồn tại
        }

        // ===== HÀM TẠO DANH MỤC MỚI VÀO SQL =====

        public IActionResult CreateCategory() // Hàm hiển thị giao diện thêm danh mục (GET)
        {
            return View(); // Trả về View (bạn cần tạo file CreateCategory.cshtml trong thư mục Views/Product)
        }

        [HttpPost] // Hàm nhận dữ liệu khi người dùng Submit thêm danh mục
        [ValidateAntiForgeryToken] // Bảo mật form
        public async Task<IActionResult> CreateCategory(Category category) // Nhận đối tượng Category
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu danh mục có hợp lệ không
            {
                _context.Categories.Add(category); // Thêm danh mục mới vào Context
                await _context.SaveChangesAsync(); // Lưu dữ liệu vào bảng Categories trong SQL Server

                TempData["Success"] = "Thêm danh mục mới thành công!"; // Báo thành công
                return RedirectToAction(nameof(Create)); // Quay lại trang Thêm Sản Phẩm để bạn có thể chọn ngay danh mục vừa tạo
            }

            return View(category); // Nếu lỗi, trả lại trang để nhập lại
        }
    }
}