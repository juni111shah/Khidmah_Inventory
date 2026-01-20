using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Users.Models;

namespace Khidmah_Inventory.Application.Features.Users.Queries.GetUser;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<UserDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetUserQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .Include(u => u.UserCompanies)
                .ThenInclude(uc => uc.Company)
            .FirstOrDefaultAsync(u => u.Id == request.Id && !u.IsDeleted, cancellationToken);

        if (user == null)
        {
            return Result<UserDto>.Failure("User not found");
        }

        // Check if user belongs to current company
        var companyId = _currentUser.CompanyId;
        if (companyId.HasValue && !user.UserCompanies.Any(uc => uc.CompanyId == companyId.Value && uc.IsActive))
        {
            return Result<UserDto>.Failure("User not found in your company");
        }

        var dto = MapToDto(user, companyId);
        return Result<UserDto>.Success(dto);
    }

    private UserDto MapToDto(Domain.Entities.User user, Guid? companyId)
    {
        var roles = user.UserRoles
            .Where(ur => !companyId.HasValue || ur.Role.CompanyId == companyId.Value)
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToList();

        var permissions = user.UserRoles
            .Where(ur => !companyId.HasValue || ur.Role.CompanyId == companyId.Value)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToList();

        var companies = user.UserCompanies
            .Where(uc => !companyId.HasValue || uc.CompanyId == companyId.Value)
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
            Permissions = permissions,
            Companies = companies,
            DefaultCompanyId = defaultCompany?.CompanyId,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt ?? user.CreatedAt
        };
    }
}

