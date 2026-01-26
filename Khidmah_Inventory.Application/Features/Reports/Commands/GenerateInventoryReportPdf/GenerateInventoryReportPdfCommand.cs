using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Reports.Commands.GenerateInventoryReportPdf;

public class GenerateInventoryReportPdfCommand : IRequest<Result<byte[]>>
{
    public Guid? WarehouseId { get; set; }
    public Guid? CategoryId { get; set; }
    public bool? LowStockOnly { get; set; }
}