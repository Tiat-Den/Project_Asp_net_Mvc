using Microsoft.AspNetCore.Mvc;
using Bai_Cuoi_Ky.Models;
using Bai_Cuoi_Ky.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace Bai_Cuoi_Ky.Controllers
{
    [Authorize(Roles = "Admin, NhanVien")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            if (status == "ChoXuLy" || status == "HoanThanh" || status == "DaHuy")
            {
                order.Status = status;
                _context.Update(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Cập nhật trạng thái đơn hàng #{id} thành công!";
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        public async Task<IActionResult> Approve(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null && order.Status == "ChoXuLy")
            {
                order.Status = "HoanThanh";
                _context.Update(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã duyệt đơn hàng #{id}!";
            }
            return RedirectToAction("Index", "Admin");
        }
    }
}
