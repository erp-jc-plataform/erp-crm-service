using ErpCrmService.Domain.Entities;
using ErpCrmService.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

namespace ErpCrmService.Infrastructure.Data;

public class ErpCrmDbContext : DbContext
{
    public ErpCrmDbContext(DbContextOptions<ErpCrmDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
    }
}
