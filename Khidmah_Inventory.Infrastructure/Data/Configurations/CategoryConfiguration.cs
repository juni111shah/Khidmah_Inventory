using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.Code)
            .HasMaxLength(50);

        builder.Property(c => c.ImageUrl)
            .HasMaxLength(500);

        builder.Property(c => c.DisplayOrder)
            .HasDefaultValue(0);

        builder.HasIndex(c => c.CompanyId);
        builder.HasIndex(c => new { c.CompanyId, c.Code })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0 AND [Code] IS NOT NULL");

        // Self-referencing relationship for hierarchical categories
        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}

