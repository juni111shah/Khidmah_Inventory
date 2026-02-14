using Khidmah_Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class WebhookDeliveryLogConfiguration : IEntityTypeConfiguration<WebhookDeliveryLog>
{
    public void Configure(EntityTypeBuilder<WebhookDeliveryLog> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EventName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.PayloadJson).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(x => x.ResponseBody).HasColumnType("nvarchar(max)");
        builder.Property(x => x.ErrorMessage).HasMaxLength(2000);
        builder.HasIndex(x => new { x.WebhookId, x.DeliveredAt });
        builder.HasIndex(x => x.CompanyId);
    }
}
