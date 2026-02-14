using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class MapZoneConfiguration : IEntityTypeConfiguration<MapZone>
{
    public void Configure(EntityTypeBuilder<MapZone> builder)
    {
        builder.HasKey(z => z.Id);
        builder.Property(z => z.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(z => z.CompanyId);
        builder.HasIndex(z => z.WarehouseMapId);
        builder.HasQueryFilter(z => !z.IsDeleted);
        builder.HasMany(z => z.Aisles)
            .WithOne(a => a.Zone)
            .HasForeignKey(a => a.MapZoneId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
