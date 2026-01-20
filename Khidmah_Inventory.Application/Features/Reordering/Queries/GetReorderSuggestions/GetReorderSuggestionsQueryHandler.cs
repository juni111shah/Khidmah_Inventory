using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Reordering.Models;

namespace Khidmah_Inventory.Application.Features.Reordering.Queries.GetReorderSuggestions;

public class GetReorderSuggestionsQueryHandler : IRequestHandler<GetReorderSuggestionsQuery, Result<List<ReorderSuggestionDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetReorderSuggestionsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<ReorderSuggestionDto>>> Handle(GetReorderSuggestionsQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<List<ReorderSuggestionDto>>.Failure("Company context is required");

        // Get stock levels that need reordering
        var stockLevelsQuery = _context.StockLevels
            .Include(sl => sl.Product)
            .Include(sl => sl.Warehouse)
            .Where(sl => sl.CompanyId == companyId.Value &&
                sl.Product.IsActive &&
                !sl.Product.IsDeleted &&
                (sl.Product.MinStockLevel.HasValue || sl.Product.ReorderPoint.HasValue));

        if (request.WarehouseId.HasValue)
            stockLevelsQuery = stockLevelsQuery.Where(sl => sl.WarehouseId == request.WarehouseId.Value);

        var stockLevels = await stockLevelsQuery.ToListAsync(cancellationToken);

        var suggestions = new List<ReorderSuggestionDto>();
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

        foreach (var stockLevel in stockLevels)
        {
            var reorderPoint = stockLevel.Product.ReorderPoint ?? stockLevel.Product.MinStockLevel ?? 0;
            var minStock = stockLevel.Product.MinStockLevel ?? 0;

            // Check if needs reordering
            bool needsReorder = stockLevel.Quantity <= reorderPoint;
            if (!needsReorder && !request.IncludeInStock)
                continue;

            // Calculate average daily sales (last 30 days)
            var salesData = await _context.SalesOrderItems
                .Include(item => item.SalesOrder)
                .Where(item => item.ProductId == stockLevel.ProductId &&
                    item.SalesOrder.CompanyId == companyId.Value &&
                    !item.SalesOrder.IsDeleted &&
                    item.SalesOrder.OrderDate >= thirtyDaysAgo)
                .GroupBy(item => item.SalesOrder.OrderDate.Date)
                .Select(g => new { Date = g.Key, Quantity = g.Sum(i => i.Quantity) })
                .ToListAsync(cancellationToken);

            var totalSales = salesData.Sum(s => s.Quantity);
            var averageDailySales = salesData.Count > 0 ? totalSales / 30.0m : 0;

            // Calculate days of stock remaining
            var daysOfStockRemaining = averageDailySales > 0 
                ? (int)(stockLevel.Quantity / averageDailySales) 
                : 999;

            // Determine priority
            string priority = "Low";
            if (stockLevel.Quantity <= 0)
                priority = "Critical";
            else if (stockLevel.Quantity <= minStock * 0.5m)
                priority = "High";
            else if (stockLevel.Quantity <= reorderPoint)
                priority = "Medium";

            if (!string.IsNullOrEmpty(request.Priority) && 
                request.Priority != "All" && 
                request.Priority != priority)
                continue;

            // Calculate suggested quantity
            var maxStock = stockLevel.Product.MaxStockLevel ?? (reorderPoint * 3);
            var suggestedQuantity = Math.Max(maxStock - stockLevel.Quantity, reorderPoint - stockLevel.Quantity + (reorderPoint * 0.5m));

            // Get supplier suggestions
            var supplierSuggestions = await GetSupplierSuggestions(stockLevel.ProductId, companyId.Value, cancellationToken);

            var suggestion = new ReorderSuggestionDto
            {
                ProductId = stockLevel.ProductId,
                ProductName = stockLevel.Product.Name,
                ProductSKU = stockLevel.Product.SKU,
                WarehouseId = stockLevel.WarehouseId,
                WarehouseName = stockLevel.Warehouse.Name,
                CurrentStock = stockLevel.Quantity,
                MinStockLevel = stockLevel.Product.MinStockLevel,
                ReorderPoint = stockLevel.Product.ReorderPoint,
                MaxStockLevel = stockLevel.Product.MaxStockLevel,
                SuggestedQuantity = Math.Round(suggestedQuantity, 2),
                AverageDailySales = averageDailySales > 0 ? Math.Round(averageDailySales, 2) : null,
                DaysOfStockRemaining = daysOfStockRemaining,
                Priority = priority,
                SupplierSuggestions = supplierSuggestions
            };

            suggestions.Add(suggestion);
        }

        // Sort by priority (Critical, High, Medium, Low)
        var priorityOrder = new Dictionary<string, int> { { "Critical", 0 }, { "High", 1 }, { "Medium", 2 }, { "Low", 3 } };
        suggestions = suggestions.OrderBy(s => priorityOrder.GetValueOrDefault(s.Priority, 99))
            .ThenBy(s => s.DaysOfStockRemaining)
            .ToList();

        return Result<List<ReorderSuggestionDto>>.Success(suggestions);
    }

    private async Task<List<SupplierSuggestionDto>> GetSupplierSuggestions(Guid productId, Guid companyId, CancellationToken cancellationToken)
    {
        // Get purchase history for this product
        var purchaseHistory = await _context.PurchaseOrderItems
            .Include(item => item.PurchaseOrder)
                .ThenInclude(po => po.Supplier)
            .Where(item => item.ProductId == productId &&
                item.PurchaseOrder.CompanyId == companyId &&
                !item.PurchaseOrder.IsDeleted)
            .GroupBy(item => item.PurchaseOrder.SupplierId)
            .Select(g => new
            {
                SupplierId = g.Key,
                SupplierName = g.First().PurchaseOrder.Supplier.Name,
                LastPurchasePrice = g.OrderByDescending(i => i.PurchaseOrder.OrderDate)
                    .First().UnitPrice,
                LastPurchaseDate = g.Max(i => i.PurchaseOrder.OrderDate),
                PurchaseCount = g.Count(),
                AverageDeliveryDays = 7 // Default, can be calculated from actual delivery dates
            })
            .ToListAsync(cancellationToken);

        var suggestions = purchaseHistory.Select(ph => new SupplierSuggestionDto
        {
            SupplierId = ph.SupplierId,
            SupplierName = ph.SupplierName,
            LastPurchasePrice = ph.LastPurchasePrice,
            LastPurchaseDate = ph.LastPurchaseDate,
            PurchaseCount = ph.PurchaseCount,
            AverageDeliveryDays = ph.AverageDeliveryDays,
            RecommendedPrice = ph.LastPurchasePrice,
            Score = CalculateSupplierScore(ph.PurchaseCount, ph.LastPurchaseDate, ph.AverageDeliveryDays)
        }).OrderByDescending(s => s.Score).Take(5).ToList();

        return suggestions;
    }

    private int CalculateSupplierScore(int purchaseCount, DateTime? lastPurchaseDate, int? averageDeliveryDays)
    {
        int score = 0;

        // More purchases = higher score
        score += Math.Min(purchaseCount * 10, 50);

        // Recent purchases = higher score
        if (lastPurchaseDate.HasValue)
        {
            var daysSinceLastPurchase = (DateTime.UtcNow - lastPurchaseDate.Value).TotalDays;
            if (daysSinceLastPurchase < 30)
                score += 30;
            else if (daysSinceLastPurchase < 90)
                score += 20;
            else if (daysSinceLastPurchase < 180)
                score += 10;
        }

        // Faster delivery = higher score
        if (averageDeliveryDays.HasValue)
        {
            if (averageDeliveryDays.Value <= 3)
                score += 20;
            else if (averageDeliveryDays.Value <= 7)
                score += 10;
        }

        return score;
    }
}

