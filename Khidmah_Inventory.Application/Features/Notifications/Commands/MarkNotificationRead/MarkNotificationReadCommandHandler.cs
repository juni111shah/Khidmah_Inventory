using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Notifications.Commands.MarkNotificationRead;

public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public MarkNotificationReadCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result.Failure("Company context is required");

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n =>
                n.Id == request.Id &&
                n.CompanyId == companyId.Value &&
                (n.UserId == null || n.UserId == _currentUser.UserId),
                cancellationToken);

        if (notification == null)
            return Result.Failure("Notification not found.");

        notification.MarkAsRead();
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
