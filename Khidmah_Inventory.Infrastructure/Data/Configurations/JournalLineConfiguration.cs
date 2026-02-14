using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class JournalLineConfiguration : IEntityTypeConfiguration<JournalLine>
{
    public void Configure(EntityTypeBuilder<JournalLine> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Debit).HasPrecision(18, 4);
        builder.Property(l => l.Credit).HasPrecision(18, 4);
        builder.Property(l => l.BaseDebit).HasPrecision(18, 4);
        builder.Property(l => l.BaseCredit).HasPrecision(18, 4);
        builder.Property(l => l.Memo).HasMaxLength(500);

        builder.HasOne(l => l.Account)
            .WithMany(a => a.JournalLines)
            .HasForeignKey(l => l.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(l => !l.IsDeleted);
    }
}
