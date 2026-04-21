using ErpCrmService.Domain.Entities;
using ErpCrmService.Domain.Repositories;
using ErpCrmService.Domain.Services;

namespace ErpCrmService.Infrastructure.Services;

public class CustomerDomainService : ICustomerDomainService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerDomainService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
    }

    public async Task<bool> CanCreateCustomerAsync(string email, string? taxId)
    {
        if (await _customerRepository.EmailExistsAsync(email))
            return false;

        if (!string.IsNullOrWhiteSpace(taxId) && await _customerRepository.TaxIdExistsAsync(taxId))
            return false;

        return true;
    }

    public Task<bool> ValidateBusinessRulesForCustomerCreationAsync(Customer customer) =>
        Task.FromResult(true);
}
