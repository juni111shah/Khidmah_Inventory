using Khidmah_Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class ApiKeyUsageLogConfiguration : IEntityTypeConfiguration<ApiKeyUsageLog>
{
    public void Configure(EntityTypeBuilder<ApiKeyUsageLog> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Method).IsRequired().HasMaxLength(10);
        builder.Property(x => x.Path).IsRequired().HasMaxLength(500);
        builder.HasIndex(x => new { x.ApiKeyId, x.CreatedAt });
        builder.HasIndex(x => x.CompanyId);
    }
}
