using Capstone.Data;
using Capstone.Models;
using Capstone.Repositories.Interfaces;
using Capstone.ViewModels.Form;
using Capstone.ViewModels.QuestionViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Repositories.Implementations;

public class FormRepository : Repository<Template>, IFormRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IAnswerRepository _answerRepository;
    private readonly IOptionAnswerRepository _optionAnswerRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    public FormRepository(ApplicationDbContext dbContext,
        IAnswerRepository answerRepository,
        IOptionAnswerRepository optionAnswerRepository,
        UserManager<ApplicationUser> userManager) : base(dbContext)
    {
        _context = dbContext;
        _answerRepository = answerRepository;
        _optionAnswerRepository = optionAnswerRepository;
        _userManager = userManager;
    }

    public async Task<FormGetViewModel> GetFormFillViewModel(Guid templateId, Guid userId)
    {
        var template = _context.Templates.Where(t => t.IsDeleted != true && t.Id == templateId)
            .Include(t => t.TemplateUsers)
            .Include(t => t.TemplateTags)
            .ThenInclude(t => t.Tag)
            .Include(t => t.Questions)
            .ThenInclude(q => q.QuestionOptions)
            .Include(t => t.Topic)
            .FirstOrDefault();

        var user = await _userManager.FindByIdAsync(userId.ToString());
        
        var isAdmin =  await _userManager.IsInRoleAsync(user, "Admin");
        
        if (template.IsPublic != true && !isAdmin && !template.TemplateUsers
                .Select(u => u.UserId).Contains(userId))
        {
            return new FormGetViewModel();
        }
        
        if (template is null)
        {
            return new FormGetViewModel();
        }
        
        var existingAnswers = await _context.Answers
            .Where(a => a.TemplateId == templateId && a.UserId == userId)
            .Include(a => a.OptionAnswers)
            .ToListAsync();

        if (existingAnswers.Any())
        {
            var prefilledQuestions = template.Questions
                .OrderBy(q => q.Order)
                .Select(q =>
                {
                    var existingAnswer = existingAnswers.FirstOrDefault(a => a.QuestionId == q.Id);
                    return new QuestionFormFillViewModel
                    {
                        Id = q.Id,
                        QuestionText = q.QuestionText,
                        Description = q.Description,
                        QuestionType = q.QuestionType,
                        QuestionOptions = q.QuestionOptions.Select(o => new QuestionOptionViewModel
                        {
                            Id = o.Id,
                            QuestionId = o.QuestionId,
                            OptionText = o.OptionText,
                            IsSelected = existingAnswer?.OptionAnswers.Any(oa => oa.QuestionOptionId == o.Id) ?? false
                        }).ToList(),
                        StringResponse = existingAnswer?.StringResponse,
                        NumericAnswer = existingAnswer?.NumericResponse,
                        ImageUrl = q.ImageUrl
                    };
                }).ToList();
            
            return new FormGetViewModel
            {
                TemplateId = template.Id,
                Title = template.Title,
                Description = template.Description,
                TopicName = template.Topic?.TopicName,
                ImageUrl = template.ImageUrl,
                TagNames = template.TemplateTags.Select(tt => tt.Tag.TagName).ToList(),
                Questions = prefilledQuestions,
                IsAlreadyAnswered = true
            };
        }

        List<QuestionFormFillViewModel> questions = template
            .Questions
            .OrderBy(q => q.Order)
            .Select(q => new QuestionFormFillViewModel
            {
                Id = q.Id,
                QuestionText = q.QuestionText,
                Description = q.Description,
                QuestionType = q.QuestionType,
                QuestionOptions = q.QuestionOptions
                    .OrderBy(o => o.OptionText)
                    .Select(o => new QuestionOptionViewModel
                {
                    Id = o.Id,
                    QuestionId = o.QuestionId,
                    OptionText = o.OptionText,
                    
                }).ToList(),
                Order = q.Order,
                ImageUrl = q.ImageUrl,
            }).ToList();
        
       var result =  new FormGetViewModel
            {
                TemplateId = template.Id,
                Title = template.Title,
                Description = template.Description,
                TopicName = template.Topic?.TopicName,
                ImageUrl = template.ImageUrl,
                TagNames = template?.TemplateTags?.Select(tt => tt.Tag.TagName).ToList(),
                Questions = questions
            };  
            return result;
    }

    public async Task<bool> FormAnswer(FormAnswerViewModel formAnswer, Guid userId)
    {
        foreach (var question in formAnswer.Questions)
        {
            Answer answer = new Answer
            {
                TemplateId = formAnswer.TemplateId,
                QuestionId = question.QuestionId,
                UserId = userId,
                StringResponse = question.StringResponse,
                NumericResponse = question.NumericAnswer,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                UpdatedAt = DateTime.UtcNow,
            };
            Guid? answerId = await _answerRepository.CreateAnswer(answer);
            
            if (question.SelectedOptionIds != null && answerId != Guid.Empty)
            {
                foreach (var optionAnswer in question.SelectedOptionIds)
                {
                    await _optionAnswerRepository.AddAsync(new OptionAnswer
                    {
                        AnswerId = answerId!.Value,
                        QuestionOptionId = optionAnswer,
                    });
                    await _optionAnswerRepository.SaveChangesAsync();
                }
            }
        }

        return true;
    }
}