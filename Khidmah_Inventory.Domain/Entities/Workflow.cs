using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class Workflow : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string EntityType { get; private set; } = string.Empty; // PurchaseOrder, SalesOrder, etc.
    public string WorkflowDefinition { get; private set; } = string.Empty; // JSON string defining workflow steps
    public bool IsActive { get; private set; } = true;
    public int Version { get; private set; } = 1;

    // Navigation properties
    public virtual ICollection<WorkflowInstance> Instances { get; private set; } = new List<WorkflowInstance>();

    private Workflow() { }

    public Workflow(
        Guid companyId,
        string name,
        string entityType,
        string workflowDefinition,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        Name = name;
        EntityType = entityType;
        WorkflowDefinition = workflowDefinition;
    }

    public void Update(string name, string description, string workflowDefinition, Guid? updatedBy = null)
    {
        Name = name;
        Description = description;
        WorkflowDefinition = workflowDefinition;
        Version++;
        UpdateAuditInfo(updatedBy);
    }

    public void Activate(Guid? updatedBy = null)
    {
        IsActive = true;
        UpdateAuditInfo(updatedBy);
    }

    public void Deactivate(Guid? updatedBy = null)
    {
        IsActive = false;
        UpdateAuditInfo(updatedBy);
    }
}

public class WorkflowInstance : Entity
{
    public Guid WorkflowId { get; private set; }
    public Guid EntityId { get; private set; }
    public string EntityType { get; private set; } = string.Empty;
    public string CurrentStep { get; private set; } = string.Empty;
    public string Status { get; private set; } = "Pending"; // Pending, InProgress, Approved, Rejected, Cancelled
    public Guid? CurrentAssigneeId { get; private set; }
    public string? Comments { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    // Navigation properties
    public virtual Workflow Workflow { get; private set; } = null!;
    public virtual User? CurrentAssignee { get; private set; }
    public virtual ICollection<WorkflowHistory> History { get; private set; } = new List<WorkflowHistory>();

    private WorkflowInstance() { }

    public WorkflowInstance(
        Guid companyId,
        Guid workflowId,
        Guid entityId,
        string entityType,
        string currentStep,
        Guid? currentAssigneeId = null,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        WorkflowId = workflowId;
        EntityId = entityId;
        EntityType = entityType;
        CurrentStep = currentStep;
        CurrentAssigneeId = currentAssigneeId;
    }

    public void MoveToNextStep(string nextStep, Guid? assigneeId = null)
    {
        CurrentStep = nextStep;
        CurrentAssigneeId = assigneeId;
        Status = "InProgress";
        UpdateAuditInfo();
    }

    public void Approve(Guid? approvedBy = null, string? comments = null)
    {
        Status = "Approved";
        Comments = comments;
        CompletedAt = DateTime.UtcNow;
        UpdateAuditInfo(approvedBy);
    }

    public void Reject(Guid? rejectedBy = null, string? comments = null)
    {
        Status = "Rejected";
        Comments = comments;
        CompletedAt = DateTime.UtcNow;
        UpdateAuditInfo(rejectedBy);
    }

    public void Cancel(Guid? cancelledBy = null)
    {
        Status = "Cancelled";
        CompletedAt = DateTime.UtcNow;
        UpdateAuditInfo(cancelledBy);
    }
}

public class WorkflowHistory : Entity
{
    public Guid WorkflowInstanceId { get; private set; }
    public string Step { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty; // Started, Approved, Rejected, Assigned
    public Guid? UserId { get; private set; }
    public string? UserName { get; private set; }
    public string? Comments { get; private set; }

    // Navigation properties
    public virtual WorkflowInstance WorkflowInstance { get; private set; } = null!;
    public virtual User? User { get; private set; }

    private WorkflowHistory() { }

    public WorkflowHistory(
        Guid companyId,
        Guid workflowInstanceId,
        string step,
        string action,
        Guid? userId = null,
        string? userName = null,
        string? comments = null) : base(companyId, userId)
    {
        WorkflowInstanceId = workflowInstanceId;
        Step = step;
        Action = action;
        UserId = userId;
        UserName = userName;
        Comments = comments;
    }
}

