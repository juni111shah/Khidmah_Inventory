using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.Application.Features.Documents.Commands.GenerateInvoicePdf;
using Khidmah_Inventory.Application.Features.Documents.Commands.GeneratePurchaseOrderPdf;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentsController : BaseApiController
{
    [HttpGet("invoice/{salesOrderId}")]
    [AuthorizePermission("Documents:Invoice:Generate")]
    public async Task<IActionResult> GenerateInvoice(Guid salesOrderId)
    {
        var command = new GenerateInvoicePdfCommand { SalesOrderId = salesOrderId };
        var result = await Mediator.Send(command);

        if (result.Succeeded && result.Data != null)
        {
            return File(result.Data, "application/pdf", $"Invoice_{salesOrderId}.pdf");
        }

        return BadRequest(result.Errors);
    }

    [HttpGet("purchase-order/{purchaseOrderId}")]
    [AuthorizePermission("Documents:PurchaseOrder:Generate")]
    public async Task<IActionResult> GeneratePurchaseOrder(Guid purchaseOrderId)
    {
        var command = new GeneratePurchaseOrderPdfCommand { PurchaseOrderId = purchaseOrderId };
        var result = await Mediator.Send(command);

        if (result.Succeeded && result.Data != null)
        {
            return File(result.Data, "application/pdf", $"PO_{purchaseOrderId}.pdf");
        }

        return BadRequest(result.Errors);
    }
}

