using ErpCrmService.Application.DTOs;
using ErpCrmService.Domain.Entities;
using ErpCrmService.Domain.ValueObjects;

namespace ErpCrmService.Application.Mappers;

public interface ICustomerMapper
{
    CustomerDto ToDto(Customer customer);
    Customer ToEntity(CreateCustomerDto dto);
    void UpdateEntity(Customer customer, UpdateCustomerDto dto);
}

public interface IAddressMapper
{
    AddressDto? ToDto(Address? address);
    Address? ToValueObject(AddressDto? dto);
}

public class AddressMapper : IAddressMapper
{
    public AddressDto? ToDto(Address? address)
    {
        if (address is null) return null;
        return new AddressDto
        {
            Street = address.Street,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country
        };
    }

    public Address? ToValueObject(AddressDto? dto)
    {
        if (dto is null) return null;
        return new Address(dto.Street, dto.City, dto.State, dto.PostalCode, dto.Country);
    }
}

public class CustomerMapper : ICustomerMapper
{
    private readonly IAddressMapper _addressMapper;

    public CustomerMapper(IAddressMapper addressMapper)
    {
        _addressMapper = addressMapper ?? throw new ArgumentNullException(nameof(addressMapper));
    }

    public CustomerDto ToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            CompanyName = customer.CompanyName,
            ContactFirstName = customer.ContactFirstName,
            ContactLastName = customer.ContactLastName,
            Email = customer.Email.Value,
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
            IsActive = customer.IsActive
        };
    }

    public Customer ToEntity(CreateCustomerDto dto)
    {
        var email = new Email(dto.Email);
        var phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : new PhoneNumber(dto.Phone);
        var address = _addressMapper.ToValueObject(dto.Address);

        return new Customer(
            dto.CompanyName, dto.ContactFirstName, dto.ContactLastName,
            email, phone, address, dto.TaxId, dto.Type, dto.CreditLimit);
    }

    public void UpdateEntity(Customer customer, UpdateCustomerDto dto)
    {
        var email = new Email(dto.Email);
        var phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : new PhoneNumber(dto.Phone);
        customer.UpdateContactInfo(dto.ContactFirstName, dto.ContactLastName, email, phone);

        var updatedAddress = _addressMapper.ToValueObject(dto.Address);
        if (updatedAddress is not null)
            customer.UpdateAddress(updatedAddress);

        customer.UpdateCreditLimit(dto.CreditLimit);
    }
}
