using ErpCrmService.Domain.Entities;
using ErpCrmService.Application.DTOs;
using ErpCrmService.Application.Models;
using ErpCrmService.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpCrmService.WebApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ErpCrmDbContext _context;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ErpCrmDbContext context, ILogger<ProductsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "Name")
    {
        try
        {
            pageSize = Math.Min(pageSize, 100);
            var query = _context.Products.Where(p => p.IsActive && !p.IsDiscontinued);
            query = ApplyOrdering(query, orderBy);

            var total = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return Ok(new PagedResult<Product>(items, total, pageNumber, pageSize));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null) return NotFound();
            return Ok(product);
        }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpGet("sku/{sku}")]
    public async Task<IActionResult> GetProductBySku(string sku)
    {
        try
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.SKU == sku);
            if (product is null) return NotFound();
            return Ok(product);
        }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts(
        [FromQuery] string searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest("El término de búsqueda no puede estar vacío");

        try
        {
            pageSize = Math.Min(pageSize, 100);
            var query = _context.Products
                .Where(p => p.IsActive && (
                    p.Name.Contains(searchTerm) ||
                    (p.Description != null && p.Description.Contains(searchTerm)) ||
                    p.SKU.Contains(searchTerm)))
                .OrderBy(p => p.Name);

            var total = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return Ok(new PagedResult<Product>(items, total, pageNumber, pageSize));
        }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpGet("category/{category:int}")]
    public async Task<IActionResult> GetProductsByCategory(int category)
    {
        try
        {
            var products = await _context.Products
                .Where(p => p.IsActive && (int)p.Category == category)
                .OrderBy(p => p.Name)
                .ToListAsync();
            return Ok(products);
        }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockProducts()
    {
        try
        {
            var products = await _context.Products
                .Where(p => p.IsActive && !p.IsDiscontinued && p.StockQuantity < p.MinimumStock)
                .OrderBy(p => p.StockQuantity)
                .ToListAsync();
            return Ok(products);
        }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpGet("discontinued")]
    public async Task<IActionResult> GetDiscontinuedProducts()
    {
        try
        {
            var products = await _context.Products
                .Where(p => p.IsDiscontinued)
                .OrderByDescending(p => p.DiscontinuedDate)
                .ToListAsync();
            return Ok(products);
        }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpGet("inventory-stats")]
    public async Task<IActionResult> GetInventoryStatistics()
    {
        try
        {
            var stats = new
            {
                TotalProducts = await _context.Products.CountAsync(p => p.IsActive),
                ActiveProducts = await _context.Products.CountAsync(p => p.IsActive && !p.IsDiscontinued),
                DiscontinuedProducts = await _context.Products.CountAsync(p => p.IsDiscontinued),
                LowStockProducts = await _context.Products.CountAsync(p => p.IsActive && !p.IsDiscontinued && p.StockQuantity < p.MinimumStock),
                OutOfStockProducts = await _context.Products.CountAsync(p => p.IsActive && !p.IsDiscontinued && p.StockQuantity == 0),
                TotalStockValue = await _context.Products.Where(p => p.IsActive).SumAsync(p => (decimal)p.StockQuantity * p.Cost),
                TotalInventoryValue = await _context.Products.Where(p => p.IsActive).SumAsync(p => (decimal)p.StockQuantity * p.UnitPrice)
            };
            return Ok(stats);
        }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpGet("top-value")]
    public async Task<IActionResult> GetTopValueProducts([FromQuery] int top = 10)
    {
        try
        {
            var products = await _context.Products
                .Where(p => p.IsActive && !p.IsDiscontinued)
                .OrderByDescending(p => p.StockQuantity * p.UnitPrice)
                .Take(top)
                .Select(p => new { p.Id, p.SKU, p.Name, p.StockQuantity, p.UnitPrice, TotalValue = p.StockQuantity * p.UnitPrice })
                .ToListAsync();
            return Ok(products);
        }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            if (await _context.Products.AnyAsync(p => p.SKU == dto.SKU))
                return BadRequest("Ya existe un producto con ese SKU");

            var maximumStock = dto.MinimumStock * 10;
            var product = new Product(dto.SKU, dto.Name, dto.Description, dto.Category,
                dto.UnitPrice, dto.Cost, dto.StockQuantity, dto.MinimumStock, maximumStock, dto.Unit);

            if (!string.IsNullOrWhiteSpace(dto.Supplier))
                product.UpdateSupplier(dto.Supplier);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Product created: {Id} - {SKU}", product.Id, product.SKU);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null) return NotFound();

            product.UpdateProductInfo(dto.Name, dto.Description);
            product.UpdatePricing(dto.UnitPrice, dto.Cost);
            product.UpdateStockLimits(dto.MinimumStock, dto.MaximumStock);

            if (!string.IsNullOrWhiteSpace(dto.Supplier))
                product.UpdateSupplier(dto.Supplier);

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(product);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpPatch("{id:guid}/stock")]
    public async Task<IActionResult> AdjustStock(Guid id, [FromBody] StockAdjustRequest request)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null) return NotFound();
            if (request.Quantity == 0) return BadRequest("La cantidad de ajuste debe ser diferente de cero");

            if (request.Quantity > 0) product.AddStock(request.Quantity);
            else product.RemoveStock(Math.Abs(request.Quantity));

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(product);
        }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpPatch("{id:guid}/pricing")]
    public async Task<IActionResult> UpdatePricing(Guid id, [FromBody] PricingRequest request)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null) return NotFound();
            product.UpdatePricing(request.UnitPrice, request.Cost);
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(product);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpPost("{id:guid}/discontinue")]
    public async Task<IActionResult> DiscontinueProduct(Guid id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null) return NotFound();
            product.Discontinue();
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(product);
        }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null) return NotFound();
            product.Deactivate();
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    private static IQueryable<Product> ApplyOrdering(IQueryable<Product> query, string orderBy) =>
        orderBy?.ToLower() switch
        {
            "name" => query.OrderBy(p => p.Name),
            "price" => query.OrderByDescending(p => p.UnitPrice),
            "stock" => query.OrderBy(p => p.StockQuantity),
            "sku" => query.OrderBy(p => p.SKU),
            "createdat" => query.OrderByDescending(p => p.CreatedAt),
            _ => query.OrderBy(p => p.Name)
        };
}

public record StockAdjustRequest(int Quantity, string? Reason);
public record PricingRequest(decimal UnitPrice, decimal Cost);
