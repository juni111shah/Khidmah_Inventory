using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Workflows.Models;

namespace Khidmah_Inventory.Application.Features.Workflows.Commands.ApproveWorkflowStep;

public class ApproveWorkflowStepCommand : IRequest<Result<WorkflowInstanceDto>>
{
    public Guid WorkflowInstanceId { get; set; }
    public string? Comments { get; set; }
}

