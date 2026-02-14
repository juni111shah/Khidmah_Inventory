using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class MapBinConfiguration : IEntityTypeConfiguration<MapBin>
{
    public void Configure(EntityTypeBuilder<MapBin> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(b => b.CompanyId);
        builder.HasIndex(b => b.MapRackId);
        builder.HasQueryFilter(b => !b.IsDeleted);
    }
}
