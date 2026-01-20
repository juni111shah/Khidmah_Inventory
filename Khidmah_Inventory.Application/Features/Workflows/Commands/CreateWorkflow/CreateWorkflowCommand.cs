using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Workflows.Models;

namespace Khidmah_Inventory.Application.Features.Workflows.Commands.CreateWorkflow;

public class CreateWorkflowCommand : IRequest<Result<WorkflowDto>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string WorkflowDefinition { get; set; } = string.Empty; // JSON string
}

