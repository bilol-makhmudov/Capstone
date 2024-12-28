namespace Capstone.ViewModels.QuestionViewModels;

public class QuestionAnswerViewModel
{
    public Guid QuestionId { get; set; }
    public string? StringResponse { get; set; }
    public int? NumericAnswer { get; set; }
    public List<Guid>? SelectedOptionIds { get; set; }
}