using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.AI.Models;

namespace Khidmah_Inventory.Application.Features.AI.Queries.GetDemandForecast;

public class GetDemandForecastQueryHandler : IRequestHandler<GetDemandForecastQuery, Result<ForecastDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMachineLearningService _mlService;

    public GetDemandForecastQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMachineLearningService mlService)
    {
        _context = context;
        _currentUser = currentUser;
        _mlService = mlService;
    }

    public async Task<Result<ForecastDto>> Handle(GetDemandForecastQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<ForecastDto>.Failure("Company context is required");

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && 
                p.CompanyId == companyId.Value && 
                !p.IsDeleted, cancellationToken);

        if (product == null)
            return Result<ForecastDto>.Failure("Product not found.");

        // Get ML forecast
        var forecastResults = await _mlService.ForecastDemandAsync(request.ProductId, request.ForecastDays);
        var forecastList = forecastResults.ToList(); // Ensure it's a list

        // Calculate average daily demand
        var avgDailyDemand = forecastList.Count > 0 
            ? (decimal)forecastList.Average(f => f.PredictedValue) 
            : 0;

        // Calculate recommended reorder quantity
        var currentStock = await _context.StockLevels
            .Where(sl => sl.ProductId == request.ProductId && sl.CompanyId == companyId.Value)
            .SumAsync(sl => (decimal?)sl.Quantity, cancellationToken) ?? 0;

        var reorderPoint = product.ReorderPoint ?? product.MinStockLevel ?? 0;
        var daysUntilReorder = avgDailyDemand > 0 
            ? (int)(currentStock / avgDailyDemand) 
            : 999;

        var recommendedReorderQuantity = Math.Max(
            (reorderPoint * 2) - currentStock,
            avgDailyDemand * request.ForecastDays);

        // Determine confidence level
        string confidence = "Medium";
        if (forecastList.Count >= 30 && forecastList.All(f => 
            Math.Abs(f.UpperBound - f.LowerBound) / f.PredictedValue < 0.2f))
            confidence = "High";
        else if (forecastList.Count < 7)
            confidence = "Low";

        // Detect trends
        var trends = new List<string>();
        if (forecastList.Count >= 7)
        {
            var firstHalf = forecastList.Take(forecastList.Count / 2).Average(f => f.PredictedValue);
            var secondHalf = forecastList.Skip(forecastList.Count / 2).Average(f => f.PredictedValue);
            
            if (secondHalf > firstHalf * 1.1f)
                trends.Add("Increasing");
            else if (secondHalf < firstHalf * 0.9f)
                trends.Add("Decreasing");
            else
                trends.Add("Stable");
        }

        var forecast = new ForecastDto
        {
            ProductId = product.Id,
            ProductName = product.Name,
            ProductSKU = product.SKU,
            ForecastData = forecastList.Select(f => new ForecastDataPoint
            {
                Date = f.Date,
                PredictedDemand = (decimal)f.PredictedValue,
                LowerBound = (decimal)f.LowerBound,
                UpperBound = (decimal)f.UpperBound
            }).ToList(),
            AverageDailyDemand = avgDailyDemand > 0 ? Math.Round(avgDailyDemand, 2) : null,
            RecommendedReorderQuantity = recommendedReorderQuantity > 0 ? Math.Round(recommendedReorderQuantity, 2) : null,
            RecommendedReorderDate = daysUntilReorder < request.ForecastDays 
                ? DateTime.UtcNow.AddDays(daysUntilReorder).Date 
                : null,
            Confidence = confidence,
            Trends = trends
        };

        return Result<ForecastDto>.Success(forecast);
    }
}

