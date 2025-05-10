using System.ComponentModel.DataAnnotations;

namespace WebApplicationVentixe.Models.Login
{
    public class LoginViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email", Prompt = "Enter email address")]
        [Required (ErrorMessage = "Email must be provided")]
        public string Email { get; set; } = null!;
        [DataType(DataType.Password)]
        [Display(Name = "Password", Prompt = "Enter password")]
        [Required(ErrorMessage = "Password must be provided")]
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }
}
