using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Bai_Cuoi_Ky.Controllers 
{
    public class AccountController : Controller 
    {
        private readonly UserManager<IdentityUser> _userManager; 
        private readonly SignInManager<IdentityUser> _signInManager; 

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) 
        { 
            _userManager = userManager; 
            _signInManager = signInManager; 
        }

        // === XỬ LÝ FORM ĐĂNG KÝ ===
        [HttpPost]
        public async Task<IActionResult> Register(string Email, string Username, string Password, string ConfirmPassword) 
        { 
            if (Password != ConfirmPassword) return BadRequest("Mật khẩu không khớp!"); 

            var user = new IdentityUser { UserName = Username, Email = Email }; 
            var result = await _userManager.CreateAsync(user, Password); 

            if (result.Succeeded) 
            { 
                await _signInManager.SignInAsync(user, isPersistent: false); 
                return RedirectToAction("Index", "Home"); 
            } 

            return BadRequest("Lỗi hệ thống: Tên tài khoản hoặc Email đã tồn tại, hoặc mật khẩu quá yếu!"); 
        }

        // === XỬ LÝ FORM ĐĂNG NHẬP ===
        [HttpPost] 
        public async Task<IActionResult> Login(string Username, string Password, string RememberMe) 
        { 
            bool isRemember = RememberMe == "on"; 

            var result = await _signInManager.PasswordSignInAsync(Username, Password, isRemember, lockoutOnFailure: false); 

            if (result.Succeeded) 
            { 
                return RedirectToAction("Index", "Home");
            }

            return BadRequest("Sai tài khoản hoặc mật khẩu!");
        } 
        // === XỬ LÝ ĐĂNG XUẤT ===
        public async Task<IActionResult> Logout()
        { 
            await _signInManager.SignOutAsync(); 
            return RedirectToAction("Index", "Home"); 

        }

        // đăng nhập google
        [HttpGet]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (remoteError != null)
            {
                return RedirectToAction("Index", "Home");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new IdentityUser { UserName = email, Email = email };
                        await _userManager.CreateAsync(user);
                    }

                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
            }

            return RedirectToAction("Index", "Home");
        }
    } 
}
