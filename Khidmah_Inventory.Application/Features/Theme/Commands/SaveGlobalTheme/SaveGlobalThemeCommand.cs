using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Theme.Models;

namespace Khidmah_Inventory.Application.Features.Theme.Commands.SaveGlobalTheme;

public class SaveGlobalThemeCommand : IRequest<Result<ThemeDto>>
{
    public ThemeDto Theme { get; set; } = null!;
}

