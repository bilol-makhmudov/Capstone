using System.ComponentModel.DataAnnotations;
using Capstone.Models;
using Capstone.ViewModels.QuestionViewModels;

namespace Capstone.ViewModels.Form;

public class FormGetViewModel
{
    [Required]
    public Guid TemplateId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? TopicName { get; set; }
    public List<string> TagNames { get; set; } = [];
    public List<QuestionFormFillViewModel> Questions { get; set; } = new List<QuestionFormFillViewModel>();
}