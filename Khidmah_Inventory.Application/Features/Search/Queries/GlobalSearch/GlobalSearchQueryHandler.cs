using MediatR;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Search.Models;

namespace Khidmah_Inventory.Application.Features.Search.Queries.GlobalSearch;

public class GlobalSearchQueryHandler : IRequestHandler<GlobalSearchQuery, Result<GlobalSearchResultDto>>
{
    private readonly ISearchService _searchService;

    public GlobalSearchQueryHandler(ISearchService searchService)
    {
        _searchService = searchService;
    }

    public async Task<Result<GlobalSearchResultDto>> Handle(GlobalSearchQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
            return Result<GlobalSearchResultDto>.Failure("Search term is required.");

        var result = await _searchService.SearchGroupedAsync(request.SearchTerm, request.LimitPerGroup);
        return Result<GlobalSearchResultDto>.Success(result);
    }
}

