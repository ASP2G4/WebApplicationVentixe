using Authentication.Entities;
using Microsoft.EntityFrameworkCore;

namespace Authentication
{
    public class AccountProfileContext(DbContextOptions<AccountProfileContext> options) : DbContext(options)
    {
        public DbSet<AccountProfile> AccountProfiles { get; set; }
        public DbSet<AccountProfileAddress> AccountProfileAddresses { get; set; }
    }
}

