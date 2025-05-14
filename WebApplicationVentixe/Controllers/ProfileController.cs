using Authentication.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplicationVentixe.Mappers;
using WebApplicationVentixe.Models.Profile;
using WebApplicationVentixe.Protos;

namespace WebApplicationVentixe.Controllers
{
    public class ProfileController(ProfileHandler.ProfileHandlerClient profileGrpcClient, UserManager<AccountUser> userManager) : Controller
    {
        private readonly ProfileHandler.ProfileHandlerClient _profileGrpcClient = profileGrpcClient;
        private readonly UserManager<AccountUser> _userManager = userManager;


        [HttpGet]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var response = await _profileGrpcClient.GetProfileByUserIdAsync(
                new GetProfileByUserIdRequest { UserId = userId }
            );

            if (response.Profile != null)
                model = response.Profile.MapToProfileViewModel();

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Index(ProfileViewModel model, string action)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                ViewBag.Message = "User not found.";
                return View(model);
            }
            var response = await _profileGrpcClient.GetProfileByUserIdAsync(
                new GetProfileByUserIdRequest { UserId = userId }
            );

            if (action == "save")
            {
                if (!ModelState.IsValid)
                    return View(model);

                if (response.Profile != null)
                {
                    var updateProfileRequest = model.MapToUpdateProfileRequest(userId);
                    var updateResponse = await _profileGrpcClient.UpdateProfileAsync(updateProfileRequest);
                    if (!updateResponse.Success)
                    {
                        ViewBag.Message = "Profile save unsuccessfull.";
                        return View(model);
                    }
                }
                else
                {
                    var createProfileRequest = model.MapToCreateProfileRequest(userId);
                    var createResponse = await _profileGrpcClient.CreateProfileAsync(createProfileRequest);
                    if (!createResponse.Success)
                    {
                        ViewBag.Message = "Profile save unsuccessfull for external.";
                        return View(model);
                    }
                }
                
                ViewBag.Message = "Profile saved successfully.";
            }
            else if (action == "delete")
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    ViewBag.Message = "User not found for deletion.";
                    return View(model);
                }
                var deleteAccountResponse = await _userManager.DeleteAsync(user);
                if (!deleteAccountResponse.Succeeded)
                {
                    ViewBag.Message = "Account deletion unsuccessfull.";
                    return View(model);
                }
                var deleteProfileRespone = await _profileGrpcClient.DeleteProfileByUserIdAsync(
                    new DeleteProfileByUserIdRequest { UserId = userId }
                );
                if (!deleteProfileRespone.Success)
                    ViewBag.Message = "Profile infromation deletion unsuccessfull.";
                else
                    ViewBag.Message = "Account deleted.";

                return RedirectToAction("Logout", "Login");
            }

            return View(model);
        }
    }
}
