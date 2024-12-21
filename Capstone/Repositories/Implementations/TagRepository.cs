using Capstone.Data;
using Capstone.Models;
using Capstone.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Repositories.Implementations;

public class TagRepository : Repository<Tag>, ITagRepository
{
    private readonly ApplicationDbContext _context;
    public TagRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<List<Tag>> SearchTagsAsync(string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
        {
            return [];
        }
        
        var tags = await _context.Tags
            .Where(t => EF.Functions.ToTsVector("english", t.TagName)
                .Matches(searchString))
                .ToListAsync();
        
        return tags;
    }
}