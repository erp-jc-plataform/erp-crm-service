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
using ErpCrmService.Domain.ValueObjects;
using ErpCrmService.Application.DTOs;
using ErpCrmService.Application.Models;
using ErpCrmService.Application.Extensions;
using Serilog;

namespace ErpCrmService.WebApi.Controllers
{
    /// <summary>
    /// API para gestión de clientes del sistema CRM
    /// Proporciona operaciones CRUD completas para la gestión de clientes
    /// </summary>
    [RoutePrefix("api/customers")]
    public class CustomersController : ApiController
    {
        private readonly ErpCrmDbContext _context;
        private readonly ILogger _logger;

        public CustomersController()
        {
            _context = new ErpCrmDbContext();
            _logger = Log.ForContext<CustomersController>();
        }

        /// <summary>
        /// Obtiene todos los clientes activos con paginación
        /// </summary>
        /// <param name="pageNumber">Número de página (por defecto 1)</param>
        /// <param name="pageSize">Tamaño de página (por defecto 20, máximo 100)</param>
        /// <param name="orderBy">Campo para ordenar (CompanyName, CreatedAt, etc.)</param>
        /// <returns>Lista paginada de clientes</returns>
        /// <response code="200">Retorna la lista paginada de clientes</response>
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(PagedResult<Customer>))]
        public IHttpActionResult GetAllCustomers(
            [FromUri] int pageNumber = 1, 
            [FromUri] int pageSize = 20,
            [FromUri] string orderBy = "CompanyName")
        {
            try
            {
                _logger.Information("Getting customers - Page: {PageNumber}, Size: {PageSize}, OrderBy: {OrderBy}", 
                    pageNumber, pageSize, orderBy);

                var query = _context.Customers.Where(c => c.IsActive);

                // Aplicar ordenamiento
                query = ApplyOrdering(query, orderBy);

                // Aplicar paginación
                var result = query.ToPagedResult(pageNumber, pageSize);

                _logger.Information("Retrieved {Count} customers from page {PageNumber} of {TotalPages}", 
                    result.CurrentPageSize, result.PageNumber, result.TotalPages);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting customers - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
                return InternalServerError(ex);
            }
        }

        private IQueryable<Customer> ApplyOrdering(IQueryable<Customer> query, string orderBy)
        {
            switch (orderBy?.ToLower())
            {
                case "companyname":
                    return query.OrderBy(c => c.CompanyName);
                case "createdat":
                    return query.OrderByDescending(c => c.CreatedAt);
                case "creditlimit":
                    return query.OrderByDescending(c => c.CreditLimit);
                case "balance":
                    return query.OrderByDescending(c => c.CurrentBalance);
                default:
                    return query.OrderBy(c => c.CompanyName);
            }
        }

        /// <summary>
        /// Obtiene un cliente por su ID
        /// </summary>
        /// <param name="id">ID del cliente (GUID)</param>
        /// <returns>Cliente solicitado</returns>
        /// <response code="200">Retorna el cliente</response>
        /// <response code="404">Cliente no encontrado</response>
        [HttpGet]
        [Route("{id:guid}")]
        [ResponseType(typeof(Customer))]
        public IHttpActionResult GetCustomer(Guid id)
        {
            try
            {
                _logger.Information("Getting customer by ID: {CustomerId}", id);

                var customer = _context.Customers.Find(id);

                if (customer == null)
                {
                    _logger.Warning("Customer not found: {CustomerId}", id);
                    return NotFound();
                }

                _logger.Information("Customer found: {CustomerId} - {CompanyName}", id, customer.CompanyName);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting customer by ID: {CustomerId}", id);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Busca clientes por nombre de empresa con paginación
        /// </summary>
        /// <param name="searchTerm">Término de búsqueda</param>
        /// <param name="pageNumber">Número de página</param>
        /// <param name="pageSize">Tamaño de página</param>
        /// <returns>Lista paginada de clientes que coinciden con la búsqueda</returns>
        /// <response code="200">Retorna los clientes encontrados</response>
        [HttpGet]
        [Route("search")]
        [ResponseType(typeof(PagedResult<Customer>))]
        public IHttpActionResult SearchCustomers(
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

                _logger.Information("Searching customers with term: {SearchTerm}, Page: {PageNumber}", searchTerm, pageNumber);

                var query = _context.Customers
                    .Where(c => c.IsActive &&
                                (c.CompanyName.Contains(searchTerm) ||
                                 c.ContactFirstName.Contains(searchTerm) ||
                                 c.ContactLastName.Contains(searchTerm)))
                    .OrderBy(c => c.CompanyName);

                var result = query.ToPagedResult(pageNumber, pageSize);

                _logger.Information("Found {Count} customers matching '{SearchTerm}'", result.TotalCount, searchTerm);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error searching customers with term: {SearchTerm}", searchTerm);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtiene clientes por estado
        /// </summary>
        /// <param name="status">Estado del cliente (1=Active, 2=Inactive, 3=Suspended, 4=Blocked)</param>
        /// <returns>Lista de clientes con el estado especificado</returns>
        /// <response code="200">Retorna los clientes filtrados por estado</response>
        [HttpGet]
        [Route("status/{status:int}")]
        [ResponseType(typeof(IEnumerable<Customer>))]
        public IHttpActionResult GetCustomersByStatus(int status)
        {
            try
            {
                var customers = _context.Customers
                    .Where(c => (int)c.Status == status)
                    .OrderBy(c => c.CompanyName)
                    .ToList();

                return Ok(customers);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtiene el balance total de todos los clientes
        /// </summary>
        /// <returns>Resumen de balances</returns>
        /// <response code="200">Retorna el resumen de balances</response>
        [HttpGet]
        [Route("balance-summary")]
        public IHttpActionResult GetBalanceSummary()
        {
            try
            {
                var summary = new
                {
                    TotalCustomers = _context.Customers.Count(c => c.IsActive),
                    TotalBalance = _context.Customers.Where(c => c.IsActive).Sum(c => c.CurrentBalance),
                    TotalCreditLimit = _context.Customers.Where(c => c.IsActive).Sum(c => c.CreditLimit),
                    AverageBalance = _context.Customers.Where(c => c.IsActive).Average(c => c.CurrentBalance)
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtiene estadísticas de clientes
        /// </summary>
        /// <returns>Estadísticas generales</returns>
        /// <response code="200">Retorna las estadísticas</response>
        [HttpGet]
        [Route("statistics")]
        public IHttpActionResult GetStatistics()
        {
            try
            {
                var stats = new
                {
                    TotalCustomers = _context.Customers.Count(c => c.IsActive),
                    ActiveCustomers = _context.Customers.Count(c => c.Status == CustomerStatus.Active),
                    InactiveCustomers = _context.Customers.Count(c => c.Status == CustomerStatus.Inactive),
                    SuspendedCustomers = _context.Customers.Count(c => c.Status == CustomerStatus.Suspended),
                    BlockedCustomers = _context.Customers.Count(c => c.Status == CustomerStatus.Blocked),
                    CorporateCustomers = _context.Customers.Count(c => c.Type == CustomerType.Corporate && c.IsActive),
                    IndividualCustomers = _context.Customers.Count(c => c.Type == CustomerType.Individual && c.IsActive)
                };

                return Ok(stats);
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
        /// Crea un nuevo cliente
        /// </summary>
        /// <param name="dto">Datos del cliente a crear</param>
        /// <returns>Cliente creado</returns>
        /// <response code="201">Cliente creado exitosamente</response>
        /// <response code="400">Datos inválidos o email duplicado</response>
        [HttpPost]
        [Route("")]
        [ResponseType(typeof(Customer))]
        public IHttpActionResult CreateCustomer([FromBody] CreateCustomerDto dto)
        {
            try
            {
                _logger.Information("Creating new customer: {CompanyName}, Email: {Email}", dto?.CompanyName, dto?.Email);

                if (!ModelState.IsValid)
                {
                    _logger.Warning("Invalid model state for customer creation");
                    return BadRequest(ModelState);
                }

                // Validar email único
                if (_context.Customers.Any(c => c.Email.Value == dto.Email))
                {
                    _logger.Warning("Duplicate email attempted: {Email}", dto.Email);
                    return BadRequest("Ya existe un cliente con ese email");
                }

                // Validar TaxId único si se proporciona
                if (!string.IsNullOrWhiteSpace(dto.TaxId) && 
                    _context.Customers.Any(c => c.TaxId == dto.TaxId))
                {
                    _logger.Warning("Duplicate TaxId attempted: {TaxId}", dto.TaxId);
                    return BadRequest("Ya existe un cliente con ese Tax ID");
                }

                // Crear Value Objects
                var email = new Email(dto.Email);
                var phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : new PhoneNumber(dto.Phone);
                var address = dto.Address != null 
                    ? new Address(
                        dto.Address.Street,
                        dto.Address.City,
                        dto.Address.State,
                        dto.Address.PostalCode,
                        dto.Address.Country)
                    : new Address(null, null, null, null, null);

                // Crear entidad
                var customer = new Customer(
                    dto.CompanyName,
                    dto.ContactFirstName,
                    dto.ContactLastName,
                    email,
                    phone,
                    address,
                    dto.TaxId,
                    dto.Type,
                    dto.CreditLimit
                );

                _context.Customers.Add(customer);
                _context.SaveChanges();

                _logger.Information("Customer created successfully: ID={CustomerId}, Company={CompanyName}", 
                    customer.Id, customer.CompanyName);

                return Created($"api/customers/{customer.Id}", customer);
            }
            catch (ArgumentException ex)
            {
                _logger.Warning(ex, "Validation error creating customer: {CompanyName}", dto?.CompanyName);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating customer: {CompanyName}", dto?.CompanyName);
                return InternalServerError(ex);
            }
        }

        // =====================================================
        // UPDATE OPERATIONS
        // =====================================================

        /// <summary>
        /// Actualiza completamente un cliente existente
        /// </summary>
        /// <param name="id">ID del cliente</param>
        /// <param name="dto">Nuevos datos del cliente</param>
        /// <returns>Cliente actualizado</returns>
        /// <response code="200">Cliente actualizado exitosamente</response>
        /// <response code="404">Cliente no encontrado</response>
        /// <response code="400">Datos inválidos</response>
        [HttpPut]
        [Route("{id:guid}")]
        [ResponseType(typeof(Customer))]
        public IHttpActionResult UpdateCustomer(Guid id, [FromBody] UpdateCustomerDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var customer = _context.Customers.Find(id);
                if (customer == null)
                {
                    return NotFound();
                }

                // Validar email único (excluyendo el actual)
                if (_context.Customers.Any(c => c.Id != id && c.Email.Value == dto.Email))
                {
                    return BadRequest("Ya existe otro cliente con ese email");
                }

                // Actualizar información de contacto
                var email = new Email(dto.Email);
                var phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : new PhoneNumber(dto.Phone);
                
                customer.UpdateContactInfo(
                    dto.ContactFirstName,
                    dto.ContactLastName,
                    email,
                    phone
                );

                // Actualizar dirección si se proporciona
                if (dto.Address != null)
                {
                    var address = new Address(
                        dto.Address.Street,
                        dto.Address.City,
                        dto.Address.State,
                        dto.Address.PostalCode,
                        dto.Address.Country
                    );
                    customer.UpdateAddress(address);
                }

                // Actualizar límite de crédito
                customer.UpdateCreditLimit(dto.CreditLimit);

                _context.Entry(customer).State = EntityState.Modified;
                _context.SaveChanges();

                return Ok(customer);
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
        /// Actualiza el estado de un cliente
        /// </summary>
        /// <param name="id">ID del cliente</param>
        /// <param name="status">Nuevo estado (1=Active, 2=Inactive, 3=Suspended, 4=Blocked)</param>
        /// <returns>Cliente actualizado</returns>
        /// <response code="200">Estado actualizado exitosamente</response>
        /// <response code="404">Cliente no encontrado</response>
        [HttpPatch]
        [Route("{id:guid}/status")]
        [ResponseType(typeof(Customer))]
        public IHttpActionResult UpdateCustomerStatus(Guid id, [FromBody] dynamic statusData)
        {
            try
            {
                var customer = _context.Customers.Find(id);
                if (customer == null)
                {
                    return NotFound();
                }

                int newStatus = (int)statusData.status;
                string reason = statusData.reason?.ToString();

                switch ((CustomerStatus)newStatus)
                {
                    case CustomerStatus.Suspended:
                        if (string.IsNullOrWhiteSpace(reason))
                        {
                            return BadRequest("Se requiere una razón para suspender el cliente");
                        }
                        customer.SuspendCustomer(reason);
                        break;
                    
                    case CustomerStatus.Active:
                        customer.ReactivateCustomer();
                        break;
                    
                    default:
                        return BadRequest("Estado no válido");
                }

                _context.Entry(customer).State = EntityState.Modified;
                _context.SaveChanges();

                return Ok(customer);
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
        /// Ajusta el balance de un cliente
        /// </summary>
        /// <param name="id">ID del cliente</param>
        /// <param name="adjustmentData">Datos del ajuste (amount, isDebit)</param>
        /// <returns>Cliente actualizado</returns>
        /// <response code="200">Balance ajustado exitosamente</response>
        /// <response code="404">Cliente no encontrado</response>
        /// <response code="400">Datos inválidos</response>
        [HttpPatch]
        [Route("{id:guid}/balance")]
        [ResponseType(typeof(Customer))]
        public IHttpActionResult AdjustBalance(Guid id, [FromBody] dynamic adjustmentData)
        {
            try
            {
                var customer = _context.Customers.Find(id);
                if (customer == null)
                {
                    return NotFound();
                }

                decimal amount = (decimal)adjustmentData.amount;
                bool isDebit = (bool)adjustmentData.isDebit;

                if (amount <= 0)
                {
                    return BadRequest("El monto debe ser mayor a cero");
                }

                if (isDebit)
                {
                    customer.DeductBalance(amount);
                }
                else
                {
                    customer.AddBalance(amount);
                }

                _context.Entry(customer).State = EntityState.Modified;
                _context.SaveChanges();

                return Ok(customer);
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

        // =====================================================
        // DELETE OPERATIONS
        // =====================================================

        /// <summary>
        /// Elimina lógicamente un cliente (soft delete)
        /// </summary>
        /// <param name="id">ID del cliente</param>
        /// <returns>Resultado de la operación</returns>
        /// <response code="204">Cliente eliminado exitosamente</response>
        /// <response code="404">Cliente no encontrado</response>
        [HttpDelete]
        [Route("{id:guid}")]
        public IHttpActionResult DeleteCustomer(Guid id)
        {
            try
            {
                var customer = _context.Customers.Find(id);
                if (customer == null)
                {
                    return NotFound();
                }

                // Soft delete: marcar como inactivo
                customer.Deactivate();
                
                _context.Entry(customer).State = EntityState.Modified;
                _context.SaveChanges();

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Restaura un cliente eliminado
        /// </summary>
        /// <param name="id">ID del cliente</param>
        /// <returns>Cliente restaurado</returns>
        /// <response code="200">Cliente restaurado exitosamente</response>
        /// <response code="404">Cliente no encontrado</response>
        [HttpPost]
        [Route("{id:guid}/restore")]
        [ResponseType(typeof(Customer))]
        public IHttpActionResult RestoreCustomer(Guid id)
        {
            try
            {
                var customer = _context.Customers.Find(id);
                if (customer == null)
                {
                    return NotFound();
                }

                customer.Activate();
                
                _context.Entry(customer).State = EntityState.Modified;
                _context.SaveChanges();

                return Ok(customer);
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
