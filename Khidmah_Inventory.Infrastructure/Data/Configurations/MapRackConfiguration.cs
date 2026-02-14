using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class MapRackConfiguration : IEntityTypeConfiguration<MapRack>
{
    public void Configure(EntityTypeBuilder<MapRack> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(r => r.CompanyId);
        builder.HasIndex(r => r.MapAisleId);
        builder.HasQueryFilter(r => !r.IsDeleted);
        builder.HasMany(r => r.Bins)
            .WithOne(b => b.Rack)
            .HasForeignKey(b => b.MapRackId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
