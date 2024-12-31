using System.ComponentModel.DataAnnotations;
using Capstone.Models;

namespace Capstone.ViewModels.QuestionViewModels;

public class QuestionViewModel
{
    public Guid Id { get; set; }
    [Required]
    [Display(Name = "Question Text")]
    public string? QuestionText { get; set; }

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required]
    [Display(Name = "Question Type")]
    public QuestionType Type { get; set; }

    [Display(Name = "Show in Results")]
    public bool ShowInResults { get; set; }
    public string? RowVersionBase64 { get; set; }
    

    public int Order { get; set; }
    public IFormFile? QuestionImage { get; set; }
    public string? QuestionImageUrl { get; set; }
    public ICollection<QuestionOptionViewModel>? QuestionOptions { get; set; } = [];
}