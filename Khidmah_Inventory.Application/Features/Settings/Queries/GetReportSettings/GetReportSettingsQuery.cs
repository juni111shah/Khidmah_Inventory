using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Features.Settings.Queries.GetReportSettings;

public class GetReportSettingsQuery : IRequest<Result<ReportSettingsDto>>
{
}

