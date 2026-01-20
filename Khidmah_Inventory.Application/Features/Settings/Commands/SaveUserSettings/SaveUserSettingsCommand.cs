using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Features.Settings.Commands.SaveUserSettings;

public class SaveUserSettingsCommand : IRequest<Result<UserSettingsDto>>
{
    public UserSettingsDto Settings { get; set; } = null!;
}

