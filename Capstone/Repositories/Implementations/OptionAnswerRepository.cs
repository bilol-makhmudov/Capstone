using Capstone.Data;
using Capstone.Models;
using Capstone.Repositories.Interfaces;

namespace Capstone.Repositories.Implementations;

public class OptionAnswerRepository : Repository<OptionAnswer>, IOptionAnswerRepository
{
    public OptionAnswerRepository(ApplicationDbContext context) : base(context)
    {
        
    }
}