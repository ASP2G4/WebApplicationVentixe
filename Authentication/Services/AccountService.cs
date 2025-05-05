using Authentication.Dtos;
using Authentication.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Services
{
    public interface IAccountService
    {
        Task<AccountServiceResult> FindByEmailAsync(string email);
        Task<AccountServiceResult<string>> CreateAccountAsync(AccountUser user, string password);
    }
    public class AccountService(UserManager<AccountUser> userManager) : IAccountService
    {
        private readonly UserManager<AccountUser> _userManager = userManager;

        public async Task<AccountServiceResult<string>> CreateAccountAsync(AccountUser user, string password)
        {
            var response = await _userManager.CreateAsync(user, password);
            return response.Succeeded ?
                new AccountServiceResult<string> { Succeeded = true, Result = user.Id } :
                new AccountServiceResult<string> { Succeeded = false, Error = string.Join(", ", response.Errors) };

        }

        public async Task<AccountServiceResult> FindByEmailAsync(string email)
        {
            var result = await _userManager.Users.AnyAsync(x => x.Email == email);
            return result ?
                new AccountServiceResult { Succeeded = true } :
                new AccountServiceResult { Succeeded = false, Error = "Account not found" };
        }
    }
}
