using Authentication.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Contexts
{
    public class AuthenticationContext(DbContextOptions<AuthenticationContext> options) : IdentityDbContext<AccountUser>(options)
    {
    }
}

