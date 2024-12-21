using System.ComponentModel.DataAnnotations;

namespace Capstone.Models;

public class Tag
{
    [Key]
    public Guid TagId { get; set; }
    [StringLength(100)]
    [Required]
    public required string TagName { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
    
    public ICollection<TemplateTag> TemplateTags { get; set; } = new List<TemplateTag>();
}