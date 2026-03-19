using Bai_Cuoi_Ky.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bai_Cuoi_Ky.Controllers
{
    public class ProductController : Controller
    {
        private static List<Product> _products = new List<Product>
        {
            // ===== VÒNG - LẮC =====
            new Product { Id = 1, Name = "Lắc tay bạc nữ Tennis chuỗi đá CZ dạng dây rút thắt nơ LILI_869163", Price = 2578000,
                Category = "Vong-Lac", Material = "Bạc", Stock = 10, IsAvailable = true,
                ImageUrl = "/images/vong1.jpg" },

            new Product { Id = 2, Name = "Lắc chân bạc nữ đính đá CZ hình cỏ 4 lá Mildred LILI_763298", Price = 2299000,
                Category = "Vong-Lac", Material = "Bạc", Stock = 8, IsAvailable = true,
                ImageUrl = "/images/Favorite/Lac-chan-bac-nu-dinh-da-CZ-hinh-co-4-la-Mildred1.jpg",
                ImageUrl2 = "/images/Favorite/Lac-chan-bac-nu-dinh-da-CZ-hinh-co-4-la-Mildred-LILI2.jpg" },

            new Product { Id = 3, Name = "Lắc tay bạc nữ đính đá CZ hình trái tim Heart Of The Sea LILI_427425", Price = 3328000,
                Category = "Vong-Lac", Material = "Bạc", Stock = 5, IsAvailable = true,
                ImageUrl = "/images/Favorite/Lac-tay-bac-nu-dinh-da-pha-le-hinh-trai-tim-LILI1.jpg",
                ImageUrl2 = "/images/Favorite/Lac-tay-bac-nu-dinh-da-pha-le-hinh-trai-tim-LILI2.jpg" },

            new Product { Id = 4, Name = "Lắc tay bạc nam dây bện dù sáp đính đá CZ hình rồng Dragon LILI_103676", Price = 2730000,
                Category = "Vong-Lac", Material = "Bạc", Stock = 6, IsAvailable = true,
                ImageUrl = "/images/Favorite/Lac-tay-bac-nam-day-ben-du-sap-dinh-da-CZ-hinh-rong-Dragon-LILI1.jpg",
                ImageUrl2 = "/images/Favorite/Lac-tay-bac-nam-day-ben-du-sap-dinh-da-CZ-hinh-rong-Dragon-LILI2.jpg" },

            new Product { Id = 5, Name = "Lắc tay bạc nữ cá tính mắt xích vuông trái tim Strong Heart LILI_414788", Price = 2685000,
                Category = "Vong-Lac", Material = "Bạc", Stock = 7, IsAvailable = true,
                ImageUrl = "/images/Discount/Lac-tay-bac-nu-ca-tinh-mat-xich-vuong-trai-tim-Strong-Heart-LILI1.jpg" },

            new Product { Id = 6, Name = "Vòng pandora bạc nữ DIY dạng chuỗi xoắn Sabrina LILI_090215", Price = 6039000,
                Category = "Vong-Lac", Material = "Bạc", Stock = 4, IsAvailable = true,
                ImageUrl = "/images/Discount/Vong-pandora-bac-nu-dang-chuoi-xoan-Sabrina-LILI1.jpg",
                ImageUrl2 = "/images/Discount/Vong-pandora-bac-nu-dang-chuoi-xoan-Sabrina-LILI2.jpg" },

            // ===== NHẪN =====
            new Product { Id = 7, Name = "Nhẫn bạc nữ đính kim cương Moissanite Aidan LILI_335168", Price = 2181000,
                Category = "Nhan", Material = "Bạc", Stock = 5, IsAvailable = true,
                ImageUrl = "/images/Favorite/Nhan-bac-nu-dinh-kim-cuong-Moissanite-Aidan1.jpg",
                ImageUrl2 = "/images/Favorite/Nhan-bac-nu-dinh-kim-cuong-Moissanite-Aidan2.jpg" },

            new Product { Id = 8, Name = "Nhẫn bạc nữ đính đá CZ hoa bướm LILI_661591", Price = 1520000,
                Category = "Nhan", Material = "Bạc", Stock = 8, IsAvailable = true,
                ImageUrl = "/images/NewProducts/Nhan-bac-nu-dinh-da-CZ-hoa-buom-LILI1.jpg",
                ImageUrl2 = "/images/NewProducts/Nhan-bac-nu-dinh-da-CZ-hoa-buom-LILI2.jpg" },

            // ===== DÂY CHUYỀN =====
            new Product { Id = 9, Name = "Dây chuyền bạc nữ đính đá CZ cá tiên LILI_831944", Price = 1824000,
                Category = "Day-Chuyen", Material = "Bạc", Stock = 6, IsAvailable = true,
                ImageUrl = "/images/Favorite/Day-chuyen-bac-nu-phong-cach-co-trang1.jpg",
                ImageUrl2 = "/images/Favorite/Day-chuyen-bac-nu-phong-cach-co-trang2.jpg" },

            new Product { Id = 10, Name = "Dây chuyền Choker bạc nữ Magic LILI_366642", Price = 1485000,
                Category = "Day-Chuyen", Material = "Bạc", Stock = 9, IsAvailable = true,
                ImageUrl = "/images/Discount/Day-chuyen-Choker-bac-Magic-LILI1.jpg",
                ImageUrl2 = "/images/Discount/Day-chuyen-Choker-bac-Magic-LILI2.jpg" },

            new Product { Id = 11, Name = "Dây chuyền bạc nữ đẹp đính pha lê Aurora trái tim hoa lá LILI_866671", Price = 1412000,
                Category = "Day-Chuyen", Material = "Bạc", Stock = 7, IsAvailable = true,
                ImageUrl = "/images/Discount/Day-chuyen-bac-nu-trai-tim-hoa-la-dinh-pha-le-Aurora-LILI1.jpg",
                ImageUrl2 = "/images/Discount/Day-chuyen-bac-nu-trai-tim-hoa-la-dinh-pha-le-Aurora-LILI2.jpg" },

            // ===== BÔNG TAI =====
            new Product { Id = 12, Name = "Bông tai bạc nữ tròn đính đá CZ hình bông hoa 5 cánh Cute LILI_749976", Price = 1484000,
                Category = "Bong-Tai", Material = "Bạc", Stock = 15, IsAvailable = true,
                ImageUrl = "/images/Favorite/Bong-tai-bac-nu-tron-hinh-bong-hoa-5-canh-Cute-LILI1.jpg",
                ImageUrl2 = "/images/Favorite/Bong-tai-bac-nu-tron-hinh-bong-hoa-5-canh-Cute-LILI2.jpg" },

            new Product { Id = 13, Name = "Bông tai bạc nữ đính đá CZ hình những bông hoa Lưu ly LILI_148289", Price = 1541000,
                Category = "Bong-Tai", Material = "Bạc", Stock = 10, IsAvailable = true,
                ImageUrl = "/images/Discount/Bong-tai-bac-nu-dinh-da-CZ-hinh-nhung-bong-hoa-Luu-ly-LILI1.jpg",
                ImageUrl2 = "/images/Discount/Bong-tai-bac-nu-dinh-da-CZ-hinh-nhung-bong-hoa-Luu-ly-LILI2.jpg" },

            new Product { Id = 14, Name = "Bông tai bạc nữ đính đá CZ hình trái tim Sofia LILI_182061", Price = 1538000,
                Category = "Bong-Tai", Material = "Bạc", Stock = 8, IsAvailable = true,
                ImageUrl = "/images/NewProducts/Bong-tai-bac-nu-dinh-da-CZ-hinh-trai-tim-Sofia-LILI1.jpg",
                ImageUrl2 = "/images/NewProducts/Bong-tai-bac-nu-dinh-da-CZ-hinh-trai-tim-Sofia-LILI2.jpg" },

            // ===== TRANG SỨC ĐÔI =====
            new Product { Id = 15, Name = "Dây chuyền đôi bạc tình yêu tình bạn thân BFF đính đá CZ Forever Love LILI_528145", Price = 3467000,
                Category = "Trang-Suc-Doi", Material = "Bạc", Stock = 5, IsAvailable = true,
                ImageUrl = "/images/Favorite/Day-chuyen-doi-bac-hinh-ca-heo-hong-Forever-Love-LILI1.jpg",
                ImageUrl2 = "/images/Favorite/Day-chuyen-doi-bac-dinh-da-CZ-Forever-Love-LILI2.jpg" },

            new Product { Id = 16, Name = "Nhẫn cặp đôi bạc đính kim cương Moissanite Theophilus LILI_672438", Price = 2914000,
                Category = "Trang-Suc-Doi", Material = "Bạc", Stock = 4, IsAvailable = true,
                ImageUrl = "/images/Favorite/Nhan-cap-doi-bac-dinh-kim-cuong-Moissanite-Theophilus-LILI1.jpg",
                ImageUrl2 = "/images/Favorite/Nhan-cap-doi-bac-dinh-kim-cuong-Moissanite-Theophilus-LILI2.jpg" },

            new Product { Id = 17, Name = "Nhẫn đôi bạc free size đính đá CZ hiệp sĩ và công chúa LILI_819229", Price = 2905000,
                Category = "Trang-Suc-Doi", Material = "Bạc", Stock = 6, IsAvailable = true,
                ImageUrl = "/images/NewProducts/Nhan-doi-bac-hiep-si-va-cong-chua-dinh-da-CZ-LILI1.jpg",
                ImageUrl2 = "/images/NewProducts/Nhan-doi-bac-hiep-si-va-cong-chua-dinh-da-CZ-LILI2.jpg" },

            new Product { Id = 18, Name = "Lắc tay bạc cặp đôi tình yêu tình bạn vĩnh cửu Forever LILI_818622", Price = 3669000,
                Category = "Trang-Suc-Doi", Material = "Bạc", Stock = 5, IsAvailable = true,
                ImageUrl = "/images/NewProducts/Lac-tay-bac-cap-doi-tinh-yeu-tinh-ban-vinh-cuu-Forever-LILI1.jpg",
                ImageUrl2 = "/images/NewProducts/Lac-tay-bac-cap-doi-tinh-yeu-tinh-ban-vinh-cuu-Forever-LILI2.jpg" },

            // ===== TRANG SỨC BỘ =====
            new Product { Id = 19, Name = "Dây chuyền bạc đôi đính đá CZ Juniper LILI_181462", Price = 4111000,
                Category = "Trang-Suc-Bo", Material = "Bạc", Stock = 3, IsAvailable = true,
                ImageUrl = "/images/NewProducts/Day-chuyen-bac-doi-dinh-da-CZ-Juniper-LILI1.jpg",
                ImageUrl2 = "/images/NewProducts/Day-chuyen-bac-doi-dinh-da-CZ-Juniper-LILI2.jpg" },

            // ===== PHỤ KIỆN =====
            new Product { Id = 20, Name = "Tủ hộp đựng đồ trang sức nam/nữ trang bọc da nhung cao cấp LILI_611947", Price = 4111000,
                Category = "Phu-Kien", Material = "Da", Stock = 10, IsAvailable = true,
                ImageUrl = "/images/Discount/Hop-dung-do-trang-suc-nam-nu-trang-dang-ngan-boc-da-cao-cap-LILI1.jpg",
                ImageUrl2 = "/images/Discount/Hop-dung-do-trang-suc-nam-nu-trang-dang-ngan-boc-da-cao-cap-LILI2.jpg" },
        };

        // ===== INDEX =====
        public IActionResult Index(string category = null, string search = null)
        {
            var products = _products.AsQueryable();

            if (!string.IsNullOrEmpty(category))
                products = products.Where(p => p.Category == category);

            if (!string.IsNullOrEmpty(search))
                products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));

            ViewBag.CurrentCategory = category;
            ViewBag.Search = search;

            return View(products.ToList());
        }

        // ===== DETAILS =====
        public IActionResult Details(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }
    }
}