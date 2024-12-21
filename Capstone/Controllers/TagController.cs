using Capstone.Models;
using Capstone.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.Controllers
{
    [Route("[controller]")]
    public class TagController : Controller
    {
        private readonly ITagRepository _tagRepository;
        public TagController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }
        [HttpGet("Search")]
        public async Task<IActionResult> Search(string q)
        {
            var tags = await _tagRepository.SearchTagsAsync(q);
            return Json(tags);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                return BadRequest(new { error = "Tag name cannot be empty." });
            }
            var newTag = new Tag{TagName = tagName};
            await _tagRepository.AddAsync(newTag);
            bool success = await _tagRepository.SaveChangesAsync();
            if (!success)
            {
                return BadRequest(new { error = "Failed to create tag." });
            }
            return Ok();
        }
    }
}

