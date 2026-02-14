using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.Webhooks.Models;

namespace Khidmah_Inventory.Application.Features.Platform.Webhooks.Queries.GetWebhookDeliveryLogs;

public class GetWebhookDeliveryLogsQuery : IRequest<Result<PagedResult<WebhookDeliveryLogDto>>>
{
    public Guid WebhookId { get; set; }
    public int PageNo { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
