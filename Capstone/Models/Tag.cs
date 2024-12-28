using System.ComponentModel.DataAnnotations;

namespace Capstone.Models;

public class Tag : BaseEntity
{
    [StringLength(100)]
    [Required]
    public required string TagName { get; set; }
    public ICollection<TemplateTag> TemplateTags { get; set; } = new List<TemplateTag>();
}