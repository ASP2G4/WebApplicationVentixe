using System.ComponentModel.DataAnnotations;

namespace WebApplicationVentixe.Models.SignUp
{
    public class SignUpViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email", Prompt = "Enter email address")]
        [Required(ErrorMessage = "Email must be provided")]
        public string Email { get; set; } = null!;
    }
    public class ProfileInformationViewModel
    {
    }
}
