using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Features.Settings.Queries.GetUserSettings;

public class GetUserSettingsQuery : IRequest<Result<UserSettingsDto>>
{
}

