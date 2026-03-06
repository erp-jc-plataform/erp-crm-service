using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using ErpCrmService.Domain.Entities;

namespace ErpCrmService.Infrastructure.Configuration
{
    /// <summary>
    /// Entity Framework configuration for Product entity
    /// </summary>
    public class ProductConfiguration : EntityTypeConfiguration<Product>
    {
        public ProductConfiguration()
        {
            // Table configuration
            ToTable("Products");
            HasKey(p => p.Id);

            // Properties configuration
            Property(p => p.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(p => p.SKU)
                .IsRequired()
                .HasMaxLength(20);

            Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            Property(p => p.Description)
                .IsOptional()
                .HasMaxLength(500);

            Property(p => p.Category)
                .IsRequired();

            Property(p => p.UnitPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            Property(p => p.Cost)
                .IsRequired()
                .HasPrecision(18, 2);

            Property(p => p.StockQuantity)
                .IsRequired();

            Property(p => p.MinimumStock)
                .IsRequired();

            Property(p => p.MaximumStock)
                .IsRequired();

            Property(p => p.Unit)
                .IsRequired()
                .HasMaxLength(50);

            Property(p => p.IsDiscontinued)
                .IsRequired();

            Property(p => p.DiscontinuedDate)
                .IsOptional();

            Property(p => p.Supplier)
                .IsOptional()
                .HasMaxLength(100);

            Property(p => p.Weight)
                .IsOptional()
                .HasPrecision(18, 2);

            Property(p => p.Length)
                .IsOptional()
                .HasPrecision(18, 2);

            Property(p => p.Width)
                .IsOptional()
                .HasPrecision(18, 2);

            Property(p => p.Height)
                .IsOptional()
                .HasPrecision(18, 2);

            // Base entity properties
            Property(p => p.CreatedAt)
                .IsRequired();

            Property(p => p.UpdatedAt)
                .IsOptional();

            Property(p => p.CreatedBy)
                .IsOptional()
                .HasMaxLength(100);

            Property(p => p.UpdatedBy)
                .IsOptional()
                .HasMaxLength(100);

            Property(p => p.IsActive)
                .IsRequired();

            // Indexes for better performance
            HasIndex(p => p.SKU)
                .IsUnique()
                .HasName("IX_Product_SKU");

            HasIndex(p => p.Name)
                .HasName("IX_Product_Name");

            HasIndex(p => new { p.Category, p.IsActive })
                .HasName("IX_Product_Category_Active");

            HasIndex(p => new { p.StockQuantity, p.MinimumStock })
                .HasName("IX_Product_Stock");
        }
    }
}