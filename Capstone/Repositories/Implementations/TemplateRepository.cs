using Capstone.Data;
using Capstone.Models;
using Capstone.Repositories.Interfaces;
using Capstone.ViewModels.Template;

namespace Capstone.Repositories.Implementations;

public class TemplateRepository(
    ApplicationDbContext context,
    ITopicRepository topicRepository,
    IImageService imageService)
    : Repository<Template>(context), ITemplateRepository
{
    public async Task<bool> CreateTemplate(CreateTemplateViewModel createTemplateViewModel, Guid userId)
    {
        string? imageUrl = null;
        if (createTemplateViewModel.Image.Length > 0)
        {
            imageUrl = await imageService.UploadImageAsync(createTemplateViewModel.Image);
        }
            
        var template = new Template
        {
            Title = createTemplateViewModel.Title,
            Description = createTemplateViewModel.Description,
            TopicId = createTemplateViewModel.TopicId,
            IsPublic = createTemplateViewModel.IsPublic,
            ImageUrl = imageUrl,
            UserId = userId
        };
            
        foreach (var tagId in createTemplateViewModel.SelectedTagIds)
        {
            template.TemplateTags.Add(new TemplateTag
            {
                TagId = tagId
            });
        }
            
        foreach (var questionVm in createTemplateViewModel.Questions.OrderBy(q => q.Order))
        {
            if (questionVm.Image?.Length > 0)
            {
                imageUrl = await imageService.UploadImageAsync(questionVm.Image);
            }
            
            var question = new Question
            {
                QuestionText = questionVm.QuestionText,
                Description = questionVm.Description,
                QuestionType = questionVm.Type,
                Order = questionVm.Order,
                ImageUrl = imageUrl,
            };
            template.Questions.Add(question);
        }
        
        await AddAsync(template);
        return await SaveChangesAsync();
    }
    
    public async Task<CreateTemplateViewModel> GetCreateTemplateViewModelAsync(CreateTemplateViewModel? existingModel = null)
    {
        var topics = await topicRepository.GetAllAsync();
        var viewModel = existingModel ?? new CreateTemplateViewModel();
        viewModel.Topics = topics.ToList();

        return viewModel;
    }
}