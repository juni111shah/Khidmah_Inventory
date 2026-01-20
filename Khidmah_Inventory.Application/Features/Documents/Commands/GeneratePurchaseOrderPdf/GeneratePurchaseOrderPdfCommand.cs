using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Documents.Commands.GeneratePurchaseOrderPdf;

public class GeneratePurchaseOrderPdfCommand : IRequest<Result<byte[]>>
{
    public Guid PurchaseOrderId { get; set; }
}

