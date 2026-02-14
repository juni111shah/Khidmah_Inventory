using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Calculations;
using Khidmah_Inventory.Application.Common.Calculations.Dto;
using Khidmah_Inventory.Application.Common.Interfaces;

namespace Khidmah_Inventory.Infrastructure.Services.Calculations;

public class KpiCalculator : IKpiCalculator
{
    private readonly IApplicationDbContext _context;
    private readonly IFinanceCalculator _financeCalculator;
    private readonly IInventoryCalculator _inventoryCalculator;

    public KpiCalculator(
        IApplicationDbContext context,
        IFinanceCalculator financeCalculator,
        IInventoryCalculator inventoryCalculator)
    {
        _context = context;
        _financeCalculator = financeCalculator;
        _inventoryCalculator = inventoryCalculator;
    }

    public async Task<ExecutiveKpisDto> GetExecutiveKpisAsync(CalculationContext context, CancellationToken cancellationToken = default)
    {
        var todayStart = (context.DateTo ?? DateTime.UtcNow).Date;
        var todayEnd = todayStart.AddDays(1);
        var todayCtx = new CalculationContext { CompanyId = context.CompanyId, DateFrom = todayStart, DateTo = todayEnd, WarehouseId = context.WarehouseId };

        var revenueToday = await _financeCalculator.GetRevenueAsync(todayCtx, cancellationToken);
        var cogsToday = await _financeCalculator.GetCogsAsync(todayCtx, cancellationToken);
        var profitToday = revenueToday - cogsToday;

        var lowStockCount = await _context.Products.AsNoTracking()
            .Where(p => p.CompanyId == context.CompanyId && !p.IsDeleted && p.MinStockLevel.HasValue)
            .Where(p => _context.StockLevels.Any(sl => sl.CompanyId == context.CompanyId && sl.ProductId == p.Id && sl.Quantity < p.MinStockLevel!.Value))
            .CountAsync(cancellationToken);

        var pendingApprovals = await _context.WorkflowInstances.AsNoTracking()
            .Where(wi => wi.CompanyId == context.CompanyId && !wi.IsDeleted)
            .Where(wi => wi.Status == "Pending" || wi.Status == "InProgress")
            .CountAsync(cancellationToken);

        var deadCount = await _inventoryCalculator.GetDeadStockCountAsync(context, cancellationToken);

        var topProducts = await (
            from i in _context.SalesOrderItems.AsNoTracking()
            join so in _context.SalesOrders.AsNoTracking() on i.SalesOrderId equals so.Id
            join p in _context.Products.AsNoTracking() on i.ProductId equals p.Id
            where so.CompanyId == context.CompanyId && !so.IsDeleted && so.Status != "Cancelled" && so.Status != "Draft"
                && (context.DateFrom == null || so.OrderDate >= context.DateFrom)
                && (context.DateTo == null || so.OrderDate <= context.DateTo)
            group new { i.Quantity, i.LineTotal } by new { p.Id, p.Name, p.SKU } into g
            orderby g.Sum(x => x.LineTotal) descending
            select new TopProductKpiDto
            {
                ProductId = g.Key.Id,
                Name = g.Key.Name,
                SKU = g.Key.SKU,
                Revenue = g.Sum(x => x.LineTotal),
                QuantitySold = g.Sum(x => x.Quantity)
            }
        ).Take(10).ToListAsync(cancellationToken);

        return new ExecutiveKpisDto
        {
            RevenueToday = ToKpi("revenueToday", "Revenue today", revenueToday, unit: "currency", format: "currency"),
            ProfitToday = ToKpi("profitToday", "Profit today", profitToday, unit: "currency", format: "currency"),
            LowStockCount = ToKpi("lowStockCount", "Low stock items", lowStockCount, format: "number"),
            PendingApprovals = ToKpi("pendingApprovals", "Pending approvals", pendingApprovals, format: "number"),
            TopProducts = topProducts,
            DeadInventoryCount = ToKpi("deadInventoryCount", "Dead stock (no sale)", deadCount, format: "number")
        };
    }

