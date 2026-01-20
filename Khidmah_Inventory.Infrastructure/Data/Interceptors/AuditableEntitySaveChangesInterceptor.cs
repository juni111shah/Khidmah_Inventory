using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Khidmah_Inventory.Domain.Common;
using Khidmah_Inventory.Application.Common.Interfaces;

namespace Khidmah_Inventory.Infrastructure.Data.Interceptors;

public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    public AuditableEntitySaveChangesInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        var userId = _currentUserService.UserId;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(nameof(BaseEntity.CreatedAt)).CurrentValue = DateTime.UtcNow;
                    entry.Property(nameof(BaseEntity.CreatedBy)).CurrentValue = userId;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdateAuditInfo(userId);
                    break;

                case EntityState.Deleted:
                    if (entry.Entity is ISoftDeletable softDeletable && !softDeletable.IsDeleted)
                    {
                        entry.State = EntityState.Modified;
                        entry.Entity.MarkAsDeleted(userId);
                    }
                    break;
            }
        }
    }
}

