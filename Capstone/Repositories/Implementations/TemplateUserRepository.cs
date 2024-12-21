using Capstone.Data;
using Capstone.Models;
using Capstone.Repositories.Interfaces;

namespace Capstone.Repositories.Implementations;

public class TemplateUserRepository : Repository<TemplateUser>, ITemplateUserRepository
{
    private readonly ApplicationDbContext _context;
    public TemplateUserRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}