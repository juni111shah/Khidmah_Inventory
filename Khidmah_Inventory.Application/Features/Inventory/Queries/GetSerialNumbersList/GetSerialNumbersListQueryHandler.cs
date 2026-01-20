using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Inventory.Models;
using Khidmah_Inventory.Application.Common.Extensions;

namespace Khidmah_Inventory.Application.Features.Inventory.Queries.GetSerialNumbersList;

public class GetSerialNumbersListQueryHandler : IRequestHandler<GetSerialNumbersListQuery, Result<PagedResult<SerialNumberDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetSerialNumbersListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<SerialNumberDto>>> Handle(GetSerialNumbersListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<PagedResult<SerialNumberDto>>.Failure("Company context is required");

        var query = _context.SerialNumbers
            .Include(s => s.Product)
            .Include(s => s.Warehouse)
            .Include(s => s.Batch)
            .Include(s => s.SalesOrder)
            .Include(s => s.Customer)
            .Where(s => s.CompanyId == companyId.Value && !s.IsDeleted)
            .AsQueryable();

        // Apply filters
        if (request.ProductId.HasValue)
            query = query.Where(s => s.ProductId == request.ProductId.Value);

        if (request.WarehouseId.HasValue)
            query = query.Where(s => s.WarehouseId == request.WarehouseId.Value);

        if (request.BatchId.HasValue)
            query = query.Where(s => s.BatchId == request.BatchId.Value);

        if (!string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(s => s.Status == request.Status);

        if (request.ExpiringSoon == true)
            query = query.Where(s => s.ExpiryDate.HasValue && 
                s.ExpiryDate.Value <= DateTime.UtcNow.AddDays(30) && 
                s.ExpiryDate.Value > DateTime.UtcNow);

        if (request.Expired == true)
            query = query.Where(s => s.ExpiryDate.HasValue && s.ExpiryDate.Value < DateTime.UtcNow);

        if (request.Recalled == true)
            query = query.Where(s => s.IsRecalled);

        // Apply search
        if (request.FilterRequest.Search != null && !string.IsNullOrWhiteSpace(request.FilterRequest.Search.Term))
        {
            var searchTerm = request.FilterRequest.Search.Term;
            query = query.Where(s => 
                s.SerialNumberValue.Contains(searchTerm) ||
                (s.BatchNumber != null && s.BatchNumber.Contains(searchTerm)) ||
                s.Product.Name.Contains(searchTerm) ||
                s.Product.SKU.Contains(searchTerm));
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
            .Select(s => new SerialNumberDto
            {
                Id = s.Id,
                ProductId = s.ProductId,
                ProductName = s.Product.Name,
                ProductSKU = s.Product.SKU,
                WarehouseId = s.WarehouseId,
                WarehouseName = s.Warehouse.Name,
                SerialNumberValue = s.SerialNumberValue,
                BatchNumber = s.BatchNumber,
                BatchId = s.BatchId,
                ManufactureDate = s.ManufactureDate,
                ExpiryDate = s.ExpiryDate,
                Status = s.Status,
                SalesOrderId = s.SalesOrderId,
                SalesOrderNumber = s.SalesOrder != null ? s.SalesOrder.OrderNumber : null,
                CustomerId = s.CustomerId,
                CustomerName = s.Customer != null ? s.Customer.Name : null,
                SoldDate = s.SoldDate,
                WarrantyExpiryDate = s.WarrantyExpiryDate,
                Notes = s.Notes,
                IsRecalled = s.IsRecalled,
                RecallDate = s.RecallDate,
                RecallReason = s.RecallReason,
                IsExpired = s.ExpiryDate.HasValue && s.ExpiryDate.Value < DateTime.UtcNow,
                IsExpiringSoon = s.ExpiryDate.HasValue && 
                    s.ExpiryDate.Value <= DateTime.UtcNow.AddDays(30) && 
                    s.ExpiryDate.Value > DateTime.UtcNow,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var result = new PagedResult<SerialNumberDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNo = pagination.PageNo,
            PageSize = pagination.PageSize
        };

        return Result<PagedResult<SerialNumberDto>>.Success(result);
    }
}

