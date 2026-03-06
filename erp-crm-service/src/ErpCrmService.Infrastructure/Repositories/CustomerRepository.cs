using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ErpCrmService.Domain.Entities;
using ErpCrmService.Domain.Repositories;
using ErpCrmService.Infrastructure.Data;

namespace ErpCrmService.Infrastructure.Repositories
{
    /// <summary>
    /// Customer repository implementation
    /// Extends generic repository with customer-specific operations
    /// Follows Single Responsibility and Interface Segregation principles
    /// </summary>
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ErpCrmDbContext context) : base(context)
        {
        }

        public async Task<Customer> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _dbSet
                .FirstOrDefaultAsync(c => c.Email.Value.ToLower() == email.ToLower());
        }

        public async Task<Customer> GetByTaxIdAsync(string taxId)
        {
            if (string.IsNullOrWhiteSpace(taxId))
                return null;

            return await _dbSet
                .FirstOrDefaultAsync(c => c.TaxId == taxId);
        }

        public async Task<IEnumerable<Customer>> GetByStatusAsync(CustomerStatus status)
        {
            return await _dbSet
                .Where(c => c.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetByTypeAsync(CustomerType type)
        {
            return await _dbSet
                .Where(c => c.Type == type)
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> SearchByCompanyNameAsync(string companyName)
        {
            if (string.IsNullOrWhiteSpace(companyName))
                return new List<Customer>();

            return await _dbSet
                .Where(c => c.CompanyName.Contains(companyName))
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetCustomersWithLowBalanceAsync(decimal threshold)
        {
            return await _dbSet
                .Where(c => c.CurrentBalance < threshold && c.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetCustomersOverCreditLimitAsync()
        {
            return await _dbSet
                .Where(c => c.CurrentBalance > c.CreditLimit && c.IsActive)
                .ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _dbSet
                .AnyAsync(c => c.Email.Value.ToLower() == email.ToLower());
        }

        public async Task<bool> TaxIdExistsAsync(string taxId)
        {
            if (string.IsNullOrWhiteSpace(taxId))
                return false;

            return await _dbSet
                .AnyAsync(c => c.TaxId == taxId);
        }

        public async Task<decimal> GetTotalBalanceAsync()
        {
            return await _dbSet
                .Where(c => c.IsActive)
                .SumAsync(c => c.CurrentBalance);
        }

        public async Task<int> GetActiveCustomersCountAsync()
        {
            return await _dbSet
                .CountAsync(c => c.IsActive && c.Status == CustomerStatus.Active);
        }
    }
}