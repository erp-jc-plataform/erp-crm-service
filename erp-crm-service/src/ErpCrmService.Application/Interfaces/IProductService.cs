using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ErpCrmService.Application.DTOs;
using ErpCrmService.Domain.Entities;

namespace ErpCrmService.Application.Interfaces
{
    /// <summary>
    /// Product application service interface
    /// Defines the contract for product-related use cases
    /// </summary>
    public interface IProductService
    {
        Task<ProductDto> GetByIdAsync(Guid id);
        Task<ProductDto> GetBySkuAsync(string sku);
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<IEnumerable<ProductDto>> GetActiveAsync();
        Task<IEnumerable<ProductDto>> GetByCategoryAsync(ProductCategory category);
        Task<IEnumerable<ProductDto>> SearchByNameAsync(string name);
        Task<IEnumerable<ProductDto>> GetLowStockProductsAsync();
        Task<ProductDto> CreateAsync(CreateProductDto createProductDto);
        Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto updateProductDto);
        Task DeleteAsync(Guid id);
        Task<ProductDto> AdjustStockAsync(StockAdjustmentDto stockAdjustmentDto);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> SkuExistsAsync(string sku);
        Task<decimal> GetTotalInventoryValueAsync();
    }
}