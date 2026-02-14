namespace Khidmah_Inventory.Application.Features.HandsFree.Models;

public class HandsFreeTaskDto
{
    public Guid TaskId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public decimal Quantity { get; set; }
    public int Sequence { get; set; }
}

public class HandsFreeSessionDto
{
    public Guid SessionId { get; set; }
    public DateTime StartedAt { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
}

public class HandsFreeTasksResult
{
    public HandsFreeSessionDto Session { get; set; } = null!;
    public List<HandsFreeTaskDto> Tasks { get; set; } = new();
    public int CurrentIndex { get; set; }
}

public class HandsFreeSupervisorSessionDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid SessionId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int CurrentTaskIndex { get; set; }
    public int Errors { get; set; }
}
