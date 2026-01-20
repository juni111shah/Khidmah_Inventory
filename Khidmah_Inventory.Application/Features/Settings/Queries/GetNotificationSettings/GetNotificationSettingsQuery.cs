using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Features.Settings.Queries.GetNotificationSettings;

public class GetNotificationSettingsQuery : IRequest<Result<NotificationSettingsDto>>
{
}

