using ErpCrmService.Application.DTOs;

namespace ErpCrmService.Application.Interfaces;

public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(Guid id);
    Task<ProductDto?> GetBySkuAsync(string sku);
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<IEnumerable<ProductDto>> GetActiveAsync();
    Task<IEnumerable<ProductDto>> SearchByNameAsync(string name);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto?> UpdateAsync(Guid id, UpdateProductDto dto);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> SkuExistsAsync(string sku);
    Task<decimal> GetTotalInventoryValueAsync();
}
