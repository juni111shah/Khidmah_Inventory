using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.Documents.Commands.GenerateInvoicePdf;
using Khidmah_Inventory.Application.Features.Documents.Commands.GeneratePurchaseOrderPdf;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Documents.Base)]
[Authorize]
public class DocumentsController : BaseController
{
    public DocumentsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet(ApiRoutes.Documents.GenerateInvoice)]
    [ValidateApiCode(ApiValidationCodes.DocumentsModuleCode.GenerateInvoice)]
    [AuthorizeResource(AuthorizePermissions.DocumentsPermissions.Controller, AuthorizePermissions.DocumentsPermissions.Actions.GenerateInvoice)]
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

    [HttpGet(ApiRoutes.Documents.GeneratePurchaseOrder)]
    [ValidateApiCode(ApiValidationCodes.DocumentsModuleCode.GeneratePurchaseOrder)]
    [AuthorizeResource(AuthorizePermissions.DocumentsPermissions.Controller, AuthorizePermissions.DocumentsPermissions.Actions.GeneratePurchaseOrder)]
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

