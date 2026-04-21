using ErpCrmService.Domain.Entities;
using ErpCrmService.Domain.ValueObjects;
using ErpCrmService.Application.DTOs;
using ErpCrmService.Application.Models;
using ErpCrmService.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpCrmService.WebApi.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ErpCrmDbContext _context;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ErpCrmDbContext context, ILogger<CustomersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCustomers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CompanyName")
    {
        try
        {
            pageSize = Math.Min(pageSize, 100);
            var query = _context.Customers.Where(c => c.IsActive);
            query = ApplyOrdering(query, orderBy);

            var total = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var result = new PagedResult<Customer>(items, total, pageNumber, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customers");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCustomer(Guid id)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer is null) return NotFound();
            return Ok(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer {Id}", id);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchCustomers(
        [FromQuery] string searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest("El término de búsqueda no puede estar vacío");

        try
        {
            pageSize = Math.Min(pageSize, 100);
            var query = _context.Customers
                .Where(c => c.IsActive && (
                    c.CompanyName.Contains(searchTerm) ||
                    c.ContactFirstName.Contains(searchTerm) ||
                    c.ContactLastName.Contains(searchTerm)))
                .OrderBy(c => c.CompanyName);

            var total = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return Ok(new PagedResult<Customer>(items, total, pageNumber, pageSize));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching customers");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("status/{status:int}")]
    public async Task<IActionResult> GetCustomersByStatus(int status)
    {
        try
        {
            var customers = await _context.Customers
                .Where(c => (int)c.Status == status)
                .OrderBy(c => c.CompanyName)
                .ToListAsync();
            return Ok(customers);
        }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpGet("balance-summary")]
    public async Task<IActionResult> GetBalanceSummary()
    {
        try
        {
            var active = _context.Customers.Where(c => c.IsActive);
            var summary = new
            {
                TotalCustomers = await active.CountAsync(),
                TotalBalance = await active.SumAsync(c => c.CurrentBalance),
                TotalCreditLimit = await active.SumAsync(c => c.CreditLimit),
                AverageBalance = await active.AverageAsync(c => c.CurrentBalance)
            };
            return Ok(summary);
        }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        try
        {
            var stats = new
            {
                TotalCustomers = await _context.Customers.CountAsync(c => c.IsActive),
                ActiveCustomers = await _context.Customers.CountAsync(c => c.Status == CustomerStatus.Active),
                InactiveCustomers = await _context.Customers.CountAsync(c => c.Status == CustomerStatus.Inactive),
                SuspendedCustomers = await _context.Customers.CountAsync(c => c.Status == CustomerStatus.Suspended),
                BlockedCustomers = await _context.Customers.CountAsync(c => c.Status == CustomerStatus.Blocked),
                CorporateCustomers = await _context.Customers.CountAsync(c => c.Type == CustomerType.Corporate && c.IsActive),
                IndividualCustomers = await _context.Customers.CountAsync(c => c.Type == CustomerType.Individual && c.IsActive)
            };
            return Ok(stats);
        }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            if (await _context.Customers.AnyAsync(c => c.Email.Value == dto.Email))
                return BadRequest("Ya existe un cliente con ese email");

            if (!string.IsNullOrWhiteSpace(dto.TaxId) &&
                await _context.Customers.AnyAsync(c => c.TaxId == dto.TaxId))
                return BadRequest("Ya existe un cliente con ese Tax ID");

            var email = new Email(dto.Email);
            var phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : new PhoneNumber(dto.Phone);
            Address address;
            if (dto.Address is not null)
                address = new Address(dto.Address.Street, dto.Address.City, dto.Address.State, dto.Address.PostalCode, dto.Address.Country);
            else
                address = new Address(string.Empty, string.Empty, null, null, string.Empty);

            var customer = new Customer(dto.CompanyName, dto.ContactFirstName, dto.ContactLastName,
                email, phone, address, dto.TaxId, dto.Type, dto.CreditLimit);

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Customer created: {Id} - {Name}", customer.Id, customer.CompanyName);
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer is null) return NotFound();

            if (!string.Equals(customer.Email.Value, dto.Email, StringComparison.OrdinalIgnoreCase) &&
                await _context.Customers.AnyAsync(c => c.Id != id && c.Email.Value == dto.Email))
                return BadRequest("Ya existe otro cliente con ese email");

            var email = new Email(dto.Email);
            var phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : new PhoneNumber(dto.Phone);
            customer.UpdateContactInfo(dto.ContactFirstName, dto.ContactLastName, email, phone);

            if (dto.Address is not null)
                customer.UpdateAddress(new Address(dto.Address.Street, dto.Address.City, dto.Address.State, dto.Address.PostalCode, dto.Address.Country));

            customer.UpdateCreditLimit(dto.CreditLimit);

            _context.Entry(customer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(customer);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateCustomerStatus(Guid id, [FromBody] StatusUpdateRequest request)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer is null) return NotFound();

            switch ((CustomerStatus)request.Status)
            {
                case CustomerStatus.Suspended:
                    if (string.IsNullOrWhiteSpace(request.Reason))
                        return BadRequest("Se requiere una razón para suspender el cliente");
                    customer.SuspendCustomer(request.Reason);
                    break;
                case CustomerStatus.Active:
                    customer.ReactivateCustomer();
                    break;
                default:
                    return BadRequest("Estado no válido");
            }

            _context.Entry(customer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(customer);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpPatch("{id:guid}/balance")]
    public async Task<IActionResult> AdjustBalance(Guid id, [FromBody] BalanceAdjustmentRequest request)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer is null) return NotFound();
            if (request.Amount <= 0) return BadRequest("El monto debe ser mayor a cero");

            if (request.IsDebit) customer.DeductBalance(request.Amount);
            else customer.AddBalance(request.Amount);

            _context.Entry(customer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(customer);
        }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCustomer(Guid id)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer is null) return NotFound();
            customer.Deactivate();
            _context.Entry(customer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpPost("{id:guid}/restore")]
    public async Task<IActionResult> RestoreCustomer(Guid id)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer is null) return NotFound();
            customer.Activate();
            _context.Entry(customer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(customer);
        }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    private static IQueryable<Customer> ApplyOrdering(IQueryable<Customer> query, string orderBy) =>
        orderBy?.ToLower() switch
        {
            "companyname" => query.OrderBy(c => c.CompanyName),
            "createdat" => query.OrderByDescending(c => c.CreatedAt),
            "creditlimit" => query.OrderByDescending(c => c.CreditLimit),
            "balance" => query.OrderByDescending(c => c.CurrentBalance),
            _ => query.OrderBy(c => c.CompanyName)
        };
}

public record StatusUpdateRequest(int Status, string? Reason);
public record BalanceAdjustmentRequest(decimal Amount, bool IsDebit);
