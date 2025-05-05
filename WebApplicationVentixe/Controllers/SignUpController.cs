using Authentication.Entities;
using Authentication.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplicationVentixe.Models.SignUp;

namespace WebApplicationVentixe.Controllers
{
    public class SignUpController(IAccountService accountService, IVerificationService verificationService) : Controller
    {
        // BYT TILL GRPC SEN och egna microservices
        private readonly IAccountService _accountService = accountService;
        private readonly IVerificationService _verificationService = verificationService;

        #region 1 - Set email
        [HttpGet("signup")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("signup")]
        public async Task<IActionResult> Index(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Invalid email Address";
                return View(model);
            }

            var findAccountResponse = await _accountService.FindByEmailAsync(model.Email);
            if (findAccountResponse.Succeeded)
            {
                ViewBag.ErrorMessage = "Account already exsits";
                return View(model);
            }

            var verificationCodeResponse = await _verificationService.SendVerificationCodeAsync(model.Email);
            if (!verificationCodeResponse.Succeeded)
            {
                ViewBag.ErrorMessage = verificationCodeResponse.Error;
                return View(model);
            }

            TempData["Email"] = model.Email;
            return RedirectToAction("AccountVerification");
        }
        #endregion

        #region 2 - Verification
        [HttpGet("account-verification")]
        public IActionResult AccountVerification()
        {
            if (TempData["Email"] == null)
                return RedirectToAction("Index");

            ViewBag.MaskedEmail = MaskEmail(TempData["Email"]!.ToString()!);
            TempData.Keep("Email");

            return View();
        }
        [HttpPost("account-verification")]
        public async Task<IActionResult> AccountVerification(AccountVerificationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var email = TempData["Email"]?.ToString();

            if (string.IsNullOrWhiteSpace(email))
                return RedirectToAction("Index");

            var response = await _verificationService.ValidateVerificationCodeAsync(email, model.Code);

            if (!response.Succeeded)
            {
                ViewBag.ErrorMessage = response.Error;
                TempData.Keep("Email");
                return View(model);
            }
            TempData.Keep("Email");
            return RedirectToAction("SetPassword");
        }

        #endregion

        #region 3 - Password
        [HttpGet("set-password")]
        public IActionResult SetPassword()
        {
            return View();
        }
        [HttpPost("set-password")]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var email = TempData["Email"]?.ToString();
            if (string.IsNullOrWhiteSpace(email))
                return RedirectToAction(nameof(Index));

            var account = new AccountUser
            {
                UserName = email,
                Email = email,
            };

            var response = await _accountService.CreateAccountAsync(account, model.Password);
            if (!response.Succeeded)
            {
                TempData.Keep("Email");
                return View(model);
            }

            TempData["UserId"] = response.Result;
            return RedirectToAction("ProfileInformation");
        }

        #endregion

        #region 4 - Profile info
        [HttpGet("profile-information")]
        public IActionResult ProfileInformation()
        {
            return View();
        }
        [HttpPost("profile-information")]
        public IActionResult ProfileInformation(ProfileInformationViewModel model)
        {
            return RedirectToAction("Index", "Login");
        }
        #endregion
        
        private string MaskEmail(string email)
        {
            var parts = email.Split('@');
            var firstChar = parts[0].First();

            return $"{firstChar}*****@{parts[1]}";
        }
    }
}
