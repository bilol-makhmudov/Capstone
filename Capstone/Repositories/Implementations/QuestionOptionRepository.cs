using Capstone.Data;
using Capstone.Models;
using Capstone.Repositories.Interfaces;

namespace Capstone.Repositories.Implementations;

public class QuestionOptionRepository : Repository<QuestionOption>, IQuestionOptionRepository
{
    public QuestionOptionRepository(ApplicationDbContext context) : base(context)
    {
        
    }
}