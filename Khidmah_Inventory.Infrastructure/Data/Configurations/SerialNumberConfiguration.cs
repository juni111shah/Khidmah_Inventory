using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class SerialNumberConfiguration : IEntityTypeConfiguration<SerialNumber>
{
    public void Configure(EntityTypeBuilder<SerialNumber> builder)
    {
        builder.ToTable("SerialNumbers");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SerialNumberValue)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.BatchNumber)
            .HasMaxLength(100);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.WarrantyExpiryDate)
            .HasMaxLength(50);

        builder.Property(s => s.Notes)
            .HasMaxLength(1000);

        builder.Property(s => s.RecallReason)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(s => s.Product)
            .WithMany()
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Warehouse)
            .WithMany()
            .HasForeignKey(s => s.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Batch)
            .WithMany()
            .HasForeignKey(s => s.BatchId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.SalesOrder)
            .WithMany()
            .HasForeignKey(s => s.SalesOrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(s => s.SalesOrderItem)
            .WithMany()
            .HasForeignKey(s => s.SalesOrderItemId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(s => s.Customer)
            .WithMany()
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(s => new { s.CompanyId, s.SerialNumberValue })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(s => new { s.CompanyId, s.ProductId, s.Status });
        builder.HasIndex(s => new { s.CompanyId, s.BatchId });
        builder.HasIndex(s => new { s.CompanyId, s.ExpiryDate });
        builder.HasIndex(s => new { s.CompanyId, s.IsRecalled });
        builder.HasIndex(s => new { s.CompanyId, s.CustomerId });
    }
}

