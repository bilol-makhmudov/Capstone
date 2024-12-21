using Capstone.Models;
using Capstone.ViewModels.Account;

namespace Capstone.Repositories.Interfaces;

public interface IAccountRepository : IRepository<ApplicationUser>
{
    Task<bool> RegisterApplicationUser(RegisterViewModel model);
    Task<bool> LoginUser(LoginViewModel model);
    Task LogoutUser();
}