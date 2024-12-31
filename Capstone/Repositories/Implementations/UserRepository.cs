
using Capstone.Data;
using Capstone.Models;
using Capstone.Repositories.Interfaces;
using Capstone.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Repositories.Implementations;

public class UserRepository : Repository<ApplicationUser>, IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    public UserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : base(context)
    {
        _context = context;
        _userManager = userManager;
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

    public async Task<List<UserViewModel>> GetUsersToManage()
    {
        List<ApplicationUser> users = await _context
            .Users.Where(u => u.IsDeleted != true)
            .ToListAsync();

        List<UserViewModel> userViewModels = users.Select(u => new UserViewModel
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            IsLocked = u.LockoutEnabled 
                       && u.LockoutEnd.HasValue 
                       && u.LockoutEnd.Value > DateTimeOffset.UtcNow
        }).ToList();
        
        return userViewModels;
    }
    
    public async Task<bool> UserLock(Guid id)
    {
        try
        {        
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;
            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, new DateTimeOffset(DateTime.UtcNow.AddYears(100)));
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> UserUnLock(Guid id)
    {
        try
        {        
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;
            await _userManager.SetLockoutEnabledAsync(user, false);
            await _userManager.SetLockoutEndDateAsync(user, new DateTimeOffset(DateTime.UtcNow));
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> UserDelete(Guid id)
    {
        try
        {        
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;
            
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}