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
            decimal totalRevenue = await _context.Orders
                .Where(o => o.Status == "HoanThanh")
                .SumAsync(o => o.TotalAmount);

            int totalOrder = await _context.Orders.CountAsync();

            int totalProducts = await _context.Products.CountAsync();

            var khachHangList = await _userManager.GetUsersInRoleAsync("KhachHang");
            int totalCustomers = khachHangList.Count;

            var chuaXuLyOrders = await _context.Orders
                .Where(o => o.Status == "ChoXuLy")
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            ViewBag.TotalRevenue = totalRevenue.ToString("N0");
            ViewBag.TotalOrder = totalOrder;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalCustomers = totalCustomers;
            ViewBag.ChuaXuLyOrdersCount = chuaXuLyOrders.Count;
            ViewBag.ChuaXuLyOrders = chuaXuLyOrders;

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