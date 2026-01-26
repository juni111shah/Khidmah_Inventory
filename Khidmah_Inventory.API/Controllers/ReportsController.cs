using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.Reports.Queries.GetSalesReport;
using Khidmah_Inventory.Application.Features.Reports.Queries.GetInventoryReport;
using Khidmah_Inventory.Application.Features.Reports.Queries.GetPurchaseReport;
using Khidmah_Inventory.Application.Features.Reports.Commands.SaveCustomReport;
using Khidmah_Inventory.Application.Features.Reports.Queries.GetCustomReportsList;
using Khidmah_Inventory.Application.Features.Reports.Queries.ExecuteCustomReport;
using Khidmah_Inventory.Application.Features.Reports.Commands.GenerateSalesReportPdf;
using Khidmah_Inventory.Application.Features.Reports.Commands.GenerateInventoryReportPdf;
using Khidmah_Inventory.Application.Features.Reports.Commands.GeneratePurchaseReportPdf;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Reports.Base)]
[Authorize]
public class ReportsController : BaseController
{
    public ReportsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet(ApiRoutes.Reports.Sales)]
    [ValidateApiCode(ApiValidationCodes.ReportsModuleCode.Sales)]
    [AuthorizeResource(AuthorizePermissions.ReportsPermissions.Controller, AuthorizePermissions.ReportsPermissions.Actions.SalesRead)]
    public async Task<IActionResult> GetSalesReport([FromQuery] GetSalesReportQuery query)
    {
        return await ExecuteRequest(query);
    }

    [HttpPost(ApiRoutes.Reports.SalesPdf)]
    [ValidateApiCode(ApiValidationCodes.ReportsModuleCode.SalesPdf)]
    [AuthorizeResource(AuthorizePermissions.ReportsPermissions.Controller, AuthorizePermissions.ReportsPermissions.Actions.SalesRead)]
    public async Task<IActionResult> GenerateSalesReportPdf([FromBody] GenerateSalesReportPdfCommand command)
    {
        var result = await Mediator.Send(command);

        if (result.Succeeded && result.Data != null)
        {
            return File(result.Data, "application/pdf", $"Sales_Report_{command.FromDate:yyyy-MM-dd}_{command.ToDate:yyyy-MM-dd}.pdf");
        }

        return BadRequest(result.Errors);
    }

    [HttpPost(ApiRoutes.Reports.InventoryPdf)]
    [ValidateApiCode(ApiValidationCodes.ReportsModuleCode.InventoryPdf)]
    [AuthorizeResource(AuthorizePermissions.ReportsPermissions.Controller, AuthorizePermissions.ReportsPermissions.Actions.InventoryRead)]
    public async Task<IActionResult> GenerateInventoryReportPdf([FromBody] GenerateInventoryReportPdfCommand command)
    {
        var result = await Mediator.Send(command);

        if (result.Succeeded && result.Data != null)
        {
            var filename = "Inventory_Report";
            if (command.LowStockOnly == true)
                filename += "_Low_Stock";
            filename += $"_{DateTime.UtcNow:yyyy-MM-dd}.pdf";

            return File(result.Data, "application/pdf", filename);
        }

        return BadRequest(result.Errors);
    }

    [HttpPost(ApiRoutes.Reports.PurchasePdf)]
    [ValidateApiCode(ApiValidationCodes.ReportsModuleCode.PurchasePdf)]
    [AuthorizeResource(AuthorizePermissions.ReportsPermissions.Controller, AuthorizePermissions.ReportsPermissions.Actions.PurchaseRead)]
    public async Task<IActionResult> GeneratePurchaseReportPdf([FromBody] GeneratePurchaseReportPdfCommand command)
    {
        var result = await Mediator.Send(command);

        if (result.Succeeded && result.Data != null)
        {
            return File(result.Data, "application/pdf", $"Purchase_Report_{command.FromDate:yyyy-MM-dd}_{command.ToDate:yyyy-MM-dd}.pdf");
        }

        return BadRequest(result.Errors);
    }

    [HttpGet(ApiRoutes.Reports.Inventory)]
    [ValidateApiCode(ApiValidationCodes.ReportsModuleCode.Inventory)]
    [AuthorizeResource(AuthorizePermissions.ReportsPermissions.Controller, AuthorizePermissions.ReportsPermissions.Actions.InventoryRead)]
    public async Task<IActionResult> GetInventoryReport([FromQuery] GetInventoryReportQuery query)
    {
        return await ExecuteRequest(query);
    }

    [HttpGet(ApiRoutes.Reports.Purchase)]
    [ValidateApiCode(ApiValidationCodes.ReportsModuleCode.Purchase)]
    [AuthorizeResource(AuthorizePermissions.ReportsPermissions.Controller, AuthorizePermissions.ReportsPermissions.Actions.PurchaseRead)]
    public async Task<IActionResult> GetPurchaseReport([FromQuery] GetPurchaseReportQuery query)
    {
        return await ExecuteRequest(query);
    }

    [HttpGet(ApiRoutes.Reports.Custom)]
    [ValidateApiCode(ApiValidationCodes.ReportsModuleCode.Custom)]
    [AuthorizeResource(AuthorizePermissions.ReportsPermissions.Controller, AuthorizePermissions.ReportsPermissions.Actions.CustomRead)]
    public async Task<IActionResult> GetCustomReports([FromQuery] GetCustomReportsListQuery query)
    {
        return await ExecuteRequest(query);
    }

    [HttpPost(ApiRoutes.Reports.Custom)]
    [ValidateApiCode(ApiValidationCodes.ReportsModuleCode.Custom)]
    [AuthorizeResource(AuthorizePermissions.ReportsPermissions.Controller, AuthorizePermissions.ReportsPermissions.Actions.CustomCreate)]
    public async Task<IActionResult> SaveCustomReport([FromBody] SaveCustomReportCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPost(ApiRoutes.Reports.ExecuteCustom)]
    [ValidateApiCode(ApiValidationCodes.ReportsModuleCode.CustomExecute)]
    [AuthorizeResource(AuthorizePermissions.ReportsPermissions.Controller, AuthorizePermissions.ReportsPermissions.Actions.CustomExecute)]
    public async Task<IActionResult> ExecuteCustomReport(Guid id, [FromBody] ExecuteCustomReportQuery query)
    {
        query.ReportId = id;
        return await ExecuteRequest(query);
    }
}

