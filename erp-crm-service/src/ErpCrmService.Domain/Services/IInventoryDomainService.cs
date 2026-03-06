using System;
using System.Threading.Tasks;
using ErpCrmService.Domain.Entities;

namespace ErpCrmService.Domain.Services
{
    /// <summary>
    /// Domain service for inventory management business logic
    /// Handles complex business rules related to stock management and pricing
    /// </summary>
    public interface IInventoryDomainService
    {
        Task<bool> CanFulfillOrderAsync(Guid productId, int requestedQuantity);
        Task<decimal> CalculateOptimalRestockQuantityAsync(Guid productId);
        Task<PricingRecommendation> GetPricingRecommendationAsync(Guid productId);
        Task<bool> ValidateStockAdjustmentAsync(Guid productId, int adjustment, string reason);
        Task<InventoryAlert[]> GetInventoryAlertsAsync();
        Task<bool> ShouldDiscontinueProductAsync(Guid productId);
    }

    /// <summary>
    /// Pricing recommendation value object
    /// </summary>
    public class PricingRecommendation
    {
        public decimal CurrentPrice { get; set; }
        public decimal RecommendedPrice { get; set; }
        public decimal MinimumPrice { get; set; }
        public decimal MaximumPrice { get; set; }
        public string Reasoning { get; set; }
        public decimal ExpectedProfitMargin { get; set; }
    }

    /// <summary>
    /// Inventory alert value object
    /// </summary>
    public class InventoryAlert
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public InventoryAlertType AlertType { get; set; }
        public string Message { get; set; }
        public int CurrentStock { get; set; }
        public int ThresholdValue { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Inventory alert types
    /// </summary>
    public enum InventoryAlertType
    {
        LowStock = 1,
        OutOfStock = 2,
        Overstock = 3,
        DiscontinuedProduct = 4,
        SlowMoving = 5
    }
}