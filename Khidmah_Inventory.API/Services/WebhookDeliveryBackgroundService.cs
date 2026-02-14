using System.Net.Http.Json;
using System.Text;
using System.Threading.Channels;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Khidmah_Inventory.API.Services;

public class WebhookDeliveryBackgroundService : BackgroundService
{
    private readonly ChannelReader<WebhookDeliveryItem> _channel;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpClientFactory _httpClientFactory;
    private const int MaxRetries = 3;
    private static readonly TimeSpan[] Backoff = { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30) };

    public WebhookDeliveryBackgroundService(
        Channel<WebhookDeliveryItem> channel,
        IServiceProvider serviceProvider,
        IHttpClientFactory httpClientFactory)
    {
        _channel = channel.Reader;
        _serviceProvider = serviceProvider;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (await _channel.WaitToReadAsync(stoppingToken))
                {
                    while (_channel.TryRead(out var item))
                        await DeliverAsync(item, stoppingToken);
                }
            }
            catch (OperationCanceledException) { break; }
            catch (Exception)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task DeliverAsync(WebhookDeliveryItem item, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var webhook = await db.Webhooks
            .FirstOrDefaultAsync(w => w.Id == item.WebhookId && !w.IsDeleted, cancellationToken);
        if (webhook == null) return;

        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(30);
        var content = new StringContent(item.PayloadJson, Encoding.UTF8, "application/json");
        int statusCode = 0;
        string? responseBody = null;
        string? errorMessage = null;
        var success = false;
        for (var attempt = 0; attempt < MaxRetries; attempt++)
        {
            try
            {
                var response = await client.PostAsync(webhook.Url, content, cancellationToken);
                statusCode = (int)response.StatusCode;
                responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                success = response.IsSuccessStatusCode;
                if (success) break;
                errorMessage = responseBody.Length > 500 ? responseBody[..500] : responseBody;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                statusCode = 0;
            }
            if (attempt < MaxRetries - 1)
                await Task.Delay(Backoff[attempt], cancellationToken);
        }

        var log = new WebhookDeliveryLog(
            item.CompanyId,
            item.WebhookId,
            item.EventName,
            item.PayloadJson,
            statusCode,
            success,
            MaxRetries - 1,
            responseBody?.Length > 2000 ? responseBody[..2000] : responseBody,
            errorMessage);
        db.WebhookDeliveryLogs.Add(log);
        await db.SaveChangesAsync(cancellationToken);
    }
}
