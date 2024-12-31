using Capstone.Models;
using Capstone.ViewModels.QuestionViewModels;
using Capstone.ViewModels.Template;

namespace Capstone.Repositories.Interfaces;

public interface IQuestionRepository : IRepository<Question>
{
    Task<List<Question>> UpdateQuestions(EditTemplateViewModel model, Guid userId);
}