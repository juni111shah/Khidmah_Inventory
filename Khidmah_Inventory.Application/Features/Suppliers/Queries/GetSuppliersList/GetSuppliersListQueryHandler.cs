using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Suppliers.Models;
using Khidmah_Inventory.Application.Common.Extensions;

namespace Khidmah_Inventory.Application.Features.Suppliers.Queries.GetSuppliersList;

public class GetSuppliersListQueryHandler : IRequestHandler<GetSuppliersListQuery, Result<PagedResult<SupplierDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetSuppliersListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<SupplierDto>>> Handle(GetSuppliersListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<PagedResult<SupplierDto>>.Failure("Company context is required");

        var filterRequest = request.FilterRequest ?? new FilterRequest
        {
            Pagination = new PaginationDto { PageNo = 1, PageSize = 10 }
        };

        var query = _context.Suppliers
            .Where(s => s.CompanyId == companyId.Value && !s.IsDeleted)
            .AsQueryable();

        if (request.IsActive.HasValue)
            query = query.Where(s => s.IsActive == request.IsActive.Value);

        if (filterRequest.Filters != null && filterRequest.Filters.Any())
            query = query.ApplyFilters(filterRequest.Filters);

        if (filterRequest.Search != null && !string.IsNullOrWhiteSpace(filterRequest.Search.Term))
        {
            var searchFields = filterRequest.Search.SearchFields.Any()
                ? filterRequest.Search.SearchFields
                : new List<string> { "Name", "Code", "Email", "PhoneNumber", "City", "Country" };

            query = query.ApplySearch(filterRequest.Search.Term, searchFields, filterRequest.Search.Mode, filterRequest.Search.IsCaseSensitive);
        }

        if (filterRequest.Pagination != null && !string.IsNullOrWhiteSpace(filterRequest.Pagination.SortBy))
            query = query.ApplySorting(filterRequest.Pagination.SortBy, filterRequest.Pagination.SortOrder ?? "ascending");
        else
            query = query.OrderBy(s => s.Name);

        var totalCount = await query.CountAsync(cancellationToken);
        var pagination = filterRequest.Pagination ?? new PaginationDto { PageNo = 1, PageSize = 10 };
        var suppliers = await query
            .Skip((pagination.PageNo - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        var supplierDtos = suppliers.Select(s => new SupplierDto
        {
            Id = s.Id,
            Name = s.Name,
            Code = s.Code,
            ContactPerson = s.ContactPerson,
            Email = s.Email,
            PhoneNumber = s.PhoneNumber,
            Address = s.Address,
            City = s.City,
            State = s.State,
            Country = s.Country,
            PostalCode = s.PostalCode,
            TaxId = s.TaxId,
            PaymentTerms = s.PaymentTerms,
            CreditLimit = s.CreditLimit,
            Balance = s.Balance,
            IsActive = s.IsActive,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        }).ToList();

        var result = new PagedResult<SupplierDto>
        {
            Items = supplierDtos,
            TotalCount = totalCount,
            PageNo = pagination.PageNo,
            PageSize = pagination.PageSize
        };

        return Result<PagedResult<SupplierDto>>.Success(result);
    }
}

