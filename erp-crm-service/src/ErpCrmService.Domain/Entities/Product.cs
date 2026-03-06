using System;
using System.ComponentModel.DataAnnotations;

namespace ErpCrmService.Domain.Entities
{
    /// <summary>
    /// Product entity representing items that can be sold in the ERP system
    /// Implements business rules for inventory management and pricing
    /// </summary>
    public class Product : BaseEntity
    {
        [Required]
        [MaxLength(20)]
        public string SKU { get; private set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; private set; }

        [MaxLength(500)]
        public string Description { get; private set; }

        public ProductCategory Category { get; private set; }

        public decimal UnitPrice { get; private set; }
        public decimal Cost { get; private set; }
        public int StockQuantity { get; private set; }
        public int MinimumStock { get; private set; }
        public int MaximumStock { get; private set; }

        [MaxLength(50)]
        public string Unit { get; private set; } // e.g., "piece", "kg", "meter"

        public bool IsDiscontinued { get; private set; }
        public DateTime? DiscontinuedDate { get; private set; }

        [MaxLength(100)]
        public string Supplier { get; private set; }

        public decimal Weight { get; private set; }
        public decimal Length { get; private set; }
        public decimal Width { get; private set; }
        public decimal Height { get; private set; }

        // Private constructor for EF
        private Product() : base() { }

        public Product(
            string sku,
            string name,
            string description,
            ProductCategory category,
            decimal unitPrice,
            decimal cost,
            int stockQuantity,
            int minimumStock,
            int maximumStock,
            string unit,
            string supplier = null) : base()
        {
            ValidateConstructorParameters(sku, name, unitPrice, cost, stockQuantity, minimumStock, unit);

            SKU = sku;
            Name = name;
            Description = description;
            Category = category;
            UnitPrice = unitPrice;
            Cost = cost;
            StockQuantity = stockQuantity;
            MinimumStock = minimumStock;
            MaximumStock = maximumStock > 0 ? maximumStock : minimumStock * 10;
            Unit = unit;
            Supplier = supplier;
            IsDiscontinued = false;
        }

        public void UpdatePricing(decimal unitPrice, decimal cost)
        {
            if (unitPrice <= 0)
                throw new ArgumentException("Unit price must be positive");

            if (cost < 0)
                throw new ArgumentException("Cost cannot be negative");

            if (cost > unitPrice)
                throw new ArgumentException("Cost cannot be higher than unit price");

            UnitPrice = unitPrice;
            Cost = cost;
            MarkAsUpdated("System");
        }

        public void UpdateStock(int newQuantity, string reason)
        {
            if (newQuantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Reason for stock update is required");

            StockQuantity = newQuantity;
            MarkAsUpdated("System");
        }

        public void AdjustStock(int adjustment, string reason)
        {
            if (adjustment == 0)
                throw new ArgumentException("Stock adjustment cannot be zero");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Reason for stock adjustment is required");

            var newQuantity = StockQuantity + adjustment;
            if (newQuantity < 0)
                throw new InvalidOperationException("Cannot adjust stock below zero");

            StockQuantity = newQuantity;
            MarkAsUpdated("System");
        }

        public void UpdateStockLimits(int minimumStock, int maximumStock)
        {
            if (minimumStock < 0)
                throw new ArgumentException("Minimum stock cannot be negative");

            if (maximumStock <= minimumStock)
                throw new ArgumentException("Maximum stock must be greater than minimum stock");

            MinimumStock = minimumStock;
            MaximumStock = maximumStock;
            MarkAsUpdated("System");
        }

        public void UpdateStockLevels(int minimumStock, int maximumStock)
        {
            UpdateStockLimits(minimumStock, maximumStock);
        }

        public void UpdateProductInfo(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty");

            Name = name;
            Description = description;
            MarkAsUpdated("System");
        }

        public void UpdateSupplier(string supplier)
        {
            Supplier = supplier;
            MarkAsUpdated("System");
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive");

            StockQuantity += quantity;
            MarkAsUpdated("System");
        }

        public void RemoveStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive");

            if (StockQuantity < quantity)
                throw new InvalidOperationException("Insufficient stock");

            StockQuantity -= quantity;
            MarkAsUpdated("System");
        }

        public void Discontinue()
        {
            if (IsDiscontinued)
                throw new InvalidOperationException("Product is already discontinued");

            IsDiscontinued = true;
            DiscontinuedDate = DateTime.UtcNow;
            MarkAsUpdated("System");
        }

        public void Activate()
        {
            IsActive = true;
            MarkAsUpdated("System");
        }

        public void Deactivate()
        {
            IsActive = false;
            MarkAsUpdated("System");
        }

        public void DiscontinueProduct(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Reason for discontinuation is required");

            IsDiscontinued = true;
            DiscontinuedDate = DateTime.UtcNow;
            Description = $"{Description} - DISCONTINUED: {reason}";
            MarkAsUpdated("System");
        }

        public void UpdateDimensions(decimal weight, decimal length, decimal width, decimal height)
        {
            if (weight < 0 || length < 0 || width < 0 || height < 0)
                throw new ArgumentException("Dimensions cannot be negative");

            Weight = weight;
            Length = length;
            Width = width;
            Height = height;
            MarkAsUpdated("System");
        }

        public bool IsLowStock()
        {
            return StockQuantity <= MinimumStock;
        }

        public bool IsOverstocked()
        {
            return StockQuantity >= MaximumStock;
        }

        public bool IsAvailable(int requestedQuantity)
        {
            return !IsDiscontinued && 
                   IsActive && 
                   StockQuantity >= requestedQuantity;
        }

        public decimal CalculateProfit()
        {
            return UnitPrice - Cost;
        }

        public decimal CalculateProfitMargin()
        {
            return UnitPrice == 0 ? 0 : ((UnitPrice - Cost) / UnitPrice) * 100;
        }

        private void ValidateConstructorParameters(string sku, string name, decimal unitPrice, decimal cost, int stockQuantity, int minimumStock, string unit)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("SKU cannot be empty");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty");

            if (unitPrice <= 0)
                throw new ArgumentException("Unit price must be positive");

            if (cost < 0)
                throw new ArgumentException("Cost cannot be negative");

            if (cost > unitPrice)
                throw new ArgumentException("Cost cannot be higher than unit price");

            if (stockQuantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative");

            if (minimumStock < 0)
                throw new ArgumentException("Minimum stock cannot be negative");

            if (string.IsNullOrWhiteSpace(unit))
                throw new ArgumentException("Unit cannot be empty");
        }
    }

    public enum ProductCategory
    {
        Electronics = 1,
        Clothing = 2,
        Food = 3,
        Books = 4,
        HomeAndGarden = 5,
        Sports = 6,
        Automotive = 7,
        Health = 8,
        Services = 9,
        Other = 99
    }
}