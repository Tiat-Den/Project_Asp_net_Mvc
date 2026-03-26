using Microsoft.AspNetCore.Mvc;
using Bai_Cuoi_Ky.Data;
using Bai_Cuoi_Ky.Models;
using System.Text.Json;

namespace Bai_Cuoi_Ky.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = GetCartItems();
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var product = _context.Products.Find(id);
            if (product == null) return Json(new { success = false });

            var cart = GetCartItems();
            var item = cart.FirstOrDefault(c => c.ProductId == id);

            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    ImageUrl = product.ImageUrl
                });
            }

            SaveCartSession(cart);

            // PHẢI TRẢ VỀ JSON THẾ NÀY THÌ AJAX MỚI HỨNG ĐƯỢC
            return Json(new
            {
                success = true,
                totalItems = cart.Sum(x => x.Quantity)
            });
        }

        public IActionResult Remove(int id)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(c => c.ProductId == id);

            if (item != null)
            {
                cart.Remove(item);
                SaveCartSession(cart);
            }

            return RedirectToAction("Index");
        }

        private string GetCartSessionKey()
        {
            return User.Identity.IsAuthenticated ? $"Cart_{User.Identity.Name}" : "Cart";
        }

        private List<CartItem> GetCartItems()
        {
            var sessionData = HttpContext.Session.GetString(GetCartSessionKey());
            if (string.IsNullOrEmpty(sessionData)) return new List<CartItem>();
            return JsonSerializer.Deserialize<List<CartItem>>(sessionData);
        }

        private void SaveCartSession(List<CartItem> cart)
        {
            var sessionData = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(GetCartSessionKey(), sessionData);
        }

        public IActionResult UpdateQuantity(int id, int amount)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(c => c.ProductId == id);

            if (item != null)
            {
                item.Quantity += amount;
                if (item.Quantity <= 0) cart.Remove(item);
            }

            SaveCartSession(cart);
            return RedirectToAction("Index");
        }
    }
}