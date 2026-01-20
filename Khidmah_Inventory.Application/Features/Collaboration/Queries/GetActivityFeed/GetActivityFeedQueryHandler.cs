using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Collaboration.Models;

namespace Khidmah_Inventory.Application.Features.Collaboration.Queries.GetActivityFeed;

public class GetActivityFeedQueryHandler : IRequestHandler<GetActivityFeedQuery, Result<List<ActivityLogDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetActivityFeedQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<ActivityLogDto>>> Handle(GetActivityFeedQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<List<ActivityLogDto>>.Failure("Company context is required");

        var query = _context.ActivityLogs
            .Where(a => a.CompanyId == companyId.Value && !a.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.EntityType))
            query = query.Where(a => a.EntityType == request.EntityType);

        if (request.EntityId.HasValue)
            query = query.Where(a => a.EntityId == request.EntityId.Value);

        var activities = await query
            .OrderByDescending(a => a.CreatedAt)
            .Take(request.Limit)
            .Select(a => new ActivityLogDto
            {
                Id = a.Id,
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                Action = a.Action,
                Description = a.Description,
                UserName = a.UserName,
                CreatedAt = a.CreatedAt,
                TimeAgo = GetTimeAgo(a.CreatedAt)
            })
            .ToListAsync(cancellationToken);

        return Result<List<ActivityLogDto>>.Success(activities);
    }

    private string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        if (timeSpan.TotalDays >= 365)
            return $"{(int)(timeSpan.TotalDays / 365)} year(s) ago";
        if (timeSpan.TotalDays >= 30)
            return $"{(int)(timeSpan.TotalDays / 30)} month(s) ago";
        if (timeSpan.TotalDays >= 1)
            return $"{(int)timeSpan.TotalDays} day(s) ago";
        if (timeSpan.TotalHours >= 1)
            return $"{(int)timeSpan.TotalHours} hour(s) ago";
        if (timeSpan.TotalMinutes >= 1)
            return $"{(int)timeSpan.TotalMinutes} minute(s) ago";
        return "Just now";
    }
}

