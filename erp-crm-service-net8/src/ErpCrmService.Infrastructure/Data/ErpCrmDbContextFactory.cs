using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ErpCrmService.Infrastructure.Data;

public class ErpCrmDbContextFactory : IDesignTimeDbContextFactory<ErpCrmDbContext>
{
    public ErpCrmDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ErpCrmDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
            ?? "Host=localhost;Port=5432;Database=erp_db;Username=erp_user;Password=erp_password";
        optionsBuilder.UseNpgsql(connectionString);
        return new ErpCrmDbContext(optionsBuilder.Options);
    }
}
