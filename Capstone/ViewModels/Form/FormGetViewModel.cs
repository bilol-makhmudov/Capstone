using System.ComponentModel.DataAnnotations;
using Capstone.Models;

namespace Capstone.ViewModels.Form;

public class FormGetViewModel
{
    [Required]
    public Guid TemplateId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public Topic? Topic { get; set; }
    public List<TemplateTag> TemplateTags { get; set; } = new List<TemplateTag>();
    public List<Question> Questions { get; set; } = new List<Question>();
}