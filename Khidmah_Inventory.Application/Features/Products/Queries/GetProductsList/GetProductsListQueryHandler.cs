using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Products.Models;
using Khidmah_Inventory.Application.Common.Extensions;

namespace Khidmah_Inventory.Application.Features.Products.Queries.GetProductsList;

public class GetProductsListQueryHandler : IRequestHandler<GetProductsListQuery, Result<PagedResult<ProductDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetProductsListQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<ProductDto>>> Handle(GetProductsListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result<PagedResult<ProductDto>>.Failure("Company context is required");
        }

        var filterRequest = request.FilterRequest ?? new FilterRequest
        {
            Pagination = new PaginationDto { PageNo = 1, PageSize = 10 }
        };

        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.UnitOfMeasure)
            .Where(p => p.CompanyId == companyId.Value && !p.IsDeleted)
            .AsQueryable();

        // Filter by category
        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);
        }

        // Filter by brand
        if (request.BrandId.HasValue)
        {
            query = query.Where(p => p.BrandId == request.BrandId.Value);
        }

        // Filter by active status
        if (request.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == request.IsActive.Value);
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
                : new List<string> { "Name", "SKU", "Barcode", "Description" };

            query = query.ApplySearch(filterRequest.Search.Term, searchFields, filterRequest.Search.Mode, filterRequest.Search.IsCaseSensitive);
        }

        // Apply sorting
        if (filterRequest.Pagination != null && !string.IsNullOrWhiteSpace(filterRequest.Pagination.SortBy))
        {
            query = query.ApplySorting(filterRequest.Pagination.SortBy, filterRequest.Pagination.SortOrder ?? "ascending");
        }
        else
        {
            query = query.OrderBy(p => p.Name);
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var pagination = filterRequest.Pagination ?? new PaginationDto { PageNo = 1, PageSize = 10 };
        var products = await query
            .Skip((pagination.PageNo - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        // Map to DTOs
        var productDtos = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            SKU = p.SKU,
            Barcode = p.Barcode,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name,
            BrandId = p.BrandId,
            BrandName = p.Brand?.Name,
            UnitOfMeasureId = p.UnitOfMeasureId,
            UnitOfMeasureName = p.UnitOfMeasure.Name,
            UnitOfMeasureCode = p.UnitOfMeasure.Code,
            PurchasePrice = p.PurchasePrice,
            SalePrice = p.SalePrice,
            CostPrice = p.CostPrice,
            MinStockLevel = p.MinStockLevel,
            MaxStockLevel = p.MaxStockLevel,
            ReorderPoint = p.ReorderPoint,
            TrackQuantity = p.TrackQuantity,
            TrackBatch = p.TrackBatch,
            TrackExpiry = p.TrackExpiry,
            IsActive = p.IsActive,
            ImageUrl = p.ImageUrl,
            Weight = p.Weight,
            WeightUnit = p.WeightUnit,
            Length = p.Length,
            Width = p.Width,
            Height = p.Height,
            DimensionsUnit = p.DimensionsUnit,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList();

        var result = new PagedResult<ProductDto>
        {
            Items = productDtos,
            TotalCount = totalCount,
            PageNo = pagination.PageNo,
            PageSize = pagination.PageSize
        };

        return Result<PagedResult<ProductDto>>.Success(result);
    }
}

