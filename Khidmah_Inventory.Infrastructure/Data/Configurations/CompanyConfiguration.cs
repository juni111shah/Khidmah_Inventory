using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Email)
            .HasMaxLength(256);

        builder.HasIndex(c => c.Name);

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasMany(c => c.UserCompanies)
            .WithOne(uc => uc.Company)
            .HasForeignKey(uc => uc.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

