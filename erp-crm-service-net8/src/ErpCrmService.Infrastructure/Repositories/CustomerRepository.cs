using ErpCrmService.Domain.Entities;
using ErpCrmService.Domain.Repositories;
using ErpCrmService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ErpCrmService.Infrastructure.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(ErpCrmDbContext context) : base(context) { }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        return await _dbSet.FirstOrDefaultAsync(c => c.Email.Value.ToLower() == email.ToLower());
    }

    public async Task<IEnumerable<Customer>> GetByStatusAsync(CustomerStatus status) =>
        await _dbSet.Where(c => c.Status == status).ToListAsync();

    public async Task<IEnumerable<Customer>> SearchByCompanyNameAsync(string companyName)
    {
        if (string.IsNullOrWhiteSpace(companyName)) return [];
        return await _dbSet.Where(c => c.CompanyName.Contains(companyName)).ToListAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        return await _dbSet.AnyAsync(c => c.Email.Value.ToLower() == email.ToLower());
    }

    public async Task<bool> TaxIdExistsAsync(string taxId)
    {
        if (string.IsNullOrWhiteSpace(taxId)) return false;
        return await _dbSet.AnyAsync(c => c.TaxId == taxId);
    }

    public async Task<decimal> GetTotalBalanceAsync() =>
        await _dbSet.Where(c => c.IsActive).SumAsync(c => c.CurrentBalance);

    public async Task<int> GetActiveCustomersCountAsync() =>
        await _dbSet.CountAsync(c => c.IsActive && c.Status == CustomerStatus.Active);
}
