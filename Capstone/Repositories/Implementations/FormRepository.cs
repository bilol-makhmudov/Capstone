using Capstone.Data;
using Capstone.Models;
using Capstone.Repositories.Interfaces;
using Capstone.ViewModels.Form;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Repositories.Implementations;

public class FormRepository : Repository<Template>, IFormRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IAnswerRepository _answerRepository;
    private readonly IOptionAnswerRepository _optionAnswerRepository;
    
    public FormRepository(ApplicationDbContext dbContext,
        IAnswerRepository answerRepository,
        IOptionAnswerRepository optionAnswerRepository) : base(dbContext)
    {
        _context = dbContext;
        _answerRepository = answerRepository;
        _optionAnswerRepository = optionAnswerRepository;
    }

    public FormGetViewModel GetFormFillViewModel(Guid templateId)
    {
        var template = _context.Templates.Where(t => t.IsDeleted != true && t.Id == templateId)
            .Include(t => t.TemplateTags)
            .ThenInclude(t => t.Tag)
            .Include(t => t.Questions)
            .ThenInclude(q => q.QuestionOptions)
            .Include(t => t.Topic)
            .FirstOrDefault();

        if (template is null)
        {
            return new FormGetViewModel();
        }
        
        return new FormGetViewModel
            {
                TemplateId = template.Id,
                Title = template.Title,
                Description = template.Description,
                Topic = template.Topic,
                ImageUrl = template.ImageUrl,
                TemplateTags = template.TemplateTags.ToList(),
                Questions = template.Questions.OrderBy(q => q.Order).ToList(),
            };
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