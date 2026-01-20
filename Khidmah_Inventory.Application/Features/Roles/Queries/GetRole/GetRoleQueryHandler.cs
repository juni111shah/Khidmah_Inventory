using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Roles.Models;

namespace Khidmah_Inventory.Application.Features.Roles.Queries.GetRole;

public class GetRoleQueryHandler : IRequestHandler<GetRoleQuery, Result<RoleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetRoleQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<RoleDto>> Handle(GetRoleQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result<RoleDto>.Failure("Company context is required");
        }

        var role = await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Id == request.Id && r.CompanyId == companyId.Value && !r.IsDeleted, cancellationToken);

        if (role == null)
        {
            return Result<RoleDto>.Failure("Role not found");
        }

        var dto = new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsSystemRole = role.IsSystemRole,
            UserCount = role.UserRoles.Count,
            PermissionCount = role.RolePermissions.Count,
            Permissions = role.RolePermissions
                .Select(rp => new PermissionDto
                {
                    Id = rp.Permission.Id,
                    Name = rp.Permission.Name,
                    Description = rp.Permission.Description,
                    Module = rp.Permission.Module,
                    Action = rp.Permission.Action
                })
                .ToList(),
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt ?? role.CreatedAt
        };

        return Result<RoleDto>.Success(dto);
    }
}

