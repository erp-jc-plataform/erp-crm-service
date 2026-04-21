using System.Linq.Expressions;
using ErpCrmService.Domain.Repositories;
using ErpCrmService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ErpCrmService.Infrastructure.Repositories;

public class Repository<T> : IQueryRepository<T> where T : class
{
    protected readonly ErpCrmDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ErpCrmDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);
    public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public virtual async Task<IEnumerable<T>> GetActiveAsync()
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "e");
        var property = System.Linq.Expressions.Expression.Property(parameter, "IsActive");
        var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(property, parameter);
        return await _dbSet.Where(lambda).ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity) { await _dbSet.AddAsync(entity); return entity; }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return entity;
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null) _dbSet.Remove(entity);
    }

    public virtual async Task<bool> ExistsAsync(Guid id) => await _dbSet.FindAsync(id) != null;
    public virtual async Task<int> CountAsync() => await _dbSet.CountAsync();
    public virtual async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
        await _dbSet.Where(predicate).ToListAsync();

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate) =>
        await _dbSet.FirstOrDefaultAsync(predicate);

    public virtual async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize) =>
        await _dbSet.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

    public virtual async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, Expression<Func<T, bool>> predicate) =>
        await _dbSet.Where(predicate).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
}
