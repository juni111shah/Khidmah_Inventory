using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.ApiKeys.Models;

namespace Khidmah_Inventory.Application.Features.Platform.ApiKeys.Queries.GetApiKeysList;

public class GetApiKeysListQueryHandler : IRequestHandler<GetApiKeysListQuery, Result<PagedResult<ApiKeyDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetApiKeysListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<ApiKeyDto>>> Handle(GetApiKeysListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<PagedResult<ApiKeyDto>>.Failure("Company context is required");

        var filter = request.FilterRequest ?? new FilterRequest { Pagination = new PaginationDto { PageNo = 1, PageSize = 10 } };
        var pagination = filter.Pagination ?? new PaginationDto { PageNo = 1, PageSize = 10 };

        var query = _context.ApiKeys
            .Where(k => k.CompanyId == companyId.Value && !k.IsDeleted)
            .AsQueryable();
        if (request.IsActive.HasValue)
            query = query.Where(k => k.IsActive == request.IsActive.Value);

        var total = await query.CountAsync(cancellationToken);
        var list = await query
            .OrderByDescending(k => k.CreatedAt)
            .Skip((pagination.PageNo - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(k => new ApiKeyDto
            {
                Id = k.Id,
                Name = k.Name,
                KeyPrefix = k.KeyPrefix,
                Permissions = k.Permissions,
                ExpiresAt = k.ExpiresAt,
                IsActive = k.IsActive,
                LastUsedAt = k.LastUsedAt,
                RequestCount = k.RequestCount,
                ErrorCount = k.ErrorCount,
                CreatedAt = k.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<ApiKeyDto>>.Success(new PagedResult<ApiKeyDto>
        {
            Items = list,
            TotalCount = total,
            PageNo = pagination.PageNo,
            PageSize = pagination.PageSize
        });
    }
}
