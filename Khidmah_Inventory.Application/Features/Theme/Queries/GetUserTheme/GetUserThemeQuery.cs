using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Theme.Models;

namespace Khidmah_Inventory.Application.Features.Theme.Queries.GetUserTheme;

public class GetUserThemeQuery : IRequest<Result<ThemeDto>>
{
}

