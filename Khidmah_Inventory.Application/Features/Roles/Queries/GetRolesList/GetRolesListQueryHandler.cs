using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Roles.Models;

namespace Khidmah_Inventory.Application.Features.Roles.Queries.GetRolesList;

public class GetRolesListQueryHandler : IRequestHandler<GetRolesListQuery, Result<List<RoleDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetRolesListQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<RoleDto>>> Handle(GetRolesListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result<List<RoleDto>>.Failure("Company context is required");
        }

        var roles = await _context.Roles
            .Include(r => r.RolePermissions)
            .Include(r => r.UserRoles)
            .Where(r => r.CompanyId == companyId.Value && !r.IsDeleted)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);

        var dtos = roles.Select(role => new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsSystemRole = role.IsSystemRole,
            UserCount = role.UserRoles.Count,
            PermissionCount = role.RolePermissions.Count,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt ?? role.CreatedAt
        }).ToList();

        return Result<List<RoleDto>>.Success(dtos);
    }
}

