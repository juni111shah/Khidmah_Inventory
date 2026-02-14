using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class MapAisleConfiguration : IEntityTypeConfiguration<MapAisle>
{
    public void Configure(EntityTypeBuilder<MapAisle> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(a => a.CompanyId);
        builder.HasIndex(a => a.MapZoneId);
        builder.HasQueryFilter(a => !a.IsDeleted);
        builder.HasMany(a => a.Racks)
            .WithOne(r => r.Aisle)
            .HasForeignKey(r => r.MapAisleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
