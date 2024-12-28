namespace Capstone.Models;

public class OptionAnswer : BaseEntity
{
    public Guid AnswerId { get; set; }
    public Guid QuestionOptionId { get; set; }
    public Answer Answer { get; set; }
    public QuestionOption QuestionOption { get; set; }
}