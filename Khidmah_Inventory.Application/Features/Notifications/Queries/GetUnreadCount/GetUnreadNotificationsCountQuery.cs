using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Notifications.Queries.GetUnreadCount;

public class GetUnreadNotificationsCountQuery : IRequest<Result<int>>
{
}
