using Capstone.Data;
using Capstone.Models;
using Capstone.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Repositories.Implementations;

public class AnswerRepository : Repository<Answer>, IAnswerRepository
{
    private readonly ApplicationDbContext _context;
    public AnswerRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Guid> CreateAnswer(Answer answer)
    {
        var templateExists = await _context.Templates
            .AnyAsync(t => t.Id == answer.TemplateId && t.IsDeleted != true);
        if (!templateExists)
        {
            throw new ArgumentException("Invalid Template ID. The template does not exist or is deleted.");
        }
        
        var questionExists = await _context.Questions
            .AnyAsync(q => q.Id == answer.QuestionId && q.IsDeleted != true);
        if (!questionExists)
        {
            throw new ArgumentException("Invalid Question ID. The question does not exist or is deleted.");
        }
        await _context.Answers.AddAsync(answer);
        await _context.SaveChangesAsync();
        return answer.Id;
    }
}