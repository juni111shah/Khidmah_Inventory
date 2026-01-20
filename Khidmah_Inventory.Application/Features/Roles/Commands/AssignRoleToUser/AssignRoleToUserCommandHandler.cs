using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Roles.Commands.AssignRoleToUser;

public class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AssignRoleToUserCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result.Failure("Company context is required");
        }

        // Check if user exists and belongs to company
        var user = await _context.Users
            .Include(u => u.UserCompanies)
            .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

        if (user == null)
        {
            return Result.Failure("User not found");
        }

        if (!user.UserCompanies.Any(uc => uc.CompanyId == companyId.Value && uc.IsActive))
        {
            return Result.Failure("User does not belong to your company");
        }

        // Check if role exists and belongs to company
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == request.RoleId && r.CompanyId == companyId.Value && !r.IsDeleted, cancellationToken);

        if (role == null)
        {
            return Result.Failure("Role not found");
        }

        // Check if user-role relationship already exists
        var existingUserRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == request.UserId && ur.RoleId == request.RoleId && ur.CompanyId == companyId.Value && !ur.IsDeleted, cancellationToken);

        if (existingUserRole != null)
        {
            return Result.Failure("User already has this role");
        }

        // Create user-role relationship
        var userRole = new UserRole(companyId.Value, request.UserId, request.RoleId, _currentUser.UserId);
        _context.UserRoles.Add(userRole);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

