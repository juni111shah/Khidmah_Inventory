using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Common.Interfaces;

namespace Khidmah_Inventory.Application.Features.Search.Queries.GlobalSearch;

public class GlobalSearchQuery : IRequest<Result<SearchResult>>
{
    public string SearchTerm { get; set; } = string.Empty;
    public List<string> EntityTypes { get; set; } = new();
    public int Limit { get; set; } = 50;
}

