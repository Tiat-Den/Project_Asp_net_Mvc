using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bai_Cuoi_Ky.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller 
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

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
    }
}