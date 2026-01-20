using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Analytics.Models;

namespace Khidmah_Inventory.Application.Features.Analytics.Queries.GetProfitAnalytics;

public class GetProfitAnalyticsQuery : IRequest<Result<ProfitAnalyticsDto>>
{
    public AnalyticsRequestDto Request { get; set; } = new();
}

