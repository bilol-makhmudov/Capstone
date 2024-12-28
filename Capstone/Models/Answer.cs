using System.ComponentModel.DataAnnotations;

namespace Capstone.Models
{
    public class Answer : BaseEntity
    {
        [Required(ErrorMessage = "Template ID is required.")]
        public Guid TemplateId { get; set; }

        [Required(ErrorMessage = "Question ID is required.")]
        public Guid QuestionId { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public Guid UserId { get; set; }

        [MaxLength(1000, ErrorMessage = "String response cannot exceed 1000 characters.")]
        public string? StringResponse { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Numeric response must be a positive number.")]
        public int? NumericResponse { get; set; }

        public Question Question { get; set; }
        public Template Template { get; set; }

        public ICollection<OptionAnswer> OptionAnswers { get; set; } = new List<OptionAnswer>();
    }
}