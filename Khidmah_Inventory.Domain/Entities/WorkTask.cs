using Khidmah_Inventory.Domain.Common;
using Khidmah_Inventory.Domain.Enums;

namespace Khidmah_Inventory.Domain.Entities;

/// <summary>
/// A unit of work in the autonomous warehouse: pick, putaway, count, or transfer.
/// </summary>
public class WorkTask : Entity
{
    public Guid WarehouseId { get; private set; }
    public WorkTaskType Type { get; private set; }
    public int Priority { get; private set; } // higher = more urgent
    public WorkTaskStatus Status { get; private set; }
    /// <summary>User id when AssignedToType is Human; robot id when Robot (future).</summary>
    public Guid? AssignedToId { get; private set; }
    public OperationAgentType? AssignedToType { get; private set; }
    /// <summary>Map bin id for spatial location; null if using LocationCode only.</summary>
    public Guid? MapBinId { get; private set; }
    public string? LocationCode { get; private set; }
    public Guid? ProductId { get; private set; }
    public string? ProductName { get; private set; }
    public string? ProductSku { get; private set; }
    public decimal Quantity { get; private set; }
    public Guid? SourceOrderId { get; private set; } // SalesOrder or PurchaseOrder
    public Guid? SourceOrderLineId { get; private set; }
    public DateTime? AssignedAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? Notes { get; private set; }

    public virtual Warehouse Warehouse { get; private set; } = null!;
    public virtual MapBin? MapBin { get; private set; }
    public virtual Product? Product { get; private set; }

    private WorkTask() { }

    public WorkTask(
        Guid companyId,
        Guid warehouseId,
        WorkTaskType type,
        int priority,
        string? locationCode,
        Guid? mapBinId,
        Guid? productId,
        string? productName,
        string? productSku,
        decimal quantity,
        Guid? sourceOrderId,
        Guid? sourceOrderLineId,
        Guid? createdBy = null)
        : base(companyId, createdBy)
    {
        WarehouseId = warehouseId;
        Type = type;
        Priority = priority;
        Status = WorkTaskStatus.Pending;
        LocationCode = locationCode;
        MapBinId = mapBinId;
        ProductId = productId;
        ProductName = productName;
        ProductSku = productSku;
        Quantity = quantity;
        SourceOrderId = sourceOrderId;
        SourceOrderLineId = sourceOrderLineId;
    }

    public void AssignTo(Guid? assignedToId, OperationAgentType agentType)
    {
        AssignedToId = assignedToId;
        AssignedToType = agentType;
        Status = WorkTaskStatus.Assigned;
        AssignedAt = DateTime.UtcNow;
        UpdateAuditInfo(null);
    }

    public void Start()
    {
        Status = WorkTaskStatus.InProgress;
        StartedAt = DateTime.UtcNow;
        UpdateAuditInfo(null);
    }

    public void Complete(string? notes = null)
    {
        Status = WorkTaskStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        Notes = notes;
        UpdateAuditInfo(null);
    }

    public void Cancel(string? notes = null)
    {
        Status = WorkTaskStatus.Cancelled;
        Notes = notes;
        UpdateAuditInfo(null);
    }

    public void MarkDelayed()
    {
        Status = WorkTaskStatus.Delayed;
        UpdateAuditInfo(null);
    }

    public void Unassign()
    {
        AssignedToId = null;
        AssignedToType = null;
        Status = WorkTaskStatus.Pending;
        AssignedAt = null;
        StartedAt = null;
        UpdateAuditInfo(null);
    }

    public void SetPriority(int priority)
    {
        Priority = priority;
        UpdateAuditInfo(null);
    }
}