    public async Task<SalesKpisDto> GetSalesKpisAsync(CalculationContext context, CancellationToken cancellationToken = default)
    {
        var previous = GetPreviousPeriodContext(context);

        var revenue = await _financeCalculator.GetRevenueAsync(context, cancellationToken);
        var revenuePrev = await _financeCalculator.GetRevenueAsync(previous, cancellationToken);
        var cogs = await _financeCalculator.GetCogsAsync(context, cancellationToken);
        var cogsPrev = await _financeCalculator.GetCogsAsync(previous, cancellationToken);
        var grossProfit = revenue - cogs;
        var grossProfitPrev = revenuePrev - cogsPrev;
        var margin = await _financeCalculator.GetGrossMarginPercentAsync(context, cancellationToken);
        var marginPrev = revenuePrev > 0 ? (grossProfitPrev / revenuePrev) * 100 : (decimal?)null;
        var aov = await _financeCalculator.GetAverageOrderValueAsync(context, cancellationToken);
        var aovPrev = await _financeCalculator.GetAverageOrderValueAsync(previous, cancellationToken);

        var orderCount = await _context.SalesOrders.AsNoTracking()
            .Where(so => so.CompanyId == context.CompanyId && !so.IsDeleted && so.Status != "Cancelled" && so.Status != "Draft")
            .Where(so => context.DateFrom == null || so.OrderDate >= context.DateFrom)
            .Where(so => context.DateTo == null || so.OrderDate <= context.DateTo)
            .CountAsync(cancellationToken);
        var orderCountPrev = await _context.SalesOrders.AsNoTracking()
            .Where(so => so.CompanyId == previous.CompanyId && !so.IsDeleted && so.Status != "Cancelled" && so.Status != "Draft")
            .Where(so => previous.DateFrom == null || so.OrderDate >= previous.DateFrom)
            .Where(so => previous.DateTo == null || so.OrderDate <= previous.DateTo)
            .CountAsync(cancellationToken);

        var growth = await _financeCalculator.GetSalesGrowthPercentAsync(context, previous, cancellationToken);

        return new SalesKpisDto
        {
            Revenue = ToKpiWithPrev("revenue", "Revenue", revenue, revenuePrev, "currency", true),
            Cogs = ToKpiWithPrev("cogs", "COGS", cogs, cogsPrev, "currency", false),
            GrossProfit = ToKpiWithPrev("grossProfit", "Gross profit", grossProfit, grossProfitPrev, "currency", true),
            GrossMarginPercent = ToKpiWithPrev("grossMarginPercent", "Gross margin %", margin ?? 0, marginPrev ?? 0, "percent", true),
            AverageOrderValue = ToKpiWithPrev("averageOrderValue", "Avg order value", aov ?? 0, aovPrev ?? 0, "currency", true),
            OrderCount = ToKpiWithPrev("orderCount", "Orders", orderCount, orderCountPrev, "number", true),
            SalesGrowthPercent = ToKpi("salesGrowthPercent", "Sales growth %", growth ?? 0, previousValue: growth, format: "percent", higherIsBetter: true)
        };
    }

    public async Task<InventoryKpisDto> GetInventoryKpisAsync(CalculationContext context, CancellationToken cancellationToken = default)
    {
        var previous = GetPreviousPeriodContext(context);

        var stockValue = await _inventoryCalculator.GetStockValueAsync(context, cancellationToken);
        var stockValuePrev = await _inventoryCalculator.GetStockValueAsync(previous, cancellationToken);
        var turnover = await _inventoryCalculator.GetInventoryTurnoverAsync(context, cancellationToken);
        var turnoverPrev = await _inventoryCalculator.GetInventoryTurnoverAsync(previous, cancellationToken);
        var daysInv = await _inventoryCalculator.GetDaysOfInventoryAsync(context, cancellationToken);
        var daysInvPrev = await _inventoryCalculator.GetDaysOfInventoryAsync(previous, cancellationToken);
        var sellThrough = await _inventoryCalculator.GetSellThroughRateAsync(context, cancellationToken);
        var deadCount = await _inventoryCalculator.GetDeadStockCountAsync(context, cancellationToken);
        var deadCountPrev = await _inventoryCalculator.GetDeadStockCountAsync(previous, cancellationToken);
        var aging = await _inventoryCalculator.GetStockAgingBucketsAsync(context, cancellationToken);

        return new InventoryKpisDto
        {
            StockValue = ToKpiWithPrev("stockValue", "Stock value", stockValue, stockValuePrev, "currency", true),
            InventoryTurnover = ToKpiWithPrev("inventoryTurnover", "Turnover", turnover ?? 0, turnoverPrev ?? 0, "number", true),
            DaysOfInventory = ToKpiWithPrev("daysOfInventory", "Days of inventory", daysInv ?? 0, daysInvPrev ?? 0, "number", false),
            SellThroughRate = ToKpi("sellThroughRate", "Sell-through rate", sellThrough ?? 0, format: "percent"),
            DeadStockCount = ToKpiWithPrev("deadStockCount", "Dead stock", deadCount, deadCountPrev, "number", false),
            AgingBuckets = aging
        };
    }

