using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Inventory.Models;
using Khidmah_Inventory.Application.Common.Extensions;

namespace Khidmah_Inventory.Application.Features.Inventory.Queries.GetStockTransactionsList;

public class GetStockTransactionsListQueryHandler : IRequestHandler<GetStockTransactionsListQuery, Result<PagedResult<StockTransactionDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetStockTransactionsListQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<StockTransactionDto>>> Handle(GetStockTransactionsListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result<PagedResult<StockTransactionDto>>.Failure("Company context is required");
        }

        var filterRequest = request.FilterRequest ?? new FilterRequest
        {
            Pagination = new PaginationDto { PageNo = 1, PageSize = 10 }
        };

        var query = _context.StockTransactions
            .Include(t => t.Product)
            .Include(t => t.Warehouse)
            .Where(t => t.CompanyId == companyId.Value)
            .AsQueryable();

        if (request.ProductId.HasValue)
        {
            query = query.Where(t => t.ProductId == request.ProductId.Value);
        }

        if (request.WarehouseId.HasValue)
        {
            query = query.Where(t => t.WarehouseId == request.WarehouseId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.TransactionType))
        {
            query = query.Where(t => t.TransactionType == request.TransactionType);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate <= request.ToDate.Value);
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
                : new List<string> { "Product.Name", "Product.SKU", "Warehouse.Name", "ReferenceNumber" };

            query = query.ApplySearch(filterRequest.Search.Term, searchFields, filterRequest.Search.Mode, filterRequest.Search.IsCaseSensitive);
        }

        // Apply sorting
        if (filterRequest.Pagination != null && !string.IsNullOrWhiteSpace(filterRequest.Pagination.SortBy))
        {
            query = query.ApplySorting(filterRequest.Pagination.SortBy, filterRequest.Pagination.SortOrder ?? "descending");
        }
        else
        {
            query = query.OrderByDescending(t => t.TransactionDate);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var pagination = filterRequest.Pagination ?? new PaginationDto { PageNo = 1, PageSize = 10 };
        var transactions = await query
            .Skip((pagination.PageNo - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        var transactionDtos = transactions.Select(t => new StockTransactionDto
        {
            Id = t.Id,
            ProductId = t.ProductId,
            ProductName = t.Product.Name,
            ProductSKU = t.Product.SKU,
            WarehouseId = t.WarehouseId,
            WarehouseName = t.Warehouse.Name,
            TransactionType = t.TransactionType,
            Quantity = t.Quantity,
            UnitCost = t.UnitCost,
            TotalCost = t.TotalCost,
            ReferenceNumber = t.ReferenceNumber,
            ReferenceType = t.ReferenceType,
            ReferenceId = t.ReferenceId,
            BatchNumber = t.BatchNumber,
            ExpiryDate = t.ExpiryDate,
            Notes = t.Notes,
            TransactionDate = t.TransactionDate,
            BalanceAfter = t.BalanceAfter,
            CreatedAt = t.CreatedAt
        }).ToList();

        var result = new PagedResult<StockTransactionDto>
        {
            Items = transactionDtos,
            TotalCount = totalCount,
            PageNo = pagination.PageNo,
            PageSize = pagination.PageSize
        };

        return Result<PagedResult<StockTransactionDto>>.Success(result);
    }
}

