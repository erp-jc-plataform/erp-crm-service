using ErpCrmService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ErpCrmService.Infrastructure.Configuration;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.SKU).IsRequired().HasMaxLength(20);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Description).HasMaxLength(500);
        builder.Property(p => p.Category).IsRequired();
        builder.Property(p => p.UnitPrice).IsRequired().HasColumnType("numeric(18,2)");
        builder.Property(p => p.Cost).IsRequired().HasColumnType("numeric(18,2)");
        builder.Property(p => p.StockQuantity).IsRequired();
        builder.Property(p => p.MinimumStock).IsRequired();
        builder.Property(p => p.MaximumStock).IsRequired();
        builder.Property(p => p.Unit).IsRequired().HasMaxLength(50);
        builder.Property(p => p.IsDiscontinued).IsRequired();
        builder.Property(p => p.Supplier).HasMaxLength(100);
        builder.Property(p => p.Weight).HasColumnType("numeric(18,2)");
        builder.Property(p => p.Length).HasColumnType("numeric(18,2)");
        builder.Property(p => p.Width).HasColumnType("numeric(18,2)");
        builder.Property(p => p.Height).HasColumnType("numeric(18,2)");
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.IsActive).IsRequired();

        builder.HasIndex(p => p.SKU).IsUnique().HasDatabaseName("IX_Product_SKU");
        builder.HasIndex(p => p.Name).HasDatabaseName("IX_Product_Name");
    }
}
