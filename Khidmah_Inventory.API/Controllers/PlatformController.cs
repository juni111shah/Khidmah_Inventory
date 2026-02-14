using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.ApiKeys.Commands.CreateApiKey;
using Khidmah_Inventory.Application.Features.Platform.ApiKeys.Commands.RevokeApiKey;
using Khidmah_Inventory.Application.Features.Platform.ApiKeys.Queries.GetApiKeysList;
using Khidmah_Inventory.Application.Features.Platform.ApiKeys.Queries.GetApiKeyUsage;
using Khidmah_Inventory.Application.Features.Platform.ApiKeys.Models;
using Khidmah_Inventory.Application.Features.Platform.Webhooks.Commands.CreateWebhook;
using Khidmah_Inventory.Application.Features.Platform.Webhooks.Commands.UpdateWebhook;
using Khidmah_Inventory.Application.Features.Platform.Webhooks.Commands.DeleteWebhook;
using Khidmah_Inventory.Application.Features.Platform.Webhooks.Queries.GetWebhooksList;
using Khidmah_Inventory.Application.Features.Platform.Webhooks.Queries.GetWebhookDeliveryLogs;
using Khidmah_Inventory.Application.Features.Platform.Integrations.Queries.GetIntegrationsList;
using Khidmah_Inventory.Application.Features.Platform.Integrations.Commands.ToggleIntegration;
using Khidmah_Inventory.Application.Features.Platform.ScheduledReports.Commands.CreateScheduledReport;
using Khidmah_Inventory.Application.Features.Platform.ScheduledReports.Commands.UpdateScheduledReport;
using Khidmah_Inventory.Application.Features.Platform.ScheduledReports.Commands.DeleteScheduledReport;
using Khidmah_Inventory.Application.Features.Platform.ScheduledReports.Queries.GetScheduledReportsList;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Platform.Base)]
[Authorize]
public class PlatformController : BaseController
{
    public PlatformController(IMediator mediator) : base(mediator)
    {
    }

    // ---------- API Keys ----------
    [HttpPost(ApiRoutes.Platform.ApiKeysList)]
    [ValidateApiCode(ApiValidationCodes.PlatformModuleCode.ApiKeysList)]
    [AuthorizeResource(AuthorizePermissions.PlatformPermissions.Controller, AuthorizePermissions.PlatformPermissions.Actions.ApiKeysList)]
    public async Task<IActionResult> GetApiKeys([FromBody] GetApiKeysListQuery query)
    {
        return await ExecuteRequest(query);
    }

    [HttpPost("api-keys")]
    [ValidateApiCode(ApiValidationCodes.PlatformModuleCode.ApiKeysCreate)]
    [AuthorizeResource(AuthorizePermissions.PlatformPermissions.Controller, AuthorizePermissions.PlatformPermissions.Actions.ApiKeysCreate)]
    public async Task<IActionResult> CreateApiKey([FromBody] CreateApiKeyCommand command)
    {
        return await ExecuteRequest<CreateApiKeyCommand, CreateApiKeyResult>(command);
    }

    [HttpPatch(ApiRoutes.Platform.ApiKeyRevoke)]
    [ValidateApiCode(ApiValidationCodes.PlatformModuleCode.ApiKeysRevoke)]
    [AuthorizeResource(AuthorizePermissions.PlatformPermissions.Controller, AuthorizePermissions.PlatformPermissions.Actions.ApiKeysRevoke)]
    public async Task<IActionResult> RevokeApiKey(Guid id)
    {
        return await ExecuteRequest(new RevokeApiKeyCommand { Id = id });
    }

    [HttpPost(ApiRoutes.Platform.ApiKeyUsage)]
    [ValidateApiCode(ApiValidationCodes.PlatformModuleCode.ApiKeysUsage)]
    [AuthorizeResource(AuthorizePermissions.PlatformPermissions.Controller, AuthorizePermissions.PlatformPermissions.Actions.ApiKeysUsage)]
    public async Task<IActionResult> GetApiKeyUsage([FromBody] GetApiKeyUsageQuery query)
    {
        return await ExecuteRequest<GetApiKeyUsageQuery, ApiKeyUsageDto>(query);
    }

    // ---------- Webhooks ----------
    [HttpPost(ApiRoutes.Platform.WebhooksList)]
    [ValidateApiCode(ApiValidationCodes.PlatformModuleCode.WebhooksList)]
    [AuthorizeResource(AuthorizePermissions.PlatformPermissions.Controller, AuthorizePermissions.PlatformPermissions.Actions.WebhooksList)]
    public async Task<IActionResult> GetWebhooks([FromBody] GetWebhooksListQuery query)
    {
        return await ExecuteRequest(query);
    }

