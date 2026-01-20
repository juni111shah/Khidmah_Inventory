using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Pricing.Models;

namespace Khidmah_Inventory.Application.Features.Pricing.Queries.GetPriceSuggestions;

public class GetPriceSuggestionsQuery : IRequest<Result<List<PriceOptimizationDto>>>
{
    public List<Guid>? ProductIds { get; set; }
    public decimal? MinMargin { get; set; }
    public decimal? MaxMargin { get; set; }
    public bool IncludeHistory { get; set; } = false;
}

