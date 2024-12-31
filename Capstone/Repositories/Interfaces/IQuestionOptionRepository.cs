using Capstone.Models;
using Capstone.ViewModels.QuestionViewModels;

namespace Capstone.Repositories.Interfaces;

public interface IQuestionOptionRepository : IRepository<QuestionOption>
{
    Task<List<QuestionOption>> UpdateQuestionOptions(List<QuestionOptionViewModel> options, Guid userId);
}