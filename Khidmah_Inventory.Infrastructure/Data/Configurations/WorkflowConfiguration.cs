using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
{
    public void Configure(EntityTypeBuilder<Workflow> builder)
    {
        builder.ToTable("Workflows");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Description)
            .HasMaxLength(1000);

        builder.Property(w => w.EntityType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(w => w.WorkflowDefinition)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        // Indexes
        builder.HasIndex(w => new { w.CompanyId, w.EntityType, w.IsActive });
    }
}

public class WorkflowInstanceConfiguration : IEntityTypeConfiguration<WorkflowInstance>
{
    public void Configure(EntityTypeBuilder<WorkflowInstance> builder)
    {
        builder.ToTable("WorkflowInstances");

        builder.HasKey(wi => wi.Id);

        builder.Property(wi => wi.EntityType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(wi => wi.CurrentStep)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(wi => wi.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(wi => wi.Comments)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(wi => wi.Workflow)
            .WithMany(w => w.Instances)
            .HasForeignKey(wi => wi.WorkflowId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(wi => wi.CurrentAssignee)
            .WithMany()
            .HasForeignKey(wi => wi.CurrentAssigneeId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(wi => new { wi.CompanyId, wi.EntityType, wi.EntityId });
        builder.HasIndex(wi => new { wi.CompanyId, wi.Status });
        builder.HasIndex(wi => new { wi.CompanyId, wi.CurrentAssigneeId });
    }
}

public class WorkflowHistoryConfiguration : IEntityTypeConfiguration<WorkflowHistory>
{
    public void Configure(EntityTypeBuilder<WorkflowHistory> builder)
    {
        builder.ToTable("WorkflowHistory");

        builder.HasKey(wh => wh.Id);

        builder.Property(wh => wh.Step)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(wh => wh.Action)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(wh => wh.UserName)
            .HasMaxLength(200);

        builder.Property(wh => wh.Comments)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(wh => wh.WorkflowInstance)
            .WithMany(wi => wi.History)
            .HasForeignKey(wh => wh.WorkflowInstanceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wh => wh.User)
            .WithMany()
            .HasForeignKey(wh => wh.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(wh => new { wh.CompanyId, wh.WorkflowInstanceId });
        builder.HasIndex(wh => new { wh.CompanyId, wh.CreatedAt });
    }
}

