using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Workflows.Models;

namespace Khidmah_Inventory.Application.Features.Workflows.Commands.StartWorkflow;

public class StartWorkflowCommand : IRequest<Result<WorkflowInstanceDto>>
{
    public Guid WorkflowId { get; set; }
    public Guid EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid? InitialAssigneeId { get; set; }
}

