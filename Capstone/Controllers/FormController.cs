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
    public IActionResult Fill(Guid templateId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var model = _formRepository.GetFormFillViewModel(templateId);
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
        if (!ModelState.IsValid)
        {
            var refillModel = _formRepository.GetFormFillViewModel(formAnswer.TemplateId);
            return View("Fill", refillModel);
        }
        
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Unauthorized();
        }
        
        try
        {
            Guid parsedUserId = Guid.Parse(userId);
            var isSaved = await _formRepository.FormAnswer(formAnswer, parsedUserId);
        
            if (isSaved)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Failed to save the form answers.");
                var refillModel = _formRepository.GetFormFillViewModel(formAnswer.TemplateId);
                return View("Fill", refillModel);
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            var refillModel = _formRepository.GetFormFillViewModel(formAnswer.TemplateId);
            return View("Fill", refillModel);
        }
        return Ok();
    }
}