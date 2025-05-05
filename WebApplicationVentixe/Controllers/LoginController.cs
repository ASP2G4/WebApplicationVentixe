using Authentication.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplicationVentixe.Models.Login;

namespace WebApplicationVentixe.Controllers
{
    public class LoginController(SignInManager<AccountUser> signInManager) : Controller
    {
        private readonly SignInManager<AccountUser> _signInManager = signInManager;
        public IActionResult Index(string returnUrl = "~/")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model, string returnUrl = "~/")
        {
            ViewBag.ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                var respone = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if(respone.Succeeded)
                {
                    return LocalRedirect(ViewBag.ReturnUrl);
                }
            }
            ViewBag.ErrorMessage = "Invalid email or password";
            return View(model);
        }
    }
}
