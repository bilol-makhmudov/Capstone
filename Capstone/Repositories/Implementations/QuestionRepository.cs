using Capstone.Data;
using Capstone.Models;
using Capstone.Repositories.Interfaces;
using Capstone.ViewModels.QuestionViewModels;
using Capstone.ViewModels.Template;
using Microsoft.EntityFrameworkCore;


namespace Capstone.Repositories.Implementations;

public class QuestionRepository : Repository<Question>, IQuestionRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IImageService _imageService;
    private readonly IQuestionOptionRepository _questionOptionRepository;
    public QuestionRepository(ApplicationDbContext context,
        IImageService imageService,
        IQuestionOptionRepository questionOptionRepository) : base(context)
    {
        _context = context;
        _imageService = imageService;
        _questionOptionRepository = questionOptionRepository;
    }

    public async Task<List<Question>> UpdateQuestions(EditTemplateViewModel model, Guid userId)
    {
        List<Question> updatedQuestions = new List<Question>();
        if (model.Questions?.Count > 0)
        {
            List<QuestionViewModel> newQuestions = model.Questions.Where(q => q.Id == Guid.Empty).ToList();
        
            List<Guid> existingQuestionIds = model.Questions
                .Where(q => q.Id != Guid.Empty)
                .Select(q => q.Id)
                .ToList();
            
            List<Question> existingQuestions = await _context
                .Questions
                .Where(cq => existingQuestionIds.Contains(cq.Id))
                .Include(q => q.QuestionOptions)
                .ToListAsync();

            foreach (Question existingQuestion in existingQuestions)
            {
                var updatedQuestion = model.Questions.FirstOrDefault(q => q.Id == existingQuestion.Id);
                if (updatedQuestion?.QuestionImage != null)
                {
                    updatedQuestion.QuestionImageUrl = await _imageService.UploadImageAsync(updatedQuestion.QuestionImage);
                }
                

                if (updatedQuestion != null)
                {
                    existingQuestion.QuestionText = updatedQuestion.QuestionText ?? existingQuestion.QuestionText;
                    existingQuestion.Description = updatedQuestion.Description ?? existingQuestion.Description;
                    existingQuestion.Order = updatedQuestion.Order;
                    existingQuestion.ShowInResults = updatedQuestion.ShowInResults;
                    existingQuestion.QuestionType = updatedQuestion.Type;
                    existingQuestion.ImageUrl = updatedQuestion.QuestionImageUrl;
                    existingQuestion.RowVersion = Guid.NewGuid().ToByteArray();
                    
                    if (existingQuestion.QuestionType == QuestionType.Checkbox && updatedQuestion.QuestionOptions != null)
                    {
                        foreach (var option in updatedQuestion.QuestionOptions)
                        {
                            option.QuestionId = existingQuestion.Id;
                        }
                        
                        var updatedOptions = await _questionOptionRepository.UpdateQuestionOptions(
                            updatedQuestion.QuestionOptions.ToList(),
                            userId);
                        existingQuestion.QuestionOptions = updatedOptions;
                    }
                    else
                    {
                        if (existingQuestion.QuestionOptions.Count > 0)
                        {
                            _context.QuestionOptions.RemoveRange(existingQuestion.QuestionOptions);
                        }
                    }
                    
                    updatedQuestions.Add(existingQuestion);
                }
                else
                {
                    _context.Questions.Remove(existingQuestion);
                }
            }

            foreach (var newQuestion in newQuestions)
            {
                string? questionImageUrl = null;

                if (newQuestion.QuestionImage != null && newQuestion.QuestionImage.Length > 0)
                {
                    questionImageUrl = await _imageService.UploadImageAsync(newQuestion.QuestionImage);
                }
                
                var question = new Question
                {
                    QuestionText = newQuestion.QuestionText,
                    Description = newQuestion.Description,
                    Order = newQuestion.Order,
                    ShowInResults = newQuestion.ShowInResults,
                    QuestionType = newQuestion.Type,
                    ImageUrl = questionImageUrl,
                    TemplateId = model.Id,
                    RowVersion = Guid.NewGuid().ToByteArray()
                };
                
                _context.Questions.Add(question);
                await _context.SaveChangesAsync();
                
                if (newQuestion.Type == QuestionType.Checkbox && newQuestion.QuestionOptions != null)
                {
                    var newOptionVms = newQuestion.QuestionOptions.ToList();
                    foreach (var optionVm in newOptionVms)
                    {
                        optionVm.QuestionId = question.Id;
                    }
                    var newOptions = await _questionOptionRepository.UpdateQuestionOptions(newOptionVms, userId);
                }
                updatedQuestions.Add(question);
            }
            await _context.SaveChangesAsync();
        }
        return updatedQuestions;
    }
}