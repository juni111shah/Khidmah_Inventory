using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Inventory.Models;
using Khidmah_Inventory.Application.Common.Extensions;

namespace Khidmah_Inventory.Application.Features.Inventory.Queries.GetBatchesList;

public class GetBatchesListQueryHandler : IRequestHandler<GetBatchesListQuery, Result<PagedResult<BatchDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetBatchesListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<BatchDto>>> Handle(GetBatchesListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<PagedResult<BatchDto>>.Failure("Company context is required");

        var query = _context.Batches
            .Include(b => b.Product)
            .Include(b => b.Warehouse)
            .Where(b => b.CompanyId == companyId.Value && !b.IsDeleted)
            .AsQueryable();

        // Apply filters
        if (request.ProductId.HasValue)
            query = query.Where(b => b.ProductId == request.ProductId.Value);

        if (request.WarehouseId.HasValue)
            query = query.Where(b => b.WarehouseId == request.WarehouseId.Value);

        if (request.ExpiringSoon == true)
            query = query.Where(b => b.ExpiryDate.HasValue && 
                b.ExpiryDate.Value <= DateTime.UtcNow.AddDays(30) && 
                b.ExpiryDate.Value > DateTime.UtcNow);

        if (request.Expired == true)
            query = query.Where(b => b.ExpiryDate.HasValue && b.ExpiryDate.Value < DateTime.UtcNow);

        if (request.Recalled == true)
            query = query.Where(b => b.IsRecalled);

        // Apply search
        if (request.FilterRequest.Search != null && !string.IsNullOrWhiteSpace(request.FilterRequest.Search.Term))
        {
            var searchTerm = request.FilterRequest.Search.Term;
            query = query.Where(b => 
                b.BatchNumber.Contains(searchTerm) ||
                (b.LotNumber != null && b.LotNumber.Contains(searchTerm)) ||
                b.Product.Name.Contains(searchTerm) ||
                b.Product.SKU.Contains(searchTerm) ||
                (b.SupplierName != null && b.SupplierName.Contains(searchTerm)));
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        var pagination = request.FilterRequest.Pagination ?? new PaginationDto { PageNo = 1, PageSize = 10 };
        if (pagination.SortBy != null)
        {
            query = query.ApplySorting(pagination.SortBy, pagination.SortOrder);
        }

        // Apply pagination
        var items = await query
            .Skip((pagination.PageNo - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(b => new BatchDto
            {
                Id = b.Id,
                ProductId = b.ProductId,
                ProductName = b.Product.Name,
                ProductSKU = b.Product.SKU,
                WarehouseId = b.WarehouseId,
                WarehouseName = b.Warehouse.Name,
                BatchNumber = b.BatchNumber,
                LotNumber = b.LotNumber,
                ManufactureDate = b.ManufactureDate,
                ExpiryDate = b.ExpiryDate,
                Quantity = b.Quantity,
                UnitCost = b.UnitCost,
                SupplierName = b.SupplierName,
                SupplierBatchNumber = b.SupplierBatchNumber,
                Notes = b.Notes,
                IsRecalled = b.IsRecalled,
                RecallDate = b.RecallDate,
                RecallReason = b.RecallReason,
                IsExpired = b.ExpiryDate.HasValue && b.ExpiryDate.Value < DateTime.UtcNow,
                IsExpiringSoon = b.ExpiryDate.HasValue && 
                    b.ExpiryDate.Value <= DateTime.UtcNow.AddDays(30) && 
                    b.ExpiryDate.Value > DateTime.UtcNow,
                DaysUntilExpiry = b.ExpiryDate.HasValue ? 
                    (int?)(b.ExpiryDate.Value - DateTime.UtcNow).TotalDays : null,
                CreatedAt = b.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var result = new PagedResult<BatchDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNo = pagination.PageNo,
            PageSize = pagination.PageSize
        };

        return Result<PagedResult<BatchDto>>.Success(result);
    }
}

