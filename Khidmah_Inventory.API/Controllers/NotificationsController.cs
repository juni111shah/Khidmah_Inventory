using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Notifications.Commands.MarkAllNotificationsRead;
using Khidmah_Inventory.Application.Features.Notifications.Commands.MarkNotificationRead;
using Khidmah_Inventory.Application.Features.Notifications.Queries.GetNotifications;
using Khidmah_Inventory.Application.Features.Notifications.Queries.GetUnreadCount;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Notifications.Base)]
[Authorize]
public class NotificationsController : BaseController
{
    public NotificationsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost(ApiRoutes.Notifications.List)]
    [ValidateApiCode(ApiValidationCodes.SettingsModuleCode.NotificationRead)]
    [AuthorizeResource(AuthorizePermissions.SettingsPermissions.Controller, AuthorizePermissions.SettingsPermissions.Actions.NotificationRead)]
    public async Task<IActionResult> GetList([FromBody] GetNotificationsRequest request)
    {
        var query = new GetNotificationsQuery
        {
            FilterRequest = request?.FilterRequest,
            UnreadOnly = request?.UnreadOnly
        };
        return await ExecuteRequest(query);
    }

    [HttpGet(ApiRoutes.Notifications.UnreadCount)]
    [ValidateApiCode(ApiValidationCodes.SettingsModuleCode.NotificationRead)]
    [AuthorizeResource(AuthorizePermissions.SettingsPermissions.Controller, AuthorizePermissions.SettingsPermissions.Actions.NotificationRead)]
    public async Task<IActionResult> GetUnreadCount()
    {
        return await ExecuteRequest(new GetUnreadNotificationsCountQuery());
    }

    [HttpPost(ApiRoutes.Notifications.MarkRead)]
    [ValidateApiCode(ApiValidationCodes.SettingsModuleCode.NotificationUpdate)]
    [AuthorizeResource(AuthorizePermissions.SettingsPermissions.Controller, AuthorizePermissions.SettingsPermissions.Actions.NotificationUpdate)]
    public async Task<IActionResult> MarkRead(Guid id)
    {
        return await ExecuteRequest(new MarkNotificationReadCommand { Id = id });
    }

    [HttpPost(ApiRoutes.Notifications.MarkAllRead)]
    [ValidateApiCode(ApiValidationCodes.SettingsModuleCode.NotificationUpdate)]
    [AuthorizeResource(AuthorizePermissions.SettingsPermissions.Controller, AuthorizePermissions.SettingsPermissions.Actions.NotificationUpdate)]
    public async Task<IActionResult> MarkAllRead()
    {
        return await ExecuteRequest(new MarkAllNotificationsReadCommand());
    }
}

public class GetNotificationsRequest
{
    public FilterRequest? FilterRequest { get; set; }
    public bool? UnreadOnly { get; set; }
}
