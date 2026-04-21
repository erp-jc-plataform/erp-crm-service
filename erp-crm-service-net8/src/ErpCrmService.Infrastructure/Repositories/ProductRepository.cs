using ErpCrmService.Domain.Entities;
using ErpCrmService.Domain.Repositories;
using ErpCrmService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ErpCrmService.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ErpCrmDbContext context) : base(context) { }

    public async Task<Product?> GetBySkuAsync(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku)) return null;
        return await _dbSet.FirstOrDefaultAsync(p => p.SKU.ToLower() == sku.ToLower());
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category) =>
        await _dbSet.Where(p => p.Category == category && p.IsActive).ToListAsync();

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync() =>
        await _dbSet.Where(p => p.IsActive && !p.IsDiscontinued && p.StockQuantity <= p.MinimumStock).ToListAsync();

    public async Task<IEnumerable<Product>> SearchByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return [];
        return await _dbSet.Where(p => p.Name.Contains(name) && p.IsActive).ToListAsync();
    }

    public async Task<bool> SkuExistsAsync(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku)) return false;
        return await _dbSet.AnyAsync(p => p.SKU.ToLower() == sku.ToLower());
    }

    public async Task<decimal> GetTotalInventoryValueAsync() =>
        await _dbSet.Where(p => p.IsActive && !p.IsDiscontinued).SumAsync(p => p.StockQuantity * p.Cost);
}