    [HttpPost(ApiRoutes.Platform.Webhooks)]
    [ValidateApiCode(ApiValidationCodes.PlatformModuleCode.WebhooksCreate)]
    [AuthorizeResource(AuthorizePermissions.PlatformPermissions.Controller, AuthorizePermissions.PlatformPermissions.Actions.WebhooksCreate)]
    public async Task<IActionResult> CreateWebhook([FromBody] CreateWebhookCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPut(ApiRoutes.Platform.WebhookById)]
    [ValidateApiCode(ApiValidationCodes.PlatformModuleCode.WebhooksUpdate)]
    [AuthorizeResource(AuthorizePermissions.PlatformPermissions.Controller, AuthorizePermissions.PlatformPermissions.Actions.WebhooksUpdate)]
    public async Task<IActionResult> UpdateWebhook(Guid id, [FromBody] UpdateWebhookCommand command)
    {
        command.Id = id;
        return await ExecuteRequest(command);
    }

    [HttpDelete(ApiRoutes.Platform.WebhookById)]
    [ValidateApiCode(ApiValidationCodes.PlatformModuleCode.WebhooksDelete)]
    [AuthorizeResource(AuthorizePermissions.PlatformPermissions.Controller, AuthorizePermissions.PlatformPermissions.Actions.WebhooksDelete)]
    public async Task<IActionResult> DeleteWebhook(Guid id)
    {
        return await ExecuteRequest(new DeleteWebhookCommand { Id = id });
    }

    [HttpPost(ApiRoutes.Platform.WebhookLogs)]
    [ValidateApiCode(ApiValidationCodes.PlatformModuleCode.WebhooksLogs)]
    [AuthorizeResource(AuthorizePermissions.PlatformPermissions.Controller, AuthorizePermissions.PlatformPermissions.Actions.WebhooksLogs)]
    public async Task<IActionResult> GetWebhookLogs(Guid id, [FromBody] GetWebhookDeliveryLogsQuery query)
    {
        query.WebhookId = id;
        return await ExecuteRequest(query);
    }

    // ---------- Integrations ----------
    [HttpPost(ApiRoutes.Platform.IntegrationsList)]
    [ValidateApiCode(ApiValidationCodes.PlatformModuleCode.IntegrationsList)]
    [AuthorizeResource(AuthorizePermissions.PlatformPermissions.Controller, AuthorizePermissions.PlatformPermissions.Actions.IntegrationsList)]
    public async Task<IActionResult> GetIntegrations()
    {
        return await ExecuteRequest(new GetIntegrationsListQuery());
    }

    [HttpPatch(ApiRoutes.Platform.IntegrationToggle)]
    [ValidateApiCode(ApiValidationCodes.PlatformModuleCode.IntegrationsToggle)]
    [AuthorizeResource(AuthorizePermissions.PlatformPermissions.Controller, AuthorizePermissions.PlatformPermissions.Actions.IntegrationsToggle)]
    public async Task<IActionResult> ToggleIntegration(string type, [FromBody] ToggleIntegrationCommand command)
    {
        command.IntegrationType = type;
        return await ExecuteRequest(command);
    }

    // ---------- Scheduled Reports ----------
    [HttpPost(ApiRoutes.Platform.ScheduledReportsList)]
    [ValidateApiCode(ApiValidationCodes.PlatformModuleCode.ScheduledReportsList)]
    [AuthorizeResource(AuthorizePermissions.PlatformPermissions.Controller, AuthorizePermissions.PlatformPermissions.Actions.ScheduledReportsList)]
    public async Task<IActionResult> GetScheduledReports([FromBody] GetScheduledReportsListQuery query)
    {
        return await ExecuteRequest(query);
    }

    [HttpPost(ApiRoutes.Platform.ScheduledReports)]
    [ValidateApiCode(ApiValidationCodes.PlatformModuleCode.ScheduledReportsCreate)]
    [AuthorizeResource(AuthorizePermissions.PlatformPermissions.Controller, AuthorizePermissions.PlatformPermissions.Actions.ScheduledReportsCreate)]
    public async Task<IActionResult> CreateScheduledReport([FromBody] CreateScheduledReportCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPut(ApiRoutes.Platform.ScheduledReportById)]
    [ValidateApiCode(ApiValidationCodes.PlatformModuleCode.ScheduledReportsUpdate)]
    [AuthorizeResource(AuthorizePermissions.PlatformPermissions.Controller, AuthorizePermissions.PlatformPermissions.Actions.ScheduledReportsUpdate)]
    public async Task<IActionResult> UpdateScheduledReport(Guid id, [FromBody] UpdateScheduledReportCommand command)
    {
        command.Id = id;
        return await ExecuteRequest(command);
    }

    [HttpDelete(ApiRoutes.Platform.ScheduledReportById)]
    [ValidateApiCode(ApiValidationCodes.PlatformModuleCode.ScheduledReportsDelete)]
    [AuthorizeResource(AuthorizePermissions.PlatformPermissions.Controller, AuthorizePermissions.PlatformPermissions.Actions.ScheduledReportsDelete)]
    public async Task<IActionResult> DeleteScheduledReport(Guid id)
    {
        return await ExecuteRequest(new DeleteScheduledReportCommand { Id = id });
    }
}
