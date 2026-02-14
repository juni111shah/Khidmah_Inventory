using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.Webhooks.Models;

namespace Khidmah_Inventory.Application.Features.Platform.Webhooks.Queries.GetWebhooksList;

public class GetWebhooksListQuery : IRequest<Result<List<WebhookDto>>>
{
    public bool? IsActive { get; set; }
}
