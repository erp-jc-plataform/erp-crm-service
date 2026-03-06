using System;
using System.ComponentModel.DataAnnotations;
using ErpCrmService.Domain.Entities;

namespace ErpCrmService.Application.DTOs
{
    /// <summary>
    /// Product DTO for data transfer between layers
    /// </summary>
    public class ProductDto : BaseDto
    {
        [Required(ErrorMessage = "SKU is required")]
        [MaxLength(20, ErrorMessage = "SKU cannot exceed 20 characters")]
        public string SKU { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [MaxLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        public ProductCategory Category { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        public decimal UnitPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Cost must be non-negative")]
        public decimal Cost { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be non-negative")]
        public int StockQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Minimum stock must be non-negative")]
        public int MinimumStock { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Maximum stock must be positive")]
        public int MaximumStock { get; set; }

        [Required(ErrorMessage = "Unit is required")]
        [MaxLength(50, ErrorMessage = "Unit cannot exceed 50 characters")]
        public string Unit { get; set; }

        public bool IsDiscontinued { get; set; }
        public DateTime? DiscontinuedDate { get; set; }

        [MaxLength(100, ErrorMessage = "Supplier cannot exceed 100 characters")]
        public string Supplier { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Weight must be non-negative")]
        public decimal Weight { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Length must be non-negative")]
        public decimal Length { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Width must be non-negative")]
        public decimal Width { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Height must be non-negative")]
        public decimal Height { get; set; }

        // Calculated properties
        public decimal ProfitMargin { get; set; }
        public bool IsLowStock { get; set; }
        public bool IsOverstocked { get; set; }
    }

    /// <summary>
    /// Create product request DTO
    /// </summary>
    public class CreateProductDto
    {
        [Required(ErrorMessage = "SKU is required")]
        [MaxLength(20, ErrorMessage = "SKU cannot exceed 20 characters")]
        public string SKU { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [MaxLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public ProductCategory Category { get; set; }

        [Required(ErrorMessage = "Unit price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        public decimal UnitPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Cost must be non-negative")]
        public decimal Cost { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be non-negative")]
        public int StockQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Minimum stock must be non-negative")]
        public int MinimumStock { get; set; }

        [Required(ErrorMessage = "Unit is required")]
        [MaxLength(50, ErrorMessage = "Unit cannot exceed 50 characters")]
        public string Unit { get; set; }

        [MaxLength(100, ErrorMessage = "Supplier cannot exceed 100 characters")]
        public string Supplier { get; set; }
    }

    /// <summary>
    /// Update product request DTO
    /// </summary>
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [MaxLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public ProductCategory Category { get; set; }

        [Required(ErrorMessage = "Unit price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        public decimal UnitPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Cost must be non-negative")]
        public decimal Cost { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Minimum stock must be non-negative")]
        public int MinimumStock { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Maximum stock must be positive")]
        public int MaximumStock { get; set; }

        [MaxLength(100, ErrorMessage = "Supplier cannot exceed 100 characters")]
        public string Supplier { get; set; }
    }

    /// <summary>
    /// Stock adjustment DTO
    /// </summary>
    public class StockAdjustmentDto
    {
        [Required(ErrorMessage = "Product ID is required")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Adjustment quantity is required")]
        public int AdjustmentQuantity { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        [MaxLength(200, ErrorMessage = "Reason cannot exceed 200 characters")]
        public string Reason { get; set; }
    }
}