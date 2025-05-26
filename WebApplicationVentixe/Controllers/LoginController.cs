using Authentication.Entities;
using Authentication.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplicationVentixe.Models.Login;

namespace WebApplicationVentixe.Controllers
{
    public class LoginController(SignInManager<AccountUser> signInManager, UserManager<AccountUser> userManager, IJwtManager jwtHandler) : Controller
    {
        private readonly SignInManager<AccountUser> _signInManager = signInManager;
        private readonly UserManager<AccountUser> _userManager = userManager;
        private readonly IJwtManager _jwtHandler = jwtHandler;


        public IActionResult Index(string returnUrl = "~/")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model, string returnUrl = "~/")
        {
            returnUrl ??= "~/";
            ViewBag.ReturnUrl = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var respone = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (respone.Succeeded && user != null)
                {
                    var success = await TrySetJwtAsync(user);

                    if (!success)
                    {
                        ViewBag.ErrorMessage = "Login succeeded, but failed to generate JWT token ";
                        await _signInManager.SignOutAsync();
                        return View(model);
                    }
                    return LocalRedirect(ViewBag.ReturnUrl);
                }
            }
            ViewBag.ErrorMessage = "Invalid email or password";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _jwtHandler.RemoveCookieAndSession(HttpContext);
            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetLink = Url.Action("ResetPassword", "Login", new { token, email = model.Email }, Request.Scheme);

                    // om vi vill lägga till skickande av email för reset gör vi det här, vi får utöka emailservicen

                    ViewBag.Message = "Password reset link has been sent to your email.";
                }
                else
                {
                    ViewBag.Message = "If the email is registered, a reset link will be sent.";
                }
            }

            return View(model);
        }


        [HttpPost]
        public IActionResult ExternalSignIn(string provider, string returnUrl= null!) 
        {
            if(string.IsNullOrEmpty(provider))
            {
                ModelState.AddModelError("", "Provider must be provided");
                return View("Index");
            }

            var redirectUrl = Url.Action("ExternalSignInCallback", "Login", new { ReturnUrl = returnUrl })!;
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalSignInCallback(string returnUrl= null!, string remoteError = null!)
        {
            returnUrl ??= Url.Content("~/");

            if(!string.IsNullOrEmpty(remoteError))
            {
                ModelState.AddModelError("", $"Error from external provider: {remoteError}");
                return View("Index");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if(info == null)
                return RedirectToAction("Index");

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Succeeded)
            {
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (user != null)
                {      
                    var success = await TrySetJwtAsync(user);

                    if (!success)
                    {
                        ViewBag.ErrorMessage = "Login succeeded, but failed to generate JWT token";
                        await _signInManager.SignOutAsync();
                        return View("Index");
                    }
                }
                return LocalRedirect(returnUrl);
            }
            else
            {   
                var success = await CreateAndLoginExternal(info);

                if (!success)
                {
                    ViewBag.ErrorMessage = "Login succeeded, but failed to generate JWT token";
                    await _signInManager.SignOutAsync();
                    return View("Index");
                }
                return LocalRedirect(returnUrl);

            }

        }

        private async Task<bool> CreateAndLoginExternal(ExternalLoginInfo info)
        {
            string email = info.Principal.FindFirstValue(ClaimTypes.Email)!;
            string username = $"ext_{info.LoginProvider.ToLower()}_{email}";

            var user = new AccountUser
            {
                UserName = username,
                Email = email,
                EmailConfirmed = true,
            };

            var identityResult = await _userManager.CreateAsync(user);
            if (!identityResult.Succeeded)
                return false;

            var roleAssignmentResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleAssignmentResult.Succeeded)
                return false;

            await _userManager.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, isPersistent: false);

            var success = await TrySetJwtAsync(user);
            return success;

        }

        private async Task<string?> GetUserRoleAsync(AccountUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault();
        }

        private async Task<bool> TrySetJwtAsync(AccountUser user)
        {
            var role = await GetUserRoleAsync(user);
            if (role == null)
                return false;
            var success = await _jwtHandler.SetJwtAsync(user.Id, user.UserName!, role, user.Email!, HttpContext);
            if (!success)
                await _signInManager.SignOutAsync();
            return success;
        }
    }
}
