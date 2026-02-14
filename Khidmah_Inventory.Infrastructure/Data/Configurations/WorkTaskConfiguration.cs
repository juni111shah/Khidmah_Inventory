using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;
using Khidmah_Inventory.Domain.Enums;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class WorkTaskConfiguration : IEntityTypeConfiguration<WorkTask>
{
    public void Configure(EntityTypeBuilder<WorkTask> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.AssignedToType).HasConversion<string>().HasMaxLength(10);
        builder.Property(t => t.LocationCode).HasMaxLength(100);
        builder.Property(t => t.ProductName).HasMaxLength(500);
        builder.Property(t => t.ProductSku).HasMaxLength(100);
        builder.Property(t => t.Quantity).HasPrecision(18, 4);
        builder.Property(t => t.Notes).HasMaxLength(2000);
        builder.HasIndex(t => t.CompanyId);
        builder.HasIndex(t => t.WarehouseId);
        builder.HasIndex(t => new { t.Status, t.Priority });
        builder.HasIndex(t => t.AssignedToId);
        builder.HasQueryFilter(t => !t.IsDeleted);
        builder.HasOne(t => t.Warehouse)
            .WithMany()
            .HasForeignKey(t => t.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.MapBin)
            .WithMany()
            .HasForeignKey(t => t.MapBinId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(t => t.Product)
            .WithMany()
            .HasForeignKey(t => t.ProductId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
