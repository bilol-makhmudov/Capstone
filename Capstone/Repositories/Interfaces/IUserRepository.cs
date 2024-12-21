using Capstone.Models;

namespace Capstone.Repositories.Interfaces;

public interface IUserRepository : IRepository<ApplicationUser>
{
    Task<List<ApplicationUser>> SearchForUser(string searchString);
}