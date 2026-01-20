using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Features.Settings.Commands.SaveNotificationSettings;

public class SaveNotificationSettingsCommand : IRequest<Result<NotificationSettingsDto>>
{
    public NotificationSettingsDto Settings { get; set; } = null!;
}

