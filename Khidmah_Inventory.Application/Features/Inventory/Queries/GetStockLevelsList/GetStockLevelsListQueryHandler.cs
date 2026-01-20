using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Inventory.Models;
using Khidmah_Inventory.Application.Common.Extensions;

namespace Khidmah_Inventory.Application.Features.Inventory.Queries.GetStockLevelsList;

public class GetStockLevelsListQueryHandler : IRequestHandler<GetStockLevelsListQuery, Result<PagedResult<StockLevelDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetStockLevelsListQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<StockLevelDto>>> Handle(GetStockLevelsListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result<PagedResult<StockLevelDto>>.Failure("Company context is required");
        }

        var filterRequest = request.FilterRequest ?? new FilterRequest
        {
            Pagination = new PaginationDto { PageNo = 1, PageSize = 10 }
        };

        var query = _context.StockLevels
            .Include(sl => sl.Product)
            .Include(sl => sl.Warehouse)
            .Where(sl => sl.CompanyId == companyId.Value)
            .AsQueryable();

        if (request.ProductId.HasValue)
        {
            query = query.Where(sl => sl.ProductId == request.ProductId.Value);
        }

        if (request.WarehouseId.HasValue)
        {
            query = query.Where(sl => sl.WarehouseId == request.WarehouseId.Value);
        }

        if (request.LowStockOnly == true)
        {
            // Filter for low stock - products where quantity is at or below minimum stock level
            var productIdsWithLowStock = await _context.Products
                .Where(p => p.CompanyId == companyId.Value && !p.IsDeleted && p.MinStockLevel.HasValue)
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            query = query.Where(sl => productIdsWithLowStock.Contains(sl.ProductId) && 
                sl.Product.MinStockLevel.HasValue && 
                sl.Quantity <= sl.Product.MinStockLevel.Value);
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
                : new List<string> { "Product.Name", "Product.SKU", "Warehouse.Name" };

            query = query.ApplySearch(filterRequest.Search.Term, searchFields, filterRequest.Search.Mode, filterRequest.Search.IsCaseSensitive);
        }

        // Apply sorting
        if (filterRequest.Pagination != null && !string.IsNullOrWhiteSpace(filterRequest.Pagination.SortBy))
        {
            query = query.ApplySorting(filterRequest.Pagination.SortBy, filterRequest.Pagination.SortOrder ?? "ascending");
        }
        else
        {
            query = query.OrderBy(sl => sl.Product.Name);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var pagination = filterRequest.Pagination ?? new PaginationDto { PageNo = 1, PageSize = 10 };
        var stockLevels = await query
            .Skip((pagination.PageNo - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        var stockLevelDtos = stockLevels.Select(sl => new StockLevelDto
        {
            Id = sl.Id,
            ProductId = sl.ProductId,
            ProductName = sl.Product.Name,
            ProductSKU = sl.Product.SKU,
            WarehouseId = sl.WarehouseId,
            WarehouseName = sl.Warehouse.Name,
            Quantity = sl.Quantity,
            ReservedQuantity = sl.ReservedQuantity ?? 0,
            AvailableQuantity = sl.AvailableQuantity,
            AverageCost = sl.AverageCost,
            LastUpdated = sl.LastUpdated
        }).ToList();

        var result = new PagedResult<StockLevelDto>
        {
            Items = stockLevelDtos,
            TotalCount = totalCount,
            PageNo = pagination.PageNo,
            PageSize = pagination.PageSize
        };

        return Result<PagedResult<StockLevelDto>>.Success(result);
    }
}

