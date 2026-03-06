using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ErpCrmService.Domain.Repositories
{
    /// <summary>
    /// Generic repository interface following Repository pattern and Interface Segregation Principle
    /// Defines the contract for data access operations
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetActiveAsync();
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> CountAsync();
        Task SaveChangesAsync();
    }

    /// <summary>
    /// Generic repository interface with additional query capabilities
    /// Follows Interface Segregation Principle by separating basic and advanced operations
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IQueryRepository<T> : IRepository<T> where T : class
    {
        Task<IEnumerable<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize);
        Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, System.Linq.Expressions.Expression<Func<T, bool>> predicate);
    }
}