using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Calculations;
using Khidmah_Inventory.Application.Common.Interfaces;

namespace Khidmah_Inventory.Infrastructure.Services.Calculations;

public class LastPurchasePriceCostingStrategy : ICostingStrategy
{
    private readonly IApplicationDbContext _context;

    public LastPurchasePriceCostingStrategy(IApplicationDbContext context)
    {
        _context = context;
    }

    public string Name => "LastPurchasePrice";

    public async Task<decimal?> GetUnitCostAsync(
        Guid companyId,
        Guid productId,
        Guid? warehouseId,
        DateTime? asOfDate,
        CancellationToken cancellationToken = default)
    {
        var date = asOfDate ?? DateTime.UtcNow;

        // Last purchase: from PO items by join to PurchaseOrder
        var lastPoPrice = await (
            from item in _context.PurchaseOrderItems.AsNoTracking()
            join po in _context.PurchaseOrders.AsNoTracking() on item.PurchaseOrderId equals po.Id
            where item.CompanyId == companyId && item.ProductId == productId && !po.IsDeleted && po.OrderDate <= date
            orderby po.OrderDate descending
            select (decimal?)item.UnitPrice
        ).FirstOrDefaultAsync(cancellationToken);

        if (lastPoPrice.HasValue && lastPoPrice.Value > 0)
            return lastPoPrice;

        // Fallback: product's purchase price
        var product = await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == productId && p.CompanyId == companyId && !p.IsDeleted)
            .Select(p => new { p.PurchasePrice, p.CostPrice })
            .FirstOrDefaultAsync(cancellationToken);

        if (product == null) return null;
        if (product.CostPrice.HasValue && product.CostPrice.Value > 0) return product.CostPrice;
        return product.PurchasePrice;
    }
}
