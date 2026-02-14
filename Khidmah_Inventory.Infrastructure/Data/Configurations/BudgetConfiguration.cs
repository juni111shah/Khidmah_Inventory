using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.PlannedAmount).HasPrecision(18, 4);
        builder.Property(b => b.MonthlyAmountsJson).HasMaxLength(2000);
        builder.Property(b => b.Notes).HasMaxLength(500);

        builder.HasIndex(b => new { b.CompanyId, b.Year, b.AccountId }).IsUnique().HasFilter("[IsDeleted] = 0");

        builder.HasOne(b => b.Account)
            .WithMany()
            .HasForeignKey(b => b.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(b => !b.IsDeleted);
    }
}
