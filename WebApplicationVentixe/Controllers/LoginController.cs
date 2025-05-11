using Authentication.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplicationVentixe.Models.Login;

namespace WebApplicationVentixe.Controllers
{
    public class LoginController(SignInManager<AccountUser> signInManager, UserManager<AccountUser> userManager) : Controller
    {
        private readonly SignInManager<AccountUser> _signInManager = signInManager;
        private readonly UserManager<AccountUser> _userManager = userManager;
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
                if (respone.Succeeded)
                {
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
            return RedirectToAction("Index", "Home");
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
                return LocalRedirect(returnUrl);
            }
            else
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
                if (identityResult.Succeeded) 
                {
                    var roleAssignmentResult = await _userManager.AddToRoleAsync(user, "User");
                    if (!roleAssignmentResult.Succeeded)
                        return RedirectToAction("Index");

                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return RedirectToAction("Index");
            }

        }
    }
}
