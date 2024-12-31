using Capstone.Data;
using Capstone.Models;
using Capstone.Repositories.Interfaces;
using Capstone.ViewModels.QuestionViewModels;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Repositories.Implementations;

public class QuestionOptionRepository : Repository<QuestionOption>, IQuestionOptionRepository
{
    private readonly ApplicationDbContext _context;

    public QuestionOptionRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<QuestionOption>> UpdateQuestionOptions(List<QuestionOptionViewModel> options, Guid userId)
    {
        List<QuestionOption> updatedOptions = new List<QuestionOption>();
        var existingOptionIds = options.Where(o => o.Id != Guid.Empty).Select(o => o.Id).ToList();
        var newOptions = options.Where(o => o.Id == Guid.Empty).ToList();
        var existingOptions = await _context.QuestionOptions
            .Where(o => existingOptionIds.Contains(o.Id))
            .ToListAsync();
        
        foreach (var existingOption in existingOptions)
        {
            var updatedOptionVm = options.FirstOrDefault(o => o.Id == existingOption.Id);
            if (updatedOptionVm != null)
            {
                existingOption.OptionText = updatedOptionVm.OptionText ?? existingOption.OptionText;
                existingOption.QuestionId = existingOption.QuestionId;
                updatedOptions.Add(existingOption);
            }
            else
            {
                _context.QuestionOptions.Remove(existingOption);
            }
        }
        
        foreach (var newOptionVm in newOptions)
        {
            var newOption = new QuestionOption
            {
                Id = Guid.NewGuid(),
                QuestionId = newOptionVm.QuestionId,
                OptionText = newOptionVm.OptionText
            };

            _context.QuestionOptions.Add(newOption);
            updatedOptions.Add(newOption);
        }

        try
        {
            await _context.SaveChangesAsync();
            return updatedOptions;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            Console.WriteLine($"Concurrency error in QuestionOptionRepository: {ex.Message}");
            throw;
        }
    }
}