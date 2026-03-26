using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Bai_Cuoi_Ky.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleManagerController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public RoleManagerController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName.Trim());
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
                    TempData["SuccessMessage"] = $"Đã tạo phân quyền {roleName} thành công.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Quyền {roleName} đã tồn tại.";
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(string id, string roleName)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null && !string.IsNullOrEmpty(roleName))
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName.Trim());
                if (!roleExist || role.Name == roleName.Trim())
                {
                    role.Name = roleName.Trim();
                    await _roleManager.UpdateAsync(role);
                    TempData["SuccessMessage"] = "Cập nhật quyền thành công.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Tên quyền đã tồn tại.";
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                if (usersInRole.Count > 0)
                {
                    TempData["ErrorMessage"] = $"Không thể xóa quyền '{role.Name}' vì có {usersInRole.Count} người dùng đang có quyền này.";
                }
                else
                {
                    await _roleManager.DeleteAsync(role);
                    TempData["SuccessMessage"] = "Xóa quyền thành công.";
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRolesViewModel>();
            
            foreach (IdentityUser user in users)
            {
                var viewModel = new UserRolesViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = await _userManager.GetRolesAsync(user)
                };
                userRolesViewModel.Add(viewModel);
            }
            
            ViewBag.AllRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            
            return View(userRolesViewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && !string.IsNullOrEmpty(roleName))
            {
                if (!await _userManager.IsInRoleAsync(user, roleName))
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                    TempData["SuccessMessage"] = $"Đã thêm quyền {roleName} cho tài khoản {user.Email}";
                }
            }
            return RedirectToAction("Users");
        }
        
        [HttpPost]
        public async Task<IActionResult> RemoveRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && !string.IsNullOrEmpty(roleName))
            {
                if (await _userManager.IsInRoleAsync(user, roleName))
                {
                    await _userManager.RemoveFromRoleAsync(user, roleName);
                    TempData["SuccessMessage"] = $"Đã gỡ quyền {roleName} khỏi tài khoản {user.Email}";
                }
            }
            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> ResetUserPassword(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Xóa mật khẩu cũ
            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (removePasswordResult.Succeeded)
            {
                // Gắn mật khẩu mới
                var addPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);
                if (addPasswordResult.Succeeded)
                {
                    TempData["SuccessMessage"] = $"Đã đặt lại mật khẩu cho tài khoản {user.Email} thành công!";
                    return RedirectToAction(nameof(Index)); 
                }
            }

            TempData["ErrorMessage"] = "Có lỗi xảy ra khi đặt lại mật khẩu.";
            return RedirectToAction(nameof(Index));
        }
    }
    
    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
