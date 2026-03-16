using Bai_Cuoi_Ky.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bai_Cuoi_Ky.Controllers
{
    public class ProductController : Controller
    {
        // Dữ liệu mẫu (thay bằng DbContext nếu dùng database)
        private static List<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "Vòng Tay Mặt Trăng", Price = 350000, Category = "Vòng - Lắc", Material = "Bạc 925", Stock = 10, IsAvailable = true, ImageUrl = "/images/vong1.jpg", CreatedAt = DateTime.Now },
            new Product { Id = 2, Name = "Nhẫn Đôi Tình Yêu", Price = 450000, Category = "Nhẫn", Material = "Bạc 925", Stock = 5, IsAvailable = true, ImageUrl = "/images/nhan1.jpg", CreatedAt = DateTime.Now },
            new Product { Id = 3, Name = "Dây Chuyền Mặt Nguyệt", Price = 520000, Category = "Dây Chuyền", Material = "Bạc 925", Stock = 8, IsAvailable = true, ImageUrl = "/images/daychuyen1.jpg", CreatedAt = DateTime.Now },
            new Product { Id = 4, Name = "Bông Tai Pha Lê", Price = 280000, Category = "Bông Tai", Material = "Bạc 925", Stock = 15, IsAvailable = true, ImageUrl = "/images/bongtai1.jpg", CreatedAt = DateTime.Now },
            new Product { Id = 5, Name = "Charm Pandora Hoa", Price = 620000, Category = "Charm Pandora", Material = "Bạc 925", Stock = 3, IsAvailable = true, ImageUrl = "/images/charm1.jpg", CreatedAt = DateTime.Now },
        };

        // ===== INDEX - Danh sách sản phẩm =====
        // GET: /Product
        public IActionResult Index(string category = null, string search = null)
        {
            var products = _products.AsQueryable();

            if (!string.IsNullOrEmpty(category))
                products = products.Where(p => p.Category == category);

            if (!string.IsNullOrEmpty(search))
                products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));

            ViewBag.Categories = _products.Select(p => p.Category).Distinct().ToList();
            ViewBag.CurrentCategory = category;
            ViewBag.Search = search;

            return View(products.ToList());
        }

        // ===== DETAILS - Chi tiết sản phẩm =====
        // GET: /Product/Details/5
        public IActionResult Details(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // ===== CREATE - Tạo sản phẩm mới =====
        // GET: /Product/Create
        public IActionResult Create()
        {
            ViewBag.Categories = new List<string>
            {
                "Vòng - Lắc", "Nhẫn", "Dây Chuyền",
                "Bông Tai", "Charm Pandora", "Khuyên Xỏ"
            };
            return View();
        }

        // POST: /Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new List<string>
                {
                    "Vòng - Lắc", "Nhẫn", "Dây Chuyền",
                    "Bông Tai", "Charm Pandora", "Khuyên Xỏ"
                };
                return View(product);
            }

            product.Id = _products.Any() ? _products.Max(p => p.Id) + 1 : 1;
            product.CreatedAt = DateTime.Now;
            product.IsAvailable = product.Stock > 0;
            _products.Add(product);

            TempData["Success"] = "Thêm sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ===== EDIT - Chỉnh sửa sản phẩm =====
        // GET: /Product/Edit/5
        public IActionResult Edit(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            ViewBag.Categories = new List<string>
            {
                "Vòng - Lắc", "Nhẫn", "Dây Chuyền",
                "Bông Tai", "Charm Pandora", "Khuyên Xỏ"
            };
            return View(product);
        }

        // POST: /Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new List<string>
                {
                    "Vòng - Lắc", "Nhẫn", "Dây Chuyền",
                    "Bông Tai", "Charm Pandora", "Khuyên Xỏ"
                };
                return View(product);
            }

            var existing = _products.FirstOrDefault(p => p.Id == id);
            if (existing == null)
                return NotFound();

            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.DiscountPrice = product.DiscountPrice;
            existing.ImageUrl = product.ImageUrl;
            existing.Category = product.Category;
            existing.Material = product.Material;
            existing.Stock = product.Stock;
            existing.IsAvailable = product.Stock > 0;

            TempData["Success"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ===== DELETE - Xóa sản phẩm =====
        // GET: /Product/Delete/5
        public IActionResult Delete(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: /Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            _products.Remove(product);

            TempData["Success"] = "Xóa sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}