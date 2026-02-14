using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Common.Extensions;
using Khidmah_Inventory.Application.Features.Companies.Models;

namespace Khidmah_Inventory.Application.Features.Companies.Queries.GetCompaniesList;

public class GetCompaniesListQueryHandler : IRequestHandler<GetCompaniesListQuery, Result<PagedResult<CompanyDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetCompaniesListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<CompanyDto>>> Handle(GetCompaniesListQuery request, CancellationToken cancellationToken)
    {
        var filterRequest = request.FilterRequest ?? new FilterRequest
        {
            Pagination = new PaginationDto { PageNo = 1, PageSize = 10 }
        };

        var query = _context.Companies.AsQueryable();

        if (request.IsActive.HasValue)
        {
            query = query.Where(c => c.IsActive == request.IsActive.Value);
        }

        if (filterRequest.Filters != null && filterRequest.Filters.Any())
        {
            query = query.ApplyFilters(filterRequest.Filters);
        }

        if (filterRequest.Search != null && !string.IsNullOrWhiteSpace(filterRequest.Search.Term))
        {
            var searchFields = filterRequest.Search.SearchFields != null && filterRequest.Search.SearchFields.Any()
                ? filterRequest.Search.SearchFields
                : new List<string> { "Name", "LegalName", "Email", "City", "Country" };

            query = query.ApplySearch(filterRequest.Search.Term, searchFields, filterRequest.Search.Mode, filterRequest.Search.IsCaseSensitive);
        }

        if (filterRequest.Pagination != null && !string.IsNullOrWhiteSpace(filterRequest.Pagination.SortBy))
        {
            query = query.ApplySorting(filterRequest.Pagination.SortBy, filterRequest.Pagination.SortOrder ?? "ascending");
        }
        else
        {
            query = query.OrderBy(c => c.Name);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var pagination = filterRequest.Pagination ?? new PaginationDto { PageNo = 1, PageSize = 10 };
        var companies = await query
            .Skip((pagination.PageNo - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = companies.Select(c => new CompanyDto
        {
            Id = c.Id,
            Name = c.Name,
            LegalName = c.LegalName,
            TaxId = c.TaxId,
            RegistrationNumber = c.RegistrationNumber,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            Address = c.Address,
            City = c.City,
            State = c.State,
            Country = c.Country,
            PostalCode = c.PostalCode,
            LogoUrl = c.LogoUrl,
            Currency = c.Currency,
            TimeZone = c.TimeZone,
            IsActive = c.IsActive,
            SubscriptionExpiresAt = c.SubscriptionExpiresAt,
            SubscriptionPlan = c.SubscriptionPlan,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();

        var result = new PagedResult<CompanyDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNo = pagination.PageNo,
            PageSize = pagination.PageSize
        };

        return Result<PagedResult<CompanyDto>>.Success(result);
    }
}
