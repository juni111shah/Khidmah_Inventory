using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.HasKey(j => j.Id);

        builder.Property(j => j.Reference).IsRequired().HasMaxLength(100);
        builder.Property(j => j.SourceModule).IsRequired().HasMaxLength(50);
        builder.Property(j => j.Description).HasMaxLength(500);
        builder.Property(j => j.ConversionRateToBase).HasPrecision(18, 8);

        builder.HasIndex(j => j.CompanyId);
        builder.HasIndex(j => new { j.CompanyId, j.Date });
        builder.HasIndex(j => new { j.SourceModule, j.SourceId });

        builder.HasMany(j => j.Lines)
            .WithOne(l => l.JournalEntry)
            .HasForeignKey(l => l.JournalEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(j => !j.IsDeleted);
    }
}
