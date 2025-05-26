using System.ComponentModel.DataAnnotations;

namespace WebApplicationVentixe.Models.SignUp
{
    public class SignUpViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email", Prompt = "Enter email address")]
        [Required(ErrorMessage = "Email must be provided")]
        [RegularExpression(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", ErrorMessage = "Please enter a valid email address.")]

        public string Email { get; set; } = null!;
    }
}
