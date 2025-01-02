using Capstone.Models;
using Capstone.ViewModels.Form;

namespace Capstone.Repositories.Interfaces;

public interface IFormRepository : IRepository<Template>
{ 
    Task<FormGetViewModel> GetFormFillViewModel(Guid templateId, Guid userId);
    Task<bool> FormAnswer(FormAnswerViewModel formAnswer, Guid userId);
}