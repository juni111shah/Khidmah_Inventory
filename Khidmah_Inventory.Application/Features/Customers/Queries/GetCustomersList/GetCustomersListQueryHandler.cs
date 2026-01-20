using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Customers.Models;
using Khidmah_Inventory.Application.Common.Extensions;

namespace Khidmah_Inventory.Application.Features.Customers.Queries.GetCustomersList;

public class GetCustomersListQueryHandler : IRequestHandler<GetCustomersListQuery, Result<PagedResult<CustomerDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetCustomersListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<CustomerDto>>> Handle(GetCustomersListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<PagedResult<CustomerDto>>.Failure("Company context is required");

        var filterRequest = request.FilterRequest ?? new FilterRequest
        {
            Pagination = new PaginationDto { PageNo = 1, PageSize = 10 }
        };

        var query = _context.Customers
            .Where(c => c.CompanyId == companyId.Value && !c.IsDeleted)
            .AsQueryable();

        if (request.IsActive.HasValue)
            query = query.Where(c => c.IsActive == request.IsActive.Value);

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
            query = query.OrderBy(c => c.Name);

        var totalCount = await query.CountAsync(cancellationToken);
        var pagination = filterRequest.Pagination ?? new PaginationDto { PageNo = 1, PageSize = 10 };
        var customers = await query
            .Skip((pagination.PageNo - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        var customerDtos = customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            Name = c.Name,
            Code = c.Code,
            ContactPerson = c.ContactPerson,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            Address = c.Address,
            City = c.City,
            State = c.State,
            Country = c.Country,
            PostalCode = c.PostalCode,
            TaxId = c.TaxId,
            PaymentTerms = c.PaymentTerms,
            CreditLimit = c.CreditLimit,
            Balance = c.Balance,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();

        var result = new PagedResult<CustomerDto>
        {
            Items = customerDtos,
            TotalCount = totalCount,
            PageNo = pagination.PageNo,
            PageSize = pagination.PageSize
        };

        return Result<PagedResult<CustomerDto>>.Success(result);
    }
}

