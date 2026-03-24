using Microsoft.AspNetCore.Mvc;
using Bai_Cuoi_Ky.Models;
using Bai_Cuoi_Ky.Data; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims; 

namespace Bai_Cuoi_Ky.Controllers
{
    [Authorize] 
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH ĐƠN HÀNG 
        public async Task<IActionResult> Index()
        {
            var query = _context.Orders.AsQueryable();

            if (!User.IsInRole("Admin") && !User.IsInRole("NhanVien"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                query = query.Where(o => o.UserId == userId);
            }

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // 2. CHI TIẾT ĐƠN HÀNG 
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var query = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .AsQueryable();

            if (!User.IsInRole("Admin") && !User.IsInRole("NhanVien"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                query = query.Where(o => o.UserId == userId);
            }

            var order = await query.FirstOrDefaultAsync(m => m.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        // 3. CẬP NHẬT TRẠNG THÁI
        [Authorize(Roles = "Admin, NhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            // Danh sách các trạng thái hợp lệ 
            var validStatuses = new[] { "Pending", "ChoXuLy", "HoanThanh", "DaHuy" };

            if (validStatuses.Contains(status))
            {
                order.Status = status;
                _context.Update(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Cập nhật trạng thái đơn hàng #{id} thành công!";
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        // 4. DUYỆT ĐƠN NHANH 
        [Authorize(Roles = "Admin, NhanVien")]
        public async Task<IActionResult> Approve(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order != null && (order.Status == "ChoXuLy" || order.Status == "Pending"))
            {
                order.Status = "HoanThanh";
                _context.Update(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã duyệt đơn hàng #{id}!";
            }

            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrder(int id, string customerName, string phoneNumber, string email, string address, string notes)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            bool isAdminOrStaff = User.IsInRole("Admin") || User.IsInRole("NhanVien");
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!isAdminOrStaff && order.UserId != currentUserId)
                return Unauthorized();

            if (!isAdminOrStaff && order.Status != "ChoXuLy" && order.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Đơn hàng đã được xử lý, bạn không thể tự thay đổi thông tin!";
                return RedirectToAction(nameof(Details), new { id = id });
            }

            order.CustomerName = customerName;
            order.PhoneNumber = phoneNumber;
            order.Email = email;
            order.Address = address;
            order.Notes = notes;

            _context.Update(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã cập nhật thông tin giao hàng thành công!";
            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (order.UserId != currentUserId) return Unauthorized();

            if (order.Status != "ChoXuLy" && order.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Đơn hàng đã được xử lý, không thể hủy!";
                return RedirectToAction(nameof(Details), new { id = id });
            }

            order.Status = "DaHuy";
            _context.Update(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Bạn đã hủy đơn hàng thành công!";
            return RedirectToAction(nameof(Details), new { id = id });
        }

        // 3. XÓA ĐƠN HÀNG
        [HttpPost]
        [Authorize(Roles = "Admin, NhanVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails) 
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã xóa thành công đơn hàng #{id}!";
            return RedirectToAction(nameof(Index)); 
        }
    }
}