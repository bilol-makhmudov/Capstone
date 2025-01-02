
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

        List<UserViewModel> userViewModels = new List<UserViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.Contains("Admin") ? "Admin" : roles.FirstOrDefault() ?? "User";
            
            userViewModels.Add(new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? "",
                IsLocked = user.LockoutEnabled 
                           && user.LockoutEnd.HasValue 
                           && user.LockoutEnd.Value > DateTimeOffset.UtcNow,
                Role = userRole
            });
        }
        
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
            await _userManager.DeleteAsync(user);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    
    public async Task<bool> ToggleUserAdmin(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return false;
            
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        if (isAdmin)
        {
            var removeAdminResult = await _userManager.RemoveFromRoleAsync(user, "Admin");
            if (!removeAdminResult.Succeeded) return false;

            var addUserResult = await _userManager.AddToRoleAsync(user, "User");
            if (!addUserResult.Succeeded) return false;
        }
        else
        {
            var removeUserResult = await _userManager.RemoveFromRoleAsync(user, "User");
            if (!removeUserResult.Succeeded) return false;

            var addAdminResult = await _userManager.AddToRoleAsync(user, "Admin");
            if (!addAdminResult.Succeeded) return false;
        }
            
        return true;
    }
}