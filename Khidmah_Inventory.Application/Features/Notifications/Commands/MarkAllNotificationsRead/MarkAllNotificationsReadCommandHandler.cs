using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Notifications.Commands.MarkAllNotificationsRead;

public class MarkAllNotificationsReadCommandHandler : IRequestHandler<MarkAllNotificationsReadCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public MarkAllNotificationsReadCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result.Failure("Company context is required");

        var userId = _currentUser.UserId;
        var notifications = await _context.Notifications
            .Where(n => n.CompanyId == companyId.Value &&
                (n.UserId == null || n.UserId == userId) &&
                !n.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var n in notifications)
            n.MarkAsRead();

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
