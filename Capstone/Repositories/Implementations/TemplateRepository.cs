using Capstone.Data;
using Capstone.Models;
using Capstone.Repositories.Interfaces;
using Capstone.ViewModels.QuestionViewModels;
using Capstone.ViewModels.Template;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Repositories.Implementations;


public class TemplateRepository : Repository<Template>, ITemplateRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IImageService _imageService;
    private readonly ITopicRepository _topicRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly UserManager<ApplicationUser> _userManager; 

    public TemplateRepository(ApplicationDbContext context,
        ITopicRepository topicRepository,
        IImageService imageService,
        IQuestionRepository questionRepository,
        UserManager<ApplicationUser> userManager) : base(context)
    {
        _context = context;
        _imageService = imageService;
        _topicRepository = topicRepository;
        _questionRepository = questionRepository;
        _userManager = userManager;
    }

    public async Task<bool> CreateTemplate(CreateTemplateViewModel createTemplateViewModel, Guid userId)
    {
        string? imageUrl = null;
        if (createTemplateViewModel.Image?.Length > 0)
        {
            imageUrl = await _imageService.UploadImageAsync(createTemplateViewModel.Image);
        }

        var template = new Template
        {
            Title = createTemplateViewModel.Title,
            Description = createTemplateViewModel.Description,
            TopicId = createTemplateViewModel.TopicId,
            IsPublic = createTemplateViewModel.IsPublic,
            TemplateUsers = createTemplateViewModel.TemplateUserIds?.Select(u => new TemplateUser{
                UserId = u,
            }).ToList(),
            ImageUrl = imageUrl,
            UserId = userId,
            RowVersion = Guid.NewGuid().ToByteArray()
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
                questionImageUrl = await _imageService.UploadImageAsync(questionViewModel.QuestionImage);
            }

            var question = new Question
            {
                QuestionText = questionViewModel.QuestionText,
                Description = questionViewModel.Description,
                QuestionType = questionViewModel.Type,
                Order = questionViewModel.Order,
                ShowInResults = questionViewModel.ShowInResults,
                ImageUrl = questionImageUrl,
                RowVersion = Guid.NewGuid().ToByteArray()
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

    public async Task<CreateTemplateViewModel> GetCreateTemplateViewModelAsync(
        CreateTemplateViewModel? existingModel = null)
    {
        var topics = await _topicRepository.GetAllAsync();
        var viewModel = existingModel ?? new CreateTemplateViewModel();
        viewModel.Topics = topics.ToList();

        return viewModel;
    }

    public async Task<EditTemplateViewModel> GetTemplateForEditAsync(Guid templateId, Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return await Task.FromResult<EditTemplateViewModel>(null);
        
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        
        var template = await _context.Templates
            .Include(t => t.Questions)
            .ThenInclude(q => q.QuestionOptions)
            .Include(t => t.TemplateTags).ThenInclude(tt => tt.Tag)
            .FirstOrDefaultAsync(t => t.Id == templateId && (isAdmin || t.UserId == userId));

        if (template == null)
        {
            return await Task.FromResult<EditTemplateViewModel>(null);
        }

        var result = new EditTemplateViewModel
        {
            Id = template.Id,
            Title = template.Title,
            Description = template.Description,
            TopicId = template.TopicId,
            IsPublic = template.IsPublic,
            SelectedTagIds = template?.TemplateTags?.Select(tt => tt.TagId).ToList(),
            ImageUrl = template?.ImageUrl,
            Tags = template?.TemplateTags?.Select(tt => new Tag
            {
                Id = tt.Tag.Id,
                TagName = tt.Tag.TagName
            }).ToList(),
            RowVersionBase64 = Convert.ToBase64String(template.RowVersion),
            Questions = template?.Questions?.Select(q => new QuestionViewModel
            {
                Id = q.Id,
                QuestionText = q.QuestionText,
                Description = q.Description,
                Type = q.QuestionType,
                Order = q.Order,
                ShowInResults = q.ShowInResults,
                QuestionImageUrl = q.ImageUrl,
                RowVersionBase64 = Convert.ToBase64String(q.RowVersion),
                QuestionOptions = q.QuestionOptions.Select(o => new QuestionOptionViewModel
                {
                    Id = o.Id,
                    QuestionId = o.QuestionId,
                    OptionText = o.OptionText
                }).ToList()
            }).OrderBy(q => q.Order).ToList()
        };
        
        return result;
    }

    public async Task<bool> UpdateTemplate(EditTemplateViewModel model, Guid userId)
    {
        var template = await _context.Templates
            .Include(t => t.Questions)
            .ThenInclude(q => q.QuestionOptions)
            .FirstOrDefaultAsync(t => t.Id == model.Id && t.UserId == userId);

        if (template == null)
        {
            return false;
        }

        template.Title = model.Title ?? template.Title;
        template.Description = model.Description ?? template.Description;
        await _context.SaveChangesAsync();

        if (model.Image?.Length > 0)
        {
            template.ImageUrl = await _imageService.UploadImageAsync(model.Image);
        }
        else
        {
            template.ImageUrl = null;
        }

        List<Question> updatedQuestions = await _questionRepository.UpdateQuestions(model, userId);

        return updatedQuestions.Any();
    }
}