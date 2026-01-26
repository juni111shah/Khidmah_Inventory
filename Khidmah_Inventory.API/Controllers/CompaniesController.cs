using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.Companies.Commands.UploadCompanyLogo;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Companies.Base)]
[Authorize]
public class CompaniesController : BaseController
{
    public CompaniesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost(ApiRoutes.Companies.UploadLogo)]
    [ValidateApiCode(ApiValidationCodes.CompaniesModuleCode.UploadLogo)]
    [AuthorizeResource(AuthorizePermissions.CompaniesPermissions.Controller, AuthorizePermissions.CompaniesPermissions.Actions.Update)]
    public async Task<IActionResult> UploadLogo(Guid id, IFormFile file)
    {
        return await ExecuteRequest(new UploadCompanyLogoCommand { CompanyId = id, File = file });
    }
}

