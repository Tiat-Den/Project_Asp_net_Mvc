using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

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
                await _userManager.AddToRoleAsync(user, "KhachHang");

                await _signInManager.SignInAsync(user, isPersistent: false); 
                return Ok();
            }

            var error = result.Errors.FirstOrDefault()?.Description ?? "Lỗi đăng ký!";
            return BadRequest(error); 
        } 

        // === XỬ LÝ FORM ĐĂNG NHẬP ===
        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password, string RememberMe)
        {
            bool isRemember = RememberMe == "on";
            string loginUsername = Username;

            // Kiểm tra xem người dùng nhập vào là Email hay Username
            if (Username.Contains("@"))
            {
                var user = await _userManager.FindByEmailAsync(Username);
                if (user != null)
                {
                    loginUsername = user.UserName; // Lấy Username thật từ Email
                }
                else
                {
                    return BadRequest("Email không tồn tại!");
                }
            }

            // Thực hiện đăng nhập bằng Username đã xác định
            var result = await _signInManager.PasswordSignInAsync(loginUsername, Password, isRemember, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(loginUsername);
                var customClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? "")
                };
                await _signInManager.SignInWithClaimsAsync(user, isRemember, customClaims);

                return Ok(); // Trả về mã thành công 200
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

        // TÀI KHOẢN
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(string PhoneNumber)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            user.PhoneNumber = PhoneNumber;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Profile");
            }

            return View(user);
        }

        // THAY ĐỔI MK 
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string OldPassword, string NewPassword, string ConfirmPassword)
        {
            if (NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu mới không khớp!");
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManager.ChangePasswordAsync(user, OldPassword, NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View();
        }

        // QUÊN MK

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user != null)
            {
                var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("datpham14563@gmail.com"),
                    Subject = "Mã xác nhận khôi phục mật khẩu",
                    Body = $"Mã xác nhận của bạn là: {code}",
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(Email);

                using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.Credentials = new NetworkCredential("datpham14563@gmail.com", "qvfb jbcn rzwq xzyk");
                    smtpClient.EnableSsl = true;
                    await smtpClient.SendMailAsync(mailMessage);
                }

                return RedirectToAction("VerifyResetCode", new { email = Email });
            }

            ModelState.AddModelError("", "Email không tồn tại trong hệ thống.");
            return View();
        }

        [HttpGet]
        public IActionResult VerifyResetCode(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyResetCode(string Email, string Code)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user != null)
            {
                var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", Code);
                if (isValid)
                {
                    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    return RedirectToAction("ResetPassword", new { email = Email, token = resetToken });
                }
            }

            ModelState.AddModelError("", "Mã xác nhận không hợp lệ hoặc đã hết hạn.");
            ViewBag.Email = Email;
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            ViewBag.Email = email;
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string Email, string Token, string NewPassword, string ConfirmPassword)
        {
            if (NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu không khớp.");
                ViewBag.Email = Email;
                ViewBag.Token = Token;
                return View();
            }

            var user = await _userManager.FindByEmailAsync(Email);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, Token, NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            ViewBag.Email = Email;
            ViewBag.Token = Token;
            return View();
        }
    } 
}
