using System.Data.Entity;
using ErpCrmService.Domain.Entities;
using ErpCrmService.Infrastructure.Configuration;

namespace ErpCrmService.Infrastructure.Data
{
    /// <summary>
    /// EF DbContext for ERP CRM Service
    /// Implements Unit of Work pattern and serves as the primary data access layer
    /// </summary>
    public class ErpCrmDbContext : DbContext
    {
        public ErpCrmDbContext() : base("DefaultConnection")
        {
            // Configure EF behavior
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            
            // Initialize database if it doesn't exist
            Database.SetInitializer(new CreateDatabaseIfNotExists<ErpCrmDbContext>());
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply entity configurations
            modelBuilder.Configurations.Add(new CustomerConfiguration());
            modelBuilder.Configurations.Add(new ProductConfiguration());

            // Global configurations
            modelBuilder.Properties<string>()
                .Configure(p => p.HasColumnType("nvarchar"));

            // Disable cascade delete globally
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.ManyToManyCascadeDeleteConvention>();
        }
    }
}