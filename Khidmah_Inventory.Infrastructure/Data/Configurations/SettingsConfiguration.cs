using Khidmah_Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class SettingsConfiguration : IEntityTypeConfiguration<Settings>
{
    public void Configure(EntityTypeBuilder<Settings> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.SettingsType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.SettingsKey)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.JsonData)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        // Index for faster lookups
        builder.HasIndex(s => new { s.CompanyId, s.SettingsType, s.SettingsKey })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        // Filter out deleted settings
        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}

