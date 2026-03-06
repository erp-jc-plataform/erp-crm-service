using System;
using System.ComponentModel.DataAnnotations;
using ErpCrmService.Domain.Entities;

namespace ErpCrmService.Application.DTOs
{
    /// <summary>
    /// Base DTO class with common properties
    /// Implements Data Transfer Object pattern
    /// </summary>
    public abstract class BaseDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Customer DTO for data transfer between layers
    /// Separates external representation from domain model
    /// </summary>
    public class CustomerDto : BaseDto
    {
        [Required(ErrorMessage = "Company name is required")]
        [MaxLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Contact first name is required")]
        [MaxLength(50, ErrorMessage = "Contact first name cannot exceed 50 characters")]
        public string ContactFirstName { get; set; }

        [Required(ErrorMessage = "Contact last name is required")]
        [MaxLength(50, ErrorMessage = "Contact last name cannot exceed 50 characters")]
        public string ContactLastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; }

        public AddressDto Address { get; set; }

        [MaxLength(20, ErrorMessage = "Tax ID cannot exceed 20 characters")]
        public string TaxId { get; set; }

        public CustomerStatus Status { get; set; }
        public CustomerType Type { get; set; }

        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Credit limit must be non-negative")]
        public decimal CreditLimit { get; set; }

        public decimal CurrentBalance { get; set; }
    }

    /// <summary>
    /// Create customer request DTO
    /// Contains only the required fields for customer creation
    /// </summary>
    public class CreateCustomerDto
    {
        [Required(ErrorMessage = "Company name is required")]
        [MaxLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Contact first name is required")]
        [MaxLength(50, ErrorMessage = "Contact first name cannot exceed 50 characters")]
        public string ContactFirstName { get; set; }

        [Required(ErrorMessage = "Contact last name is required")]
        [MaxLength(50, ErrorMessage = "Contact last name cannot exceed 50 characters")]
        public string ContactLastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; }

        public AddressDto Address { get; set; }

        [MaxLength(20, ErrorMessage = "Tax ID cannot exceed 20 characters")]
        public string TaxId { get; set; }

        [Required(ErrorMessage = "Customer type is required")]
        public CustomerType Type { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Credit limit must be non-negative")]
        public decimal CreditLimit { get; set; } = 0;
    }

    /// <summary>
    /// Update customer request DTO
    /// Contains fields that can be updated
    /// </summary>
    public class UpdateCustomerDto
    {
        [Required(ErrorMessage = "Company name is required")]
        [MaxLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Contact first name is required")]
        [MaxLength(50, ErrorMessage = "Contact first name cannot exceed 50 characters")]
        public string ContactFirstName { get; set; }

        [Required(ErrorMessage = "Contact last name is required")]
        [MaxLength(50, ErrorMessage = "Contact last name cannot exceed 50 characters")]
        public string ContactLastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; }

        public AddressDto Address { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Credit limit must be non-negative")]
        public decimal CreditLimit { get; set; }

        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; }
    }
}