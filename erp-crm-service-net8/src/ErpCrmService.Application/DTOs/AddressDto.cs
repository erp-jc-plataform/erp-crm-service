using System.ComponentModel.DataAnnotations;

namespace ErpCrmService.Application.DTOs;

public class AddressDto
{
    [Required] public string Street { get; set; } = default!;
    [Required] public string City { get; set; } = default!;
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    [Required] public string Country { get; set; } = default!;
}
