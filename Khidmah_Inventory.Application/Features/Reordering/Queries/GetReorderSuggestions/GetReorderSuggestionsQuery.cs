using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Reordering.Models;

namespace Khidmah_Inventory.Application.Features.Reordering.Queries.GetReorderSuggestions;

public class GetReorderSuggestionsQuery : IRequest<Result<List<ReorderSuggestionDto>>>
{
    public Guid? WarehouseId { get; set; }
    public string? Priority { get; set; } // Critical, High, Medium, Low, All
    public bool IncludeInStock { get; set; } = false; // Include items that are in stock but below reorder point
}

