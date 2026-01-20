using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.Application.Features.Workflows.Commands.CreateWorkflow;
using Khidmah_Inventory.Application.Features.Workflows.Commands.StartWorkflow;
using Khidmah_Inventory.Application.Features.Workflows.Commands.ApproveWorkflowStep;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkflowsController : BaseApiController
{
    [HttpPost]
    [AuthorizePermission("Workflows:Create")]
    public async Task<IActionResult> CreateWorkflow([FromBody] CreateWorkflowCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Workflow created successfully");
    }

    [HttpPost("start")]
    [AuthorizePermission("Workflows:Start")]
    public async Task<IActionResult> StartWorkflow([FromBody] StartWorkflowCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Workflow started successfully");
    }

    [HttpPost("{id}/approve")]
    [AuthorizePermission("Workflows:Approve")]
    public async Task<IActionResult> ApproveWorkflowStep(Guid id, [FromBody] ApproveWorkflowStepCommand command)
    {
        command.WorkflowInstanceId = id;
        var result = await Mediator.Send(command);
        return HandleResult(result, "Workflow step approved successfully");
    }
}

