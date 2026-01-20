using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Features.Settings.Commands.SaveReportSettings;

public class SaveReportSettingsCommand : IRequest<Result<ReportSettingsDto>>
{
    public ReportSettingsDto Settings { get; set; } = null!;
}

