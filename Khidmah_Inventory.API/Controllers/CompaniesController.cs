using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Companies.Commands.CreateCompany;
using Khidmah_Inventory.Application.Features.Companies.Commands.UpdateCompany;
using Khidmah_Inventory.Application.Features.Companies.Commands.ActivateCompany;
using Khidmah_Inventory.Application.Features.Companies.Commands.DeactivateCompany;
using Khidmah_Inventory.Application.Features.Companies.Commands.UploadCompanyLogo;
using Khidmah_Inventory.Application.Features.Companies.Queries.GetCompany;
using Khidmah_Inventory.Application.Features.Companies.Queries.GetCompaniesList;
using Khidmah_Inventory.Application.Features.Companies.Models;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Companies.Base)]
[Authorize]
public class CompaniesController : BaseController
{
    public CompaniesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost(ApiRoutes.Companies.Index)]
    [ValidateApiCode(ApiValidationCodes.CompaniesModuleCode.ViewAll)]
    [AuthorizeResource(AuthorizePermissions.CompaniesPermissions.Controller, AuthorizePermissions.CompaniesPermissions.Actions.ViewAll)]
    public async Task<IActionResult> GetList([FromBody] FilterRequest? request)
    {
        var query = new GetCompaniesListQuery { FilterRequest = request };
        return await ExecuteRequest<GetCompaniesListQuery, PagedResult<CompanyDto>>(query);
    }

    [HttpGet(ApiRoutes.Companies.GetById)]
    [ValidateApiCode(ApiValidationCodes.CompaniesModuleCode.ViewById)]
    [AuthorizeResource(AuthorizePermissions.CompaniesPermissions.Controller, AuthorizePermissions.CompaniesPermissions.Actions.ViewById)]
    public async Task<IActionResult> GetById(Guid id)
    {
        return await ExecuteRequest<GetCompanyQuery, CompanyDto>(new GetCompanyQuery { Id = id });
    }

    [HttpPost(ApiRoutes.Companies.Add)]
    [ValidateApiCode(ApiValidationCodes.CompaniesModuleCode.Add)]
    [AuthorizeResource(AuthorizePermissions.CompaniesPermissions.Controller, AuthorizePermissions.CompaniesPermissions.Actions.Add)]
    public async Task<IActionResult> Create([FromBody] CreateCompanyCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPut(ApiRoutes.Companies.Update)]
    [ValidateApiCode(ApiValidationCodes.CompaniesModuleCode.Update)]
    [AuthorizeResource(AuthorizePermissions.CompaniesPermissions.Controller, AuthorizePermissions.CompaniesPermissions.Actions.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCompanyCommand command)
    {
        command.Id = id;
        return await ExecuteRequest(command);
    }

    [HttpPatch(ApiRoutes.Companies.Activate)]
    [ValidateApiCode(ApiValidationCodes.CompaniesModuleCode.UpdateStatus)]
    [AuthorizeResource(AuthorizePermissions.CompaniesPermissions.Controller, AuthorizePermissions.CompaniesPermissions.Actions.Update)]
    public async Task<IActionResult> Activate(Guid id)
    {
        return await ExecuteRequest(new ActivateCompanyCommand { Id = id });
    }

    [HttpPatch(ApiRoutes.Companies.Deactivate)]
    [ValidateApiCode(ApiValidationCodes.CompaniesModuleCode.UpdateStatus)]
    [AuthorizeResource(AuthorizePermissions.CompaniesPermissions.Controller, AuthorizePermissions.CompaniesPermissions.Actions.Update)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        return await ExecuteRequest(new DeactivateCompanyCommand { Id = id });
    }

    [HttpPost(ApiRoutes.Companies.UploadLogo)]
    [ValidateApiCode(ApiValidationCodes.CompaniesModuleCode.UploadLogo)]
    [AuthorizeResource(AuthorizePermissions.CompaniesPermissions.Controller, AuthorizePermissions.CompaniesPermissions.Actions.Update)]
    public async Task<IActionResult> UploadLogo(Guid id, IFormFile file)
    {
        return await ExecuteRequest(new UploadCompanyLogoCommand { CompanyId = id, File = file });
    }
}
