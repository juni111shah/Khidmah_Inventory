using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Features.Settings.Commands.SaveSystemSettings;

public class SaveSystemSettingsCommand : IRequest<Result<SystemSettingsDto>>
{
    public SystemSettingsDto Settings { get; set; } = null!;
}

