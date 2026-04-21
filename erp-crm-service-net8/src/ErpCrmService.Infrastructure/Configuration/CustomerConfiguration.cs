using ErpCrmService.Domain.Entities;
using ErpCrmService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ErpCrmService.Infrastructure.Configuration;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.CompanyName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.ContactFirstName).IsRequired().HasMaxLength(50);
        builder.Property(c => c.ContactLastName).IsRequired().HasMaxLength(50);

        builder.OwnsOne(c => c.Email, email =>
        {
            email.Property(e => e.Value).HasColumnName("Email").IsRequired().HasMaxLength(150);
            email.HasIndex(e => e.Value).IsUnique().HasDatabaseName("IX_Customer_Email");
        });

        builder.OwnsOne(c => c.Phone, phone =>
        {
            phone.Property(p => p.Value).HasColumnName("Phone").HasMaxLength(20);
        });

        builder.OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.Street).HasColumnName("AddressStreet").HasMaxLength(200);
            address.Property(a => a.City).HasColumnName("AddressCity").HasMaxLength(100);
            address.Property(a => a.State).HasColumnName("AddressState").HasMaxLength(100);
            address.Property(a => a.PostalCode).HasColumnName("AddressPostalCode").HasMaxLength(20);
            address.Property(a => a.Country).HasColumnName("AddressCountry").HasMaxLength(100);
        });

        builder.Property(c => c.TaxId).HasMaxLength(20);
        builder.Property(c => c.Status).IsRequired();
        builder.Property(c => c.Type).IsRequired();
        builder.Property(c => c.Notes).HasMaxLength(500);
        builder.Property(c => c.CreditLimit).IsRequired().HasColumnType("numeric(18,2)");
        builder.Property(c => c.CurrentBalance).IsRequired().HasColumnType("numeric(18,2)");
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.IsActive).IsRequired();

        builder.HasIndex(c => c.TaxId).IsUnique().HasDatabaseName("IX_Customer_TaxId")
            .HasFilter("\"TaxId\" IS NOT NULL");
        builder.HasIndex(c => c.CompanyName).HasDatabaseName("IX_Customer_CompanyName");
    }
}
