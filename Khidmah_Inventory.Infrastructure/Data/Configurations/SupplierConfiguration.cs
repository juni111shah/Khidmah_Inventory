using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Code)
            .HasMaxLength(50);

        builder.HasIndex(s => s.CompanyId);
        builder.HasIndex(s => new { s.CompanyId, s.Code })
            .IsUnique()
            .HasFilter("[Code] IS NOT NULL AND [IsDeleted] = 0");

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}

