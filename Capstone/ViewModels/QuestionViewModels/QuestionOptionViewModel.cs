namespace Capstone.ViewModels.QuestionViewModels;

public class QuestionOptionViewModel
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string? OptionText { get; set; }
}