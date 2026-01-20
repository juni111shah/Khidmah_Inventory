using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Documents.Commands.GenerateInvoicePdf;

public class GenerateInvoicePdfCommand : IRequest<Result<byte[]>>
{
    public Guid SalesOrderId { get; set; }
}

