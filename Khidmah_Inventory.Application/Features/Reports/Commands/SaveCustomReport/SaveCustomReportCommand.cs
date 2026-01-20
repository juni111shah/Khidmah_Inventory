using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Reports.Models;

namespace Khidmah_Inventory.Application.Features.Reports.Commands.SaveCustomReport;

public class SaveCustomReportCommand : IRequest<Result<CustomReportDto>>
{
    public Guid? Id { get; set; } // For update
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string ReportDefinition { get; set; } = string.Empty; // JSON string
    public bool IsPublic { get; set; } = false;
}

