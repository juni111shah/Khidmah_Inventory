using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.Webhooks.Models;

namespace Khidmah_Inventory.Application.Features.Platform.Webhooks.Commands.CreateWebhook;

public class CreateWebhookCommand : IRequest<Result<WebhookDto>>
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Events { get; set; } = string.Empty;
    public string? Secret { get; set; }
    public string? Description { get; set; }
}
