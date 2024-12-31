using System.ComponentModel.DataAnnotations;
using Capstone.Models;
using Capstone.ViewModels.QuestionViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.ViewModels.Template;

public class EditTemplateViewModel
{
    public Guid Id { get; set; }
    [StringLength(100, MinimumLength = 5)]
    public string Title { get; set; }
    
    [StringLength(1000, MinimumLength = 10)]
    public string? Description { get; set; }
    
    [Display(Name = "Topic")]
    public Guid? TopicId { get; set; }

    [Display(Name = "Public")]
    public bool IsPublic { get; set; } = true;

    [Display(Name = "Image on the top")]
    public string? ImageUrl { get; set; }
    public IFormFile? Image { get; set; }
    
    public string RowVersionBase64 { get; set; }

    [Display(Name = "Tags")]
    public List<Guid>? SelectedTagIds { get; set; } = new List<Guid>();

    [Display(Name = "Permitted Users")] public List<Guid>? TemplateUserIds { get; set; } = [];
    public List<Tag>? Tags { get; set; } = new List<Tag>();
    public List<Topic>? Topics { get; set; } = new List<Topic>();
    public List<QuestionViewModel>? Questions { get; set; } = new List<QuestionViewModel>();
}