using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.Application.Features.Reports.Queries.GetSalesReport;
using Khidmah_Inventory.Application.Features.Reports.Queries.GetInventoryReport;
using Khidmah_Inventory.Application.Features.Reports.Queries.GetPurchaseReport;
using Khidmah_Inventory.Application.Features.Reports.Commands.SaveCustomReport;
using Khidmah_Inventory.Application.Features.Reports.Queries.GetCustomReportsList;
using Khidmah_Inventory.Application.Features.Reports.Queries.ExecuteCustomReport;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : BaseApiController
{
    [HttpGet("sales")]
    [AuthorizePermission("Reports:Sales:Read")]
    public async Task<IActionResult> GetSalesReport([FromQuery] GetSalesReportQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Sales report retrieved successfully");
    }

    [HttpGet("inventory")]
    [AuthorizePermission("Reports:Inventory:Read")]
    public async Task<IActionResult> GetInventoryReport([FromQuery] GetInventoryReportQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Inventory report retrieved successfully");
    }

    [HttpGet("purchase")]
    [AuthorizePermission("Reports:Purchase:Read")]
    public async Task<IActionResult> GetPurchaseReport([FromQuery] GetPurchaseReportQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Purchase report retrieved successfully");
    }

    [HttpGet("custom")]
    [AuthorizePermission("Reports:Custom:Read")]
    public async Task<IActionResult> GetCustomReports([FromQuery] GetCustomReportsListQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Custom reports retrieved successfully");
    }

    [HttpPost("custom")]
    [AuthorizePermission("Reports:Custom:Create")]
    public async Task<IActionResult> SaveCustomReport([FromBody] SaveCustomReportCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Custom report saved successfully");
    }

    [HttpPost("custom/{id}/execute")]
    [AuthorizePermission("Reports:Custom:Execute")]
    public async Task<IActionResult> ExecuteCustomReport(Guid id, [FromBody] ExecuteCustomReportQuery query)
    {
        query.ReportId = id;
        var result = await Mediator.Send(query);
        return HandleResult(result, "Custom report executed successfully");
    }
}

