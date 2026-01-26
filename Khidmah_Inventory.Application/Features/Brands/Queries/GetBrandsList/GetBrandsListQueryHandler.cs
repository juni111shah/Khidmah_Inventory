using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Common.Extensions;
using Khidmah_Inventory.Application.Features.Brands.Models;

namespace Khidmah_Inventory.Application.Features.Brands.Queries.GetBrandsList;

public class GetBrandsListQueryHandler : IRequestHandler<GetBrandsListQuery, Result<PagedResult<BrandDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetBrandsListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<BrandDto>>> Handle(GetBrandsListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<PagedResult<BrandDto>>.Failure("Company context is required");

        var filterRequest = request.FilterRequest ?? new FilterRequest
        {
            Pagination = new PaginationDto { PageNo = 1, PageSize = 10 }
        };

        var query = _context.Brands
            .Where(b => b.CompanyId == companyId.Value)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(b => b.Name.Contains(request.SearchTerm));
        }

        if (filterRequest.Filters != null && filterRequest.Filters.Any())
            query = query.ApplyFilters(filterRequest.Filters);

        var totalCount = await query.CountAsync(cancellationToken);

        var brands = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((filterRequest.Pagination.PageNo - 1) * filterRequest.Pagination.PageSize)
            .Take(filterRequest.Pagination.PageSize)
            .Select(b => new BrandDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                LogoUrl = b.LogoUrl,
                Website = b.Website,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        var result = new PagedResult<BrandDto>
        {
            Items = brands,
            TotalCount = totalCount,
            PageNo = filterRequest.Pagination.PageNo,
            PageSize = filterRequest.Pagination.PageSize
        };

        return Result<PagedResult<BrandDto>>.Success(result);
    }
}