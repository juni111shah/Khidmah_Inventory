using Khidmah_Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class ScheduledReportConfiguration : IEntityTypeConfiguration<ScheduledReport>
{
    public void Configure(EntityTypeBuilder<ScheduledReport> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.ReportType).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Frequency).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CronExpression).HasMaxLength(100);
        builder.Property(x => x.RecipientsJson).IsRequired().HasMaxLength(4000);
        builder.Property(x => x.Format).HasMaxLength(20);
        builder.HasIndex(x => new { x.CompanyId, x.NextRunAt }).HasFilter("[IsActive] = 1 AND [IsDeleted] = 0");
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
