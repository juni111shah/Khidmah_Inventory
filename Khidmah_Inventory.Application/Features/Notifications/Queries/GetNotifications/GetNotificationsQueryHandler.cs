using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Common.Extensions;
using Khidmah_Inventory.Application.Features.Notifications.Models;

namespace Khidmah_Inventory.Application.Features.Notifications.Queries.GetNotifications;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, Result<PagedResult<NotificationDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetNotificationsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<NotificationDto>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<PagedResult<NotificationDto>>.Failure("Company context is required");

        var userId = _currentUser.UserId;

        var query = _context.Notifications
            .Where(n => n.CompanyId == companyId.Value &&
                (n.UserId == null || n.UserId == userId))
            .AsQueryable();

        if (request.UnreadOnly == true)
            query = query.Where(n => !n.IsRead);

        if (request.FilterRequest?.Filters != null && request.FilterRequest.Filters.Any())
            query = query.ApplyFilters(request.FilterRequest.Filters);

        if (request.FilterRequest?.Search != null && !string.IsNullOrWhiteSpace(request.FilterRequest.Search.Term))
        {
            var term = request.FilterRequest.Search.Term;
            var searchFields = request.FilterRequest.Search.SearchFields?.Any() == true
                ? request.FilterRequest.Search.SearchFields
                : new List<string> { "Title", "Message" };
            query = query.ApplySearch(term, searchFields,
                request.FilterRequest.Search.Mode,
                request.FilterRequest.Search.IsCaseSensitive);
        }

        var pagination = request.FilterRequest?.Pagination ?? new PaginationDto { PageNo = 1, PageSize = 20 };
        if (!string.IsNullOrWhiteSpace(pagination.SortBy))
            query = query.ApplySorting(pagination.SortBy, pagination.SortOrder ?? "descending");
        else
            query = query.OrderByDescending(n => n.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagination.PageNo - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                CompanyId = n.CompanyId,
                UserId = n.UserId,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                EntityType = n.EntityType,
                EntityId = n.EntityId,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var result = new PagedResult<NotificationDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNo = pagination.PageNo,
            PageSize = pagination.PageSize
        };

        return Result<PagedResult<NotificationDto>>.Success(result);
    }
}
