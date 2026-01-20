using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class BatchConfiguration : IEntityTypeConfiguration<Batch>
{
    public void Configure(EntityTypeBuilder<Batch> builder)
    {
        builder.ToTable("Batches");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.BatchNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.LotNumber)
            .HasMaxLength(100);

        builder.Property(b => b.Quantity)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(b => b.UnitCost)
            .HasPrecision(18, 2);

        builder.Property(b => b.SupplierName)
            .HasMaxLength(200);

        builder.Property(b => b.SupplierBatchNumber)
            .HasMaxLength(100);

        builder.Property(b => b.Notes)
            .HasMaxLength(1000);

        builder.Property(b => b.RecallReason)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(b => b.Product)
            .WithMany()
            .HasForeignKey(b => b.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Warehouse)
            .WithMany()
            .HasForeignKey(b => b.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(b => new { b.CompanyId, b.ProductId, b.BatchNumber })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(b => new { b.CompanyId, b.ExpiryDate });
        builder.HasIndex(b => new { b.CompanyId, b.IsRecalled });
    }
}

