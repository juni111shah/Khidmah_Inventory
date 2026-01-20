using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Users.Models;
using Khidmah_Inventory.Application.Common.Extensions;
using CompanyDto = Khidmah_Inventory.Application.Features.Users.Models.CompanyDto;

namespace Khidmah_Inventory.Application.Features.Users.Queries.GetUsersList;

public class GetUsersListQueryHandler : IRequestHandler<GetUsersListQuery, Result<PagedResult<UserDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetUsersListQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<UserDto>>> Handle(GetUsersListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result<PagedResult<UserDto>>.Failure("Company context is required");
        }

        var filterRequest = request.FilterRequest ?? new FilterRequest
        {
            Pagination = new PaginationDto { PageNo = 1, PageSize = 10 }
        };

        // Base query: users in current company
        var query = _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserCompanies)
                .ThenInclude(uc => uc.Company)
            .Where(u => u.UserCompanies.Any(uc => uc.CompanyId == companyId.Value && uc.IsActive) && !u.IsDeleted)
            .AsQueryable();

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
                : new List<string> { "FirstName", "LastName", "Email", "UserName" };

            query = query.ApplySearch(filterRequest.Search.Term, searchFields, filterRequest.Search.Mode, filterRequest.Search.IsCaseSensitive);
        }

        // Apply sorting
        if (filterRequest.Pagination != null && !string.IsNullOrWhiteSpace(filterRequest.Pagination.SortBy))
        {
            query = query.ApplySorting(filterRequest.Pagination.SortBy, filterRequest.Pagination.SortOrder ?? "ascending");
        }
        else
        {
            query = query.OrderBy(u => u.FirstName).ThenBy(u => u.LastName);
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var pagination = filterRequest.Pagination ?? new PaginationDto { PageNo = 1, PageSize = 10 };
        var users = await query
            .Skip((pagination.PageNo - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        // Map to DTOs
        var userDtos = users.Select(u => MapToDto(u, companyId.Value)).ToList();

        var result = new PagedResult<UserDto>
        {
            Items = userDtos,
            TotalCount = totalCount,
            PageNo = pagination.PageNo,
            PageSize = pagination.PageSize
        };

        return Result<PagedResult<UserDto>>.Success(result);
    }

    private UserDto MapToDto(Domain.Entities.User user, Guid companyId)
    {
        var roles = user.UserRoles
            .Where(ur => ur.Role.CompanyId == companyId)
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToList();

        var companies = user.UserCompanies
            .Where(uc => uc.CompanyId == companyId)
            .Select(uc => new CompanyDto
            {
                Id = uc.CompanyId,
                Name = uc.Company.Name,
                IsDefault = uc.IsDefault,
                IsActive = uc.IsActive
            })
            .ToList();

        var defaultCompany = user.UserCompanies.FirstOrDefault(uc => uc.IsDefault && uc.IsActive);

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            EmailConfirmed = user.EmailConfirmed,
            LastLoginAt = user.LastLoginAt,
            Roles = roles,
            Companies = companies,
            DefaultCompanyId = defaultCompany?.CompanyId,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt ?? user.CreatedAt
        };
    }
}

