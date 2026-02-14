using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Notifications.Models;

namespace Khidmah_Inventory.Application.Features.Notifications.Queries.GetNotifications;

public class GetNotificationsQuery : IRequest<Result<PagedResult<NotificationDto>>>
{
    public FilterRequest? FilterRequest { get; set; }
    public bool? UnreadOnly { get; set; }
}
