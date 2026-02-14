using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Platform.ScheduledReports.Commands.DeleteScheduledReport;

public class DeleteScheduledReportCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
