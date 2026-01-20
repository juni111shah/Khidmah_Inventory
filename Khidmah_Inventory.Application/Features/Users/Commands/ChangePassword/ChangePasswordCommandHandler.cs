using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUser;

    public ChangePasswordCommandHandler(
        IApplicationDbContext context,
        IIdentityService identityService,
        ICurrentUserService currentUser)
    {
        _context = context;
        _identityService = identityService;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

        if (user == null)
        {
            return Result.Failure("User not found");
        }

        // Check if user can change this password
        var currentUserId = _currentUser.UserId;
        if (currentUserId != request.UserId && !_currentUser.HasRole("Admin"))
        {
            return Result.Failure("You can only change your own password");
        }

        // Verify current password (unless admin is changing someone else's password)
        if (currentUserId == request.UserId)
        {
            var isPasswordValid = await _identityService.VerifyPasswordAsync(request.CurrentPassword, user.PasswordHash);
            if (!isPasswordValid)
            {
                return Result.Failure("Current password is incorrect");
            }
        }

        // Hash new password
        var newPasswordHash = await _identityService.GeneratePasswordHashAsync(request.NewPassword);
        user.ChangePassword(newPasswordHash, currentUserId);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

