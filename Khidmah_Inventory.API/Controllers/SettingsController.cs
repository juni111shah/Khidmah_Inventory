using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.Settings.Queries.GetCompanySettings;
using Khidmah_Inventory.Application.Features.Settings.Commands.SaveCompanySettings;
using Khidmah_Inventory.Application.Features.Settings.Queries.GetUserSettings;
using Khidmah_Inventory.Application.Features.Settings.Commands.SaveUserSettings;
using Khidmah_Inventory.Application.Features.Settings.Queries.GetSystemSettings;
using Khidmah_Inventory.Application.Features.Settings.Commands.SaveSystemSettings;
using Khidmah_Inventory.Application.Features.Settings.Queries.GetNotificationSettings;
using Khidmah_Inventory.Application.Features.Settings.Commands.SaveNotificationSettings;
using Khidmah_Inventory.Application.Features.Settings.Queries.GetUISettings;
using Khidmah_Inventory.Application.Features.Settings.Commands.SaveUISettings;
using Khidmah_Inventory.Application.Features.Settings.Queries.GetReportSettings;
using Khidmah_Inventory.Application.Features.Settings.Commands.SaveReportSettings;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Settings.Base)]
[Authorize]
public class SettingsController : BaseController
{
    public SettingsController(IMediator mediator) : base(mediator)
    {
    }
    [HttpGet(ApiRoutes.Settings.Company)]
    [ValidateApiCode(ApiValidationCodes.SettingsModuleCode.CompanyRead)]
    [AuthorizeResource(AuthorizePermissions.SettingsPermissions.Controller, AuthorizePermissions.SettingsPermissions.Actions.CompanyRead)]
    public async Task<IActionResult> GetCompanySettings()
    {
        return await ExecuteRequest(new GetCompanySettingsQuery());
    }

    [HttpPost(ApiRoutes.Settings.Company)]
    [ValidateApiCode(ApiValidationCodes.SettingsModuleCode.CompanyUpdate)]
    [AuthorizeResource(AuthorizePermissions.SettingsPermissions.Controller, AuthorizePermissions.SettingsPermissions.Actions.CompanyUpdate)]
    public async Task<IActionResult> SaveCompanySettings([FromBody] SaveCompanySettingsCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpGet(ApiRoutes.Settings.User)]
    [ValidateApiCode(ApiValidationCodes.SettingsModuleCode.UserRead)]
    [AuthorizeResource(AuthorizePermissions.SettingsPermissions.Controller, AuthorizePermissions.SettingsPermissions.Actions.UserRead)]
    public async Task<IActionResult> GetUserSettings()
    {
        return await ExecuteRequest(new GetUserSettingsQuery());
    }

    [HttpPost(ApiRoutes.Settings.User)]
    [ValidateApiCode(ApiValidationCodes.SettingsModuleCode.UserUpdate)]
    [AuthorizeResource(AuthorizePermissions.SettingsPermissions.Controller, AuthorizePermissions.SettingsPermissions.Actions.UserUpdate)]
    public async Task<IActionResult> SaveUserSettings([FromBody] SaveUserSettingsCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpGet(ApiRoutes.Settings.System)]
    [ValidateApiCode(ApiValidationCodes.SettingsModuleCode.SystemRead)]
    [AuthorizeResource(AuthorizePermissions.SettingsPermissions.Controller, AuthorizePermissions.SettingsPermissions.Actions.SystemRead)]
    public async Task<IActionResult> GetSystemSettings()
    {
        return await ExecuteRequest(new GetSystemSettingsQuery());
    }

    [HttpPost(ApiRoutes.Settings.System)]
    [ValidateApiCode(ApiValidationCodes.SettingsModuleCode.SystemUpdate)]
    [AuthorizeResource(AuthorizePermissions.SettingsPermissions.Controller, AuthorizePermissions.SettingsPermissions.Actions.SystemUpdate)]
    public async Task<IActionResult> SaveSystemSettings([FromBody] SaveSystemSettingsCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpGet(ApiRoutes.Settings.Notification)]
    [ValidateApiCode(ApiValidationCodes.SettingsModuleCode.NotificationRead)]
    [AuthorizeResource(AuthorizePermissions.SettingsPermissions.Controller, AuthorizePermissions.SettingsPermissions.Actions.NotificationRead)]
    public async Task<IActionResult> GetNotificationSettings()
    {
        return await ExecuteRequest(new GetNotificationSettingsQuery());
    }

    [HttpPost(ApiRoutes.Settings.Notification)]
    [ValidateApiCode(ApiValidationCodes.SettingsModuleCode.NotificationUpdate)]
    [AuthorizeResource(AuthorizePermissions.SettingsPermissions.Controller, AuthorizePermissions.SettingsPermissions.Actions.NotificationUpdate)]
    public async Task<IActionResult> SaveNotificationSettings([FromBody] SaveNotificationSettingsCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpGet(ApiRoutes.Settings.UI)]
    [ValidateApiCode(ApiValidationCodes.SettingsModuleCode.UIRead)]
    [AuthorizeResource(AuthorizePermissions.SettingsPermissions.Controller, AuthorizePermissions.SettingsPermissions.Actions.UIRead)]
    public async Task<IActionResult> GetUISettings()
    {
        return await ExecuteRequest<GetUISettingsQuery, UISettingsDto>(new GetUISettingsQuery());
    }

    [HttpPost(ApiRoutes.Settings.UI)]
    [ValidateApiCode(ApiValidationCodes.SettingsModuleCode.UIUpdate)]
    [AuthorizeResource(AuthorizePermissions.SettingsPermissions.Controller, AuthorizePermissions.SettingsPermissions.Actions.UIUpdate)]
    public async Task<IActionResult> SaveUISettings([FromBody] SaveUISettingsCommand command)
    {
        return await ExecuteRequest<SaveUISettingsCommand, UISettingsDto>(command);
    }

    [HttpGet(ApiRoutes.Settings.Report)]
    [ValidateApiCode(ApiValidationCodes.SettingsModuleCode.ReportRead)]
    [AuthorizeResource(AuthorizePermissions.SettingsPermissions.Controller, AuthorizePermissions.SettingsPermissions.Actions.ReportRead)]
    public async Task<IActionResult> GetReportSettings()
    {
        return await ExecuteRequest(new GetReportSettingsQuery());
    }

    [HttpPost(ApiRoutes.Settings.Report)]
    [ValidateApiCode(ApiValidationCodes.SettingsModuleCode.ReportUpdate)]
    [AuthorizeResource(AuthorizePermissions.SettingsPermissions.Controller, AuthorizePermissions.SettingsPermissions.Actions.ReportUpdate)]
    public async Task<IActionResult> SaveReportSettings([FromBody] SaveReportSettingsCommand command)
    {
        return await ExecuteRequest(command);
    }
}

