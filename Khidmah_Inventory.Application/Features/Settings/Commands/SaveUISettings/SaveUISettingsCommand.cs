using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Features.Settings.Commands.SaveUISettings;

public class SaveUISettingsCommand : IRequest<Result<UISettingsDto>>
{
    public UISettingsDto Settings { get; set; } = null!;
}

