using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.Pos.Commands;
using Khidmah_Inventory.Application.Features.Pos.Queries;
using Khidmah_Inventory.API.Models;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Pos.Base)]
[Authorize]
public class PosController : BaseController
{
    public PosController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost("sessions/open")]
    [ValidateApiCode(ApiValidationCodes.PosModuleCode.StartSession)]
    [AuthorizeResource(AuthorizePermissions.PosPermissions.Controller, AuthorizePermissions.PosPermissions.Actions.StartSession)]
    public async Task<IActionResult> OpenSession([FromBody] OpenPosSessionCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPost("sessions/close")]
    [ValidateApiCode(ApiValidationCodes.PosModuleCode.EndSession)]
    [AuthorizeResource(AuthorizePermissions.PosPermissions.Controller, AuthorizePermissions.PosPermissions.Actions.EndSession)]
    public async Task<IActionResult> CloseSession([FromBody] ClosePosSessionCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpGet("sessions/active")]
    [ValidateApiCode(ApiValidationCodes.PosModuleCode.Session)]
    [AuthorizeResource(AuthorizePermissions.PosPermissions.Controller, AuthorizePermissions.PosPermissions.Actions.Session)]
    public async Task<IActionResult> GetActiveSession()
    {
        var result = await Mediator.Send(new GetActiveSessionQuery());
        if (result.Succeeded && result.Data == null)
        {
            return Ok(ApiResponse<Guid?>.SuccessResponse(null, "No active session found", 200));
        }
        return await ExecuteRequest(new GetActiveSessionQuery());
    }

    [HttpPost("sales")]
    [ValidateApiCode(ApiValidationCodes.PosModuleCode.Session)]
    [AuthorizeResource(AuthorizePermissions.PosPermissions.Controller, AuthorizePermissions.PosPermissions.Actions.Session)]
    public async Task<IActionResult> CreateSale([FromBody] CreatePosSaleCommand command)
    {
        return await ExecuteRequest(command);
    }
}
