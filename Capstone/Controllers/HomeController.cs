using System.Diagnostics;
using Capstone.Data;
using Capstone.Models;
using Capstone.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Capstone.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;

namespace Capstone.Controllers;
[Authorize]
public class HomeController : Controller
{
    private readonly ITemplateRepository _templateRepository;
    public HomeController(ITemplateRepository templateRepository)
    {
        _templateRepository = templateRepository;
    }

    public async Task<IActionResult> Index()
    {
        IEnumerable<Template> templates = await _templateRepository.GetAllAsync();
        return View(templates);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public IActionResult CultureManagement(string? culture, string? returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture ?? "en")),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
        
        return Redirect(returnUrl ?? "/");
    }
}