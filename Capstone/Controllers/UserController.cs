using Capstone.Repositories.Interfaces;
using Capstone.ViewModels.User;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.Controllers;

[Route("[controller]")]
public class UserController : Controller
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    [HttpGet("Manage")]
    public IActionResult Manage()
    {
        return View();
    }
    
    [HttpGet("Search")]
    public async Task<IActionResult> Search(string q)
    {
        var users = await _userRepository.SearchForUser(q);
        return Json(users);
    }

    [HttpGet("GetUsers")]
    public async Task<IActionResult> GetUsers()
    {
        List<UserViewModel> users = await _userRepository.GetUsersToManage();
        return Json(new { data = users });
    }
    
    [HttpPost("LockUser")]
    public async Task<IActionResult> LockUser([FromForm] Guid id)
    {
        if (!ModelState.IsValid || id == Guid.Empty)
        {
            return BadRequest();
        }
        return Ok(await _userRepository.UserLock(id));
    }

    [HttpPost("UnlockUser")]
    public async Task<IActionResult> UnlockUser([FromForm] Guid id)
    {
        if (!ModelState.IsValid || id == Guid.Empty)
        {
            return BadRequest();
        }
        return Ok(await _userRepository.UserUnLock(id));
    }
    
    
    [HttpDelete("DeleteUser")]
    public async Task<IActionResult> DeleteUser([FromForm] Guid id)
    {
        if (!ModelState.IsValid || id == Guid.Empty)
        {
            return BadRequest();
        }
        return Ok(await _userRepository.UserDelete(id));
    }
}