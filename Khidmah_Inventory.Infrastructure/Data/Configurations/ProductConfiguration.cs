using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.SKU)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Barcode)
            .HasMaxLength(100);

        builder.HasIndex(p => new { p.CompanyId, p.SKU })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(p => new { p.CompanyId, p.Barcode })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0 AND [Barcode] IS NOT NULL");

        builder.HasIndex(p => p.CompanyId);

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.UnitOfMeasure)
            .WithMany()
            .HasForeignKey(p => p.UnitOfMeasureId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

