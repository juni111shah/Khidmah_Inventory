using Khidmah_Inventory.Domain.Enums;

namespace Khidmah_Inventory.Application.Features.AutonomousWarehouse.Models;

public class WorkTaskDto
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public WorkTaskType Type { get; set; }
    public string TypeDisplay { get; set; } = string.Empty;
    public int Priority { get; set; }
    public WorkTaskStatus Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public Guid? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public OperationAgentType? AssignedToType { get; set; }
    public Guid? MapBinId { get; set; }
    public string? LocationCode { get; set; }
    public Guid? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductSku { get; set; }
    public decimal Quantity { get; set; }
    public Guid? SourceOrderId { get; set; }
    public DateTime? AssignedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
