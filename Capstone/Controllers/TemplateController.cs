using System.Security.Claims;
using Capstone.Models;
using Capstone.Repositories.Interfaces;
using Capstone.ViewModels.Template;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.Controllers;

public class TemplateController : Controller
{
    private readonly ITemplateRepository _templateRepository;
    public TemplateController(
        ITemplateRepository templateRepository)
    {
        _templateRepository = templateRepository;
    }
    public async Task<IActionResult> Create()
    {
        var viewModel = await _templateRepository.GetCreateTemplateViewModelAsync();
        return View(viewModel);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTemplateViewModel model)
    {
        if (ModelState.IsValid)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return BadRequest("Invalid user ID.");
            }
            
            bool isCreated = await _templateRepository.CreateTemplate(model, userId);
            if (isCreated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "An error occurred while creating the template.");
            }
        }
        return View();
    }
    
    public IActionResult Index()
    {
        return View();
    }
}