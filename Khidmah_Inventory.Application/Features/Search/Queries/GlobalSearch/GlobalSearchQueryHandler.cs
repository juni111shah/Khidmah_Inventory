using MediatR;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Search.Queries.GlobalSearch;

public class GlobalSearchQueryHandler : IRequestHandler<GlobalSearchQuery, Result<SearchResult>>
{
    private readonly ISearchService _searchService;

    public GlobalSearchQueryHandler(ISearchService searchService)
    {
        _searchService = searchService;
    }

    public async Task<Result<SearchResult>> Handle(GlobalSearchQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
            return Result<SearchResult>.Failure("Search term is required.");

        var result = await _searchService.SearchAsync(request.SearchTerm, request.EntityTypes, request.Limit);
        return Result<SearchResult>.Success(result);
    }
}

