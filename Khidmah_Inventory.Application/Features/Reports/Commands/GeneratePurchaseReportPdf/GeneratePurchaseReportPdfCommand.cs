using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Reports.Commands.GeneratePurchaseReportPdf;

public class GeneratePurchaseReportPdfCommand : IRequest<Result<byte[]>>
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public Guid? SupplierId { get; set; }
}