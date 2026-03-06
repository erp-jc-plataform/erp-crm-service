using ErpCrmService.Application.DTOs;
using ErpCrmService.Domain.Entities;

namespace ErpCrmService.Application.Mappers
{
    /// <summary>
    /// Customer mapper interface following Interface Segregation Principle
    /// Defines contract for mapping between Customer entities and DTOs
    /// </summary>
    public interface ICustomerMapper
    {
        CustomerDto ToDto(Customer customer);
        Customer ToEntity(CreateCustomerDto createCustomerDto);
        void UpdateEntity(Customer customer, UpdateCustomerDto updateCustomerDto);
    }

    /// <summary>
    /// Address mapper interface
    /// </summary>
    public interface IAddressMapper
    {
        AddressDto ToDto(Domain.ValueObjects.Address address);
        Domain.ValueObjects.Address ToValueObject(AddressDto addressDto);
    }
}