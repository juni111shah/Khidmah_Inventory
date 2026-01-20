using Microsoft.AspNetCore.Authorization;
using Khidmah_Inventory.Application.Common.Interfaces;

namespace Khidmah_Inventory.API.Authorization;

/// <summary>
/// Authorization handler for permission-based access control
/// </summary>
public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ICurrentUserService _currentUser;

    public PermissionHandler(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (_currentUser.IsAuthenticated && _currentUser.HasPermission(requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

