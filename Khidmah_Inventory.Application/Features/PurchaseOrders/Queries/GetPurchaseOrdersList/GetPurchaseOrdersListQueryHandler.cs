using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.PurchaseOrders.Models;
using Khidmah_Inventory.Application.Common.Extensions;

namespace Khidmah_Inventory.Application.Features.PurchaseOrders.Queries.GetPurchaseOrdersList;

public class GetPurchaseOrdersListQueryHandler : IRequestHandler<GetPurchaseOrdersListQuery, Result<PagedResult<PurchaseOrderDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetPurchaseOrdersListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<PurchaseOrderDto>>> Handle(GetPurchaseOrdersListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<PagedResult<PurchaseOrderDto>>.Failure("Company context is required");

        var filterRequest = request.FilterRequest ?? new FilterRequest
        {
            Pagination = new PaginationDto { PageNo = 1, PageSize = 10 }
        };

        var query = _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Where(po => po.CompanyId == companyId.Value && !po.IsDeleted)
            .AsQueryable();

        if (request.SupplierId.HasValue)
            query = query.Where(po => po.SupplierId == request.SupplierId.Value);

        if (!string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(po => po.Status == request.Status);

        if (request.FromDate.HasValue)
            query = query.Where(po => po.OrderDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(po => po.OrderDate <= request.ToDate.Value);

        if (filterRequest.Filters != null && filterRequest.Filters.Any())
            query = query.ApplyFilters(filterRequest.Filters);

        if (filterRequest.Search != null && !string.IsNullOrWhiteSpace(filterRequest.Search.Term))
        {
            var searchFields = filterRequest.Search.SearchFields.Any()
                ? filterRequest.Search.SearchFields
                : new List<string> { "OrderNumber", "Supplier.Name" };

            query = query.ApplySearch(filterRequest.Search.Term, searchFields, filterRequest.Search.Mode, filterRequest.Search.IsCaseSensitive);
        }

        if (filterRequest.Pagination != null && !string.IsNullOrWhiteSpace(filterRequest.Pagination.SortBy))
            query = query.ApplySorting(filterRequest.Pagination.SortBy, filterRequest.Pagination.SortOrder ?? "descending");
        else
            query = query.OrderByDescending(po => po.OrderDate);

        var totalCount = await query.CountAsync(cancellationToken);
        var pagination = filterRequest.Pagination ?? new PaginationDto { PageNo = 1, PageSize = 10 };
        var purchaseOrders = await query
            .Skip((pagination.PageNo - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        var purchaseOrderDtos = purchaseOrders.Select(po => new PurchaseOrderDto
        {
            Id = po.Id,
            OrderNumber = po.OrderNumber,
            SupplierId = po.SupplierId,
            SupplierName = po.Supplier.Name,
            OrderDate = po.OrderDate,
            ExpectedDeliveryDate = po.ExpectedDeliveryDate,
            Status = po.Status,
            SubTotal = po.SubTotal,
            TaxAmount = po.TaxAmount,
            DiscountAmount = po.DiscountAmount,
            TotalAmount = po.TotalAmount,
            Notes = po.Notes,
            TermsAndConditions = po.TermsAndConditions,
            Items = new List<PurchaseOrderItemDto>(), // Items not loaded for list view
            CreatedAt = po.CreatedAt,
            UpdatedAt = po.UpdatedAt
        }).ToList();

        var result = new PagedResult<PurchaseOrderDto>
        {
            Items = purchaseOrderDtos,
            TotalCount = totalCount,
            PageNo = pagination.PageNo,
            PageSize = pagination.PageSize
        };

        return Result<PagedResult<PurchaseOrderDto>>.Success(result);
    }
}

