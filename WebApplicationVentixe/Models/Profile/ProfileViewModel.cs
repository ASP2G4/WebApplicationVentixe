using System.ComponentModel.DataAnnotations;

namespace WebApplicationVentixe.Models.Profile
{
    public class ProfileViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "First Name", Prompt = "Enter your first name")]
        [Required(ErrorMessage = "First Name must be provided")]
        public string FirstName { get; set; } = null!;
        [DataType(DataType.Text)]
        [Display(Name = "Last Name", Prompt = "Enter your last name")]
        [Required(ErrorMessage = "Last Name must be provided")]
        public string LastName { get; set; } = null!;
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number", Prompt = "Enter your phone number")]
        [Required(ErrorMessage = "Phone Number must be provided")]
        public string PhoneNumber { get; set; } = null!;
        [DataType(DataType.Text)]
        [Display(Name = "Street Name", Prompt = "Enter your street name")]
        [Required(ErrorMessage = "Street Name must be provided")]
        public string StreetName { get; set; } = null!;
        [DataType(DataType.PostalCode)]
        [Display(Name = "Postal Code", Prompt = "Enter your postal code")]
        [Required(ErrorMessage = "Postal Code must be provided")]
        public string PostalCode { get; set; } = null!;
        [DataType(DataType.Text)]
        [Display(Name = "City", Prompt = "Enter your city")]
        [Required(ErrorMessage = "City must be provided")]
        public string City { get; set; } = null!;
    }
}
