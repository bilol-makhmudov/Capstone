namespace Capstone.Models;

public class Question
{
    public Guid QuestionId { get; set; }
    public required string QuestionText { get; set; }
    public string? Description { get; set; }
    public QuestionType QuestionType { get; set; }
    public Guid TemplateId { get; set; }
    public int Order { get; set; }
    public string? ImageUrl { get; set; }

    public Template Template { get; set; }
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}