using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.Application.Features.Inventory.Commands.CreateStockTransaction;
using Khidmah_Inventory.Application.Features.Inventory.Queries.GetStockTransactionsList;
using Khidmah_Inventory.Application.Features.Inventory.Queries.GetStockLevelsList;
using Khidmah_Inventory.Application.Features.Inventory.Commands.CreateBatch;
using Khidmah_Inventory.Application.Features.Inventory.Commands.CreateSerialNumber;
using Khidmah_Inventory.Application.Features.Inventory.Queries.GetBatchesList;
using Khidmah_Inventory.Application.Features.Inventory.Queries.GetSerialNumbersList;
using Khidmah_Inventory.Application.Features.Inventory.Commands.RecallBatch;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : BaseApiController
{
    [HttpPost("transactions")]
    [AuthorizePermission("Inventory:StockTransaction:Create")]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateStockTransactionCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Stock transaction created successfully");
    }

    [HttpPost("transactions/list")]
    [AuthorizePermission("Inventory:StockTransaction:List")]
    public async Task<IActionResult> GetTransactions([FromBody] GetStockTransactionsListQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Stock transactions retrieved successfully");
    }

    [HttpPost("stock-levels/list")]
    [AuthorizePermission("Inventory:StockLevel:List")]
    public async Task<IActionResult> GetStockLevels([FromBody] GetStockLevelsListQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Stock levels retrieved successfully");
    }

    [HttpPost("transfer")]
    [AuthorizePermission("Inventory:StockTransaction:Create")]
    public async Task<IActionResult> TransferStock([FromBody] Khidmah_Inventory.Application.Features.Inventory.Commands.TransferStock.TransferStockCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Stock transferred successfully");
    }

    [HttpPost("batches")]
    [AuthorizePermission("Inventory:Batch:Create")]
    public async Task<IActionResult> CreateBatch([FromBody] CreateBatchCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Batch created successfully");
    }

    [HttpPost("batches/list")]
    [AuthorizePermission("Inventory:Batch:List")]
    public async Task<IActionResult> GetBatches([FromBody] GetBatchesListQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Batches retrieved successfully");
    }

    [HttpPost("batches/{id}/recall")]
    [AuthorizePermission("Inventory:Batch:Update")]
    public async Task<IActionResult> RecallBatch(Guid id, [FromBody] RecallBatchCommand command)
    {
        command.BatchId = id;
        var result = await Mediator.Send(command);
        return HandleResult(result, "Batch recalled successfully");
    }

    [HttpPost("serial-numbers")]
    [AuthorizePermission("Inventory:SerialNumber:Create")]
    public async Task<IActionResult> CreateSerialNumber([FromBody] CreateSerialNumberCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Serial number created successfully");
    }

    [HttpPost("serial-numbers/list")]
    [AuthorizePermission("Inventory:SerialNumber:List")]
    public async Task<IActionResult> GetSerialNumbers([FromBody] GetSerialNumbersListQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Serial numbers retrieved successfully");
    }
}

