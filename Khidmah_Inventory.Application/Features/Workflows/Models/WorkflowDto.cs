namespace Khidmah_Inventory.Application.Features.Workflows.Models;

public class WorkflowDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string WorkflowDefinition { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class WorkflowInstanceDto
{
    public Guid Id { get; set; }
    public Guid WorkflowId { get; set; }
    public string WorkflowName { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string CurrentStep { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? CurrentAssigneeId { get; set; }
    public string? CurrentAssigneeName { get; set; }
    public string? Comments { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<WorkflowHistoryDto> History { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class WorkflowHistoryDto
{
    public Guid Id { get; set; }
    public string Step { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Comments { get; set; }
    public DateTime CreatedAt { get; set; }
}

