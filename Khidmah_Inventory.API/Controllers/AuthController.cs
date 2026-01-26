using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.Auth.Commands.Login;
using Khidmah_Inventory.Application.Features.Auth.Commands.Register;
using Khidmah_Inventory.Application.Features.Auth.Commands.RefreshToken;
using Khidmah_Inventory.Application.Features.Auth.Commands.Logout;
using LoginResponseDto = Khidmah_Inventory.Application.Features.Auth.Commands.Login.LoginResponseDto;
using RegisterResponseDto = Khidmah_Inventory.Application.Features.Auth.Commands.Register.RegisterResponseDto;
using RefreshTokenResponseDto = Khidmah_Inventory.Application.Features.Auth.Commands.RefreshToken.RefreshTokenResponseDto;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Auth.Base)]
public class AuthController : BaseController
{
    public AuthController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost(ApiRoutes.Auth.Login)]
    [ValidateApiCode(ApiValidationCodes.AuthModuleCode.Login)]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        return await ExecuteRequest<LoginCommand, LoginResponseDto>(command);
    }

    [HttpPost(ApiRoutes.Auth.Register)]
    [ValidateApiCode(ApiValidationCodes.AuthModuleCode.Register)]
    [AuthorizeResource(AuthorizePermissions.AuthPermissions.Controller, AuthorizePermissions.AuthPermissions.Actions.Create)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        return await ExecuteRequest<RegisterCommand, RegisterResponseDto>(command);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        return await ExecuteRequest<RefreshTokenCommand, RefreshTokenResponseDto>(command);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        return await ExecuteRequest(new LogoutCommand());
    }
}

