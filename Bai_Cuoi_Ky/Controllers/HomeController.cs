using Bai_Cuoi_Ky.Data;
using Bai_Cuoi_Ky.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using System.Net;
using System.Net.Mail;

namespace Bai_Cuoi_Ky.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public class ChatRequest
        {
            public string Text { get; set; }
        }

        public HomeController(ApplicationDbContext context, IConfiguration configuration) 
        { 
            _context = context; 
            _configuration = configuration; 
        }

        public async Task<IActionResult> Index()
        {
            // Lấy toàn bộ danh sách sản phẩm kèm theo thông tin Danh mục từ SQL
            var allProducts = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            // Truyền danh sách sang View
            return View(allProducts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public async Task<IActionResult> GetAiResponse([FromBody] ChatRequest request)
        {
            // 1. Lấy API Key từ file appsettings.json
            string apiKey = _configuration["GeminiApiKey"];

            string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={apiKey}";

            var products = await _context.Products.Where(p => p.IsAvailable).Take(20).ToListAsync(); 
            string productData = "DANH SÁCH SẢN PHẨM ĐANG BÁN:\n"; 

            foreach (var item in products) 
            {
                productData += $"- Tên: {item.Name} \n Giá: {item.Price:N0}đ \n Link: <a href='/Product/Details/{item.Id}' target='_blank'>Xem chi tiết</a> \n Hình ảnh: {item.ImageUrl}\n-------------------\n";
            }

            string prompt = $@"Bạn là một chuyên viên tư vấn cao cấp tại cửa hàng trang sức DKH Premium Jewelry. 
                Nhiệm vụ của bạn là tư vấn, hỗ trợ và thuyết phục khách hàng mua sắm một cách lịch sự, tinh tế và nhiệt tình.

                THÔNG TIN HỖ TRỢ KHÁCH HÀNG (CƠ SỞ DỮ LIỆU ĐỂ TRẢ LỜI):
                    - Chính sách đổi trả: Hỗ trợ đổi trả trong 07 ngày kể từ ngày nhận nếu sản phẩm chưa qua sử dụng, còn nguyên hộp và tem mác. Không áp dụng cho hàng thiết kế riêng hoặc đã chỉnh sửa size.
                    - Chính sách bảo hành: Miễn phí làm sạch, đánh bóng trọn đời. Bảo hành rơi rớt đá CZ/Moissanite trong 06 tháng đầu. Không bảo hành đứt gãy do va đập, tiếp xúc hóa chất.
                    - Hướng dẫn đo size: Có 2 cách. Cách 1: Đo đường kính bên trong của chiếc nhẫn cũ. Cách 2: Dùng sợi chỉ/mảnh giấy quấn quanh ngón tay để đo chu vi, sau đó lấy chu vi chia cho 3.14 để ra đường kính.
                    - Thông tin liên hệ: Showroom đặt tại BR-VT, TP. Hồ Chí Minh. Hotline: 0123 456 789. Mở cửa từ 08:00 - 22:00 tất cả các ngày trong tuần.

                DANH SÁCH CÁC TRANG WEB (ĐỂ ĐIỀU HƯỚNG KHÁCH HÀNG):
                    - Trang chủ: /
                    - Tất cả sản phẩm: /Product
                    - Sản phẩm yêu thích: Product/Favorite
                    - Sản phẩm mới nhất: /Product/NewArrivals
                    - Sản phẩm khuyến mãi: /Product/Sale
                    - Giỏ hàng: /Cart/Index
                    - Thanh toán:/Checkout/Index
                    - Đăng nhập: /Account/Login
                    - Đăng ký: /Account/Register
                    - Thông tin tài khoản: /Account/Profile
                    - Lịch sử đơn hàng: /Order
                    - Các danh mục: 
                        + Vòng - lắc: /Product?categoryId=1
                        + Nhẫn: /Product?categoryId=2
                        + Dây chuyền: /Product?categoryId=3
                        + Bông tai: /Product?categoryId=4
                        + Trang sức bộ: /Product?categoryId=5
                        + Phụ kiện: /Product?categoryId=6  
                    - Các trang hỗ trợ: 
                        + Chính sách đổi trả: /Home/ReturnPolicy
                        + Chính sách đổi trả: /Home/SizeGuide
                        + Chính sách đổi trả: /Home/Warranty
                        + Chính sách đổi trả: /Home/Contact


                DƯỚI ĐÂY LÀ DANH SÁCH SẢN PHẨM HIỆN CÓ TẠI CỬA HÀNG:
                {productData}

                QUY TẮC TRẢ LỜI (BẮT BUỘC TUÂN THỦ NGHIÊM NGẶT):
                1. TRUNG THỰC TỐI ĐA: TUYỆT ĐỐI KHÔNG tự bịa ra bất kỳ sản phẩm, giá tiền, hình ảnh sản phẩm, hay chương trình khuyến mãi nào không có trong dữ liệu được cung cấp.
                2. XỬ LÝ KHI HẾT HÀNG: Nếu khách hỏi một món đồ không có trong danh sách, hãy xin lỗi khéo léo và chủ động gợi ý 1-2 sản phẩm tương tự đang có sẵn.
                3. GỬI HÌNH ẢNH SẢN PHẨM: Khi gợi ý một sản phẩm, BẮT BUỘC phải hiển thị hình ảnh của nó bằng thẻ HTML <img>.
                   - Định dạng chuẩn: <br><img src='[Link hình ảnh]' style='width: 150px; border-radius: 8px; margin-top: 5px; box-shadow: 0 2px 5px rgba(0,0,0,0.1);' /><br>
                   - TUYỆT ĐỐI KHÔNG in ra URL thô bằng chữ.
                4. ĐỊNH DẠNG LINK MUA HÀNG: Khi nhắc đến tên một sản phẩm, BẮT BUỘC phải bọc nó trong thẻ HTML <a> kèm link tương ứng lấy từ dữ liệu. 
                   - Định dạng chuẩn: <a href='[Link sản phẩm]' style='color: #c9a84c; font-weight: bold; text-decoration: underline;'>[Tên sản phẩm]</a>
                   - Tuyệt đối không in ra URL thô của link mua hàng (ví dụ: không in http://... của trang sản phẩm).
                5. NGẮN GỌN & DỄ ĐỌC: Trả lời súc tích, tối đa 150 chữ (không tính phần URL hình ảnh sản phẩm). Sử dụng thẻ <br> để xuống dòng hoặc <ul><li> để tạo danh sách cho khách dễ đọc trên khung chat nhỏ.
                6. GIỮ ĐÚNG VAI TRÒ: Nếu khách hàng hỏi những thứ không liên quan đến trang sức, mua sắm hoặc cửa hàng (ví dụ: thời tiết, lập trình, toán học...), hãy từ chối lịch sự và khéo léo điều hướng khách quay lại chủ đề trang sức.
                7. THÁI ĐỘ: Luôn xưng hô lịch sự (dạ/vâng, gọi khách là anh/chị hoặc bạn), thể hiện sự sang trọng của một thương hiệu Premium.
                8. ĐIỀU HƯỚNG TRANG WEB: Nếu khách hàng yêu cầu chuyển đến một trang cụ thể (ví dụ: 'đưa tôi đến giỏ hàng', 'muốn đăng nhập', 'về trang chủ'), BẮT BUỘC cung cấp link đến trang đó dưới dạng một nút bấm HTML.
                   - Định dạng chuẩn: <br><a href='[Link trang tương ứng lấy ở trên]' style='display: inline-block; padding: 8px 15px; background-color: #1a1410; border: 1px solid #c9a84c; color: #c9a84c; text-decoration: none; border-radius: 5px; margin-top: 10px; font-weight: bold; text-align: center;'>Đi đến [Tên trang] ➔</a><br>
                   - TUYỆT ĐỐI không bịa ra link không có trong danh sách.
                9. CUNG CẤP THÔNG TIN HỖ TRỢ: Khi khách hỏi về bảo hành, đổi trả, đo size hoặc địa chỉ, hãy trả lời ngắn gọn dựa trên phần 'THÔNG TIN HỖ TRỢ KHÁCH HÀNG' ở trên. TẠO NÚT BẤM điều hướng họ đến trang chi tiết khi họ yêu cầu.
                Tin nhắn của khách hàng: '{request.Text}'

                Hãy đưa ra câu trả lời của bạn ngay bên dưới (chỉ trả lời nội dung, không giải thích gì thêm):";

            try
            {
                using (var client = new HttpClient())
                {
                    var payload = new
                    {
                        contents = new[]
                        {
                        new { parts = new[] { new { text = prompt } } }
                    }
                    };

                    string jsonPayload = JsonSerializer.Serialize(payload);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();

                        using (JsonDocument document = JsonDocument.Parse(responseString))
                        {
                            var root = document.RootElement;
                            var replyText = root
                                .GetProperty("candidates")[0]
                                .GetProperty("content")
                                .GetProperty("parts")[0]
                                .GetProperty("text").GetString();

                            return Json(new { reply = replyText });
                        }
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        return Json(new { reply = "Lỗi từ Google API: " + error });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi Exception: " + ex.Message);
            }

            return Json(new { reply = "Xin lỗi, hệ thống AI đang nâng cấp. Quý khách vui lòng để lại số điện thoại hoặc gọi hotline để DKH Jewelry hỗ trợ nhé!" });
        }

        public IActionResult ReturnPolicy() => View();
        public IActionResult SizeGuide() => View();
        public IActionResult Warranty() => View();
        public IActionResult Contact() => View();

        [HttpPost]
        public async Task<IActionResult> SubmitContact(string customerName, string customerEmail, string messageContent)
        {
            try
            {
                // 1. Soạn nội dung HTML
                string htmlBody = $@"
            <div style='font-family: Arial, sans-serif; color: #333;'>
                <h2 style='color: #c9a84c;'>Xin chào {customerName},</h2>
                <p>Cảm ơn bạn đã liên hệ với <strong>DKH Premium Jewelry</strong>.</p>
                <p>Hệ thống đã ghi nhận yêu cầu của bạn với nội dung:</p>
                <blockquote style='border-left: 4px solid #c9a84c; padding-left: 10px; font-style: italic; background-color: #f9f9f9; padding: 10px;'>
                    {messageContent}
                </blockquote>
                <p>Đội ngũ Chăm sóc khách hàng của chúng tôi sẽ xử lý và phản hồi bạn trong thời gian sớm nhất.</p>
                <p>Trân trọng,<br><strong>DKH Premium Jewelry Team</strong></p>
            </div>";

                // 2. Gọi Service ra và gửi 
                var emailSender = new Bai_Cuoi_Ky.Services.EmailSender();
                await emailSender.SendEmailAsync(customerEmail, "Xác nhận yêu cầu hỗ trợ từ DKH Premium Jewelry", htmlBody);

                TempData["SuccessMsg"] = "Gửi yêu cầu thành công! Vui lòng kiểm tra hộp thư Email của bạn.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = "Gửi thất bại, lỗi hệ thống: " + ex.Message;
            }

            return RedirectToAction("Contact");
        }

    }
}