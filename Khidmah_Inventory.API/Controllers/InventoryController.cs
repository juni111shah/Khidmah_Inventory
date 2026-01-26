using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Inventory.Commands.CreateStockTransaction;
using Khidmah_Inventory.Application.Features.Inventory.Queries.GetStockTransactionsList;
using Khidmah_Inventory.Application.Features.Inventory.Queries.GetStockLevelsList;
using Khidmah_Inventory.Application.Features.Inventory.Commands.CreateBatch;
using Khidmah_Inventory.Application.Features.Inventory.Commands.CreateSerialNumber;
using Khidmah_Inventory.Application.Features.Inventory.Queries.GetBatchesList;
using Khidmah_Inventory.Application.Features.Inventory.Queries.GetSerialNumbersList;
using Khidmah_Inventory.Application.Features.Inventory.Commands.RecallBatch;
using Khidmah_Inventory.Application.Features.Inventory.Models;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Inventory.Base)]
[Authorize]
public class InventoryController : BaseController
{
    public InventoryController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost(ApiRoutes.Inventory.StockTransaction)]
    [ValidateApiCode(ApiValidationCodes.InventoryModuleCode.StockTransaction)]
    [AuthorizeResource(AuthorizePermissions.InventoryPermissions.Controller, AuthorizePermissions.InventoryPermissions.Actions.StockTransactionCreate)]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateStockTransactionCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPost("transactions/list")]
    [ValidateApiCode(ApiValidationCodes.InventoryModuleCode.StockTransactions)]
    [AuthorizeResource(AuthorizePermissions.InventoryPermissions.Controller, AuthorizePermissions.InventoryPermissions.Actions.StockTransactionList)]
    public async Task<IActionResult> GetTransactions([FromBody] GetStockTransactionsListQuery query)
    {
        return await ExecuteRequest(query);
    }

    [HttpPost("stock-levels/list")]
    [ValidateApiCode(ApiValidationCodes.InventoryModuleCode.StockLevels)]
    [AuthorizeResource(AuthorizePermissions.InventoryPermissions.Controller, AuthorizePermissions.InventoryPermissions.Actions.StockLevelList)]
    public async Task<IActionResult> GetStockLevels([FromBody] GetStockLevelsListQuery query)
    {
        return await ExecuteRequest(query);
    }

    [HttpPost(ApiRoutes.Inventory.AdjustStock)]
    [ValidateApiCode(ApiValidationCodes.InventoryModuleCode.AdjustStock)]
    [AuthorizeResource(AuthorizePermissions.InventoryPermissions.Controller, AuthorizePermissions.InventoryPermissions.Actions.StockTransactionCreate)]
    public async Task<IActionResult> TransferStock([FromBody] Khidmah_Inventory.Application.Features.Inventory.Commands.TransferStock.TransferStockCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPost(ApiRoutes.Inventory.Batch)]
    [ValidateApiCode(ApiValidationCodes.InventoryModuleCode.Batch)]
    [AuthorizeResource(AuthorizePermissions.InventoryPermissions.Controller, AuthorizePermissions.InventoryPermissions.Actions.BatchCreate)]
    public async Task<IActionResult> CreateBatch([FromBody] CreateBatchCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPost("batches/list")]
    [ValidateApiCode(ApiValidationCodes.InventoryModuleCode.Batches)]
    [AuthorizeResource(AuthorizePermissions.InventoryPermissions.Controller, AuthorizePermissions.InventoryPermissions.Actions.BatchList)]
    public async Task<IActionResult> GetBatches([FromBody] GetBatchesListQuery query)
    {
        return await ExecuteRequest<GetBatchesListQuery, PagedResult<BatchDto>>(query);
    }

    [HttpPost("batches/{id}/recall")]
    [ValidateApiCode(ApiValidationCodes.InventoryModuleCode.BatchUpdate)]
    [AuthorizeResource(AuthorizePermissions.InventoryPermissions.Controller, AuthorizePermissions.InventoryPermissions.Actions.BatchUpdate)]
    public async Task<IActionResult> RecallBatch(Guid id, [FromBody] RecallBatchCommand command)
    {
        command.BatchId = id;
        return await ExecuteRequest(command);
    }

    [HttpPost("serial-numbers")]
    [ValidateApiCode(ApiValidationCodes.InventoryModuleCode.SerialNumber)]
    [AuthorizeResource(AuthorizePermissions.InventoryPermissions.Controller, AuthorizePermissions.InventoryPermissions.Actions.SerialNumberCreate)]
    public async Task<IActionResult> CreateSerialNumber([FromBody] CreateSerialNumberCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPost("serial-numbers/list")]
    [ValidateApiCode(ApiValidationCodes.InventoryModuleCode.SerialNumbers)]
    [AuthorizeResource(AuthorizePermissions.InventoryPermissions.Controller, AuthorizePermissions.InventoryPermissions.Actions.SerialNumberList)]
    public async Task<IActionResult> GetSerialNumbers([FromBody] GetSerialNumbersListQuery query)
    {
        return await ExecuteRequest(query);
    }
}

