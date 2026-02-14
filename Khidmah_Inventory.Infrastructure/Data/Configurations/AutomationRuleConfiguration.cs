using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class AutomationRuleConfiguration : IEntityTypeConfiguration<AutomationRule>
{
    public void Configure(EntityTypeBuilder<AutomationRule> builder)
    {
        builder.ToTable("AutomationRules");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Trigger).IsRequired().HasMaxLength(100);
        builder.Property(r => r.ConditionJson).HasColumnType("nvarchar(max)");
        builder.Property(r => r.ActionJson).IsRequired().HasColumnType("nvarchar(max)");
        builder.HasIndex(r => new { r.CompanyId, r.Trigger, r.IsActive });
    }
}
