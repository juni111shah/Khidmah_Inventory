using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class CustomReportConfiguration : IEntityTypeConfiguration<CustomReport>
{
    public void Configure(EntityTypeBuilder<CustomReport> builder)
    {
        builder.ToTable("CustomReports");

        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(cr => cr.Description)
            .HasMaxLength(1000);

        builder.Property(cr => cr.ReportType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(cr => cr.ReportDefinition)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        // Relationships
        builder.HasOne(cr => cr.CreatedByUser)
            .WithMany()
            .HasForeignKey(cr => cr.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(cr => new { cr.CompanyId, cr.ReportType });
        builder.HasIndex(cr => new { cr.CompanyId, cr.CreatedByUserId });
    }
}

