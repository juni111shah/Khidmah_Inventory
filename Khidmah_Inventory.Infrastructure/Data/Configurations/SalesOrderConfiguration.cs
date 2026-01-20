using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.OrderNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(s => s.CompanyId);
        builder.HasIndex(s => new { s.CompanyId, s.OrderNumber })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasOne(s => s.Customer)
            .WithMany(c => c.SalesOrders)
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Items)
            .WithOne(i => i.SalesOrder)
            .HasForeignKey(i => i.SalesOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}

