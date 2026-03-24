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

            try
            {
                var emailSender = new Bai_Cuoi_Ky.Services.EmailSender();
                string subject = $"Xác nhận đặt hàng thành công - Đơn hàng #{order.Id}";

                string productRows = ""; 
                foreach (var item in cart) 
                { 
                    string productImgSrc = "";

                    if (!string.IsNullOrEmpty(item.ImageUrl) && item.ImageUrl.StartsWith("http"))
                    {
                        productImgSrc = item.ImageUrl; 
                    }
                    else if (!string.IsNullOrEmpty(item.ImageUrl))
                    {
                        try
                        {
                            string relativePath = item.ImageUrl.TrimStart('~', '/');

                            relativePath = relativePath.Replace("/", "\\");

                            string imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

                            if (System.IO.File.Exists(imgPath))
                            {
                                byte[] imgBytes = System.IO.File.ReadAllBytes(imgPath); 
                                string base64 = Convert.ToBase64String(imgBytes); 

                                string extension = Path.GetExtension(imgPath).Replace(".", "");
                                productImgSrc = $"data:image/{extension};base64,{base64}"; 
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Lỗi khi convert ảnh SP: " + ex.Message); 
                        }
                    }

                    productRows += $@"
    <tr>
        <td style='border: 1px solid #ddd; padding: 10px; text-align: center;'>
            <img src='{productImgSrc}' alt='{item.ProductName}' style='width: 60px; height: 60px; object-fit: cover; border-radius: 4px; background-color: #f8f9fa;' />
        </td>
        <td style='border: 1px solid #ddd; padding: 10px;'>
            <strong>{item.ProductName}</strong>
        </td>
        <td style='border: 1px solid #ddd; padding: 10px; text-align: center;'>{item.Quantity}</td>
        <td style='border: 1px solid #ddd; padding: 10px; text-align: right;'>{item.Price.ToString("N0")}đ</td>
        <td style='border: 1px solid #ddd; padding: 10px; text-align: right; font-weight: bold;'>{item.Total.ToString("N0")}đ</td>
    </tr>";
                }

                // 2. TẠO KHU VỰC HIỂN THỊ MÃ QR (Chỉ tạo khi khách chọn QR)
                string qrCodeSection = ""; 
                if (order.PaymentMethod == "QR") 
                { 
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "QR-Code", "QR_PTD.jpg");
                    byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath); 
                    string base64String = Convert.ToBase64String(imageBytes); 
                    string qrUrl = $"data:image/jpeg;base64,{base64String}"; 

                    qrCodeSection = $@" 
    <div style='margin-top: 25px; text-align: center; border: 2px dashed #007bff; padding: 20px; border-radius: 8px; background-color: #f8fbff;'> 
        <h3 style='color: #007bff; margin-top: 0;'>MÃ QR THANH TOÁN CHUYỂN KHOẢN</h3> 
        <p style='color: #555; font-size: 15px;'>Vui lòng quét mã dưới đây bằng App Ngân hàng để thanh toán cho đơn hàng <strong>#{order.Id}</strong></p> 
        
        <img src='{qrUrl}' alt='QR Code' style='width: 250px; border: 5px solid white; box-shadow: 0 4px 8px rgba(0,0,0,0.1); border-radius: 10px;' /> 
        
        <div style='margin-top: 15px; font-size: 16px; line-height: 1.5;'> =
            <p style='margin: 0;'><strong>Chủ tài khoản:</strong> PHAM TIEN DAT</p> 
            <p style='margin: 0;'><strong>Số tài khoản:</strong> 393905052005 - MB Bank</p> 
            <p style='margin: 0;'><strong>Số tiền:</strong> <span style='color: red; font-weight: bold;'>{order.TotalAmount.ToString("N0")} VNĐ</span></p> 
            <p style='margin: 0; color: #d9534f;'><strong>Nội dung CK:</strong> Thanh toan don hang {order.Id}</p> 
        </div> 
    </div>"; 
                }

                // 3. GỘP TẤT CẢ VÀO BODY (Trình bày dạng bảng HTML chuẩn cho Email)
                string body = $@"
