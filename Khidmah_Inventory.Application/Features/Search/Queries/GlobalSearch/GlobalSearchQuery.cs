using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Search.Models;

namespace Khidmah_Inventory.Application.Features.Search.Queries.GlobalSearch;

public class GlobalSearchQuery : IRequest<Result<GlobalSearchResultDto>>
{
    public string SearchTerm { get; set; } = string.Empty;
    public int LimitPerGroup { get; set; } = 10;
}

