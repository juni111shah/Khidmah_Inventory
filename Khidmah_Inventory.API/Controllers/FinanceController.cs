using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.Finance.Accounts.Commands.CreateAccount;
using Khidmah_Inventory.Application.Features.Finance.Accounts.Commands.UpdateAccount;
using Khidmah_Inventory.Application.Features.Finance.Accounts.Commands.DeleteAccount;
using Khidmah_Inventory.Application.Features.Finance.Accounts.Commands.ImportStandardChart;
using Khidmah_Inventory.Application.Features.Finance.Accounts.Queries.GetAccount;
using Khidmah_Inventory.Application.Features.Finance.Accounts.Queries.GetAccountsTree;
using Khidmah_Inventory.Application.Features.Finance.Journals.Queries.GetJournalEntries;
using Khidmah_Inventory.Application.Features.Finance.Statements.Queries.GetProfitLoss;
using Khidmah_Inventory.Application.Features.Finance.Statements.Queries.GetBalanceSheet;
using Khidmah_Inventory.Application.Features.Finance.Statements.Queries.GetCashFlow;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Finance.Base)]
[Authorize]
public class FinanceController : BaseController
{
    public FinanceController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet(ApiRoutes.Finance.AccountsTree)]
    [ValidateApiCode(ApiValidationCodes.FinanceModuleCode.AccountsTree)]
    [AuthorizeResource(AuthorizePermissions.FinancePermissions.Controller, AuthorizePermissions.FinancePermissions.Actions.AccountsList)]
    public async Task<IActionResult> GetAccountsTree([FromQuery] bool includeInactive = false)
    {
        return await ExecuteRequest(new GetAccountsTreeQuery { IncludeInactive = includeInactive });
    }

    [HttpGet(ApiRoutes.Finance.AccountById)]
    [ValidateApiCode(ApiValidationCodes.FinanceModuleCode.AccountById)]
    [AuthorizeResource(AuthorizePermissions.FinancePermissions.Controller, AuthorizePermissions.FinancePermissions.Actions.AccountsRead)]
    public async Task<IActionResult> GetAccount(Guid id)
    {
        return await ExecuteRequest(new GetAccountQuery { Id = id });
    }

    [HttpPost(ApiRoutes.Finance.Accounts)]
    [ValidateApiCode(ApiValidationCodes.FinanceModuleCode.AccountsCreate)]
    [AuthorizeResource(AuthorizePermissions.FinancePermissions.Controller, AuthorizePermissions.FinancePermissions.Actions.AccountsCreate)]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPut(ApiRoutes.Finance.AccountById)]
    [ValidateApiCode(ApiValidationCodes.FinanceModuleCode.AccountsUpdate)]
    [AuthorizeResource(AuthorizePermissions.FinancePermissions.Controller, AuthorizePermissions.FinancePermissions.Actions.AccountsUpdate)]
    public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] UpdateAccountCommand command)
    {
        command.Id = id;
        return await ExecuteRequest(command);
    }

    [HttpDelete(ApiRoutes.Finance.AccountById)]
    [ValidateApiCode(ApiValidationCodes.FinanceModuleCode.AccountsDelete)]
    [AuthorizeResource(AuthorizePermissions.FinancePermissions.Controller, AuthorizePermissions.FinancePermissions.Actions.AccountsDelete)]
    public async Task<IActionResult> DeleteAccount(Guid id)
    {
        return await ExecuteRequest(new DeleteAccountCommand { Id = id });
    }

    [HttpPost(ApiRoutes.Finance.ImportChart)]
    [ValidateApiCode(ApiValidationCodes.FinanceModuleCode.ImportChart)]
    [AuthorizeResource(AuthorizePermissions.FinancePermissions.Controller, AuthorizePermissions.FinancePermissions.Actions.AccountsCreate)]
    public async Task<IActionResult> ImportStandardChart()
    {
        return await ExecuteRequest(new ImportStandardChartCommand());
    }

    [HttpGet(ApiRoutes.Finance.Journals)]
    [ValidateApiCode(ApiValidationCodes.FinanceModuleCode.Journals)]
    [AuthorizeResource(AuthorizePermissions.FinancePermissions.Controller, AuthorizePermissions.FinancePermissions.Actions.JournalsRead)]
    public async Task<IActionResult> GetJournalEntries([FromQuery] GetJournalEntriesQuery query)
    {
        return await ExecuteRequest(query);
    }

    [HttpGet(ApiRoutes.Finance.StatementsPl)]
    [ValidateApiCode(ApiValidationCodes.FinanceModuleCode.StatementsPl)]
    [AuthorizeResource(AuthorizePermissions.FinancePermissions.Controller, AuthorizePermissions.FinancePermissions.Actions.StatementsRead)]
    public async Task<IActionResult> GetProfitLoss([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
    {
        return await ExecuteRequest(new GetProfitLossQuery { FromDate = fromDate, ToDate = toDate });
    }

    [HttpGet(ApiRoutes.Finance.StatementsBalanceSheet)]
    [ValidateApiCode(ApiValidationCodes.FinanceModuleCode.StatementsBalanceSheet)]
    [AuthorizeResource(AuthorizePermissions.FinancePermissions.Controller, AuthorizePermissions.FinancePermissions.Actions.StatementsRead)]
    public async Task<IActionResult> GetBalanceSheet([FromQuery] DateTime asOfDate)
    {
        return await ExecuteRequest(new GetBalanceSheetQuery { AsOfDate = asOfDate });
    }

    [HttpGet(ApiRoutes.Finance.StatementsCashFlow)]
    [ValidateApiCode(ApiValidationCodes.FinanceModuleCode.StatementsCashFlow)]
    [AuthorizeResource(AuthorizePermissions.FinancePermissions.Controller, AuthorizePermissions.FinancePermissions.Actions.StatementsRead)]
    public async Task<IActionResult> GetCashFlow([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
    {
        return await ExecuteRequest(new GetCashFlowQuery { FromDate = fromDate, ToDate = toDate });
    }
}
