using Capstone.Data;
using Capstone.Models;
using Capstone.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Repositories.Implementations;

public class UserRepository : Repository<ApplicationUser>, IUserRepository
{
    private readonly ApplicationDbContext _context;
    public UserRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<ApplicationUser>> SearchForUser(string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
        {
            return [];
        }
        
        var users = await _context.Users
            .Where(u => EF.Functions.ILike(u.UserName, $"%{searchString}%") ||
                        EF.Functions.ILike(u.FirstName, $"%{searchString}%") ||
                        EF.Functions.ILike(u.LastName, $"%{searchString}%"))
            .ToListAsync();
        return users;
    }
}