using System.Linq.Expressions;

namespace Capstone.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> GetByIdAsync(int id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task<bool> SaveChangesAsync();
}