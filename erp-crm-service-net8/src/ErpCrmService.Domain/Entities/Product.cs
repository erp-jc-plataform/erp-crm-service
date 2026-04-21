namespace ErpCrmService.Domain.Entities;

public class Product : BaseEntity
{
    public string SKU { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public ProductCategory Category { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Cost { get; private set; }
    public int StockQuantity { get; private set; }
    public int MinimumStock { get; private set; }
    public int MaximumStock { get; private set; }
    public string Unit { get; private set; } = default!;
    public bool IsDiscontinued { get; private set; }
    public DateTime? DiscontinuedDate { get; private set; }
    public string? Supplier { get; private set; }
    public decimal Weight { get; private set; }
    public decimal Length { get; private set; }
    public decimal Width { get; private set; }
    public decimal Height { get; private set; }

    private Product() : base() { }

    public Product(string sku, string name, string? description, ProductCategory category,
        decimal unitPrice, decimal cost, int stockQuantity, int minimumStock,
        int maximumStock, string unit, string? supplier = null) : base()
    {
        if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("SKU cannot be empty");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty");
        if (unitPrice <= 0) throw new ArgumentException("Unit price must be positive");
        if (cost < 0) throw new ArgumentException("Cost cannot be negative");
        if (stockQuantity < 0) throw new ArgumentException("Stock quantity cannot be negative");
        if (minimumStock < 0) throw new ArgumentException("Minimum stock cannot be negative");
        if (string.IsNullOrWhiteSpace(unit)) throw new ArgumentException("Unit cannot be empty");
        SKU = sku; Name = name; Description = description; Category = category;
        UnitPrice = unitPrice; Cost = cost; StockQuantity = stockQuantity;
        MinimumStock = minimumStock;
        MaximumStock = maximumStock > 0 ? maximumStock : minimumStock * 10;
        Unit = unit; Supplier = supplier; IsDiscontinued = false;
    }

    public void UpdatePricing(decimal unitPrice, decimal cost)
    {
        if (unitPrice <= 0) throw new ArgumentException("Unit price must be positive");
        if (cost < 0) throw new ArgumentException("Cost cannot be negative");
        UnitPrice = unitPrice; Cost = cost;
        MarkAsUpdated("System");
    }

    public void UpdateProductInfo(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Product name cannot be empty");
        Name = name; Description = description;
        MarkAsUpdated("System");
    }

    public void UpdateStockLimits(int minimumStock, int maximumStock)
    {
        if (minimumStock < 0) throw new ArgumentException("Minimum stock cannot be negative");
        if (maximumStock <= minimumStock) throw new ArgumentException("Maximum stock must be greater than minimum stock");
        MinimumStock = minimumStock; MaximumStock = maximumStock;
        MarkAsUpdated("System");
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive");
        StockQuantity += quantity;
        MarkAsUpdated("System");
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive");
        if (StockQuantity < quantity) throw new InvalidOperationException("Insufficient stock");
        StockQuantity -= quantity;
        MarkAsUpdated("System");
    }

    public void AdjustStock(int adjustment, string reason)
    {
        if (adjustment == 0) throw new ArgumentException("Stock adjustment cannot be zero");
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Reason is required");
        var newQuantity = StockQuantity + adjustment;
        if (newQuantity < 0) throw new InvalidOperationException("Cannot adjust stock below zero");
        StockQuantity = newQuantity;
        MarkAsUpdated("System");
    }

    public void Discontinue()
    {
        IsDiscontinued = true;
        DiscontinuedDate = DateTime.UtcNow;
        MarkAsUpdated("System");
    }

    public void UpdateSupplier(string? supplier) { Supplier = supplier; MarkAsUpdated("System"); }

    public bool IsLowStock() => StockQuantity <= MinimumStock;
    public bool IsOverstocked() => StockQuantity >= MaximumStock;
    public bool IsAvailable(int qty) => !IsDiscontinued && IsActive && StockQuantity >= qty;
    public decimal CalculateProfitMargin() => UnitPrice == 0 ? 0 : ((UnitPrice - Cost) / UnitPrice) * 100;
}

public enum ProductCategory
{
    Electronics = 1, Clothing = 2, Food = 3, Furniture = 4,
    Software = 5, Services = 6, Raw_Materials = 7, Other = 8
}
