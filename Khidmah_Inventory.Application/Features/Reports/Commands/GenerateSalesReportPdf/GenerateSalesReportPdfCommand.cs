using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Reports.Commands.GenerateSalesReportPdf;

public class GenerateSalesReportPdfCommand : IRequest<Result<byte[]>>
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public Guid? CustomerId { get; set; }
}