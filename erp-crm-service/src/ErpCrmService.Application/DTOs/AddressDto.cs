using System.ComponentModel.DataAnnotations;

namespace ErpCrmService.Application.DTOs
{
    /// <summary>
    /// Address DTO for data transfer
    /// Represents address information in API contracts
    /// </summary>
    public class AddressDto
    {
        [Required(ErrorMessage = "Street is required")]
        [MaxLength(200, ErrorMessage = "Street cannot exceed 200 characters")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City is required")]
        [MaxLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string City { get; set; }

        [MaxLength(100, ErrorMessage = "State cannot exceed 100 characters")]
        public string State { get; set; }

        [MaxLength(20, ErrorMessage = "Postal code cannot exceed 20 characters")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [MaxLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
        public string Country { get; set; }
    }
}