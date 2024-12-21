using Capstone.Models;

namespace Capstone.Repositories.Interfaces;

public interface ITagRepository : IRepository<Tag>
{
    Task<List<Tag>> SearchTagsAsync(string searchString);
    
}