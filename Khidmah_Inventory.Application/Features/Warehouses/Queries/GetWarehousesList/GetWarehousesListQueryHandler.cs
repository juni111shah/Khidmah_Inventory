using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Warehouses.Models;
using Khidmah_Inventory.Application.Common.Extensions;

namespace Khidmah_Inventory.Application.Features.Warehouses.Queries.GetWarehousesList;

public class GetWarehousesListQueryHandler : IRequestHandler<GetWarehousesListQuery, Result<PagedResult<WarehouseDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetWarehousesListQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<WarehouseDto>>> Handle(GetWarehousesListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result<PagedResult<WarehouseDto>>.Failure("Company context is required");
        }

        var filterRequest = request.FilterRequest ?? new FilterRequest
        {
            Pagination = new PaginationDto { PageNo = 1, PageSize = 10 }
        };

        var query = _context.Warehouses
            .Where(w => w.CompanyId == companyId.Value && !w.IsDeleted)
            .AsQueryable();

        // Filter by active status
        if (request.IsActive.HasValue)
        {
            query = query.Where(w => w.IsActive == request.IsActive.Value);
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
                : new List<string> { "Name", "Code", "Description", "City", "State", "Country" };

            query = query.ApplySearch(filterRequest.Search.Term, searchFields, filterRequest.Search.Mode, filterRequest.Search.IsCaseSensitive);
        }

        // Apply sorting
        if (filterRequest.Pagination != null && !string.IsNullOrWhiteSpace(filterRequest.Pagination.SortBy))
        {
            query = query.ApplySorting(filterRequest.Pagination.SortBy, filterRequest.Pagination.SortOrder ?? "ascending");
        }
        else
        {
            query = query.OrderBy(w => w.Name);
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var pagination = filterRequest.Pagination ?? new PaginationDto { PageNo = 1, PageSize = 10 };
        var warehouses = await query
            .Skip((pagination.PageNo - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        // Map to DTOs
        var warehouseDtos = new List<WarehouseDto>();
        foreach (var warehouse in warehouses)
        {
            var zoneCount = await _context.WarehouseZones
                .CountAsync(z => z.WarehouseId == warehouse.Id && z.CompanyId == companyId.Value && !z.IsDeleted, cancellationToken);

            warehouseDtos.Add(new WarehouseDto
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Code = warehouse.Code,
                Description = warehouse.Description,
                Address = warehouse.Address,
                City = warehouse.City,
                State = warehouse.State,
                Country = warehouse.Country,
                PostalCode = warehouse.PostalCode,
                PhoneNumber = warehouse.PhoneNumber,
                Email = warehouse.Email,
                IsDefault = warehouse.IsDefault,
                IsActive = warehouse.IsActive,
                ZoneCount = zoneCount,
                CreatedAt = warehouse.CreatedAt,
                UpdatedAt = warehouse.UpdatedAt
            });
        }

        var result = new PagedResult<WarehouseDto>
        {
            Items = warehouseDtos,
            TotalCount = totalCount,
            PageNo = pagination.PageNo,
            PageSize = pagination.PageSize
        };

        return Result<PagedResult<WarehouseDto>>.Success(result);
    }
}

