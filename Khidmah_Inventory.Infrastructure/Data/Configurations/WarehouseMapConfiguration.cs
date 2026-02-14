using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class WarehouseMapConfiguration : IEntityTypeConfiguration<WarehouseMap>
{
    public void Configure(EntityTypeBuilder<WarehouseMap> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(w => w.CompanyId);
        builder.HasIndex(w => w.WarehouseId);
        builder.HasQueryFilter(w => !w.IsDeleted);
        builder.HasOne(w => w.Warehouse)
            .WithMany()
            .HasForeignKey(w => w.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(w => w.Zones)
            .WithOne(z => z.WarehouseMap)
            .HasForeignKey(z => z.WarehouseMapId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
