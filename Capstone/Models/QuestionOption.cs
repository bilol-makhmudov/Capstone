namespace Capstone.Models;

public class QuestionOption : BaseEntity
{
    public Guid QuestionId { get; set; }
    public string? OptionText { get; set; }
    public Question Question { get; set; }
    public ICollection<OptionAnswer> OptionAnswers { get; set; }
}