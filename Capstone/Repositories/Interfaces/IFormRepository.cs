using Capstone.Models;
using Capstone.ViewModels.Form;

namespace Capstone.Repositories.Interfaces;

public interface IFormRepository : IRepository<Template>
{ 
    FormGetViewModel GetFormFillViewModel(Guid templateId);
    Task<bool> FormAnswer(FormAnswerViewModel formAnswer, Guid userId);
}