using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Calculations;
using Khidmah_Inventory.Application.Common.Calculations.Dto;
using Khidmah_Inventory.Application.Common.Interfaces;

namespace Khidmah_Inventory.Infrastructure.Services.Calculations;

public class InventoryCalculator : IInventoryCalculator
{
    private readonly IApplicationDbContext _context;
    private readonly IFinanceCalculator _financeCalculator;
    private readonly ICostingStrategy _costingStrategy;

    public InventoryCalculator(
        IApplicationDbContext context,
        IFinanceCalculator financeCalculator,
        ICostingStrategy costingStrategy)
    {
        _context = context;
        _financeCalculator = financeCalculator;
        _costingStrategy = costingStrategy;
    }

    public async Task<decimal> GetStockValueAsync(CalculationContext ctx, CancellationToken cancellationToken = default)
    {
        var query = _context.StockLevels.AsNoTracking()
            .Where(sl => sl.CompanyId == ctx.CompanyId && sl.Quantity > 0);
        if (ctx.WarehouseId.HasValue) query = query.Where(sl => sl.WarehouseId == ctx.WarehouseId.Value);
        if (ctx.ProductId.HasValue) query = query.Where(sl => sl.ProductId == ctx.ProductId.Value);

        var levels = await query.Select(sl => new { sl.ProductId, sl.WarehouseId, sl.Quantity, sl.AverageCost }).ToListAsync(cancellationToken);
        decimal total = 0;
        var asOf = ctx.DateTo ?? DateTime.UtcNow;
        foreach (var l in levels)
        {
            var cost = l.AverageCost ?? await _costingStrategy.GetUnitCostAsync(ctx.CompanyId, l.ProductId, l.WarehouseId, asOf, cancellationToken) ?? 0;
            total += l.Quantity * cost;
        }
        return total;
    }

    public async Task<decimal?> GetInventoryTurnoverAsync(CalculationContext ctx, CancellationToken cancellationToken = default)
    {
        var cogs = await _financeCalculator.GetCogsAsync(ctx, cancellationToken);
        var avgInventory = await GetStockValueAsync(ctx, cancellationToken);
        if (avgInventory == 0) return null;
        return cogs / avgInventory;
    }

    public async Task<decimal?> GetDaysOfInventoryAsync(CalculationContext ctx, CancellationToken cancellationToken = default)
    {
        var turnover = await GetInventoryTurnoverAsync(ctx, cancellationToken);
        if (turnover == null || turnover == 0) return null;
        return 365 / turnover.Value;
    }

    public async Task<decimal?> GetSellThroughRateAsync(CalculationContext ctx, CancellationToken cancellationToken = default)
    {
        if (!ctx.DateFrom.HasValue || !ctx.DateTo.HasValue) return null;

        var soldQty = await (
            from i in _context.SalesOrderItems.AsNoTracking()
            join so in _context.SalesOrders.AsNoTracking() on i.SalesOrderId equals so.Id
            where i.CompanyId == ctx.CompanyId && so.CompanyId == ctx.CompanyId && !so.IsDeleted
                && so.Status != "Cancelled" && so.Status != "Draft"
                && so.OrderDate >= ctx.DateFrom && so.OrderDate <= ctx.DateTo
            select i.Quantity
        ).SumAsync(cancellationToken);

        // Received qty: StockIn in period or PO received in period
        var receivedQty = await _context.StockTransactions.AsNoTracking()
            .Where(st => st.CompanyId == ctx.CompanyId && st.TransactionType == "StockIn")
            .Where(st => st.TransactionDate >= ctx.DateFrom && st.TransactionDate <= ctx.DateTo)
            .SumAsync(st => st.Quantity, cancellationToken);

        if (receivedQty == 0) return null;
        return soldQty / receivedQty;
    }

    public async Task<int> GetDeadStockCountAsync(CalculationContext ctx, CancellationToken cancellationToken = default)
    {
        var days = ctx.DeadStockDays ?? 90;
        var cutoff = (ctx.DateTo ?? DateTime.UtcNow).AddDays(-days);

        var productIdsWithStock = await _context.StockLevels.AsNoTracking()
            .Where(sl => sl.CompanyId == ctx.CompanyId && sl.Quantity > 0)
            .Select(sl => sl.ProductId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (productIdsWithStock.Count == 0) return 0;

        var soldInPeriod = await (
            from i in _context.SalesOrderItems.AsNoTracking()
            join so in _context.SalesOrders.AsNoTracking() on i.SalesOrderId equals so.Id
            where i.CompanyId == ctx.CompanyId && !so.IsDeleted && so.OrderDate >= cutoff
            select i.ProductId
        ).Distinct().ToListAsync(cancellationToken);

        var dead = productIdsWithStock.Except(soldInPeriod).Count();
        return dead;
    }

    public async Task<StockAgingBucketsDto> GetStockAgingBucketsAsync(CalculationContext ctx, CancellationToken cancellationToken = default)
    {
        var asOf = ctx.DateTo ?? DateTime.UtcNow;
        var query = _context.StockLevels.AsNoTracking()
            .Where(sl => sl.CompanyId == ctx.CompanyId && sl.Quantity > 0);
        if (ctx.WarehouseId.HasValue) query = query.Where(sl => sl.WarehouseId == ctx.WarehouseId.Value);
        if (ctx.ProductId.HasValue) query = query.Where(sl => sl.ProductId == ctx.ProductId.Value);

        var levels = await query.Select(sl => new { sl.ProductId, sl.WarehouseId, sl.Quantity, sl.AverageCost }).ToListAsync(cancellationToken);

        // Last movement per product: from StockTransaction
        var lastMovement = await _context.StockTransactions.AsNoTracking()
            .Where(st => st.CompanyId == ctx.CompanyId)
            .GroupBy(st => st.ProductId)
            .Select(g => new { ProductId = g.Key, LastDate = g.Max(st => st.TransactionDate) })
            .ToListAsync(cancellationToken);
        var lastMovementDict = lastMovement.ToDictionary(x => x.ProductId, x => x.LastDate);

        decimal d0_30 = 0, d30_60 = 0, d60_90 = 0, d90 = 0;
        foreach (var l in levels)
        {
            var daysSince = lastMovementDict.TryGetValue(l.ProductId, out var last) ? (asOf - last).TotalDays : (double)365;
            var cost = l.AverageCost ?? await _costingStrategy.GetUnitCostAsync(ctx.CompanyId, l.ProductId, l.WarehouseId, asOf, cancellationToken) ?? 0;
            var value = (decimal)l.Quantity * cost;
            if (daysSince <= 30) d0_30 += value;
            else if (daysSince <= 60) d30_60 += value;
            else if (daysSince <= 90) d60_90 += value;
            else d90 += value;
        }

        return new StockAgingBucketsDto { Days0To30 = d0_30, Days30To60 = d30_60, Days60To90 = d60_90, Days90Plus = d90 };
    }
}
