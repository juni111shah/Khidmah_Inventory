using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Roles.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Roles.Commands.CreateRole;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<RoleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateRoleCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result<RoleDto>.Failure("Company context is required");
        }

        // Check if role name already exists
        var existingRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.CompanyId == companyId.Value && r.Name == request.Name && !r.IsDeleted, cancellationToken);

        if (existingRole != null)
        {
            return Result<RoleDto>.Failure("Role with this name already exists");
        }

        // Create role
        var role = new Role(companyId.Value, request.Name, request.Description, false, _currentUser.UserId);

        // Assign permissions
        if (request.PermissionIds.Any())
        {
            var permissions = await _context.Permissions
                .Where(p => request.PermissionIds.Contains(p.Id) && p.CompanyId == companyId.Value && !p.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var permission in permissions)
            {
                var rolePermission = new RolePermission(companyId.Value, role.Id, permission.Id, _currentUser.UserId);
                role.RolePermissions.Add(rolePermission);
            }
        }

        _context.Roles.Add(role);
        await _context.SaveChangesAsync(cancellationToken);

        // Reload with includes
        var createdRole = await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Id == role.Id, cancellationToken);

        var dto = new RoleDto
        {
            Id = createdRole!.Id,
            Name = createdRole.Name,
            Description = createdRole.Description,
            IsSystemRole = createdRole.IsSystemRole,
            UserCount = createdRole.UserRoles.Count,
            PermissionCount = createdRole.RolePermissions.Count,
            Permissions = createdRole.RolePermissions
                .Select(rp => new PermissionDto
                {
                    Id = rp.Permission.Id,
                    Name = rp.Permission.Name,
                    Description = rp.Permission.Description,
                    Module = rp.Permission.Module,
                    Action = rp.Permission.Action
                })
                .ToList(),
            CreatedAt = createdRole.CreatedAt,
            UpdatedAt = createdRole.UpdatedAt ?? createdRole.CreatedAt
        };

        return Result<RoleDto>.Success(dto);
    }
}

