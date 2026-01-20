using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.SalesOrders.Models;
using Khidmah_Inventory.Application.Common.Extensions;

namespace Khidmah_Inventory.Application.Features.SalesOrders.Queries.GetSalesOrdersList;

public class GetSalesOrdersListQueryHandler : IRequestHandler<GetSalesOrdersListQuery, Result<PagedResult<SalesOrderDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetSalesOrdersListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<SalesOrderDto>>> Handle(GetSalesOrdersListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<PagedResult<SalesOrderDto>>.Failure("Company context is required");

        var filterRequest = request.FilterRequest ?? new FilterRequest
        {
            Pagination = new PaginationDto { PageNo = 1, PageSize = 10 }
        };

        var query = _context.SalesOrders
            .Include(so => so.Customer)
            .Where(so => so.CompanyId == companyId.Value && !so.IsDeleted)
            .AsQueryable();

        if (request.CustomerId.HasValue)
            query = query.Where(so => so.CustomerId == request.CustomerId.Value);

        if (!string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(so => so.Status == request.Status);

        if (request.FromDate.HasValue)
            query = query.Where(so => so.OrderDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(so => so.OrderDate <= request.ToDate.Value);

        if (filterRequest.Filters != null && filterRequest.Filters.Any())
            query = query.ApplyFilters(filterRequest.Filters);

        if (filterRequest.Search != null && !string.IsNullOrWhiteSpace(filterRequest.Search.Term))
        {
            var searchFields = filterRequest.Search.SearchFields.Any()
                ? filterRequest.Search.SearchFields
                : new List<string> { "OrderNumber", "Customer.Name" };

            query = query.ApplySearch(filterRequest.Search.Term, searchFields, filterRequest.Search.Mode, filterRequest.Search.IsCaseSensitive);
        }

        if (filterRequest.Pagination != null && !string.IsNullOrWhiteSpace(filterRequest.Pagination.SortBy))
            query = query.ApplySorting(filterRequest.Pagination.SortBy, filterRequest.Pagination.SortOrder ?? "descending");
        else
            query = query.OrderByDescending(so => so.OrderDate);

        var totalCount = await query.CountAsync(cancellationToken);
        var pagination = filterRequest.Pagination ?? new PaginationDto { PageNo = 1, PageSize = 10 };
        var salesOrders = await query
            .Skip((pagination.PageNo - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        var salesOrderDtos = salesOrders.Select(so => new SalesOrderDto
        {
            Id = so.Id,
            OrderNumber = so.OrderNumber,
            CustomerId = so.CustomerId,
            CustomerName = so.Customer.Name,
            OrderDate = so.OrderDate,
            ExpectedDeliveryDate = so.ExpectedDeliveryDate,
            Status = so.Status,
            SubTotal = so.SubTotal,
            TaxAmount = so.TaxAmount,
            DiscountAmount = so.DiscountAmount,
            TotalAmount = so.TotalAmount,
            Notes = so.Notes,
            TermsAndConditions = so.TermsAndConditions,
            Items = new List<SalesOrderItemDto>(),
            CreatedAt = so.CreatedAt,
            UpdatedAt = so.UpdatedAt
        }).ToList();

        var result = new PagedResult<SalesOrderDto>
        {
            Items = salesOrderDtos,
            TotalCount = totalCount,
            PageNo = pagination.PageNo,
            PageSize = pagination.PageSize
        };

        return Result<PagedResult<SalesOrderDto>>.Success(result);
    }
}

