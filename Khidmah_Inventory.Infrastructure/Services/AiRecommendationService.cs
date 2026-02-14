using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Features.Intelligence.Models;

namespace Khidmah_Inventory.Infrastructure.Services;

public class AiRecommendationService : IAiRecommendationService
{
    private readonly IApplicationDbContext _context;
    private readonly IMachineLearningService _machineLearningService;

    public AiRecommendationService(IApplicationDbContext context, IMachineLearningService machineLearningService)
    {
        _context = context;
        _machineLearningService = machineLearningService;
    }

    public async Task<List<AiRecommendationDto>> GetRecommendationsAsync(
        Guid companyId,
        Guid? productId = null,
        int horizonDays = 14,
        CancellationToken cancellationToken = default)
    {
        var productsQuery = _context.Products
            .Where(p => p.CompanyId == companyId && p.IsActive && !p.IsDeleted);
        if (productId.HasValue)
            productsQuery = productsQuery.Where(p => p.Id == productId.Value);

        var products = await productsQuery
            .OrderBy(p => p.Name)
            .Take(productId.HasValue ? 1 : 50)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        var last30 = now.AddDays(-30);
        var recommendations = new List<AiRecommendationDto>(products.Count);

        foreach (var product in products)
        {
            var stock = await _context.StockLevels
                .Where(s => s.CompanyId == companyId && s.ProductId == product.Id)
                .SumAsync(s => s.Quantity, cancellationToken);

            var soldLast30 = await _context.SalesOrderItems
                .Include(i => i.SalesOrder)
                .Where(i => i.ProductId == product.Id &&
                            i.SalesOrder.CompanyId == companyId &&
                            !i.SalesOrder.IsDeleted &&
                            i.SalesOrder.OrderDate >= last30)
                .SumAsync(i => i.Quantity, cancellationToken);

            var avgDailySales = soldLast30 / 30m;
            var forecast = await _machineLearningService.ForecastDemandAsync(product.Id, horizonDays);
            var projectedDemand = forecast.Count > 0 ? (decimal)forecast.Average(x => x.PredictedValue) * horizonDays : avgDailySales * horizonDays;
            var suggestedReorder = Math.Max(0, projectedDemand - stock);

            var desiredMargin = 0.35m;
            var suggestedPrice = product.PurchasePrice > 0
                ? Math.Round(product.PurchasePrice / (1 - desiredMargin), 2)
                : product.SalePrice;

            var preferredSupplierName = await _context.PurchaseOrderItems
                .Include(i => i.PurchaseOrder)
                    .ThenInclude(po => po.Supplier)
                .Where(i => i.ProductId == product.Id &&
                            i.PurchaseOrder.CompanyId == companyId &&
                            !i.PurchaseOrder.IsDeleted)
                .OrderByDescending(i => i.PurchaseOrder.OrderDate)
                .Select(i => i.PurchaseOrder.Supplier.Name)
                .FirstOrDefaultAsync(cancellationToken) ?? "N/A";

            var stockoutProbability = projectedDemand <= 0 ? 0 : Math.Min(100m, Math.Round((projectedDemand / Math.Max(stock, 1)) * 20m, 2));
            var abnormalSales = avgDailySales > 0 && projectedDemand > avgDailySales * horizonDays * 1.5m;
            var risk = stockoutProbability >= 80 ? "Critical"
                : stockoutProbability >= 60 ? "High"
                : stockoutProbability >= 30 ? "Medium"
                : "Low";

            recommendations.Add(new AiRecommendationDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductSKU = product.SKU,
                RiskLevel = risk,
                SuggestedReorderQuantity = Math.Round(suggestedReorder, 2),
                SuggestedSalePrice = suggestedPrice,
                RecommendedSupplierName = preferredSupplierName,
                StockoutProbability = stockoutProbability,
                AbnormalSalesDetected = abnormalSales,
                Reasoning = $"Based on {horizonDays}-day demand forecast, current stock {stock}, avg daily sales {Math.Round(avgDailySales, 2)}."
            });
        }

        return recommendations
            .OrderByDescending(x => x.StockoutProbability)
            .ThenByDescending(x => x.AbnormalSalesDetected)
            .ToList();
    }
}
