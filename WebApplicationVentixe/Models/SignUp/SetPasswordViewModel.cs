using System.ComponentModel.DataAnnotations;

namespace WebApplicationVentixe.Models.SignUp
{
    public class SetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Password", Prompt = "Enter Password")]
        public string Password { get; set; } = null!;
        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", Prompt = "Confirm Password")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match!")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
