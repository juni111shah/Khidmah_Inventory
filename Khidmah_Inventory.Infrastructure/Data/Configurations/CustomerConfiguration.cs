using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Code)
            .HasMaxLength(50);

        builder.HasIndex(c => c.CompanyId);
        builder.HasIndex(c => new { c.CompanyId, c.Code })
            .IsUnique()
            .HasFilter("[Code] IS NOT NULL AND [IsDeleted] = 0");

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}

