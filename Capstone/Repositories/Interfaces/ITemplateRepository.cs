using Capstone.Models;
using Capstone.ViewModels.Template;

namespace Capstone.Repositories.Interfaces;

public interface ITemplateRepository : IRepository<Template>
{
    Task<bool> CreateTemplate(CreateTemplateViewModel createTemplateViewModel, Guid userId);
    Task<CreateTemplateViewModel> GetCreateTemplateViewModelAsync(CreateTemplateViewModel? existingModel = null);
    Task<EditTemplateViewModel> GetTemplateForEditAsync(Guid templateId, Guid userId);
    Task<bool> UpdateTemplate(EditTemplateViewModel model, Guid userId);
}