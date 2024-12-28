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
        if (createTemplateViewModel.Image?.Length > 0)
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
            
        foreach (var questionViewModel in createTemplateViewModel.Questions.OrderBy(q => q.Order))
        {
            string? questionImageUrl = null;
            if (questionViewModel.QuestionImage?.Length > 0)
            {
                questionImageUrl = await imageService.UploadImageAsync(questionViewModel.QuestionImage);
            }
            
            var question = new Question
            {
                QuestionText = questionViewModel.QuestionText,
                Description = questionViewModel.Description,
                QuestionType = questionViewModel.Type,
                Order = questionViewModel.Order,
                ShowInResults = questionViewModel.ShowInResults,
                ImageUrl = questionImageUrl,
            };
            
            if (questionViewModel.Type == QuestionType.Checkbox)
            {
                foreach (var optionVm in questionViewModel.QuestionOptions)
                {
                    question.QuestionOptions.Add(new QuestionOption
                    {
                        OptionText = optionVm.OptionText,
                    });
                }
            }
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