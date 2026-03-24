using Microsoft.AspNetCore.Mvc;
using Bai_Cuoi_Ky.Data;
using Bai_Cuoi_Ky.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Bai_Cuoi_Ky.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CheckoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = GetCartItems();
            if (cart.Count == 0) return RedirectToAction("Index", "Cart");
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessCheckout(string customerName, string phoneNumber, string address, string email, string paymentMethod, string notes)
        {
            // 1. Lấy giỏ hàng
            var cart = GetCartItems();
            if (cart == null || !cart.Any()) return RedirectToAction("Index", "Cart");

            // 2. Lấy ID người dùng
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 3. Khởi tạo Order
            var order = new Order
            {
                UserId = userId,
                CustomerName = customerName,
                PhoneNumber = phoneNumber,
                Address = address,
                Email = email,           
                PaymentMethod = paymentMethod,
                Notes = notes,
                TotalAmount = cart.Sum(x => x.Total),
                OrderDate = DateTime.Now,
                Status = "Pending",
                OrderDetails = cart.Select(x => new OrderDetail
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    UnitPrice = x.Price
                }).ToList()
            };

            // 4. Lưu vào Database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); 

            // 5. Xóa giỏ hàng
            HttpContext.Session.Remove("Cart");

            return View("Success");
        }

        private List<CartItem> GetCartItems()
        {
            var sessionData = HttpContext.Session.GetString("Cart");
            return string.IsNullOrEmpty(sessionData) ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(sessionData);
        }
    }
}