    public async Task<CustomerKpisDto> GetCustomerKpisAsync(CalculationContext context, CancellationToken cancellationToken = default)
    {
        var previous = GetPreviousPeriodContext(context);

        var customerCount = await _context.Customers.AsNoTracking()
            .Where(c => c.CompanyId == context.CompanyId && !c.IsDeleted)
            .CountAsync(cancellationToken);
        var customerCountPrev = await _context.Customers.AsNoTracking()
            .Where(c => c.CompanyId == previous.CompanyId && !c.IsDeleted)
            .CountAsync(cancellationToken);

        var ordersPerCustomer = await (
            from so in _context.SalesOrders.AsNoTracking()
            where so.CompanyId == context.CompanyId && !so.IsDeleted && so.Status != "Cancelled" && so.Status != "Draft"
                && (context.DateFrom == null || so.OrderDate >= context.DateFrom)
                && (context.DateTo == null || so.OrderDate <= context.DateTo)
            group so by so.CustomerId into g
            select g.Count()
        ).ToListAsync(cancellationToken);
        var repeatCustomers = ordersPerCustomer.Count(c => c > 1);
        var totalWithOrders = ordersPerCustomer.Count;
        var repeatRate = totalWithOrders > 0 ? (decimal)repeatCustomers / totalWithOrders * 100 : 0;

        var totalRevenue = await _financeCalculator.GetRevenueAsync(context, cancellationToken);
        var avgLifetimeValue = customerCount > 0 ? totalRevenue / customerCount : 0;

        return new CustomerKpisDto
        {
            CustomerCount = ToKpiWithPrev("customerCount", "Customers", customerCount, customerCountPrev, "number", true),
            RepeatRatePercent = ToKpi("repeatRatePercent", "Repeat rate %", repeatRate, format: "percent"),
            AverageLifetimeValue = ToKpi("averageLifetimeValue", "Avg lifetime value", avgLifetimeValue, unit: "currency", format: "currency")
        };
    }

    private static CalculationContext GetPreviousPeriodContext(CalculationContext ctx)
    {
        var from = ctx.DateFrom ?? DateTime.UtcNow.AddMonths(-1);
        var to = ctx.DateTo ?? DateTime.UtcNow;
        var span = to - from;
        return new CalculationContext
        {
            CompanyId = ctx.CompanyId,
            DateFrom = from.AddDays(-span.TotalDays),
            DateTo = from.AddMilliseconds(-1),
            WarehouseId = ctx.WarehouseId,
            ProductId = ctx.ProductId,
            CategoryId = ctx.CategoryId,
            DeadStockDays = ctx.DeadStockDays
        };
    }

    private static KpiValueDto ToKpi(string key, string label, decimal currentValue, decimal? previousValue = null, string? unit = null, string? format = null, bool? higherIsBetter = null)
    {
        decimal? pct = null;
        string trend = "neutral";
        if (previousValue.HasValue && previousValue.Value != 0)
        {
            pct = ((currentValue - previousValue.Value) / Math.Abs(previousValue.Value)) * 100;
            trend = pct > 0 ? "up" : pct < 0 ? "down" : "neutral";
            if (higherIsBetter == false && trend != "neutral") trend = trend == "up" ? "down" : "up";
        }
        return new KpiValueDto { Key = key, Label = label, CurrentValue = currentValue, PreviousValue = previousValue, PercentageChange = pct, TrendIndicator = trend, Unit = unit, Format = format };
    }

    private static KpiValueDto ToKpiWithPrev(string key, string label, decimal currentValue, decimal previousValue, string? format, bool higherIsBetter)
    {
        return ToKpi(key, label, currentValue, previousValue, unit: null, format, higherIsBetter);
    }
}
