using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using ErpCrmService.Domain.Entities;

namespace ErpCrmService.Infrastructure.Configuration
{
    /// <summary>
    /// Entity Framework configuration for Customer entity
    /// Implements Fluent API configuration following EF best practices
    /// </summary>
    public class CustomerConfiguration : EntityTypeConfiguration<Customer>
    {
        public CustomerConfiguration()
        {
            // Table configuration
            ToTable("Customers");
            HasKey(c => c.Id);

            // Properties configuration
            Property(c => c.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(c => c.CompanyName)
                .IsRequired()
                .HasMaxLength(100);

            Property(c => c.ContactFirstName)
                .IsRequired()
                .HasMaxLength(50);

            Property(c => c.ContactLastName)
                .IsRequired()
                .HasMaxLength(50);

            // Email value object configuration
            Property(c => c.Email.Value)
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnName("Email");

            // Phone value object configuration (optional)
            Property(c => c.Phone.Value)
                .IsOptional()
                .HasMaxLength(20)
                .HasColumnName("Phone");

            // Address value object configuration
            Property(c => c.Address.Street)
                .IsOptional()
                .HasMaxLength(200)
                .HasColumnName("AddressStreet");

            Property(c => c.Address.City)
                .IsOptional()
                .HasMaxLength(100)
                .HasColumnName("AddressCity");

            Property(c => c.Address.State)
                .IsOptional()
                .HasMaxLength(100)
                .HasColumnName("AddressState");

            Property(c => c.Address.PostalCode)
                .IsOptional()
                .HasMaxLength(20)
                .HasColumnName("AddressPostalCode");

            Property(c => c.Address.Country)
                .IsOptional()
                .HasMaxLength(100)
                .HasColumnName("AddressCountry");

            Property(c => c.TaxId)
                .IsOptional()
                .HasMaxLength(20);

            Property(c => c.Status)
                .IsRequired();

            Property(c => c.Type)
                .IsRequired();

            Property(c => c.Notes)
                .IsOptional()
                .HasMaxLength(500);

            Property(c => c.CreditLimit)
                .IsRequired()
                .HasPrecision(18, 2);

            Property(c => c.CurrentBalance)
                .IsRequired()
                .HasPrecision(18, 2);

            // Base entity properties
            Property(c => c.CreatedAt)
                .IsRequired();

            Property(c => c.UpdatedAt)
                .IsOptional();

            Property(c => c.CreatedBy)
                .IsOptional()
                .HasMaxLength(100);

            Property(c => c.UpdatedBy)
                .IsOptional()
                .HasMaxLength(100);

            Property(c => c.IsActive)
                .IsRequired();

            // Indexes for better performance
            HasIndex(c => c.Email.Value)
                .IsUnique()
                .HasName("IX_Customer_Email");

            HasIndex(c => c.TaxId)
                .IsUnique()
                .HasName("IX_Customer_TaxId");

            HasIndex(c => c.CompanyName)
                .HasName("IX_Customer_CompanyName");

            HasIndex(c => new { c.Status, c.IsActive })
                .HasName("IX_Customer_Status_Active");
        }
    }
}