using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Khidmah_Inventory.Application.Common.Constants;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Domain.Common;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Data.Interceptors;

public class DomainChangeBroadcastInterceptor : SaveChangesInterceptor
{
    private readonly IOperationsBroadcast _operationsBroadcast;
    private readonly List<PendingRealtimeEvent> _pendingEvents = new();

    public DomainChangeBroadcastInterceptor(IOperationsBroadcast operationsBroadcast)
    {
        _operationsBroadcast = operationsBroadcast;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        CaptureChanges(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        CaptureChanges(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (_pendingEvents.Count == 0)
            return await base.SavedChangesAsync(eventData, result, cancellationToken);

        var snapshot = _pendingEvents
            .DistinctBy(e => $"{e.EventName}:{e.CompanyId}:{e.EntityId}:{e.EntityType}:{e.ChangeKind}")
            .ToList();
        _pendingEvents.Clear();

        foreach (var evt in snapshot)
        {
            var payload = new
            {
                change = evt.ChangeKind,
                fields = evt.ModifiedFields,
                entityType = evt.EntityType
            };

            await _operationsBroadcast.BroadcastAsync(
                evt.EventName,
                evt.CompanyId,
                evt.EntityId,
                evt.EntityType,
                payload,
                cancellationToken);

            // Always emit generic event so any screen can refresh selectively.
            await _operationsBroadcast.BroadcastAsync(
                evt.ChangeKind == "Deleted" ? OperationsEventNames.EntityDeleted : OperationsEventNames.EntityChanged,
                evt.CompanyId,
                evt.EntityId,
                evt.EntityType,
                payload,
                cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private void CaptureChanges(DbContext? context)
    {
        if (context == null) return;

        _pendingEvents.Clear();

        var trackedEntries = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Where(e => e.Entity.CompanyId != Guid.Empty)
            .ToList();

        foreach (var entry in trackedEntries)
        {
            var entityType = entry.Entity.GetType().Name;
            var changeKind = entry.State.ToString();
            var modifiedFields = entry.State == EntityState.Modified
                ? entry.Properties.Where(p => p.IsModified).Select(p => p.Metadata.Name).ToArray()
                : Array.Empty<string>();

            _pendingEvents.Add(new PendingRealtimeEvent
            {
                CompanyId = entry.Entity.CompanyId,
                EntityId = entry.Entity.Id,
                EntityType = entityType,
                ChangeKind = changeKind,
                ModifiedFields = modifiedFields,
                EventName = ResolveEventName(entry)
            });
        }
    }

    private static string ResolveEventName(EntityEntry<BaseEntity> entry)
    {
        var isDeleted = entry.State == EntityState.Deleted;
        return entry.Entity switch
        {
            Product => isDeleted ? OperationsEventNames.ProductDeleted
                : entry.State == EntityState.Added ? OperationsEventNames.ProductCreated
                : OperationsEventNames.ProductUpdated,
            StockTransaction or StockLevel => OperationsEventNames.StockChanged,
            SalesOrder or PurchaseOrder => entry.State == EntityState.Modified ? OperationsEventNames.OrderUpdated : OperationsEventNames.OrderCreated,
            SalesOrderItem or PurchaseOrderItem => OperationsEventNames.OrderUpdated,
            Customer => OperationsEventNames.CustomerUpdated,
            Supplier => OperationsEventNames.SupplierUpdated,
            WorkflowInstance or WorkflowHistory => OperationsEventNames.OrderStatusChanged,
            JournalEntry or JournalLine => OperationsEventNames.FinancePosted,
            Notification => OperationsEventNames.NotificationRaised,
            _ => OperationsEventNames.EntityChanged
        };
    }

    private sealed class PendingRealtimeEvent
    {
        public Guid CompanyId { get; init; }
        public Guid EntityId { get; init; }
        public string EntityType { get; init; } = string.Empty;
        public string ChangeKind { get; init; } = string.Empty;
        public string EventName { get; init; } = string.Empty;
        public string[] ModifiedFields { get; init; } = Array.Empty<string>();
    }
}
