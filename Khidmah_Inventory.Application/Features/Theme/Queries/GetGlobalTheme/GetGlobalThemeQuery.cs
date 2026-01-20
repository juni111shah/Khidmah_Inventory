using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Theme.Models;

namespace Khidmah_Inventory.Application.Features.Theme.Queries.GetGlobalTheme;

public class GetGlobalThemeQuery : IRequest<Result<ThemeDto>>
{
}

