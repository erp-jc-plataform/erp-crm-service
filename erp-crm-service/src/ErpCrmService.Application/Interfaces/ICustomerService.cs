using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ErpCrmService.Application.DTOs;

namespace ErpCrmService.Application.Interfaces
{
    /// <summary>
    /// Customer application service interface
    /// Defines the contract for customer-related use cases
    /// Follows Interface Segregation Principle
    /// </summary>
    public interface ICustomerService
    {
        Task<CustomerDto> GetByIdAsync(Guid id);
        Task<CustomerDto> GetByEmailAsync(string email);
        Task<IEnumerable<CustomerDto>> GetAllAsync();
        Task<IEnumerable<CustomerDto>> GetActiveAsync();
        Task<IEnumerable<CustomerDto>> SearchByCompanyAsync(string companyName);
        Task<CustomerDto> CreateAsync(CreateCustomerDto createCustomerDto);
        Task<CustomerDto> UpdateAsync(Guid id, UpdateCustomerDto updateCustomerDto);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> EmailExistsAsync(string email);
        Task<decimal> GetTotalBalanceAsync();
        Task<int> GetActiveCustomersCountAsync();
    }
}