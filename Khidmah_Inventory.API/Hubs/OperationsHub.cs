using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Khidmah_Inventory.Application.Common.Interfaces;

namespace Khidmah_Inventory.API.Hubs;

[Authorize]
public class OperationsHub : Hub
{
    private readonly ICurrentUserService _currentUserService;

    public OperationsHub(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override async Task OnConnectedAsync()
    {
        var companyId = _currentUserService.CompanyId;
        if (companyId.HasValue)
        {
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
}
