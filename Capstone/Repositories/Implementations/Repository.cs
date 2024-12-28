using System.Linq.Expressions;
using Capstone.Data;
using Capstone.Models;
using Capstone.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Repositories.Implementations;

public class Repository<T> : IRepository<T> where T : class, IBaseEntity
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly DbSet<T> _entities;

    protected Repository(ApplicationDbContext context)
    {
        _applicationDbContext = context;
        _entities = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _entities.Where(e => e.IsDeleted != true).ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _entities.Where(predicate).ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        var entity = await _entities.FindAsync(id);
        return entity != null && entity.IsDeleted != true ? entity : null;
    }

    public async Task AddAsync(T entity)
    {
        await _entities.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _entities.Update(entity);
    }

    public void Remove(T entity)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _entities.Update(entity);
    }
    public async Task<bool> SaveChangesAsync()
    {
        return await _applicationDbContext.SaveChangesAsync() > 0;
    }
}