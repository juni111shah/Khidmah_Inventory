using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Theme.Models;

namespace Khidmah_Inventory.Application.Features.Theme.Commands.SaveUserTheme;

public class SaveUserThemeCommand : IRequest<Result<ThemeDto>>
{
    public ThemeDto Theme { get; set; } = null!;
}