<div style='font-family: Arial, sans-serif; max-width: 650px; margin: auto; color: #333; line-height: 1.6;'>
    <div style='text-align: center; padding-bottom: 20px; border-bottom: 2px solid #eee;'>
        <h2 style='color: #28a745; margin-bottom: 5px;'>ĐẶT HÀNG THÀNH CÔNG!</h2>
        <p style='margin: 0; color: #777;'>Cảm ơn <strong>{order.CustomerName}</strong> đã tin tưởng và mua sắm.</p>
    </div>

    <div style='padding: 20px 0;'>
        <h4 style='color: #333; border-left: 4px solid #007bff; padding-left: 10px;'>1. Thông tin giao hàng (Đơn #{order.Id})</h4>
        <table style='width: 100%; border-collapse: collapse; font-size: 15px;'>
            <tr>
                <td style='padding: 8px 0; width: 160px;'><strong>Người nhận:</strong></td>
                <td style='padding: 8px 0;'>{order.CustomerName}</td>
            </tr>
            <tr>
                <td style='padding: 8px 0;'><strong>Số điện thoại:</strong></td>
                <td style='padding: 8px 0;'>{order.PhoneNumber}</td>
            </tr>
            <tr>
                <td style='padding: 8px 0;'><strong>Địa chỉ giao hàng:</strong></td>
                <td style='padding: 8px 0;'>{order.Address}</td>
            </tr>
            <tr>
                <td style='padding: 8px 0;'><strong>Hình thức thanh toán:</strong></td>
                <td style='padding: 8px 0;'><span style='background: #e9ecef; padding: 4px 8px; border-radius: 4px; font-weight:bold;'>{(order.PaymentMethod == "QR" ? "Chuyển khoản (QR Code)" : "Thanh toán khi nhận hàng (COD)")}</span></td>
            </tr>
        </table>
    </div>

    <div>
        <h4 style='color: #333; border-left: 4px solid #007bff; padding-left: 10px;'>2. Chi tiết đơn hàng</h4>
        <table style='width: 100%; border-collapse: collapse; font-size: 15px;'>
            <thead>
                <tr style='background-color: #f4f4f4;'>
                    <th style='border: 1px solid #ddd; padding: 10px; text-align: center;'>Hình ảnh</th>
                    <th style='border: 1px solid #ddd; padding: 10px; text-align: left;'>Sản phẩm</th>
                    <th style='border: 1px solid #ddd; padding: 10px; text-align: center;'>SL</th>
                    <th style='border: 1px solid #ddd; padding: 10px; text-align: right;'>Đơn giá</th>
                    <th style='border: 1px solid #ddd; padding: 10px; text-align: right;'>Thành tiền</th>
                </tr>
            </thead>
            <tbody>
                {productRows} </tbody>
            <tfoot>
                <tr style='background-color: #fffaf0;'>
                    <td colspan='4' style='border: 1px solid #ddd; padding: 12px; text-align: right; font-weight: bold; font-size: 16px;'>Tổng cộng:</td>
                    <td style='border: 1px solid #ddd; padding: 12px; text-align: right; font-weight: bold; color: #dc3545; font-size: 18px;'>{order.TotalAmount.ToString("N0")} đ</td>
                </tr>
            </tfoot>
        </table>
    </div>

    {qrCodeSection} <div style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; text-align: center; color: #666; font-size: 14px;'>
        <p style='margin-bottom: 5px;'>Chúng tôi sẽ sớm liên hệ lại với bạn để xác nhận đơn hàng.</p>
        <p style='margin: 0;'>Mọi thắc mắc xin vui lòng liên hệ Hotline: <strong>0123 456 789</strong></p>
    </div>
</div>";

                await emailSender.SendEmailAsync(order.Email, subject, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi gửi email: " + ex.Message);
            }

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