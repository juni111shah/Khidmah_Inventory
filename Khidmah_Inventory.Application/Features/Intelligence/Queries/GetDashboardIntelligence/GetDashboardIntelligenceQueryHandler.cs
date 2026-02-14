using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Intelligence.Models;

namespace Khidmah_Inventory.Application.Features.Intelligence.Queries.GetDashboardIntelligence;

public class GetDashboardIntelligenceQueryHandler : IRequestHandler<GetDashboardIntelligenceQuery, Result<DashboardIntelligenceDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetDashboardIntelligenceQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<DashboardIntelligenceDto>> Handle(GetDashboardIntelligenceQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<DashboardIntelligenceDto>.Failure("Company context is required");

        var result = new DashboardIntelligenceDto();
        var today = DateTime.UtcNow.Date;
        var last7 = today.AddDays(-7);
        var last30 = today.AddDays(-30);

        // --- Predictions (simple: use last 7d avg for next 7d)
        var salesLast7 = await _context.SalesOrders
            .Where(so => so.CompanyId == companyId.Value && !so.IsDeleted && so.OrderDate >= last7)
            .SumAsync(so => so.TotalAmount, cancellationToken);
        var dailyAvg = salesLast7 / 7m;
        var predDays = request.PredictionDays ?? 7;
        result.Predictions.Add(new PredictionDto
        {
            Label = "Sales",
            Value = (dailyAvg * predDays).ToString("N0"),
            Trend = dailyAvg > 0 ? "Up" : "Stable",
            Period = $"Next {predDays} days"
        });

        var lowStockCount = await _context.StockLevels
            .Include(sl => sl.Product)
            .Where(sl => sl.CompanyId == companyId.Value && sl.Product.MinStockLevel.HasValue && sl.Quantity <= sl.Product.MinStockLevel!.Value)
            .CountAsync(cancellationToken);
        result.Predictions.Add(new PredictionDto
        {
            Label = "Low stock items",
            Value = lowStockCount.ToString(),
            Trend = lowStockCount > 5 ? "Up" : "Stable",
            Period = "Current"
        });

        // --- Anomalies (e.g. today's sales far from 7d average)
        var todaySales = await _context.SalesOrders
            .Where(so => so.CompanyId == companyId.Value && !so.IsDeleted && so.OrderDate.Date == today)
            .SumAsync(so => so.TotalAmount, cancellationToken);
        if (dailyAvg > 0 && Math.Abs((decimal)(todaySales - dailyAvg) / dailyAvg) > 0.5m)
        {
            result.Anomalies.Add(new AnomalyDto
            {
                Metric = "Today's sales",
                Description = todaySales > dailyAvg ? "Significantly above daily average" : "Significantly below daily average",
                Severity = "Medium",
                DetectedAt = DateTime.UtcNow,
                DrillDownRoute = "/reports"
            });
        }

        // --- Risks
        if (lowStockCount > 0)
        {
            result.Risks.Add(new RiskDto
            {
                Title = "Low stock",
                Description = $"{lowStockCount} product(s) at or below minimum level.",
                Severity = lowStockCount > 10 ? "High" : "Medium",
                ActionRoute = "/inventory/stock-levels"
            });
        }

        var pendingPo = await _context.PurchaseOrders
            .CountAsync(po => po.CompanyId == companyId.Value && !po.IsDeleted && po.Status != "Completed" && po.Status != "Cancelled", cancellationToken);
        if (pendingPo > 5)
        {
            result.Risks.Add(new RiskDto
            {
                Title = "Pending purchase orders",
                Description = $"{pendingPo} PO(s) not yet completed.",
                Severity = "Low",
                ActionRoute = "/purchase-orders"
            });
        }

        // --- Opportunities
        var topProducts = await _context.SalesOrderItems
            .Include(i => i.SalesOrder)
            .Where(i => i.SalesOrder.CompanyId == companyId.Value && !i.SalesOrder.IsDeleted && i.SalesOrder.OrderDate >= last30)
            .GroupBy(i => i.ProductId)
            .Select(g => new { ProductId = g.Key, Revenue = g.Sum(i => i.LineTotal) })
            .OrderByDescending(x => x.Revenue)
            .Take(1)
            .ToListAsync(cancellationToken);
        if (topProducts.Any())
        {
            result.Opportunities.Add(new OpportunityDto
            {
                Title = "Top seller",
                Description = "Review top product performance and stock.",
                ActionRoute = "/products"
            });
        }

        result.Opportunities.Add(new OpportunityDto
        {
            Title = "Reorder suggestions",
            Description = "Generate PO from reorder suggestions.",
            ActionRoute = "/reorder"
        });

        return Result<DashboardIntelligenceDto>.Success(result);
    }
}
