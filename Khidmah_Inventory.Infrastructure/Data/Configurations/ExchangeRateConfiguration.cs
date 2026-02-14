using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
{
    public void Configure(EntityTypeBuilder<ExchangeRate> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Rate).HasPrecision(18, 8);

        builder.HasIndex(r => r.CompanyId);
        builder.HasIndex(r => new { r.CompanyId, r.Date });
        builder.HasIndex(r => new { r.FromCurrencyId, r.ToCurrencyId, r.Date });

        builder.HasOne(r => r.FromCurrency)
            .WithMany()
            .HasForeignKey(r => r.FromCurrencyId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(r => r.ToCurrency)
            .WithMany()
            .HasForeignKey(r => r.ToCurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}
