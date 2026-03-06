using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ErpCrmService.Domain.Entities;

namespace ErpCrmService.Domain.Repositories
{
    /// <summary>
    /// Customer repository interface with specific business operations
    /// Extends the generic repository with customer-specific queries
    /// Follows Interface Segregation Principle
    /// </summary>
    public interface ICustomerRepository : IQueryRepository<Customer>
    {
        Task<Customer> GetByEmailAsync(string email);
        Task<Customer> GetByTaxIdAsync(string taxId);
        Task<IEnumerable<Customer>> GetByStatusAsync(CustomerStatus status);
        Task<IEnumerable<Customer>> GetByTypeAsync(CustomerType type);
        Task<IEnumerable<Customer>> SearchByCompanyNameAsync(string companyName);
        Task<IEnumerable<Customer>> GetCustomersWithLowBalanceAsync(decimal threshold);
        Task<IEnumerable<Customer>> GetCustomersOverCreditLimitAsync();
        Task<bool> EmailExistsAsync(string email);
        Task<bool> TaxIdExistsAsync(string taxId);
        Task<decimal> GetTotalBalanceAsync();
        Task<int> GetActiveCustomersCountAsync();
    }
}