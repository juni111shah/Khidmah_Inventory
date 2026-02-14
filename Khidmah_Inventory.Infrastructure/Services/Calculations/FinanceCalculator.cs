using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Calculations;
using Khidmah_Inventory.Application.Common.Interfaces;

namespace Khidmah_Inventory.Infrastructure.Services.Calculations;

public class FinanceCalculator : IFinanceCalculator
{
    private readonly IApplicationDbContext _context;
    private readonly ICostingStrategy _costingStrategy;

    public FinanceCalculator(IApplicationDbContext context, ICostingStrategy costingStrategy)
    {
        _context = context;
        _costingStrategy = costingStrategy;
    }

    public async Task<decimal> GetRevenueAsync(CalculationContext ctx, CancellationToken cancellationToken = default)
    {
        var q = _context.SalesOrders.AsNoTracking()
            .Where(so => so.CompanyId == ctx.CompanyId && !so.IsDeleted)
            .Where(so => so.Status != "Cancelled" && so.Status != "Draft");

        if (ctx.DateFrom.HasValue) q = q.Where(so => so.OrderDate >= ctx.DateFrom.Value);
        if (ctx.DateTo.HasValue) q = q.Where(so => so.OrderDate <= ctx.DateTo.Value);

        var sum = await q.SumAsync(so => so.TotalAmount, cancellationToken);
        return sum;
    }

    public async Task<decimal> GetCogsAsync(CalculationContext ctx, CancellationToken cancellationToken = default)
    {
        var orderIds = await _context.SalesOrders.AsNoTracking()
            .Where(so => so.CompanyId == ctx.CompanyId && !so.IsDeleted && so.Status != "Cancelled" && so.Status != "Draft")
            .Where(so => ctx.DateFrom == null || so.OrderDate >= ctx.DateFrom.Value)
            .Where(so => ctx.DateTo == null || so.OrderDate <= ctx.DateTo.Value)
            .Select(so => so.Id)
            .ToListAsync(cancellationToken);

        if (orderIds.Count == 0) return 0;

        var itemsWithDate = await (
            from i in _context.SalesOrderItems.AsNoTracking()
            join so in _context.SalesOrders.AsNoTracking() on i.SalesOrderId equals so.Id
            where orderIds.Contains(i.SalesOrderId)
            select new { i.ProductId, i.Quantity, OrderDate = so.OrderDate }
        ).ToListAsync(cancellationToken);

        decimal cogs = 0;
        foreach (var line in itemsWithDate)
        {
            var unitCost = await _costingStrategy.GetUnitCostAsync(ctx.CompanyId, line.ProductId, ctx.WarehouseId, line.OrderDate, cancellationToken);
            cogs += line.Quantity * (unitCost ?? 0);
        }
        return cogs;
    }

    public async Task<decimal> GetGrossProfitAsync(CalculationContext ctx, CancellationToken cancellationToken = default)
    {
        var revenue = await GetRevenueAsync(ctx, cancellationToken);
        var cogs = await GetCogsAsync(ctx, cancellationToken);
        return revenue - cogs;
    }

    public async Task<decimal?> GetGrossMarginPercentAsync(CalculationContext ctx, CancellationToken cancellationToken = default)
    {
        var revenue = await GetRevenueAsync(ctx, cancellationToken);
        if (revenue == 0) return null;
        var grossProfit = await GetGrossProfitAsync(ctx, cancellationToken);
        return (grossProfit / revenue) * 100;
    }

    public async Task<decimal> GetPurchaseSpendAsync(CalculationContext ctx, CancellationToken cancellationToken = default)
    {
        var q = _context.PurchaseOrders.AsNoTracking()
            .Where(po => po.CompanyId == ctx.CompanyId && !po.IsDeleted)
            .Where(po => po.Status != "Cancelled");

        if (ctx.DateFrom.HasValue) q = q.Where(po => po.OrderDate >= ctx.DateFrom.Value);
        if (ctx.DateTo.HasValue) q = q.Where(po => po.OrderDate <= ctx.DateTo.Value);

        return await q.SumAsync(po => po.TotalAmount, cancellationToken);
    }

    public async Task<decimal?> GetAverageOrderValueAsync(CalculationContext ctx, CancellationToken cancellationToken = default)
    {
        var q = _context.SalesOrders.AsNoTracking()
            .Where(so => so.CompanyId == ctx.CompanyId && !so.IsDeleted && so.Status != "Cancelled" && so.Status != "Draft");
        if (ctx.DateFrom.HasValue) q = q.Where(so => so.OrderDate >= ctx.DateFrom.Value);
        if (ctx.DateTo.HasValue) q = q.Where(so => so.OrderDate <= ctx.DateTo.Value);

        var count = await q.CountAsync(cancellationToken);
        if (count == 0) return null;
        var revenue = await GetRevenueAsync(ctx, cancellationToken);
        return revenue / count;
    }

    public async Task<decimal?> GetSalesGrowthPercentAsync(CalculationContext currentPeriod, CalculationContext previousPeriod, CancellationToken cancellationToken = default)
    {
        var current = await GetRevenueAsync(currentPeriod, cancellationToken);
        var previous = await GetRevenueAsync(previousPeriod, cancellationToken);
        if (previous == 0) return null;
        return ((current - previous) / previous) * 100;
    }
}
