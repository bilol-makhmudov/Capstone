using System.ComponentModel.DataAnnotations;
using Capstone.Models;
using Capstone.ViewModels.QuestionViewModels;

namespace Capstone.ViewModels.Template;

public class CreateTemplateViewModel
{
    [Required]
    [StringLength(100, MinimumLength = 5)]
    public string Title { get; set; }

    [Required]
    [StringLength(1000, MinimumLength = 10)]
    public string Description { get; set; }

    [Required]
    [Display(Name = "Topic")]
    public Guid TopicId { get; set; }

    [Display(Name = "Public")]
    public bool IsPublic { get; set; } = true;

    [Display(Name = "Image on the top")]
    public IFormFile? Image { get; set; }

    [Display(Name = "Tags")]
    public List<Guid> SelectedTagIds { get; set; } = new List<Guid>();

    [Display(Name = "Permitted Users")] public List<Guid> TemplateUserIds { get; set; } = [];
    public List<Tag> Tags { get; set; } = new List<Tag>();
    public List<Topic> Topics { get; set; } = new List<Topic>();
    public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();

}