using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Roles.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Roles.Commands.UpdateRole;

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result<RoleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateRoleCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result<RoleDto>.Failure("Company context is required");
        }

        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Id == request.Id && r.CompanyId == companyId.Value && !r.IsDeleted, cancellationToken);

        if (role == null)
        {
            return Result<RoleDto>.Failure("Role not found");
        }

        if (role.IsSystemRole)
        {
            return Result<RoleDto>.Failure("Cannot update system role");
        }

        // Check if name is already taken by another role
        var existingRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.CompanyId == companyId.Value && r.Name == request.Name && r.Id != request.Id && !r.IsDeleted, cancellationToken);

        if (existingRole != null)
        {
            return Result<RoleDto>.Failure("Role with this name already exists");
        }

        // Update role
        role.Update(request.Name, request.Description, _currentUser.UserId);

        // Update permissions
        var currentPermissionIds = role.RolePermissions.Select(rp => rp.PermissionId).ToList();
        var permissionsToAdd = request.PermissionIds.Except(currentPermissionIds).ToList();
        var permissionsToRemove = currentPermissionIds.Except(request.PermissionIds).ToList();

        // Remove permissions
        if (permissionsToRemove.Any())
        {
            var rolePermissionsToRemove = role.RolePermissions
                .Where(rp => permissionsToRemove.Contains(rp.PermissionId))
                .ToList();
            foreach (var rp in rolePermissionsToRemove)
            {
                _context.RolePermissions.Remove(rp);
            }
        }

        // Add permissions
        if (permissionsToAdd.Any())
        {
            var permissions = await _context.Permissions
                .Where(p => permissionsToAdd.Contains(p.Id) && p.CompanyId == companyId.Value && !p.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var permission in permissions)
            {
                var rolePermission = new RolePermission(companyId.Value, role.Id, permission.Id, _currentUser.UserId);
                role.RolePermissions.Add(rolePermission);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Reload with includes
        role = await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Id == role.Id, cancellationToken);

        var dto = new RoleDto
        {
            Id = role!.Id,
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

