using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Pricing.Models;

namespace Khidmah_Inventory.Application.Features.Pricing.Queries.GetPriceSuggestions;

public class GetPriceSuggestionsQueryHandler : IRequestHandler<GetPriceSuggestionsQuery, Result<List<PriceOptimizationDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetPriceSuggestionsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<PriceOptimizationDto>>> Handle(GetPriceSuggestionsQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<List<PriceOptimizationDto>>.Failure("Company context is required");

        var productsQuery = _context.Products
            .Where(p => p.CompanyId == companyId.Value && p.IsActive && !p.IsDeleted);

        if (request.ProductIds != null && request.ProductIds.Any())
            productsQuery = productsQuery.Where(p => request.ProductIds.Contains(p.Id));

        var products = await productsQuery.ToListAsync(cancellationToken);

        var optimizations = new List<PriceOptimizationDto>();
        var ninetyDaysAgo = DateTime.UtcNow.AddDays(-90);

        foreach (var product in products)
        {
            // Get average purchase price (last 90 days)
            var avgPurchasePrice = await _context.PurchaseOrderItems
                .Include(item => item.PurchaseOrder)
                .Where(item => item.ProductId == product.Id &&
                    item.PurchaseOrder.CompanyId == companyId.Value &&
                    !item.PurchaseOrder.IsDeleted &&
                    item.PurchaseOrder.OrderDate >= ninetyDaysAgo)
                .AverageAsync(item => (decimal?)item.UnitPrice, cancellationToken) ?? product.PurchasePrice;

            // Calculate current margin
            var currentMargin = product.SalePrice > 0 
                ? ((product.SalePrice - avgPurchasePrice) / product.SalePrice) * 100 
                : 0;

            // Determine optimal margin (target 30-40% for most products)
            var optimalMargin = request.MinMargin ?? 35m;
            if (request.MaxMargin.HasValue)
                optimalMargin = (request.MinMargin ?? 35m + request.MaxMargin.Value) / 2;

            // Calculate recommended price
            var recommendedPrice = avgPurchasePrice / (1 - (optimalMargin / 100));

            // Determine recommendation
            string recommendation = "Maintain";
            decimal? priceChangeAmount = null;
            decimal? priceChangePercentage = null;

            if (recommendedPrice > product.SalePrice * 1.05m)
            {
                recommendation = "Increase";
                priceChangeAmount = recommendedPrice - product.SalePrice;
                priceChangePercentage = ((recommendedPrice - product.SalePrice) / product.SalePrice) * 100;
            }
            else if (recommendedPrice < product.SalePrice * 0.95m)
            {
                recommendation = "Decrease";
                priceChangeAmount = recommendedPrice - product.SalePrice;
                priceChangePercentage = ((recommendedPrice - product.SalePrice) / product.SalePrice) * 100;
            }

            var optimization = new PriceOptimizationDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductSKU = product.SKU,
                CurrentPrice = product.SalePrice,
                RecommendedPrice = Math.Round(recommendedPrice, 2),
                MinPrice = avgPurchasePrice * 1.1m, // Minimum 10% margin
                MaxPrice = avgPurchasePrice * 2.0m, // Maximum 100% margin
                CurrentMargin = Math.Round(currentMargin, 2),
                RecommendedMargin = optimalMargin,
                OptimalMargin = optimalMargin,
                Recommendation = recommendation,
                PriceChangeAmount = priceChangeAmount.HasValue ? Math.Round(priceChangeAmount.Value, 2) : null,
                PriceChangePercentage = priceChangePercentage.HasValue ? Math.Round(priceChangePercentage.Value, 2) : null
            };

            // Get price history if requested
            if (request.IncludeHistory)
            {
                var purchaseHistory = await _context.PurchaseOrderItems
                    .Include(item => item.PurchaseOrder)
                    .Where(item => item.ProductId == product.Id &&
                        item.PurchaseOrder.CompanyId == companyId.Value &&
                        !item.PurchaseOrder.IsDeleted &&
                        item.PurchaseOrder.OrderDate >= ninetyDaysAgo)
                    .OrderBy(item => item.PurchaseOrder.OrderDate)
                    .Select(item => new PriceHistoryDto
                    {
                        Date = item.PurchaseOrder.OrderDate,
                        Price = item.UnitPrice,
                        Type = "Purchase"
                    })
                    .ToListAsync(cancellationToken);

                optimization.PriceHistory = purchaseHistory;
            }

            optimizations.Add(optimization);
        }

        return Result<List<PriceOptimizationDto>>.Success(optimizations);
    }
}

