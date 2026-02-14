using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Khidmah_Inventory.Application.Common.Constants;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Workflows.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Workflows.Commands.ApproveWorkflowStep;

public class ApproveWorkflowStepCommandHandler : IRequestHandler<ApproveWorkflowStepCommand, Result<WorkflowInstanceDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IOperationsBroadcast? _broadcast;
    private readonly IWebhookDispatchService? _webhookDispatch;
    private readonly IAutomationExecutor? _automationExecutor;

    public ApproveWorkflowStepCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IOperationsBroadcast? broadcast = null,
        IWebhookDispatchService? webhookDispatch = null,
        IAutomationExecutor? automationExecutor = null)
    {
        _context = context;
        _currentUser = currentUser;
        _broadcast = broadcast;
        _webhookDispatch = webhookDispatch;
        _automationExecutor = automationExecutor;
    }

    public async Task<Result<WorkflowInstanceDto>> Handle(ApproveWorkflowStepCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<WorkflowInstanceDto>.Failure("Company context is required");

        var instance = await _context.WorkflowInstances
            .Include(wi => wi.Workflow)
            .FirstOrDefaultAsync(wi => wi.Id == request.WorkflowInstanceId && 
                wi.CompanyId == companyId.Value && 
                !wi.IsDeleted, cancellationToken);

        if (instance == null)
            return Result<WorkflowInstanceDto>.Failure("Workflow instance not found.");

        if (instance.Status != "InProgress" && instance.Status != "Pending")
            return Result<WorkflowInstanceDto>.Failure("Workflow instance is not in a valid state for approval.");

        // Check if current user is the assignee
        if (instance.CurrentAssigneeId != _currentUser.UserId)
            return Result<WorkflowInstanceDto>.Failure("You are not assigned to this workflow step.");

        // Parse workflow definition to get next step
        string? nextStep = null;
        try
        {
            var definition = JsonSerializer.Deserialize<WorkflowDefinition>(instance.Workflow.WorkflowDefinition);
            if (definition?.Steps != null)
            {
                var currentStepIndex = definition.Steps.FindIndex(s => s.Name == instance.CurrentStep);
                if (currentStepIndex >= 0 && currentStepIndex < definition.Steps.Count - 1)
                {
                    nextStep = definition.Steps[currentStepIndex + 1].Name;
                }
            }
        }
        catch
        {
            // Continue without next step
        }

        // Create history entry
        var history = new WorkflowHistory(
            companyId.Value,
            instance.Id,
            instance.CurrentStep,
            "Approved",
            _currentUser.UserId,
            null,
            request.Comments);

        instance.History.Add(history);

        if (nextStep != null)
        {
            instance.MoveToNextStep(nextStep);
        }
        else
        {
            instance.Approve(_currentUser.UserId, request.Comments);
        }

        await _context.SaveChangesAsync(cancellationToken);

        if (_broadcast != null)
        {
            await _broadcast.BroadcastAsync(
                OperationsEventNames.OrderApproved,
                companyId.Value,
                instance.EntityId,
                instance.EntityType,
                new { WorkflowInstanceId = instance.Id, Status = instance.Status, CurrentStep = instance.CurrentStep },
                cancellationToken);
        }

        if (instance.Status == "Approved" && _webhookDispatch != null)
        {
            var payload = new { workflowInstanceId = instance.Id, entityId = instance.EntityId, entityType = instance.EntityType, completedAt = instance.CompletedAt };
            await _webhookDispatch.DispatchAsync(companyId.Value, "ApprovalDone", payload, cancellationToken);
        }

        if (instance.Status == "Approved" &&
            _automationExecutor != null &&
            string.Equals(instance.EntityType, "PurchaseOrder", StringComparison.OrdinalIgnoreCase))
        {
            await _automationExecutor.ExecutePOApprovedAsync(companyId.Value, instance.EntityId, cancellationToken);
        }

        var dto = new WorkflowInstanceDto
        {
            Id = instance.Id,
            WorkflowId = instance.WorkflowId,
            WorkflowName = instance.Workflow.Name,
            EntityId = instance.EntityId,
            EntityType = instance.EntityType,
            CurrentStep = instance.CurrentStep,
            Status = instance.Status,
            CurrentAssigneeId = instance.CurrentAssigneeId,
            Comments = instance.Comments,
            CompletedAt = instance.CompletedAt,
            History = instance.History.Select(h => new WorkflowHistoryDto
            {
                Id = h.Id,
                Step = h.Step,
                Action = h.Action,
                UserId = h.UserId,
                UserName = h.UserName,
                Comments = h.Comments,
                CreatedAt = h.CreatedAt
            }).ToList(),
            CreatedAt = instance.CreatedAt
        };

        return Result<WorkflowInstanceDto>.Success(dto);
    }

    private class WorkflowDefinition
    {
        public List<WorkflowStep>? Steps { get; set; }
    }

    private class WorkflowStep
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}

