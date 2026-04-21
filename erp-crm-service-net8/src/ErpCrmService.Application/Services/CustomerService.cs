using ErpCrmService.Application.DTOs;
using ErpCrmService.Application.Interfaces;
using ErpCrmService.Application.Mappers;
using ErpCrmService.Domain.Entities;
using ErpCrmService.Domain.Exceptions;
using ErpCrmService.Domain.Repositories;
using ErpCrmService.Domain.Services;

namespace ErpCrmService.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerDomainService _customerDomainService;
    private readonly ICustomerMapper _customerMapper;

    public CustomerService(
        ICustomerRepository customerRepository,
        ICustomerDomainService customerDomainService,
        ICustomerMapper customerMapper)
    {
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        _customerDomainService = customerDomainService ?? throw new ArgumentNullException(nameof(customerDomainService));
        _customerMapper = customerMapper ?? throw new ArgumentNullException(nameof(customerMapper));
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        return customer is null ? null : _customerMapper.ToDto(customer);
    }

    public async Task<CustomerDto?> GetByEmailAsync(string email)
    {
        var customer = await _customerRepository.GetByEmailAsync(email);
        return customer is null ? null : _customerMapper.ToDto(customer);
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        return customers.Select(_customerMapper.ToDto);
    }

    public async Task<IEnumerable<CustomerDto>> GetActiveAsync()
    {
        var customers = await _customerRepository.GetActiveAsync();
        return customers.Select(_customerMapper.ToDto);
    }

    public async Task<IEnumerable<CustomerDto>> SearchByCompanyAsync(string companyName)
    {
        var customers = await _customerRepository.SearchByCompanyNameAsync(companyName);
        return customers.Select(_customerMapper.ToDto);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
    {
        if (!await _customerDomainService.CanCreateCustomerAsync(dto.Email, dto.TaxId))
            throw new BusinessRuleViolationException("DuplicateCustomer", "A customer with this email or tax ID already exists");

        var customer = _customerMapper.ToEntity(dto);

        if (!await _customerDomainService.ValidateBusinessRulesForCustomerCreationAsync(customer))
            throw new BusinessRuleViolationException("CustomerCreationRules", "Customer creation violates business rules");

        await _customerRepository.AddAsync(customer);
        await _customerRepository.SaveChangesAsync();
        return _customerMapper.ToDto(customer);
    }

    public async Task<CustomerDto?> UpdateAsync(Guid id, UpdateCustomerDto dto)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer is null) return null;

        if (!string.Equals(customer.Email.Value, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (await _customerRepository.EmailExistsAsync(dto.Email))
                throw new EntityAlreadyExistsException(nameof(Customer), "Email", dto.Email);
        }

        _customerMapper.UpdateEntity(customer, dto);
        await _customerRepository.UpdateAsync(customer);
        await _customerRepository.SaveChangesAsync();
        return _customerMapper.ToDto(customer);
    }

    public async Task DeleteAsync(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer is null) throw new EntityNotFoundException(nameof(Customer), id.ToString());
        await _customerRepository.DeleteAsync(id);
        await _customerRepository.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id) => await _customerRepository.ExistsAsync(id);
    public async Task<bool> EmailExistsAsync(string email) => await _customerRepository.EmailExistsAsync(email);
    public async Task<decimal> GetTotalBalanceAsync() => await _customerRepository.GetTotalBalanceAsync();
    public async Task<int> GetActiveCustomersCountAsync() => await _customerRepository.GetActiveCustomersCountAsync();
}
