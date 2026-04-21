using ErpCrmService.Domain.Entities;

namespace ErpCrmService.Domain.Repositories;

public interface ICustomerRepository : IQueryRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email);
    Task<IEnumerable<Customer>> GetByStatusAsync(CustomerStatus status);
    Task<IEnumerable<Customer>> SearchByCompanyNameAsync(string companyName);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> TaxIdExistsAsync(string taxId);
    Task<decimal> GetTotalBalanceAsync();
    Task<int> GetActiveCustomersCountAsync();
}
