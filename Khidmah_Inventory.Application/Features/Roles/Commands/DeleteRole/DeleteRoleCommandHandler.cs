using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Roles.Commands.DeleteRole;

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteRoleCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result.Failure("Company context is required");
        }

        var role = await _context.Roles
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Id == request.Id && r.CompanyId == companyId.Value && !r.IsDeleted, cancellationToken);

        if (role == null)
        {
            return Result.Failure("Role not found");
        }

        if (role.IsSystemRole)
        {
            return Result.Failure("Cannot delete system role");
        }

        if (role.UserRoles.Any())
        {
            return Result.Failure("Cannot delete role that is assigned to users. Please remove all user assignments first.");
        }

        // Soft delete
        role.MarkAsDeleted(_currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

