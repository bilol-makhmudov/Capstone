using System.Security.Claims;
using Capstone.Models;
using Capstone.Repositories.Interfaces;
using Capstone.ViewModels.Form;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace Capstone.Controllers;

[Route("[controller]")]
public class FormController : Controller
{
    private readonly IFormRepository _formRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    public FormController(IFormRepository formRepository,
        UserManager<ApplicationUser> userManager)
    {
        _formRepository = formRepository;
        _userManager = userManager;
    }

    [HttpGet("{templateId:guid}")]
    public async Task<IActionResult> Fill(Guid templateId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        
        if (!Guid.TryParse(userIdClaim.Value, out Guid userId)) return BadRequest("Invalid user ID.");
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var model = await _formRepository.GetFormFillViewModel(templateId, userId);
            return View(model);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    [HttpPost("Submit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(FormAnswerViewModel formAnswer)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        
        if (!Guid.TryParse(userIdClaim.Value, out Guid userId)) return BadRequest("Invalid user ID.");
        
        if (!ModelState.IsValid)
        {
            var refillModel = _formRepository.GetFormFillViewModel(formAnswer.TemplateId, userId);
            return View("Fill", await refillModel);
        }
        
        try
        {
            var isSaved = await _formRepository.FormAnswer(formAnswer, userId);
        
            if (isSaved)
            {
                return RedirectToAction("Index", "Home");
            }
            
            ModelState.AddModelError("", "Failed to save the form answers.");
            var refillModel = await _formRepository.GetFormFillViewModel(formAnswer.TemplateId, userId);
            return View("Fill", refillModel);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            var refillModel = await _formRepository.GetFormFillViewModel(formAnswer.TemplateId, userId);
            return View("Fill", refillModel);
        }
    }
}