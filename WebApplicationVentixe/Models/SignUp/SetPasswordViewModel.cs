using System.ComponentModel.DataAnnotations;

namespace WebApplicationVentixe.Models.SignUp
{
    public class SetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Password", Prompt = "Enter Password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must be at least 8 characters long, include one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; } = null!;
        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", Prompt = "Confirm Password")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match!")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
