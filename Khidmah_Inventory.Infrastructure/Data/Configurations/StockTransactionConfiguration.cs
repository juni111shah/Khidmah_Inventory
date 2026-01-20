using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Configurations;

public class StockTransactionConfiguration : IEntityTypeConfiguration<StockTransaction>
{
    public void Configure(EntityTypeBuilder<StockTransaction> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.TransactionType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.ReferenceType)
            .HasMaxLength(50);

        builder.Property(s => s.ReferenceNumber)
            .HasMaxLength(100);

        builder.HasIndex(s => s.CompanyId);
        builder.HasIndex(s => s.ProductId);
        builder.HasIndex(s => s.WarehouseId);
        builder.HasIndex(s => s.TransactionDate);

        builder.HasOne(s => s.Product)
            .WithMany(p => p.StockTransactions)
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Warehouse)
            .WithMany(w => w.StockTransactions)
            .HasForeignKey(s => s.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

