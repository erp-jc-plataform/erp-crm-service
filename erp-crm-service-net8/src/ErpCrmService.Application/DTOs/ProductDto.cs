using System.ComponentModel.DataAnnotations;
using ErpCrmService.Domain.Entities;

namespace ErpCrmService.Application.DTOs;

public class ProductDto : BaseDto
{
    public string SKU { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public ProductCategory Category { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Cost { get; set; }
    public int StockQuantity { get; set; }
    public int MinimumStock { get; set; }
    public int MaximumStock { get; set; }
    public string Unit { get; set; } = default!;
    public bool IsDiscontinued { get; set; }
    public DateTime? DiscontinuedDate { get; set; }
    public string? Supplier { get; set; }
    public decimal ProfitMargin { get; set; }
    public bool IsLowStock { get; set; }
}

public class CreateProductDto
{
    [Required][MaxLength(20)] public string SKU { get; set; } = default!;
    [Required][MaxLength(100)] public string Name { get; set; } = default!;
    [MaxLength(500)] public string? Description { get; set; }
    [Required] public ProductCategory Category { get; set; }
    [Required][Range(0.01, double.MaxValue)] public decimal UnitPrice { get; set; }
    [Range(0, double.MaxValue)] public decimal Cost { get; set; }
    [Range(0, int.MaxValue)] public int StockQuantity { get; set; }
    [Range(0, int.MaxValue)] public int MinimumStock { get; set; }
    [Required][MaxLength(50)] public string Unit { get; set; } = default!;
    [MaxLength(100)] public string? Supplier { get; set; }
}

public class UpdateProductDto
{
    [Required][MaxLength(100)] public string Name { get; set; } = default!;
    [MaxLength(500)] public string? Description { get; set; }
    [Required] public ProductCategory Category { get; set; }
    [Required][Range(0.01, double.MaxValue)] public decimal UnitPrice { get; set; }
    [Range(0, double.MaxValue)] public decimal Cost { get; set; }
    [Range(0, int.MaxValue)] public int MinimumStock { get; set; }
    [Range(1, int.MaxValue)] public int MaximumStock { get; set; }
    [MaxLength(100)] public string? Supplier { get; set; }
}

public class StockAdjustmentDto
{
    [Required] public int AdjustmentQuantity { get; set; }
    [Required][MaxLength(200)] public string Reason { get; set; } = default!;
}
