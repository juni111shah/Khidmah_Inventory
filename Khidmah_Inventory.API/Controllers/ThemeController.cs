using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.Application.Features.Theme.Queries.GetUserTheme;
using Khidmah_Inventory.Application.Features.Theme.Queries.GetGlobalTheme;
using Khidmah_Inventory.Application.Features.Theme.Commands.SaveUserTheme;
using Khidmah_Inventory.Application.Features.Theme.Commands.SaveGlobalTheme;
using Khidmah_Inventory.Application.Features.Theme.Commands.UploadLogo;
using Khidmah_Inventory.Application.Features.Theme.Models;
using Khidmah_Inventory.API.Attributes;

namespace Khidmah_Inventory.API.Controllers;

[Authorize]
public class ThemeController : BaseApiController
{
    [HttpGet("user")]
    [AuthorizePermission("Theme:Read")]
    public async Task<IActionResult> GetUserTheme()
    {
        var query = new GetUserThemeQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("global")]
    [AuthorizePermission("Theme:Read")]
    public async Task<IActionResult> GetGlobalTheme()
    {
        var query = new GetGlobalThemeQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost("user")]
    [AuthorizePermission("Theme:Update")]
    public async Task<IActionResult> SaveUserTheme([FromBody] ThemeDto theme)
    {
        var command = new SaveUserThemeCommand { Theme = theme };
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("global")]
    [AuthorizePermission("Theme:Update")]
    public async Task<IActionResult> SaveGlobalTheme([FromBody] ThemeDto theme)
    {
        var command = new SaveGlobalThemeCommand { Theme = theme };
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("logo")]
    [AuthorizePermission("Theme:Update")]
    public async Task<IActionResult> UploadLogo(IFormFile file)
    {
        var command = new UploadLogoCommand { File = file };
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}

