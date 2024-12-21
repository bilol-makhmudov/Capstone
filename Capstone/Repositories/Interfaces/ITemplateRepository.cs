using Capstone.Models;
using Capstone.ViewModels.Template;

namespace Capstone.Repositories.Interfaces;

public interface ITemplateRepository : IRepository<Template>
{
    Task<bool> CreateTemplate(CreateTemplateViewModel createTemplateViewModel, Guid userId);
    Task<CreateTemplateViewModel> GetCreateTemplateViewModelAsync(CreateTemplateViewModel? existingModel = null);
}