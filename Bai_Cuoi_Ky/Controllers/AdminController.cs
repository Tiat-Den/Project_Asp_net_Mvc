using Bai_Cuoi_Ky.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Bai_Cuoi_Ky.Models;

namespace Bai_Cuoi_Ky.Controllers
{
    [Authorize(Roles = "Admin, NhanVien")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // 1. Thống kê tổng
            decimal totalRevenue = await _context.Orders
                .Where(o => o.Status == "HoanThanh" || o.Status == "DaGiao")
                .SumAsync(o => o.TotalAmount);

            int totalOrder = await _context.Orders.CountAsync();
            int totalProducts = await _context.Products.CountAsync();
            var khachHangList = await _userManager.GetUsersInRoleAsync("KhachHang");
            int totalCustomers = khachHangList.Count;

            // ==========================================
            // 2. CHIA LÀM 3 BẢNG RÕ RÀNG
            // ==========================================

            // Mảng 1: Bọn Chưa Xử Lý (Gồm ChuaXuLy và Pending)
            var chuaXuLyStatuses = new[] { "ChuaXuLy", "Pending" };
            var bangChuaXuLy = await _context.Orders
                .Where(o => chuaXuLyStatuses.Contains(o.Status))
                .OrderByDescending(o => o.OrderDate).ToListAsync();

            // Mảng 2: Bọn Chờ Xử Lý (Chỉ có ChoXuLy)
            var choXuLyStatuses = new[] { "ChoXuLy" };
            var bangChoXuLy = await _context.Orders
                .Where(o => choXuLyStatuses.Contains(o.Status))
                .OrderByDescending(o => o.OrderDate).ToListAsync();

            // Mảng 3: Bọn Đã Xử Lý (Các trạng thái còn lại)
            var bangDaXuLy = await _context.Orders
                .Where(o => !chuaXuLyStatuses.Contains(o.Status) && !choXuLyStatuses.Contains(o.Status))
                .OrderByDescending(o => o.OrderDate).Take(10).ToListAsync();

            // Đẩy sang View
            ViewBag.TotalRevenue = totalRevenue.ToString("N0");
            ViewBag.TotalOrder = totalOrder;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalCustomers = totalCustomers;

            ViewBag.BangChuaXuLy = bangChuaXuLy;
            ViewBag.BangChoXuLy = bangChoXuLy;
            ViewBag.BangDaXuLy = bangDaXuLy;

            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> PromoteToNhanVien(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("Không tìm thấy người dùng");

            if (await _userManager.IsInRoleAsync(user, "KhachHang"))
            {
                await _userManager.RemoveFromRoleAsync(user, "KhachHang");
            }

            await _userManager.AddToRoleAsync(user, "NhanVien");

            return Ok("Đã thăng chức thành Nhân Viên thành công!");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DemoteToKhachHang(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("Không tìm thấy người dùng");

            if (await _userManager.IsInRoleAsync(user, "NhanVien"))
            {
                await _userManager.RemoveFromRoleAsync(user, "NhanVien");
                await _userManager.AddToRoleAsync(user, "KhachHang");
            }

            return Ok("Đã giáng chức xuống Khách Hàng!");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult RoleManager()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Users()
        {
            return View();
        }
    }
}