using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Calculations;
using Khidmah_Inventory.Application.Common.Interfaces;

namespace Khidmah_Inventory.Infrastructure.Services.Calculations;

public class AverageCostCostingStrategy : ICostingStrategy
{
    private readonly IApplicationDbContext _context;

    public AverageCostCostingStrategy(IApplicationDbContext context)
    {
        _context = context;
    }

    public string Name => "AverageCost";

    public async Task<decimal?> GetUnitCostAsync(
        Guid companyId,
        Guid productId,
        Guid? warehouseId,
        DateTime? asOfDate,
        CancellationToken cancellationToken = default)
    {
        // Prefer StockLevel.AverageCost when available (per warehouse)
        var query = _context.StockLevels
            .AsNoTracking()
            .Where(sl => sl.CompanyId == companyId && sl.ProductId == productId && sl.Quantity > 0);

        if (warehouseId.HasValue)
            query = query.Where(sl => sl.WarehouseId == warehouseId.Value);

        var levels = await query.Select(sl => new { sl.Quantity, sl.AverageCost }).ToListAsync(cancellationToken);
        if (levels.Count == 0)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Where(p => p.Id == productId && p.CompanyId == companyId && !p.IsDeleted)
                .Select(p => new { p.CostPrice, p.PurchasePrice })
                .FirstOrDefaultAsync(cancellationToken);
            return product?.CostPrice ?? product?.PurchasePrice;
        }

        decimal totalQty = 0;
        decimal totalValue = 0;
        foreach (var l in levels)
        {
            var cost = l.AverageCost ?? 0;
            totalQty += l.Quantity;
            totalValue += l.Quantity * cost;
        }
        if (totalQty <= 0) return null;
        return totalValue / totalQty;
    }
}
