using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Intelligence.Models;

namespace Khidmah_Inventory.Application.Features.Intelligence.Queries.GetProductIntelligence;

public class GetProductIntelligenceQueryHandler : IRequestHandler<GetProductIntelligenceQuery, Result<ProductIntelligenceDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetProductIntelligenceQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<ProductIntelligenceDto>> Handle(GetProductIntelligenceQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<ProductIntelligenceDto>.Failure("Company context is required");

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.CompanyId == companyId.Value && !p.IsDeleted, cancellationToken);
        if (product == null)
            return Result<ProductIntelligenceDto>.Failure("Product not found");

        var days = Math.Max(7, request.DaysForVelocity ?? 30);
        var periodEnd = DateTime.UtcNow.Date;
        var periodStart = periodEnd.AddDays(-days);
        var previousStart = periodStart.AddDays(-days);

        var dto = new ProductIntelligenceDto { ProductId = product.Id };

        // Total stock across warehouses
        var totalStock = await _context.StockLevels
            .Where(sl => sl.CompanyId == companyId.Value && sl.ProductId == product.Id)
            .SumAsync(sl => sl.Quantity, cancellationToken);

        // Sales velocity: quantity sold in period / days
        var sold = await _context.SalesOrderItems
            .Include(i => i.SalesOrder)
            .Where(i => i.ProductId == product.Id && i.SalesOrder.CompanyId == companyId.Value && !i.SalesOrder.IsDeleted
                && i.SalesOrder.OrderDate >= periodStart && i.SalesOrder.OrderDate <= periodEnd)
            .SumAsync(i => i.Quantity, cancellationToken);
        dto.SalesVelocity = days > 0 ? Math.Round(sold / days, 4) : 0;

        // Stock days remaining
        if (dto.SalesVelocity > 0 && totalStock > 0)
        {
            dto.StockDaysRemaining = (int)Math.Floor(totalStock / dto.SalesVelocity);
        }

        // Reorder risk
        var reorderPoint = product.ReorderPoint ?? product.MinStockLevel;
        if (reorderPoint.HasValue && totalStock <= reorderPoint.Value)
            dto.ReorderRisk = "Critical";
        else if (dto.StockDaysRemaining.HasValue && dto.StockDaysRemaining.Value <= 7)
            dto.ReorderRisk = "Critical";
        else if (dto.StockDaysRemaining.HasValue && dto.StockDaysRemaining.Value <= 14)
            dto.ReorderRisk = "High";
        else if (dto.StockDaysRemaining.HasValue && dto.StockDaysRemaining.Value <= 30)
            dto.ReorderRisk = "Medium";
        else
            dto.ReorderRisk = "Low";

        // Margin: current vs previous period (using Product.PurchasePrice as cost proxy)
        var cost = product.CostPrice ?? product.PurchasePrice;
        var currentRevenue = await _context.SalesOrderItems
            .Include(i => i.SalesOrder)
            .Where(i => i.ProductId == product.Id && i.SalesOrder.CompanyId == companyId.Value && !i.SalesOrder.IsDeleted
                && i.SalesOrder.OrderDate >= periodStart && i.SalesOrder.OrderDate <= periodEnd)
            .SumAsync(i => i.LineTotal, cancellationToken);
        var currentQty = await _context.SalesOrderItems
            .Include(i => i.SalesOrder)
            .Where(i => i.ProductId == product.Id && i.SalesOrder.CompanyId == companyId.Value && !i.SalesOrder.IsDeleted
                && i.SalesOrder.OrderDate >= periodStart && i.SalesOrder.OrderDate <= periodEnd)
            .SumAsync(i => i.Quantity, cancellationToken);
        var currentCost = currentQty * cost;
        dto.CurrentMarginPercent = currentRevenue > 0 ? Math.Round((currentRevenue - currentCost) / currentRevenue * 100, 2) : 0;

        var prevRevenue = await _context.SalesOrderItems
            .Include(i => i.SalesOrder)
            .Where(i => i.ProductId == product.Id && i.SalesOrder.CompanyId == companyId.Value && !i.SalesOrder.IsDeleted
                && i.SalesOrder.OrderDate >= previousStart && i.SalesOrder.OrderDate < periodStart)
            .SumAsync(i => i.LineTotal, cancellationToken);
        var prevQty = await _context.SalesOrderItems
            .Include(i => i.SalesOrder)
            .Where(i => i.ProductId == product.Id && i.SalesOrder.CompanyId == companyId.Value && !i.SalesOrder.IsDeleted
                && i.SalesOrder.OrderDate >= previousStart && i.SalesOrder.OrderDate < periodStart)
            .SumAsync(i => i.Quantity, cancellationToken);
        var prevCost = prevQty * cost;
        dto.PreviousMarginPercent = prevRevenue > 0 ? Math.Round((prevRevenue - prevCost) / prevRevenue * 100, 2) : (decimal?)null;

        if (dto.PreviousMarginPercent.HasValue)
        {
            var diff = dto.CurrentMarginPercent - dto.PreviousMarginPercent.Value;
            dto.MarginTrend = diff > 1 ? "Up" : diff < -1 ? "Down" : "Stable";
        }
        else
            dto.MarginTrend = "Stable";

        // Price history: last 10 distinct dates from sales
        var priceHistory = await _context.SalesOrderItems
            .Include(i => i.SalesOrder)
            .Where(i => i.ProductId == product.Id && i.SalesOrder.CompanyId == companyId.Value && !i.SalesOrder.IsDeleted)
            .OrderByDescending(i => i.SalesOrder.OrderDate)
            .Select(i => new { i.SalesOrder.OrderDate, i.UnitPrice })
            .Take(50)
            .ToListAsync(cancellationToken);
        dto.PriceHistory = priceHistory
            .GroupBy(x => x.OrderDate.Date)
            .OrderByDescending(g => g.Key)
            .Take(10)
            .Select(g => new PriceHistoryPointDto
            {
                Date = g.Key,
                SalePrice = g.Average(x => x.UnitPrice),
                CostPrice = cost
            })
            .OrderBy(p => p.Date)
            .ToList();

        // ABC: company total revenue last 90d, then this product's share
        var ninetyDaysAgo = DateTime.UtcNow.Date.AddDays(-90);
        var companyRevenue = await _context.SalesOrderItems
            .Include(i => i.SalesOrder)
            .Where(i => i.SalesOrder.CompanyId == companyId.Value && !i.SalesOrder.IsDeleted && i.SalesOrder.OrderDate >= ninetyDaysAgo)
            .SumAsync(i => i.LineTotal, cancellationToken);
        var productRevenue = await _context.SalesOrderItems
            .Include(i => i.SalesOrder)
            .Where(i => i.ProductId == product.Id && i.SalesOrder.CompanyId == companyId.Value && !i.SalesOrder.IsDeleted && i.SalesOrder.OrderDate >= ninetyDaysAgo)
            .SumAsync(i => i.LineTotal, cancellationToken);
        dto.AbcRevenueSharePercent = companyRevenue > 0 ? Math.Round(productRevenue / companyRevenue * 100, 2) : 0;

        var productRevenues = await _context.SalesOrderItems
            .Include(i => i.SalesOrder)
            .Where(i => i.SalesOrder.CompanyId == companyId.Value && !i.SalesOrder.IsDeleted && i.SalesOrder.OrderDate >= ninetyDaysAgo)
            .GroupBy(i => i.ProductId)
            .Select(g => new { ProductId = g.Key, Revenue = g.Sum(i => i.LineTotal) })
            .OrderByDescending(x => x.Revenue)
            .ToListAsync(cancellationToken);
        decimal cumul = 0;
        string abc = "C";
        foreach (var pr in productRevenues)
        {
            cumul += pr.Revenue;
            if (pr.ProductId == product.Id)
            {
                var pct = companyRevenue > 0 ? cumul / companyRevenue * 100 : 0;
                abc = pct <= 80 ? "A" : pct <= 95 ? "B" : "C";
                break;
            }
        }
        dto.AbcClassification = abc;

        // Recommended actions
        if (dto.ReorderRisk == "Critical" || dto.ReorderRisk == "High")
            dto.RecommendedActions.Add("Reorder now");
        if (dto.MarginTrend == "Down" && dto.CurrentMarginPercent < 20)
            dto.RecommendedActions.Add("Review price or cost");
        if (dto.SalesVelocity > 0 && dto.AbcClassification == "A")
            dto.RecommendedActions.Add("Ensure stock availability");
        if (dto.StockDaysRemaining.HasValue && dto.StockDaysRemaining.Value > 90 && dto.SalesVelocity > 0)
            dto.RecommendedActions.Add("Consider promotion");
        if (dto.RecommendedActions.Count == 0)
            dto.RecommendedActions.Add("No action needed");

        return Result<ProductIntelligenceDto>.Success(dto);
    }
}
