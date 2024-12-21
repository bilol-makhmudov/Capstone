namespace Capstone.Models
{
    public class Answer : BaseEntity
    {
        public Guid AnswerId { get; set; }
        public Guid FormId { get; set; }
        public Guid QuestionId { get; set; }
        public string Response { get; set; }
        
        public Form Form { get; set; }
        public Question Question { get; set; }
    }
}