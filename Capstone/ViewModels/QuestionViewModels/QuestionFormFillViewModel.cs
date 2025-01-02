using System.ComponentModel.DataAnnotations;
using Capstone.Models;

namespace Capstone.ViewModels.QuestionViewModels;

public class QuestionFormFillViewModel
{
    public Guid Id { get; set; }
    
    [Display(Name = "Question Text")]
    public string QuestionText { get; set; }
    
    [Display(Name = "Description")]
    public string? Description { get; set; }
    
    [Display(Name = "Question Type")]
    public QuestionType? QuestionType { get; set; }

    public int Order { get; set; }
    public string? ImageUrl { get; set; }
    
    public string? StringResponse { get; set; }
    public int? NumericAnswer { get; set; }
    
    public ICollection<QuestionOptionViewModel> QuestionOptions { get; set; } = [];
}