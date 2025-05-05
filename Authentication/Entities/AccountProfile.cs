using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Authentication.Entities
{
    public class AccountProfile
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }

    }

    public class AccountProfileAddress
    {
        [Key, ForeignKey(nameof(AccountProfile))]
        public string AccountProfileId { get; set; } = null!;
        public AccountProfile? AccountProfile { get; set; } = null!;
        public string? StreetName { get; set; }
        public string? PostalCode { get; set; }
        public string? City { get; set; }

    }
}
