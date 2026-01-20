using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.OrderNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.CompanyId);
        builder.HasIndex(p => new { p.CompanyId, p.OrderNumber })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasOne(p => p.Supplier)
            .WithMany(s => s.PurchaseOrders)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Items)
            .WithOne(i => i.PurchaseOrder)
            .HasForeignKey(i => i.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}

