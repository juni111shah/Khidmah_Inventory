using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.Theme.Queries.GetUserTheme;
using Khidmah_Inventory.Application.Features.Theme.Queries.GetGlobalTheme;
using Khidmah_Inventory.Application.Features.Theme.Commands.SaveUserTheme;
using Khidmah_Inventory.Application.Features.Theme.Commands.SaveGlobalTheme;
using Khidmah_Inventory.Application.Features.Theme.Commands.UploadLogo;
using Khidmah_Inventory.Application.Features.Theme.Models;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Theme.Base)]
[Authorize]
public class ThemeController : BaseController
{
    public ThemeController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet(ApiRoutes.Theme.User)]
    [ValidateApiCode(ApiValidationCodes.ThemeModuleCode.ViewUser)]
    [AuthorizeResource(AuthorizePermissions.ThemePermissions.Controller, AuthorizePermissions.ThemePermissions.Actions.Read)]
    public async Task<IActionResult> GetUserTheme()
    {
        return await ExecuteRequest(new GetUserThemeQuery());
    }

    [HttpGet(ApiRoutes.Theme.Global)]
    [ValidateApiCode(ApiValidationCodes.ThemeModuleCode.ViewGlobal)]
    [AuthorizeResource(AuthorizePermissions.ThemePermissions.Controller, AuthorizePermissions.ThemePermissions.Actions.Read)]
    public async Task<IActionResult> GetGlobalTheme()
    {
        return await ExecuteRequest(new GetGlobalThemeQuery());
    }

    [HttpPost(ApiRoutes.Theme.User)]
    [ValidateApiCode(ApiValidationCodes.ThemeModuleCode.UpdateUser)]
    [AuthorizeResource(AuthorizePermissions.ThemePermissions.Controller, AuthorizePermissions.ThemePermissions.Actions.Update)]
    public async Task<IActionResult> SaveUserTheme([FromBody] ThemeDto theme)
    {
        return await ExecuteRequest(new SaveUserThemeCommand { Theme = theme });
    }

    [HttpPost(ApiRoutes.Theme.Global)]
    [ValidateApiCode(ApiValidationCodes.ThemeModuleCode.UpdateGlobal)]
    [AuthorizeResource(AuthorizePermissions.ThemePermissions.Controller, AuthorizePermissions.ThemePermissions.Actions.Update)]
    public async Task<IActionResult> SaveGlobalTheme([FromBody] ThemeDto theme)
    {
        return await ExecuteRequest(new SaveGlobalThemeCommand { Theme = theme });
    }

    [HttpPost(ApiRoutes.Theme.Logo)]
    [ValidateApiCode(ApiValidationCodes.ThemeModuleCode.UploadLogo)]
    [AuthorizeResource(AuthorizePermissions.ThemePermissions.Controller, AuthorizePermissions.ThemePermissions.Actions.Update)]
    public async Task<IActionResult> UploadLogo(IFormFile file)
    {
        return await ExecuteRequest(new UploadLogoCommand { File = file });
    }
}

