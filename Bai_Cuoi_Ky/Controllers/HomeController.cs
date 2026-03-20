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
                productData += $"- Tên: {item.Name} | Giá: {item.Price:N0}đ | Link: <a href='/Product/Details/{item.Id}' target='_blank'>Xem chi tiết</a>\n"; 
            }

            string prompt = $"Bạn là nhân viên tư vấn của DKH Premium Jewelry. " + 
                    $"Dưới đây là dữ liệu các món trang sức cửa hàng đang có:\n{productData}\n" + 
                    $"Khách hàng vừa nhắn: '{request.Text}'. " + 
                    $"Hãy trả lời thân thiện. Nếu gợi ý sản phẩm, BẮT BUỘC phải dùng thẻ HTML <a> trong phần Link ở danh sách trên. TUYỆT ĐỐI KHÔNG tự bịa ra sản phẩm không có trong danh sách.";
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