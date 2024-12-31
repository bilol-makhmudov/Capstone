using Capstone.Models;
using Capstone.ViewModels.User;

namespace Capstone.Repositories.Interfaces;

public interface IUserRepository : IRepository<ApplicationUser>
{
    Task<List<ApplicationUser>> SearchForUser(string searchString);
    Task<List<UserViewModel>> GetUsersToManage();
    Task<bool> UserLock(Guid id);
    Task<bool> UserUnLock(Guid id);
    Task<bool> UserDelete(Guid id);
}