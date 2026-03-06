using ErpCrmService.Application.DTOs;
using ErpCrmService.Domain.Entities;
using ErpCrmService.Domain.ValueObjects;

namespace ErpCrmService.Application.Mappers
{
    /// <summary>
    /// Customer mapper implementation
    /// Handles conversion between Customer entities and DTOs
    /// Follows Single Responsibility Principle
    /// </summary>
    public class CustomerMapper : ICustomerMapper
    {
        private readonly IAddressMapper _addressMapper;

        public CustomerMapper(IAddressMapper addressMapper)
        {
            _addressMapper = addressMapper ?? throw new System.ArgumentNullException(nameof(addressMapper));
        }

        public CustomerDto ToDto(Customer customer)
        {
            if (customer == null)
                return null;

            return new CustomerDto
            {
                Id = customer.Id,
                CompanyName = customer.CompanyName,
                ContactFirstName = customer.ContactFirstName,
                ContactLastName = customer.ContactLastName,
                Email = customer.Email?.Value,
                Phone = customer.Phone?.Value,
                Address = _addressMapper.ToDto(customer.Address),
                TaxId = customer.TaxId,
                Status = customer.Status,
                Type = customer.Type,
                Notes = customer.Notes,
                CreditLimit = customer.CreditLimit,
                CurrentBalance = customer.CurrentBalance,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt,
                CreatedBy = customer.CreatedBy,
                UpdatedBy = customer.UpdatedBy,
                IsActive = customer.IsActive
            };
        }

        public Customer ToEntity(CreateCustomerDto createCustomerDto)
        {
            if (createCustomerDto == null)
                return null;

            var email = new Email(createCustomerDto.Email);
            var phone = !string.IsNullOrEmpty(createCustomerDto.Phone) 
                ? new PhoneNumber(createCustomerDto.Phone) 
                : null;
            var address = _addressMapper.ToValueObject(createCustomerDto.Address);

            return new Customer(
                createCustomerDto.CompanyName,
                createCustomerDto.ContactFirstName,
                createCustomerDto.ContactLastName,
                email,
                phone,
                address,
                createCustomerDto.TaxId,
                createCustomerDto.Type,
                createCustomerDto.CreditLimit);
        }

        public void UpdateEntity(Customer customer, UpdateCustomerDto updateCustomerDto)
        {
            if (customer == null || updateCustomerDto == null)
                return;

            var email = new Email(updateCustomerDto.Email);
            var phone = !string.IsNullOrEmpty(updateCustomerDto.Phone) 
                ? new PhoneNumber(updateCustomerDto.Phone) 
                : null;

            customer.UpdateContactInfo(
                updateCustomerDto.ContactFirstName,
                updateCustomerDto.ContactLastName,
                email,
                phone);

            if (updateCustomerDto.Address != null)
            {
                var address = _addressMapper.ToValueObject(updateCustomerDto.Address);
                customer.UpdateAddress(address);
            }

            customer.UpdateCreditLimit(updateCustomerDto.CreditLimit);
        }
    }

    /// <summary>
    /// Address mapper implementation
    /// Handles conversion between Address value objects and DTOs
    /// </summary>
    public class AddressMapper : IAddressMapper
    {
        public AddressDto ToDto(Address address)
        {
            if (address == null)
                return null;

            return new AddressDto
            {
                Street = address.Street,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Country = address.Country
            };
        }

        public Address ToValueObject(AddressDto addressDto)
        {
            if (addressDto == null)
                return null;

            return new Address(
                addressDto.Street,
                addressDto.City,
                addressDto.State,
                addressDto.PostalCode,
                addressDto.Country);
        }
    }
}