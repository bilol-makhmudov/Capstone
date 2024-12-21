using Capstone.Data;
using Capstone.Models;
using Capstone.Repositories.Interfaces;

namespace Capstone.Repositories.Implementations;

public class TopicRepository : Repository<Topic>, ITopicRepository
{
    public TopicRepository(ApplicationDbContext context) : base(context)
    {
        
    }
}