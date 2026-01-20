using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public LogoutCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
        {
            return Result.Failure("User not authenticated");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId.Value && !u.IsDeleted, cancellationToken);

        if (user != null)
        {
            // Clear refresh token
            user.ClearRefreshToken();
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}

