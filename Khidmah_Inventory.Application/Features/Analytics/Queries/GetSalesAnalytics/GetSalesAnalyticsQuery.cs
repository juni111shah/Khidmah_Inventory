using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Analytics.Models;

namespace Khidmah_Inventory.Application.Features.Analytics.Queries.GetSalesAnalytics;

public class GetSalesAnalyticsQuery : IRequest<Result<SalesAnalyticsDto>>
{
    public AnalyticsRequestDto Request { get; set; } = new();
}

