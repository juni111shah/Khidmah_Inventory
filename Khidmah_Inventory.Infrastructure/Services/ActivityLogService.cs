using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Khidmah_Inventory.Infrastructure.Services;

public interface IActivityLogService
{
    Task LogActivityAsync(string entityType, Guid entityId, string action, string description, string? oldValues = null, string? newValues = null, string? ipAddress = null);
}

public class ActivityLogService : IActivityLogService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ActivityLogService(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task LogActivityAsync(string entityType, Guid entityId, string action, string description, string? oldValues = null, string? newValues = null, string? ipAddress = null)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return;

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId);

        var activityLog = new ActivityLog(
            companyId.Value,
            entityType,
            entityId,
            action,
            description,
            _currentUser.UserId,
            user?.UserName,
            ipAddress);

        if (!string.IsNullOrWhiteSpace(oldValues) || !string.IsNullOrWhiteSpace(newValues))
            activityLog.SetValues(oldValues, newValues);

        _context.ActivityLogs.Add(activityLog);
        await _context.SaveChangesAsync();
    }
}

