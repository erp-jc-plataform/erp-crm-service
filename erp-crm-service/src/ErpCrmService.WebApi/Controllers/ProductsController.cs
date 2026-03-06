using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ErpCrmService.Infrastructure.Data;
using ErpCrmService.Domain.Entities;
using ErpCrmService.Application.DTOs;
using ErpCrmService.Application.Models;
using ErpCrmService.Application.Extensions;
using Serilog;

namespace ErpCrmService.WebApi.Controllers
{
    /// <summary>
    /// API para gestión de productos del sistema ERP
    /// Proporciona operaciones CRUD completas para la gestión de productos
    /// </summary>
    [RoutePrefix("api/products")]
    public class ProductsController : ApiController
    {
        private readonly ErpCrmDbContext _context;
        private readonly ILogger _logger;

        public ProductsController()
        {
            _context = new ErpCrmDbContext();
            _logger = Log.ForContext<ProductsController>();
        }

        /// <summary>
        /// Obtiene todos los productos activos con paginación
        /// </summary>
        /// <param name="pageNumber">Número de página (por defecto 1)</param>
        /// <param name="pageSize">Tamaño de página (por defecto 20, máximo 100)</param>
        /// <param name="orderBy">Campo para ordenar (Name, Price, Stock, etc.)</param>
        /// <returns>Lista paginada de productos</returns>
        /// <response code="200">Retorna la lista paginada de productos</response>
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(PagedResult<Product>))]
        public IHttpActionResult GetAllProducts(
            [FromUri] int pageNumber = 1,
            [FromUri] int pageSize = 20,
            [FromUri] string orderBy = "Name")
        {
            try
            {
                _logger.Information("Getting products - Page: {PageNumber}, Size: {PageSize}, OrderBy: {OrderBy}",
                    pageNumber, pageSize, orderBy);

                var query = _context.Products
                    .Where(p => p.IsActive && !p.IsDiscontinued);

                // Aplicar ordenamiento
                query = ApplyProductOrdering(query, orderBy);

                // Aplicar paginación
                var result = query.ToPagedResult(pageNumber, pageSize);

                _logger.Information("Retrieved {Count} products from page {PageNumber} of {TotalPages}",
                    result.CurrentPageSize, result.PageNumber, result.TotalPages);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting products - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
                return InternalServerError(ex);
            }
        }

        private IQueryable<Product> ApplyProductOrdering(IQueryable<Product> query, string orderBy)
        {
            switch (orderBy?.ToLower())
            {
                case "name":
                    return query.OrderBy(p => p.Name);
                case "price":
                    return query.OrderByDescending(p => p.UnitPrice);
                case "stock":
                    return query.OrderBy(p => p.StockQuantity);
                case "sku":
                    return query.OrderBy(p => p.SKU);
                case "createdat":
                    return query.OrderByDescending(p => p.CreatedAt);
                default:
                    return query.OrderBy(p => p.Name);
            }
        }

        /// <summary>
        /// Obtiene un producto por su ID
        /// </summary>
        /// <param name="id">ID del producto (GUID)</param>
        /// <returns>Producto solicitado</returns>
        /// <response code="200">Retorna el producto</response>
        /// <response code="404">Producto no encontrado</response>
        [HttpGet]
        [Route("{id:guid}")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetProduct(Guid id)
        {
            try
            {
                _logger.Information("Getting product by ID: {ProductId}", id);

                var product = _context.Products.Find(id);

                if (product == null)
                {
                    _logger.Warning("Product not found: {ProductId}", id);
                    return NotFound();
                }

                _logger.Information("Product found: {ProductId} - {ProductName}", id, product.Name);
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting product by ID: {ProductId}", id);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Busca productos por SKU
        /// </summary>
        /// <param name="sku">SKU del producto</param>
        /// <returns>Producto encontrado</returns>
        /// <response code="200">Retorna el producto</response>
        /// <response code="404">Producto no encontrado</response>
        [HttpGet]
        [Route("sku/{sku}")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetProductBySku(string sku)
        {
            try
            {
                var product = _context.Products.FirstOrDefault(p => p.SKU == sku);

                if (product == null)
                {
                    return NotFound();
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Busca productos por nombre o descripción con paginación
        /// </summary>
        /// <param name="searchTerm">Término de búsqueda</param>
        /// <param name="pageNumber">Número de página</param>
        /// <param name="pageSize">Tamaño de página</param>
        /// <returns>Lista paginada de productos que coinciden con la búsqueda</returns>
        /// <response code="200">Retorna los productos encontrados</response>
        [HttpGet]
        [Route("search")]
        [ResponseType(typeof(PagedResult<Product>))]
        public IHttpActionResult SearchProducts(
            [FromUri] string searchTerm,
            [FromUri] int pageNumber = 1,
            [FromUri] int pageSize = 20)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    _logger.Warning("Search attempted with empty search term");
                    return BadRequest("El término de búsqueda no puede estar vacío");
                }

                _logger.Information("Searching products with term: {SearchTerm}, Page: {PageNumber}", searchTerm, pageNumber);

                var query = _context.Products
                    .Where(p => p.IsActive &&
                                (p.Name.Contains(searchTerm) ||
                                 p.Description.Contains(searchTerm) ||
                                 p.SKU.Contains(searchTerm)))
                    .OrderBy(p => p.Name);

                var result = query.ToPagedResult(pageNumber, pageSize);

                _logger.Information("Found {Count} products matching '{SearchTerm}'", result.TotalCount, searchTerm);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error searching products with term: {SearchTerm}", searchTerm);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtiene productos por categoría
        /// </summary>
        /// <param name="category">ID de categoría</param>
        /// <returns>Lista de productos de la categoría</returns>
        /// <response code="200">Retorna los productos de la categoría</response>
        [HttpGet]
        [Route("category/{category:int}")]
        [ResponseType(typeof(IEnumerable<Product>))]
        public IHttpActionResult GetProductsByCategory(int category)
        {
            try
            {
                var products = _context.Products
                    .Where(p => p.IsActive && (int)p.Category == category)
                    .OrderBy(p => p.Name)
                    .ToList();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtiene productos con stock bajo (cantidad menor al mínimo)
        /// </summary>
        /// <returns>Lista de productos con stock bajo</returns>
        /// <response code="200">Retorna los productos con stock bajo</response>
        [HttpGet]
        [Route("low-stock")]
        [ResponseType(typeof(IEnumerable<Product>))]
        public IHttpActionResult GetLowStockProducts()
        {
            try
            {
                var products = _context.Products
                    .Where(p => p.IsActive && !p.IsDiscontinued &&
                                p.StockQuantity < p.MinimumStock)
                    .OrderBy(p => p.StockQuantity)
                    .ToList();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtiene productos descontinuados
        /// </summary>
        /// <returns>Lista de productos descontinuados</returns>
        /// <response code="200">Retorna los productos descontinuados</response>
        [HttpGet]
        [Route("discontinued")]
        [ResponseType(typeof(IEnumerable<Product>))]
        public IHttpActionResult GetDiscontinuedProducts()
        {
            try
            {
                var products = _context.Products
                    .Where(p => p.IsDiscontinued)
                    .OrderByDescending(p => p.DiscontinuedDate)
                    .ToList();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtiene estadísticas de inventario
        /// </summary>
        /// <returns>Estadísticas del inventario</returns>
        /// <response code="200">Retorna las estadísticas</response>
        [HttpGet]
        [Route("inventory-stats")]
        public IHttpActionResult GetInventoryStatistics()
        {
            try
            {
                var stats = new
                {
                    TotalProducts = _context.Products.Count(p => p.IsActive),
                    ActiveProducts = _context.Products.Count(p => p.IsActive && !p.IsDiscontinued),
                    DiscontinuedProducts = _context.Products.Count(p => p.IsDiscontinued),
                    LowStockProducts = _context.Products.Count(p => p.IsActive && !p.IsDiscontinued && p.StockQuantity < p.MinimumStock),
                    OutOfStockProducts = _context.Products.Count(p => p.IsActive && !p.IsDiscontinued && p.StockQuantity == 0),
                    TotalStockValue = _context.Products.Where(p => p.IsActive).Sum(p => p.StockQuantity * p.Cost),
                    TotalInventoryValue = _context.Products.Where(p => p.IsActive).Sum(p => p.StockQuantity * p.UnitPrice)
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtiene el top de productos por valor de inventario
        /// </summary>
        /// <param name="top">Número de productos a retornar (por defecto 10)</param>
        /// <returns>Lista de productos con mayor valor de inventario</returns>
        /// <response code="200">Retorna los productos con mayor valor</response>
        [HttpGet]
        [Route("top-value")]
        [ResponseType(typeof(IEnumerable<object>))]
        public IHttpActionResult GetTopValueProducts([FromUri] int top = 10)
        {
            try
            {
                var products = _context.Products
                    .Where(p => p.IsActive && !p.IsDiscontinued)
                    .OrderByDescending(p => p.StockQuantity * p.UnitPrice)
                    .Take(top)
                    .Select(p => new
                    {
                        p.Id,
                        p.SKU,
                        p.Name,
                        p.StockQuantity,
                        p.UnitPrice,
                        TotalValue = p.StockQuantity * p.UnitPrice
                    })
                    .ToList();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // =====================================================
        // CREATE OPERATIONS
        // =====================================================

        /// <summary>
        /// Crea un nuevo producto
        /// </summary>
        /// <param name="dto">Datos del producto a crear</param>
        /// <returns>Producto creado</returns>
        /// <response code="201">Producto creado exitosamente</response>
        /// <response code="400">Datos inválidos o SKU duplicado</response>
        [HttpPost]
        [Route("")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult CreateProduct([FromBody] CreateProductDto dto)
        {
            try
            {
                _logger.Information("Creating new product: SKU={SKU}, Name={Name}", dto?.SKU, dto?.Name);

                if (!ModelState.IsValid)
                {
                    _logger.Warning("Invalid model state for product creation");
                    return BadRequest(ModelState);
                }

                // Validar SKU único
                if (_context.Products.Any(p => p.SKU == dto.SKU))
                {
                    _logger.Warning("Duplicate SKU attempted: {SKU}", dto.SKU);
                    return BadRequest("Ya existe un producto con ese SKU");
                }

                // Calcular MaximumStock automáticamente si no se proporciona
                var maximumStock = dto.MinimumStock * 10; // Por defecto 10x el mínimo

                // Crear entidad
                var product = new Product(
                    dto.SKU,
                    dto.Name,
                    dto.Description,
                    dto.Category,
                    dto.UnitPrice,
                    dto.Cost,
                    dto.StockQuantity,
                    dto.MinimumStock,
                    maximumStock,
                    dto.Unit
                );

                // Establecer supplier si se proporciona
                if (!string.IsNullOrWhiteSpace(dto.Supplier))
                {
                    product.UpdateSupplier(dto.Supplier);
                }

                _context.Products.Add(product);
                _context.SaveChanges();

                _logger.Information("Product created successfully: ID={ProductId}, SKU={SKU}, Name={Name}",
                    product.Id, product.SKU, product.Name);

                return Created($"api/products/{product.Id}", product);
            }
            catch (ArgumentException ex)
            {
                _logger.Warning(ex, "Validation error creating product: SKU={SKU}", dto?.SKU);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating product: SKU={SKU}", dto?.SKU);
                return InternalServerError(ex);
            }
        }

        // =====================================================
        // UPDATE OPERATIONS
        // =====================================================

        /// <summary>
        /// Actualiza completamente un producto existente
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <param name="dto">Nuevos datos del producto</param>
        /// <returns>Producto actualizado</returns>
        /// <response code="200">Producto actualizado exitosamente</response>
        /// <response code="404">Producto no encontrado</response>
        /// <response code="400">Datos inválidos</response>
        [HttpPut]
        [Route("{id:guid}")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult UpdateProduct(Guid id, [FromBody] UpdateProductDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var product = _context.Products.Find(id);
                if (product == null)
                {
                    return NotFound();
                }

                // Actualizar información del producto
                product.UpdateProductInfo(dto.Name, dto.Description);
                product.UpdatePricing(dto.UnitPrice, dto.Cost);
                product.UpdateStockLevels(dto.MinimumStock, dto.MaximumStock);

                if (!string.IsNullOrWhiteSpace(dto.Supplier))
                {
                    product.UpdateSupplier(dto.Supplier);
                }

                _context.Entry(product).State = EntityState.Modified;
                _context.SaveChanges();

                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Ajusta el stock de un producto
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <param name="stockData">Datos del ajuste (quantity, reason)</param>
        /// <returns>Producto actualizado</returns>
        /// <response code="200">Stock ajustado exitosamente</response>
        /// <response code="404">Producto no encontrado</response>
        /// <response code="400">Datos inválidos</response>
        [HttpPatch]
        [Route("{id:guid}/stock")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult AdjustStock(Guid id, [FromBody] dynamic stockData)
        {
            try
            {
                var product = _context.Products.Find(id);
                if (product == null)
                {
                    return NotFound();
                }

                int adjustmentQuantity = (int)stockData.quantity;
                string reason = stockData.reason?.ToString();

                if (adjustmentQuantity == 0)
                {
                    return BadRequest("La cantidad de ajuste debe ser diferente de cero");
                }

                // Ajustar stock
                if (adjustmentQuantity > 0)
                {
                    product.AddStock(adjustmentQuantity);
                }
                else
                {
                    product.RemoveStock(Math.Abs(adjustmentQuantity));
                }

                _context.Entry(product).State = EntityState.Modified;
                _context.SaveChanges();

                return Ok(product);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Actualiza el precio de un producto
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <param name="pricingData">Datos de precios (unitPrice, cost)</param>
        /// <returns>Producto actualizado</returns>
        /// <response code="200">Precio actualizado exitosamente</response>
        /// <response code="404">Producto no encontrado</response>
        /// <response code="400">Datos inválidos</response>
        [HttpPatch]
        [Route("{id:guid}/pricing")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult UpdatePricing(Guid id, [FromBody] dynamic pricingData)
        {
            try
            {
                var product = _context.Products.Find(id);
                if (product == null)
                {
                    return NotFound();
                }

                decimal unitPrice = (decimal)pricingData.unitPrice;
                decimal cost = (decimal)pricingData.cost;

                product.UpdatePricing(unitPrice, cost);

                _context.Entry(product).State = EntityState.Modified;
                _context.SaveChanges();

                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Descontinúa un producto
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <returns>Producto descontinuado</returns>
        /// <response code="200">Producto descontinuado exitosamente</response>
        /// <response code="404">Producto no encontrado</response>
        [HttpPost]
        [Route("{id:guid}/discontinue")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult DiscontinueProduct(Guid id)
        {
            try
            {
                var product = _context.Products.Find(id);
                if (product == null)
                {
                    return NotFound();
                }

                product.Discontinue();

                _context.Entry(product).State = EntityState.Modified;
                _context.SaveChanges();

                return Ok(product);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // =====================================================
        // DELETE OPERATIONS
        // =====================================================

        /// <summary>
        /// Elimina lógicamente un producto (soft delete)
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="204">Producto eliminado exitosamente</response>
        /// <response code="404">Producto no encontrado</response>
        [HttpDelete]
        [Route("{id:guid}")]
        public IHttpActionResult DeleteProduct(Guid id)
        {
            try
            {
                var product = _context.Products.Find(id);
                if (product == null)
                {
                    return NotFound();
                }

                // Soft delete: marcar como inactivo
                product.Deactivate();

                _context.Entry(product).State = EntityState.Modified;
                _context.SaveChanges();

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Restaura un producto eliminado
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <returns>Producto restaurado</returns>
        /// <response code="200">Producto restaurado exitosamente</response>
        /// <response code="404">Producto no encontrado</response>
        [HttpPost]
        [Route("{id:guid}/restore")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult RestoreProduct(Guid id)
        {
            try
            {
                var product = _context.Products.Find(id);
                if (product == null)
                {
                    return NotFound();
                }

                product.Activate();

                _context.Entry(product).State = EntityState.Modified;
                _context.SaveChanges();

                return Ok(product);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
