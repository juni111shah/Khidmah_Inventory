using System.Threading.Channels;
using Khidmah_Inventory.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Khidmah_Inventory.API.Services;

public class WebhookDispatchService : IWebhookDispatchService
{
    private readonly ChannelWriter<WebhookDeliveryItem> _channel;
    private readonly IServiceProvider _serviceProvider;

    public WebhookDispatchService(Channel<WebhookDeliveryItem> channel, IServiceProvider serviceProvider)
    {
        _channel = channel.Writer;
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync(Guid companyId, string eventName, object payload, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var webhooks = await db.Webhooks
            .Where(w => w.CompanyId == companyId && w.IsActive && !w.IsDeleted
                && w.Events.Contains(eventName))
            .Select(w => w.Id)
            .ToListAsync(cancellationToken);
        var payloadJson = System.Text.Json.JsonSerializer.Serialize(payload);
        foreach (var webhookId in webhooks)
        {
            var item = new WebhookDeliveryItem(webhookId, companyId, eventName, payloadJson);
            await _channel.WriteAsync(item, cancellationToken);
        }
    }
}
