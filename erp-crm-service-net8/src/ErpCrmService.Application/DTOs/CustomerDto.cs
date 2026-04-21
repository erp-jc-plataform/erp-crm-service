using System.ComponentModel.DataAnnotations;
using ErpCrmService.Domain.Entities;

namespace ErpCrmService.Application.DTOs;

public class BaseDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class CustomerDto : BaseDto
{
    public string CompanyName { get; set; } = default!;
    public string ContactFirstName { get; set; } = default!;
    public string ContactLastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Phone { get; set; }
    public AddressDto? Address { get; set; }
    public string? TaxId { get; set; }
    public CustomerStatus Status { get; set; }
    public CustomerType Type { get; set; }
    public string? Notes { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; }
}

public class CreateCustomerDto
{
    [Required][MaxLength(100)] public string CompanyName { get; set; } = default!;
    [Required][MaxLength(50)] public string ContactFirstName { get; set; } = default!;
    [Required][MaxLength(50)] public string ContactLastName { get; set; } = default!;
    [Required][EmailAddress] public string Email { get; set; } = default!;
    [Phone] public string? Phone { get; set; }
    public AddressDto? Address { get; set; }
    [MaxLength(20)] public string? TaxId { get; set; }
    [Required] public CustomerType Type { get; set; }
    [Range(0, double.MaxValue)] public decimal CreditLimit { get; set; } = 0;
}

public class UpdateCustomerDto
{
    [Required][MaxLength(100)] public string CompanyName { get; set; } = default!;
    [Required][MaxLength(50)] public string ContactFirstName { get; set; } = default!;
    [Required][MaxLength(50)] public string ContactLastName { get; set; } = default!;
    [Required][EmailAddress] public string Email { get; set; } = default!;
    [Phone] public string? Phone { get; set; }
    public AddressDto? Address { get; set; }
    [Range(0, double.MaxValue)] public decimal CreditLimit { get; set; }
    [MaxLength(500)] public string? Notes { get; set; }
}
