using Bai_Cuoi_Ky.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bai_Cuoi_Ky.Controllers
{
    public class ProductController : Controller
    {
        
        private static List<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "Lắc tay bạc nữ Tennis chuỗi đá CZ dạng dây rút thắt nơ LILI_869163",
                Price = 2578000, Category = "Vòng - Lắc", Material = "Bạc", Stock = 10, IsAvailable = true, 
                ImageUrl = "/images/Products/Lac-tay-bac-nu-chuoi-da-CZ-dang-day-rut-that-no1.jpg",
                ImageUrl2 = "/images/Products/Lac-tay-bac-nu-chuoi-da-CZ-dang-day-rut-that-no2.jpg",},

            new Product { Id = 2, Name = "Nhẫn bạc nữ đính kim cương Moissanite Aidan LILI_335168", Price = 2181000, Category = "Nhẫn", Material = "Bạc", Stock = 5, IsAvailable = true, 
                ImageUrl = "/images/Products/Nhan-bac-nu-dinh-kim-cuong-Moissanite-Aidan1.jpg",
                ImageUrl2="/images/Products/Nhan-bac-nu-dinh-kim-cuong-Moissanite-Aidan2.jpg"},

            new Product { Id = 3, Name = "Dây chuyền đôi bạc đính đá CZ hình cá voi và bướm Brenna LILI_123985", Price = 520000, Category = "Dây Chuyền", Material = "Bạc", Stock = 8, IsAvailable = true, 
                ImageUrl = "/images/Products/Day-chuyen-doi-bac-dinh-da-CZ-hinh-ca-voi-va-buom1.jpg",
                ImageUrl2="/images/Products/Day-chuyen-doi-bac-dinh-da-CZ-hinh-ca-voi-va-buom2.jpg"},

            new Product { Id = 4, Name = "Bông tai bạc nữ đính đá CZ hình chiếc nơ sang trọng LILI_698154", Price = 1918000, Category = "Bông Tai", Material = "Bạc", Stock = 15, IsAvailable = true, 
                ImageUrl = "/images/Products/Bong-tai-bac-nu-hinh-that-no-dinh-da1.jpg",
                ImageUrl2="/images/Products/Bong-tai-bac-nu-hinh-that-no-dinh-da2.jpg"},

             new Product { Id = 5, Name = "Lắc tay bạc cặp đôi tình yêu Forever Love LILI_986852", Price = 3458000, Category = "Trang Sức Đôi", Material = "Bạc", Stock = 15, IsAvailable = true,
                ImageUrl = "/images/Products/Lac-tay-bac-cap-doi-tinh-yeu-Forever-Love1.jpg",
                ImageUrl2="/images/Products/Lac-tay-bac-cap-doi-tinh-yeu-Forever-Love2.jpg"},

             new Product { Id = 6, Name = "Bộ trang sức bạc nữ đính đá Garnet, CZ hoa hồng tình yêu LILI_727966", Price = 1408000, Category = "Trang Sức Bộ", Material = "Bạc", Stock = 15, IsAvailable = true,
                ImageUrl = "/images/Products/Bo-trang-suc-bac-nu-dinh-da-Garnet-CZ-hoa-hong-tinh-yeu1.jpg",
                ImageUrl2="/images/Products/Bo-trang-suc-bac-nu-dinh-da-Garnet-CZ-hoa-hong-tinh-yeu2.jpg"},

              new Product { Id = 7, Name = "Tủ hộp đựng đồ trang sức phụ kiện có khóa gỗ óc chó Holly LILI_161276", Price = 1809000, Category = "Phụ Kiện", Stock = 15, IsAvailable = true,
                ImageUrl = "/images/Products/Tu-hop-dung-do-trang-suc-phu-kien-dep-go-oc-cho1.jpg",
                ImageUrl2="/images/Products/Tu-hop-dung-do-trang-suc-phu-kien-dep-go-oc-cho2.jpg"},

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
                "Bông Tai","Trang Sức Đôi","Trang Sức Bộ","Phụ Kiện"
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
                "Bông Tai","Trang Sức Đôi","Trang Sức Bộ","Phụ Kiện"
                };
                return View(product);
            }

            product.Id = _products.Any() ? _products.Max(p => p.Id) + 1 : 1;
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
                "Bông Tai", "Trang Sức Đôi" , "Trang Sức Bộ" , "Phụ Kiện"
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
                    "Bông Tai", "Trang Sức Đôi" , "Trang Sức Bộ" , "Phụ Kiện"
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