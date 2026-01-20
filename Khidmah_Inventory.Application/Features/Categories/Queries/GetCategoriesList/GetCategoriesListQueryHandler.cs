using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Categories.Models;
using Khidmah_Inventory.Application.Common.Extensions;

namespace Khidmah_Inventory.Application.Features.Categories.Queries.GetCategoriesList;

public class GetCategoriesListQueryHandler : IRequestHandler<GetCategoriesListQuery, Result<PagedResult<CategoryDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetCategoriesListQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<CategoryDto>>> Handle(GetCategoriesListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result<PagedResult<CategoryDto>>.Failure("Company context is required");
        }

        var filterRequest = request.FilterRequest ?? new FilterRequest
        {
            Pagination = new PaginationDto { PageNo = 1, PageSize = 10 }
        };

        var query = _context.Categories
            .Include(c => c.ParentCategory)
            .Where(c => c.CompanyId == companyId.Value && !c.IsDeleted)
            .AsQueryable();

        // Filter by parent category if specified
        if (request.ParentCategoryId.HasValue)
        {
            query = query.Where(c => c.ParentCategoryId == request.ParentCategoryId.Value);
        }
        else if (request.ParentCategoryId == Guid.Empty)
        {
            // Explicitly request root categories (no parent)
            query = query.Where(c => c.ParentCategoryId == null);
        }

        // Apply filters
        if (filterRequest.Filters != null && filterRequest.Filters.Any())
        {
            query = query.ApplyFilters(filterRequest.Filters);
        }

        // Apply search
        if (filterRequest.Search != null && !string.IsNullOrWhiteSpace(filterRequest.Search.Term))
        {
            var searchFields = filterRequest.Search.SearchFields.Any()
                ? filterRequest.Search.SearchFields
                : new List<string> { "Name", "Code", "Description" };

            query = query.ApplySearch(filterRequest.Search.Term, searchFields, filterRequest.Search.Mode, filterRequest.Search.IsCaseSensitive);
        }

        // Apply sorting
        if (filterRequest.Pagination != null && !string.IsNullOrWhiteSpace(filterRequest.Pagination.SortBy))
        {
            query = query.ApplySorting(filterRequest.Pagination.SortBy, filterRequest.Pagination.SortOrder ?? "ascending");
        }
        else
        {
            query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name);
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var pagination = filterRequest.Pagination ?? new PaginationDto { PageNo = 1, PageSize = 10 };
        var categories = await query
            .Skip((pagination.PageNo - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        // Map to DTOs
        var categoryDtos = new List<CategoryDto>();
        foreach (var category in categories)
        {
            var productCount = await _context.Products
                .CountAsync(p => p.CategoryId == category.Id && p.CompanyId == companyId.Value && !p.IsDeleted, cancellationToken);

            var subCategoryCount = await _context.Categories
                .CountAsync(c => c.ParentCategoryId == category.Id && c.CompanyId == companyId.Value && !c.IsDeleted, cancellationToken);

            categoryDtos.Add(new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Code = category.Code,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategoryName = category.ParentCategory?.Name,
                DisplayOrder = category.DisplayOrder,
                ImageUrl = category.ImageUrl,
                ProductCount = productCount,
                SubCategoryCount = subCategoryCount,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            });
        }

        var result = new PagedResult<CategoryDto>
        {
            Items = categoryDtos,
            TotalCount = totalCount,
            PageNo = pagination.PageNo,
            PageSize = pagination.PageSize
        };

        return Result<PagedResult<CategoryDto>>.Success(result);
    }
}

