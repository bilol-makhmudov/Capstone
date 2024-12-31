using System.Data.Common;
using Capstone.Models;
using Capstone.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Create([FromBody] List<string> tagNames)
        {
            if (tagNames == null || tagNames.Count == 0)
            {
                return BadRequest(new { error = "Tag names cannot be empty." });
            }

            foreach (var tagName in tagNames)
            {
                if (string.IsNullOrWhiteSpace(tagName)) continue;

                var existingTag = await _tagRepository.FindAsync(t => t.TagName == tagName);
                if (!existingTag.Any())
                {
                    var newTag = new Tag { TagName = tagName };
                    await _tagRepository.AddAsync(newTag);
                }
            }

            try
            {
                await _tagRepository.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { error = "An error occurred while saving tags.", details = ex.InnerException?.Message });
            }

            return Ok();
        }
    }
}

