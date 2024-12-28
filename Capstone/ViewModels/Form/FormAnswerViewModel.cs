using Capstone.Models;
using Capstone.ViewModels.QuestionViewModels;

namespace Capstone.ViewModels.Form;

public class FormAnswerViewModel
{
    public Guid TemplateId { get; set; }
    public ICollection<QuestionAnswerViewModel> Questions { get; set; } = [];
}