using Bai_Cuoi_Ky.Data; // Gọi không gian tên chứa ApplicationDbContext
using Bai_Cuoi_Ky.Models; // Gọi không gian tên chứa các class Models
using Microsoft.AspNetCore.Mvc; // Gọi thư viện hỗ trợ MVC (Model-View-Controller)
using Microsoft.AspNetCore.Mvc.Rendering; // Thư viện hỗ trợ tạo SelectList cho Dropdown list
using Microsoft.EntityFrameworkCore; // Gọi thư viện Entity Framework Core để thao tác SQL

namespace Bai_Cuoi_Ky.Controllers // Khai báo không gian tên cho Controllers
{
    public class ProductController : Controller // Khai báo lớp ProductController kế thừa từ Controller
    {
        private readonly ApplicationDbContext _context; // Khai báo biến DbContext để tương tác với CSDL

        // Constructor: Inject ApplicationDbContext từ DI container
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===== INDEX: Hiển thị danh sách sản phẩm =====
        public async Task<IActionResult> Index(int? categoryId, string search)
        {
            // Lấy tất cả sản phẩm kèm theo thông tin Category từ CSDL
            var products = _context.Products.Include(p => p.Category).AsQueryable();

            // Lọc theo danh mục nếu có
            if (categoryId.HasValue)
                products = products.Where(p => p.CategoryId == categoryId.Value);

            // Tìm kiếm theo tên nếu có
            if (!string.IsNullOrEmpty(search))
                products = products.Where(p => p.Name.Contains(search));

            ViewBag.CurrentCategory = categoryId;
            ViewBag.Search = search;

            return View(await products.ToListAsync());
        }

        // ===== DETAILS: Xem chi tiết 1 sản phẩm =====
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // Tìm sản phẩm theo Id, kèm theo thông tin Category
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // ===== CREATE (GET): Hiển thị form thêm sản phẩm mới =====
        public IActionResult Create()
        {
            // Tạo SelectList danh mục để hiển thị dropdown
            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // ===== CREATE (POST): Xử lý thêm sản phẩm mới vào CSDL =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? ImageFile1, IFormFile? ImageFile2)
        {
            // Loại bỏ Category khỏi validation vì nó là navigation property
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                // Nếu có file upload từ máy → chuyển thành base64 lưu vào SQL
                if (ImageFile1 != null && ImageFile1.Length > 0)
                    product.ImageUrl = await ConvertToBase64DataUri(ImageFile1);

                if (ImageFile2 != null && ImageFile2.Length > 0)
                    product.ImageUrl2 = await ConvertToBase64DataUri(ImageFile2);

                // Nếu không upload file thì giữ nguyên URL đã nhập (cũng lưu vào SQL)
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // ===== CHUYỂN FILE UPLOAD THÀNH BASE64 DATA URI ĐỂ LƯU VÀO SQL =====
        private async Task<string> ConvertToBase64DataUri(IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var base64 = Convert.ToBase64String(ms.ToArray());
            var contentType = file.ContentType ?? "image/jpeg";
            return $"data:{contentType};base64,{base64}";
        }

        // ===== EDIT (GET): Hiển thị form chỉnh sửa sản phẩm =====
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id); // Tìm sản phẩm theo Id
            if (product == null) return NotFound();

            // Tạo SelectList danh mục, chọn sẵn danh mục hiện tại
            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // ===== EDIT (POST): Xử lý cập nhật sản phẩm trong CSDL =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id) return NotFound(); // Kiểm tra Id khớp

            // Loại bỏ Category khỏi validation vì nó là navigation property
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product); // Đánh dấu sản phẩm đã thay đổi
                    await _context.SaveChangesAsync(); // Lưu thay đổi vào CSDL
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Kiểm tra sản phẩm còn tồn tại không (đề phòng xung đột)
                    if (!_context.Products.Any(e => e.Id == product.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index)); // Quay về danh sách
            }

            // Nếu validation thất bại, hiển thị lại form
            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // ===== DELETE (GET): Hiển thị trang xác nhận xóa =====
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            // Tìm sản phẩm kèm Category để hiển thị thông tin
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // ===== DELETE (POST): Xóa sản phẩm khỏi CSDL =====
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id); // Tìm sản phẩm
            if (product != null)
            {
                _context.Products.Remove(product); // Xóa khỏi DbSet
                await _context.SaveChangesAsync(); // Lưu thay đổi vào CSDL
            }
            return RedirectToAction(nameof(Index)); // Quay về danh sách
        }
    }
}