using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.Webhooks.Models;

namespace Khidmah_Inventory.Application.Features.Platform.Webhooks.Commands.UpdateWebhook;

public class UpdateWebhookCommand : IRequest<Result<WebhookDto>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Events { get; set; } = string.Empty;
    public string? Secret { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
