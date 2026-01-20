using Microsoft.AspNetCore.SignalR;
using Khidmah_Inventory.API.Hubs;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Features.Dashboard.Models;
using Khidmah_Inventory.Application.Features.Dashboard.Queries.GetDashboardData;
using MediatR;

namespace Khidmah_Inventory.API.Services;

public class AnalyticsBroadcastService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<AnalyticsHub> _hubContext;
    private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(30); // Update every 30 seconds

    public AnalyticsBroadcastService(
        IServiceProvider serviceProvider,
        IHubContext<AnalyticsHub> hubContext)
    {
        _serviceProvider = serviceProvider;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await BroadcastAnalyticsUpdates(stoppingToken);
                await Task.Delay(_updateInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                // Log error but continue
                Console.WriteLine($"Error broadcasting analytics: {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task BroadcastAnalyticsUpdates(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();

        // Get all active company groups (in a real scenario, you'd track active companies)
        // For now, we'll broadcast to all groups when data changes
        // In production, you'd maintain a list of active companies

        // This is a simplified version - in production, you'd track which companies have active connections
        // and only broadcast to those companies
    }

    public async Task BroadcastDashboardUpdate(Guid companyId, DashboardDto dashboard)
    {
        await _hubContext.Clients.Group($"Company_{companyId}")
            .SendAsync("DashboardUpdated", dashboard, cancellationToken: default);
    }

    public async Task BroadcastAnalyticsUpdate(Guid companyId, string analyticsType, object data)
    {
        await _hubContext.Clients.Group($"Analytics_{analyticsType}_Company_{companyId}")
            .SendAsync("AnalyticsUpdated", analyticsType, data, cancellationToken: default);
    }
}

