using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.Application.Features.Auth.Commands.Login;
using Khidmah_Inventory.Application.Features.Auth.Commands.Register;
using Khidmah_Inventory.Application.Features.Auth.Commands.RefreshToken;
using Khidmah_Inventory.Application.Features.Auth.Commands.Logout;
using Khidmah_Inventory.API.Attributes;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseApiController
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Login successful");
    }

    [HttpPost("register")]
    [AuthorizePermission("Auth:Create")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "User registered successfully");
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Token refreshed successfully");
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var command = new LogoutCommand();
        var result = await Mediator.Send(command);
        return HandleResult(result, "Logout successful");
    }
}

