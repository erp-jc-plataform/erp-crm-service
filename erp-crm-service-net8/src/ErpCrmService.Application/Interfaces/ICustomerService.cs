using ErpCrmService.Application.DTOs;

namespace ErpCrmService.Application.Interfaces;

public interface ICustomerService
{
    Task<CustomerDto?> GetByIdAsync(Guid id);
    Task<CustomerDto?> GetByEmailAsync(string email);
    Task<IEnumerable<CustomerDto>> GetAllAsync();
    Task<IEnumerable<CustomerDto>> GetActiveAsync();
    Task<IEnumerable<CustomerDto>> SearchByCompanyAsync(string companyName);
    Task<CustomerDto> CreateAsync(CreateCustomerDto dto);
    Task<CustomerDto?> UpdateAsync(Guid id, UpdateCustomerDto dto);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> EmailExistsAsync(string email);
    Task<decimal> GetTotalBalanceAsync();
    Task<int> GetActiveCustomersCountAsync();
}
