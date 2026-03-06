using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ErpCrmService.Domain.Repositories;
using ErpCrmService.Infrastructure.Data;

namespace ErpCrmService.Infrastructure.Repositories
{
    /// <summary>
    /// Generic repository implementation using Entity Framework
    /// Implements Repository pattern and follows DRY principle
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
    public class Repository<T> : IQueryRepository<T> where T : class
    {
        protected readonly ErpCrmDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ErpCrmDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetActiveAsync()
        {
            // Assumes entities have an IsActive property
            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.Property(parameter, "IsActive");
            var lambda = Expression.Lambda<Func<T, bool>>(property, parameter);
            
            return await _dbSet.Where(lambda).ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            _dbSet.Add(entity);
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public virtual async Task<bool> ExistsAsync(Guid id)
        {
            return await _dbSet.FindAsync(id) != null;
        }

        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public virtual async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize)
        {
            return await _dbSet
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, Expression<Func<T, bool>> predicate)
        {
            return await _dbSet
                .Where(predicate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}