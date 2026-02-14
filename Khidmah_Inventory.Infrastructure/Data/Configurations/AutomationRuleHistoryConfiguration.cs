using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class AutomationRuleHistoryConfiguration : IEntityTypeConfiguration<AutomationRuleHistory>
{
    public void Configure(EntityTypeBuilder<AutomationRuleHistory> builder)
    {
        builder.ToTable("AutomationRuleHistories");
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Trigger).IsRequired().HasMaxLength(100);
        builder.Property(h => h.ActionExecuted).IsRequired().HasMaxLength(200);
        builder.Property(h => h.TriggerContextJson).HasColumnType("nvarchar(max)");
        builder.Property(h => h.ErrorMessage).HasMaxLength(2000);
        builder.HasOne(h => h.AutomationRule).WithMany().HasForeignKey(h => h.AutomationRuleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(h => new { h.CompanyId, h.AutomationRuleId, h.CreatedAt });
    }
}
