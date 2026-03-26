using Bai_Cuoi_Ky.Data;
using Bai_Cuoi_Ky.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;
using System.Text;

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
                productData += $"- Tên: {item.Name} \n Giá: {item.Price:N0}đ \n Link: <a href='/Product/Details/{item.Id}' target='_blank'>Xem chi tiết</a>\n"; 
            }

            string prompt = $@"Bạn là một chuyên viên tư vấn cao cấp tại cửa hàng trang sức DKH Premium Jewelry. 
                        Nhiệm vụ của bạn là tư vấn, hỗ trợ và thuyết phục khách hàng mua sắm một cách lịch sự, tinh tế và nhiệt tình.

                        DƯỚI ĐÂY LÀ DANH SÁCH SẢN PHẨM HIỆN CÓ TẠI CỬA HÀNG:
                        {productData}

                        QUY TẮC TRẢ LỜI (BẮT BUỘC TUÂN THỦ NGHIÊM NGẶT):
                        1. TRUNG THỰC TỐI ĐA: TUYỆT ĐỐI KHÔNG tự bịa ra bất kỳ sản phẩm, giá tiền, hay chương trình khuyến mãi nào không có trong dữ liệu được cung cấp.
                        2. XỬ LÝ KHI HẾT HÀNG: Nếu khách hỏi một món đồ không có trong danh sách, hãy xin lỗi khéo léo và chủ động gợi ý 1-2 sản phẩm tương tự đang có sẵn.
                        3. ĐỊNH DẠNG LINK MUA HÀNG: Khi nhắc đến tên một sản phẩm, BẮT BUỘC phải bọc nó trong thẻ HTML <a> kèm link tương ứng lấy từ dữ liệu. 
                           - Định dạng chuẩn: <a href='[Link sản phẩm]' style='color: #c9a84c; font-weight: bold; text-decoration: underline;'>[Tên sản phẩm]</a>
                           - Tuyệt đối không in ra URL thô (ví dụ: không in http://...).
                        4. NGẮN GỌN & DỄ ĐỌC: Trả lời súc tích, tối đa 150 chữ. Sử dụng thẻ <br> để xuống dòng hoặc <ul><li> để tạo danh sách cho khách dễ đọc trên khung chat nhỏ.
                        5. GIỮ ĐÚNG VAI TRÒ: Nếu khách hàng hỏi những thứ không liên quan đến trang sức, mua sắm hoặc cửa hàng (ví dụ: thời tiết, lập trình, toán học...), hãy từ chối lịch sự và khéo léo điều hướng khách quay lại chủ đề trang sức.
                        6. THÁI ĐỘ: Luôn xưng hô lịch sự (dạ/vâng, gọi khách là anh/chị hoặc bạn), thể hiện sự sang trọng của một thương hiệu Premium.

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

    }
}