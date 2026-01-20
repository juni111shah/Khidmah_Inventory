using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
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

namespace Khidmah_Inventory.API.Controllers;

[Authorize]
public class SettingsController : BaseApiController
{
    // Company Settings
    [HttpGet("company")]
    [AuthorizePermission("Settings:Company:Read")]
    public async Task<IActionResult> GetCompanySettings()
    {
        var query = new GetCompanySettingsQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result, "Company settings retrieved successfully");
    }

    [HttpPost("company")]
    [AuthorizePermission("Settings:Company:Update")]
    public async Task<IActionResult> SaveCompanySettings([FromBody] SaveCompanySettingsCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Company settings saved successfully");
    }

    // User Settings
    [HttpGet("user")]
    [AuthorizePermission("Settings:User:Read")]
    public async Task<IActionResult> GetUserSettings()
    {
        var query = new GetUserSettingsQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result, "User settings retrieved successfully");
    }

    [HttpPost("user")]
    [AuthorizePermission("Settings:User:Update")]
    public async Task<IActionResult> SaveUserSettings([FromBody] SaveUserSettingsCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "User settings saved successfully");
    }

    // System Settings
    [HttpGet("system")]
    [AuthorizePermission("Settings:System:Read")]
    public async Task<IActionResult> GetSystemSettings()
    {
        var query = new GetSystemSettingsQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result, "System settings retrieved successfully");
    }

    [HttpPost("system")]
    [AuthorizePermission("Settings:System:Update")]
    public async Task<IActionResult> SaveSystemSettings([FromBody] SaveSystemSettingsCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "System settings saved successfully");
    }

    // Notification Settings
    [HttpGet("notifications")]
    [AuthorizePermission("Settings:Notification:Read")]
    public async Task<IActionResult> GetNotificationSettings()
    {
        var query = new GetNotificationSettingsQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result, "Notification settings retrieved successfully");
    }

    [HttpPost("notifications")]
    [AuthorizePermission("Settings:Notification:Update")]
    public async Task<IActionResult> SaveNotificationSettings([FromBody] SaveNotificationSettingsCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Notification settings saved successfully");
    }

    // UI Settings
    [HttpGet("ui")]
    [AuthorizePermission("Settings:UI:Read")]
    public async Task<IActionResult> GetUISettings()
    {
        var query = new GetUISettingsQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result, "UI settings retrieved successfully");
    }

    [HttpPost("ui")]
    [AuthorizePermission("Settings:UI:Update")]
    public async Task<IActionResult> SaveUISettings([FromBody] SaveUISettingsCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "UI settings saved successfully");
    }

    // Report Settings
    [HttpGet("reports")]
    [AuthorizePermission("Settings:Report:Read")]
    public async Task<IActionResult> GetReportSettings()
    {
        var query = new GetReportSettingsQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result, "Report settings retrieved successfully");
    }

    [HttpPost("reports")]
    [AuthorizePermission("Settings:Report:Update")]
    public async Task<IActionResult> SaveReportSettings([FromBody] SaveReportSettingsCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Report settings saved successfully");
    }
}

