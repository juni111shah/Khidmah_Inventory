using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Khidmah_Inventory.Application.Common.Interfaces;

namespace Khidmah_Inventory.API.Hubs;

[Authorize]
public class AnalyticsHub : Hub
{
    private readonly ICurrentUserService _currentUserService;

    public AnalyticsHub(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override async Task OnConnectedAsync()
    {
        var companyId = _currentUserService.CompanyId;
        if (companyId.HasValue)
        {
            // Add user to company-specific group for real-time updates
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Company_{companyId.Value}");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var companyId = _currentUserService.CompanyId;
        if (companyId.HasValue)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Company_{companyId.Value}");
        }
        await base.OnDisconnectedAsync(exception);
    }

    // Client can subscribe to specific analytics streams
    public async Task SubscribeToAnalytics(string analyticsType)
    {
        var companyId = _currentUserService.CompanyId;
        if (companyId.HasValue)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Analytics_{analyticsType}_Company_{companyId.Value}");
        }
    }

    public async Task UnsubscribeFromAnalytics(string analyticsType)
    {
        var companyId = _currentUserService.CompanyId;
        if (companyId.HasValue)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Analytics_{analyticsType}_Company_{companyId.Value}");
        }
    }
}

