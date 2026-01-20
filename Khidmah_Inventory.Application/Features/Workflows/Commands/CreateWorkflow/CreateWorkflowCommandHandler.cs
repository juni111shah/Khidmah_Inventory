using MediatR;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Workflows.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Workflows.Commands.CreateWorkflow;

public class CreateWorkflowCommandHandler : IRequestHandler<CreateWorkflowCommand, Result<WorkflowDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateWorkflowCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<WorkflowDto>> Handle(CreateWorkflowCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<WorkflowDto>.Failure("Company context is required");

        var workflow = new Workflow(
            companyId.Value,
            request.Name,
            request.EntityType,
            request.WorkflowDefinition,
            _currentUser.UserId);

        workflow.Update(request.Name, request.Description, request.WorkflowDefinition, _currentUser.UserId);

        _context.Workflows.Add(workflow);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new WorkflowDto
        {
            Id = workflow.Id,
            Name = workflow.Name,
            Description = workflow.Description,
            EntityType = workflow.EntityType,
            WorkflowDefinition = workflow.WorkflowDefinition,
            IsActive = workflow.IsActive,
            Version = workflow.Version,
            CreatedAt = workflow.CreatedAt
        };

        return Result<WorkflowDto>.Success(dto);
    }
}

