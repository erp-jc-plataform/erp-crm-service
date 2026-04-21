using ErpCrmService.Domain.Entities;

namespace ErpCrmService.Domain.Repositories;

public interface IProductRepository : IQueryRepository<Product>
{
    Task<Product?> GetBySkuAsync(string sku);
    Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category);
    Task<IEnumerable<Product>> GetLowStockProductsAsync();
    Task<IEnumerable<Product>> SearchByNameAsync(string name);
    Task<bool> SkuExistsAsync(string sku);
    Task<decimal> GetTotalInventoryValueAsync();
}
