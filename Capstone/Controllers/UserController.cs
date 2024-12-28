using Capstone.Repositories.Interfaces;
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
    
    [HttpGet("Search")]
    public async Task<IActionResult> Search(string q)
    {
        var users = await _userRepository.SearchForUser(q);
        return Json(users);
    }
}