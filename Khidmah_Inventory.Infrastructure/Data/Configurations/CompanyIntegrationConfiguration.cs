using Khidmah_Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class CompanyIntegrationConfiguration : IEntityTypeConfiguration<CompanyIntegration>
{
    public void Configure(EntityTypeBuilder<CompanyIntegration> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.IntegrationType).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ConfigJson).HasColumnType("nvarchar(max)");
        builder.Property(x => x.DisplayName).HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.HasIndex(x => new { x.CompanyId, x.IntegrationType }).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
