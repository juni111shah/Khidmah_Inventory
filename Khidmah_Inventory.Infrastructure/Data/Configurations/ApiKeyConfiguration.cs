using Khidmah_Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.KeyPrefix).IsRequired().HasMaxLength(32);
        builder.Property(x => x.KeyHash).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Permissions).IsRequired().HasMaxLength(2000);
        builder.HasIndex(x => new { x.CompanyId, x.KeyPrefix });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
