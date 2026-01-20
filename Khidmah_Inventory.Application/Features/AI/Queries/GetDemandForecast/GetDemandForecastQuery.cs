using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.AI.Models;

namespace Khidmah_Inventory.Application.Features.AI.Queries.GetDemandForecast;

public class GetDemandForecastQuery : IRequest<Result<ForecastDto>>
{
    public Guid ProductId { get; set; }
    public int ForecastDays { get; set; } = 30; // Forecast for next 30 days
}

