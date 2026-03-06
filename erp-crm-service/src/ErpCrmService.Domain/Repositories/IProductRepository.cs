using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ErpCrmService.Domain.Entities;

namespace ErpCrmService.Domain.Repositories
{
    /// <summary>
    /// Product repository interface with product-specific business operations
    /// Extends the generic repository with product-specific queries
    /// </summary>
    public interface IProductRepository : IQueryRepository<Product>
    {
        Task<Product> GetBySkuAsync(string sku);
        Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category);
        Task<IEnumerable<Product>> GetLowStockProductsAsync();
        Task<IEnumerable<Product>> GetOverstockedProductsAsync();
        Task<IEnumerable<Product>> GetDiscontinuedProductsAsync();
        Task<IEnumerable<Product>> SearchByNameAsync(string name);
        Task<IEnumerable<Product>> GetBySupplierAsync(string supplier);
        Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<bool> SkuExistsAsync(string sku);
        Task<decimal> GetTotalInventoryValueAsync();
        Task<int> GetTotalStockQuantityAsync();
        Task<IEnumerable<Product>> GetProductsRequiringRestockAsync();
    }
}