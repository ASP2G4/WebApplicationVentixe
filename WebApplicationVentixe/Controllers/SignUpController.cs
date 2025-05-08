using Authentication.Entities;
using Authentication.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplicationVentixe.Models.SignUp;
using WebApplicationVentixe.Protos;

namespace WebApplicationVentixe.Controllers
{
    public class SignUpController(IAccountService accountService, IVerificationService verificationService, ProfileHandler.ProfileHandlerClient profileGrpcClient, UserManager<AccountUser> userManager) : Controller
    {
        private readonly IAccountService _accountService = accountService;
        private readonly IVerificationService _verificationService = verificationService;
        private readonly ProfileHandler.ProfileHandlerClient _profileGrpcClient = profileGrpcClient;
        private readonly UserManager<AccountUser> _userManager = userManager; 

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
                EmailConfirmed = true,
            };

            var response = await _accountService.CreateAccountAsync(account, model.Password);
            if (!response.Succeeded)
            {
                TempData.Keep("Email");
                return View(model);
            }

            var roleAssignmentResult = await _userManager.AddToRoleAsync(account, "User");
            if (!roleAssignmentResult.Succeeded)
            {
                TempData.Keep("Email");
                ModelState.AddModelError("", "Failed to assign the 'User' role.");
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
        public async Task<IActionResult> ProfileInformation(ProfileInformationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = TempData["UserId"]?.ToString();

            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "User ID is missing.");
                return View(model);
            }

            var createProfileRequest = new CreateProfileRequest
            {
                Profile = new Profile
                {
                    UserId = userId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Address = new ProfileAddress
                    {
                        StreetName = model.StreetName,
                        PostalCode = model.PostalCode,
                        City = model.City
                    }
                }
            };

            var response = await _profileGrpcClient.CreateProfileAsync(createProfileRequest);

            if (!response.Success)
            {
                ModelState.AddModelError("", response.Message);
                return View(model);
            }

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
