using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErpCrmService.Application.DTOs;
using ErpCrmService.Application.Interfaces;
using ErpCrmService.Application.Mappers;
using ErpCrmService.Domain.Entities;
using ErpCrmService.Domain.Exceptions;
using ErpCrmService.Domain.Repositories;
using ErpCrmService.Domain.Services;

namespace ErpCrmService.Application.Services
{
    /// <summary>
    /// Customer application service implementation
    /// Orchestrates customer-related use cases and business logic
    /// Follows Single Responsibility and Dependency Inversion principles
    /// </summary>
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

        public async Task<CustomerDto> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Customer ID cannot be empty", nameof(id));

            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                throw new EntityNotFoundException(nameof(Customer), id.ToString());

            return _customerMapper.ToDto(customer);
        }

        public async Task<CustomerDto> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));

            var customer = await _customerRepository.GetByEmailAsync(email);
            if (customer == null)
                throw new EntityNotFoundException(nameof(Customer), email);

            return _customerMapper.ToDto(customer);
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
            if (string.IsNullOrWhiteSpace(companyName))
                throw new ArgumentException("Company name cannot be null or empty", nameof(companyName));

            var customers = await _customerRepository.SearchByCompanyNameAsync(companyName);
            return customers.Select(_customerMapper.ToDto);
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerDto createCustomerDto)
        {
            if (createCustomerDto == null)
                throw new ArgumentNullException(nameof(createCustomerDto));

            // Validate business rules
            if (!await _customerDomainService.CanCreateCustomerAsync(createCustomerDto.Email, createCustomerDto.TaxId))
                throw new BusinessRuleViolationException(
                    "DuplicateCustomer", 
                    "A customer with this email or tax ID already exists");

            // Map DTO to domain entity
            var customer = _customerMapper.ToEntity(createCustomerDto);

            // Validate additional business rules
            if (!await _customerDomainService.ValidateBusinessRulesForCustomerCreationAsync(customer))
                throw new BusinessRuleViolationException(
                    "CustomerCreationRules", 
                    "Customer creation violates business rules");

            // Save the customer
            var createdCustomer = await _customerRepository.AddAsync(customer);
            await _customerRepository.SaveChangesAsync();

            return _customerMapper.ToDto(createdCustomer);
        }

        public async Task<CustomerDto> UpdateAsync(Guid id, UpdateCustomerDto updateCustomerDto)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Customer ID cannot be empty", nameof(id));

            if (updateCustomerDto == null)
                throw new ArgumentNullException(nameof(updateCustomerDto));

            var existingCustomer = await _customerRepository.GetByIdAsync(id);
            if (existingCustomer == null)
                throw new EntityNotFoundException(nameof(Customer), id.ToString());

            // Check if email is changing and if new email already exists
            if (existingCustomer.Email.Value != updateCustomerDto.Email)
            {
                if (await _customerRepository.EmailExistsAsync(updateCustomerDto.Email))
                    throw new EntityAlreadyExistsException(nameof(Customer), "Email", updateCustomerDto.Email);
            }

            // Update the customer using domain methods
            _customerMapper.UpdateEntity(existingCustomer, updateCustomerDto);

            var updatedCustomer = await _customerRepository.UpdateAsync(existingCustomer);
            await _customerRepository.SaveChangesAsync();

            return _customerMapper.ToDto(updatedCustomer);
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Customer ID cannot be empty", nameof(id));

            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                throw new EntityNotFoundException(nameof(Customer), id.ToString());

            await _customerRepository.DeleteAsync(id);
            await _customerRepository.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _customerRepository.ExistsAsync(id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _customerRepository.EmailExistsAsync(email);
        }

        public async Task<decimal> GetTotalBalanceAsync()
        {
            return await _customerRepository.GetTotalBalanceAsync();
        }

        public async Task<int> GetActiveCustomersCountAsync()
        {
            return await _customerRepository.GetActiveCustomersCountAsync();
        }
    }
}