using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Notifications.Queries.GetUnreadCount;

public class GetUnreadNotificationsCountQueryHandler : IRequestHandler<GetUnreadNotificationsCountQuery, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetUnreadNotificationsCountQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<int>> Handle(GetUnreadNotificationsCountQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<int>.Failure("Company context is required");

        var userId = _currentUser.UserId;
        var count = await _context.Notifications
            .CountAsync(n =>
                n.CompanyId == companyId.Value &&
                (n.UserId == null || n.UserId == userId) &&
                !n.IsRead,
                cancellationToken);

        return Result<int>.Success(count);
    }
}
