using Khidmah_Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class WebhookConfiguration : IEntityTypeConfiguration<Webhook>
{
    public void Configure(EntityTypeBuilder<Webhook> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Url).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.Secret).HasMaxLength(256);
        builder.Property(x => x.Events).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.HasIndex(x => x.CompanyId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
