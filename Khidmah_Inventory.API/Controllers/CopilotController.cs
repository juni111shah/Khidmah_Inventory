using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.Copilot.Commands.ExecuteCopilot;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Copilot.Base)]
[Authorize]
public class CopilotController : BaseController
{
    public CopilotController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost(ApiRoutes.Copilot.Execute)]
    [ValidateApiCode(ApiValidationCodes.CopilotModuleCode.Execute)]
    [AuthorizeResource(AuthorizePermissions.CopilotPermissions.Controller, AuthorizePermissions.CopilotPermissions.Actions.Execute)]
    public async Task<IActionResult> Execute([FromBody] ExecuteCopilotCommand command)
    {
        return await ExecuteRequest(command);
    }
}
