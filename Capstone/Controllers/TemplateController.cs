using System.Security.Claims;
using Capstone.Repositories.Interfaces;
using Capstone.ViewModels.Template;
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
        model = await _templateRepository.GetCreateTemplateViewModelAsync(model);
        return View(model);
    }
    
    public async Task<IActionResult> Edit(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        if (!Guid.TryParse(userIdClaim.Value, out Guid userId)) return BadRequest("Invalid user ID.");

        var viewModel = await _templateRepository.GetTemplateForEditAsync(id, userId);
        if (viewModel == null) return NotFound();

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditTemplateViewModel model)
    {
        if (ModelState.IsValid)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId)) return BadRequest("Invalid user ID.");
            
            bool isUpdated = await _templateRepository.UpdateTemplate(model, userId);
            if (isUpdated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "An error occurred while updating the template.");
            }
        }
        
        var userGuid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var refill = await _templateRepository.GetTemplateForEditAsync(model.Id, userGuid);
        if (refill != null)
        {
            refill.Title = model.Title;
            refill.Description = model.Description;
            refill.IsPublic = model.IsPublic;
            refill.TopicId = model.TopicId;
        }
        return View(refill);
    }
    
    public IActionResult Index()
    {
        return View();
    }
}