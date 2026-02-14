using Microsoft.AspNetCore.SignalR;
using Khidmah_Inventory.API.Hubs;
using Khidmah_Inventory.API.Models;
using Khidmah_Inventory.Application.Common.Interfaces;

namespace Khidmah_Inventory.API.Services;

public class OperationsBroadcastService : IOperationsBroadcast
{
    private readonly IHubContext<OperationsHub> _hubContext;

    public OperationsBroadcastService(IHubContext<OperationsHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task BroadcastAsync(
        string eventName,
        Guid companyId,
        Guid? entityId,
        string? entityType,
        object? payload,
        CancellationToken cancellationToken = default)
    {
        var envelope = new OperationsEventPayload
        {
            CompanyId = companyId,
            EntityId = entityId,
            EntityType = entityType,
            Payload = payload,
            TimestampUtc = DateTime.UtcNow
        };
        await _hubContext.Clients
            .Group($"Company_{companyId}")
            .SendAsync(eventName, envelope, cancellationToken);
    }
}
