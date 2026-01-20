using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Workflows.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Workflows.Commands.StartWorkflow;

public class StartWorkflowCommandHandler : IRequestHandler<StartWorkflowCommand, Result<WorkflowInstanceDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public StartWorkflowCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<WorkflowInstanceDto>> Handle(StartWorkflowCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<WorkflowInstanceDto>.Failure("Company context is required");

        var workflow = await _context.Workflows
            .FirstOrDefaultAsync(w => w.Id == request.WorkflowId && 
                w.CompanyId == companyId.Value && 
                !w.IsDeleted && 
                w.IsActive, cancellationToken);

        if (workflow == null)
            return Result<WorkflowInstanceDto>.Failure("Workflow not found or inactive.");

        // Parse workflow definition to get first step
        string firstStep = "Step1";
        try
        {
            var definition = JsonSerializer.Deserialize<WorkflowDefinition>(workflow.WorkflowDefinition);
            if (definition?.Steps != null && definition.Steps.Any())
                firstStep = definition.Steps.First().Name;
        }
        catch
        {
            // Use default first step
        }

        var instance = new WorkflowInstance(
            companyId.Value,
            request.WorkflowId,
            request.EntityId,
            request.EntityType,
            firstStep,
            request.InitialAssigneeId ?? _currentUser.UserId,
            _currentUser.UserId);

        // Create history entry
        var history = new WorkflowHistory(
            companyId.Value,
            instance.Id,
            firstStep,
            "Started",
            _currentUser.UserId,
            null,
            "Workflow started");

        instance.History.Add(history);

        _context.WorkflowInstances.Add(instance);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new WorkflowInstanceDto
        {
            Id = instance.Id,
            WorkflowId = instance.WorkflowId,
            WorkflowName = workflow.Name,
            EntityId = instance.EntityId,
            EntityType = instance.EntityType,
            CurrentStep = instance.CurrentStep,
            Status = instance.Status,
            CurrentAssigneeId = instance.CurrentAssigneeId,
            Comments = instance.Comments,
            CompletedAt = instance.CompletedAt,
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

