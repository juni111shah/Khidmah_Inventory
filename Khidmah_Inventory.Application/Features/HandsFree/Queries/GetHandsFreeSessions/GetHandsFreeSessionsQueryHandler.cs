using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.HandsFree.Models;

namespace Khidmah_Inventory.Application.Features.HandsFree.Queries.GetHandsFreeSessions;

public class GetHandsFreeSessionsQueryHandler : IRequestHandler<GetHandsFreeSessionsQuery, Result<List<HandsFreeSupervisorSessionDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetHandsFreeSessionsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<HandsFreeSupervisorSessionDto>>> Handle(GetHandsFreeSessionsQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<List<HandsFreeSupervisorSessionDto>>.Failure("Company context is required.");

        var threshold = DateTime.UtcNow.AddMinutes(-Math.Abs(request.ActiveWithinMinutes <= 0 ? 60 : request.ActiveWithinMinutes));

        var grouped = await _context.StockTransactions
            .AsNoTracking()
            .Where(t => t.CompanyId == companyId.Value &&
                        !t.IsDeleted &&
                        t.ReferenceType == "HandsFree" &&
                        t.CreatedBy != null)
            .GroupBy(t => new { UserId = t.CreatedBy!.Value, t.WarehouseId })
            .Select(g => new
            {
                g.Key.UserId,
                g.Key.WarehouseId,
                StartedAt = g.Min(x => x.CreatedAt),
                LastActivityAt = g.Max(x => x.CreatedAt),
                CompletedTasks = g.Count()
            })
            .Where(x => x.LastActivityAt >= threshold)
            .OrderByDescending(x => x.LastActivityAt)
            .ToListAsync(cancellationToken);

        if (grouped.Count == 0)
            return Result<List<HandsFreeSupervisorSessionDto>>.Success(new List<HandsFreeSupervisorSessionDto>());

        var userIds = grouped.Select(x => x.UserId).Distinct().ToList();
        var warehouseIds = grouped.Select(x => x.WarehouseId).Distinct().ToList();

        var users = await _context.Users
            .AsNoTracking()
            .Where(u => u.CompanyId == companyId.Value && userIds.Contains(u.Id) && !u.IsDeleted)
            .ToDictionaryAsync(u => u.Id, u => $"{u.FirstName} {u.LastName}".Trim(), cancellationToken);

        var warehouses = await _context.Warehouses
            .AsNoTracking()
            .Where(w => w.CompanyId == companyId.Value && warehouseIds.Contains(w.Id) && !w.IsDeleted)
            .ToDictionaryAsync(w => w.Id, w => w.Name, cancellationToken);

        var sessions = grouped.Select(x =>
        {
            var name = users.TryGetValue(x.UserId, out var displayName) && !string.IsNullOrWhiteSpace(displayName)
                ? displayName
                : x.UserId.ToString();
            var warehouseName = warehouses.TryGetValue(x.WarehouseId, out var wn) ? wn : "Unknown warehouse";
            var completed = x.CompletedTasks;
            return new HandsFreeSupervisorSessionDto
            {
                UserId = x.UserId,
                UserName = name,
                SessionId = Guid.NewGuid(),
                StartedAt = x.StartedAt,
                LastActivityAt = x.LastActivityAt,
                WarehouseId = x.WarehouseId,
                WarehouseName = warehouseName,
                TotalTasks = Math.Max(completed, 1),
                CompletedTasks = completed,
                CurrentTaskIndex = completed,
                Errors = 0
            };
        }).ToList();

        return Result<List<HandsFreeSupervisorSessionDto>>.Success(sessions);
    }
}
