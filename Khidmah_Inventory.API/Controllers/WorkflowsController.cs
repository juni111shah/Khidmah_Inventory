using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.Workflows.Commands.CreateWorkflow;
using Khidmah_Inventory.Application.Features.Workflows.Commands.StartWorkflow;
using Khidmah_Inventory.Application.Features.Workflows.Commands.ApproveWorkflowStep;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Workflows.Base)]
[Authorize]
public class WorkflowsController : BaseController
{
    public WorkflowsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost(ApiRoutes.Workflows.Create)]
    [ValidateApiCode(ApiValidationCodes.WorkflowsModuleCode.Create)]
    [AuthorizeResource(AuthorizePermissions.WorkflowsPermissions.Controller, AuthorizePermissions.WorkflowsPermissions.Actions.Create)]
    public async Task<IActionResult> CreateWorkflow([FromBody] CreateWorkflowCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPost(ApiRoutes.Workflows.Start)]
    [ValidateApiCode(ApiValidationCodes.WorkflowsModuleCode.Start)]
    [AuthorizeResource(AuthorizePermissions.WorkflowsPermissions.Controller, AuthorizePermissions.WorkflowsPermissions.Actions.Start)]
    public async Task<IActionResult> StartWorkflow([FromBody] StartWorkflowCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPost("{id}/approve")]
    [ValidateApiCode(ApiValidationCodes.WorkflowsModuleCode.Approve)]
    [AuthorizeResource(AuthorizePermissions.WorkflowsPermissions.Controller, AuthorizePermissions.WorkflowsPermissions.Actions.Approve)]
    public async Task<IActionResult> ApproveWorkflowStep(Guid id, [FromBody] ApproveWorkflowStepCommand command)
    {
        command.WorkflowInstanceId = id;
        return await ExecuteRequest(command);
    }
}

