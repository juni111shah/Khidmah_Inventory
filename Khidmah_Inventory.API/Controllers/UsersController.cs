using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Users.Queries.GetUser;
using Khidmah_Inventory.Application.Features.Users.Queries.GetUsersList;
using Khidmah_Inventory.Application.Features.Users.Queries.GetCurrentUser;
using Khidmah_Inventory.Application.Features.Users.Commands.UpdateUserProfile;
using Khidmah_Inventory.Application.Features.Users.Commands.ChangePassword;
using Khidmah_Inventory.Application.Features.Users.Commands.ActivateUser;
using Khidmah_Inventory.Application.Features.Users.Commands.DeactivateUser;
using Khidmah_Inventory.Application.Features.Users.Commands.UploadUserAvatar;
using Khidmah_Inventory.Application.Features.Users.Commands.CreateUser;
using Khidmah_Inventory.API.Models;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Users.Base)]
[Authorize]
public class UsersController : BaseController
{
    public UsersController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet(ApiRoutes.Users.Current)]
    [ValidateApiCode(ApiValidationCodes.UsersModuleCode.ViewCurrent)]
    public async Task<IActionResult> GetCurrent()
    {
        return await ExecuteRequest(new GetCurrentUserQuery());
    }

    [HttpGet(ApiRoutes.Users.New)]
    [ValidateApiCode(ApiValidationCodes.UsersModuleCode.Add)]
    [AuthorizeResource(AuthorizePermissions.UsersPermissions.Controller, AuthorizePermissions.UsersPermissions.Actions.Add)]
    public IActionResult GetNewUserTemplate()
    {
        var template = new
        {
            email = "",
            userName = "",
            firstName = "",
            lastName = "",
            phoneNumber = "",
            roles = new List<string>(),
            companyId = (string?)null
        };

        return Ok(ApiResponse<object>.SuccessResponse(template, "New user template retrieved successfully", 200));
    }

    [HttpPost(ApiRoutes.Users.Add)]
    [ValidateApiCode(ApiValidationCodes.UsersModuleCode.Add)]
    [AuthorizeResource(AuthorizePermissions.UsersPermissions.Controller, AuthorizePermissions.UsersPermissions.Actions.Add)]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpGet(ApiRoutes.Users.GetById)]
    [ValidateApiCode(ApiValidationCodes.UsersModuleCode.ViewById)]
    [AuthorizeResource(AuthorizePermissions.UsersPermissions.Controller, AuthorizePermissions.UsersPermissions.Actions.ViewById)]
    public async Task<IActionResult> GetById(Guid id)
    {
        return await ExecuteRequestWithCache(new GetUserQuery { Id = id });
    }

    [HttpPost(ApiRoutes.Users.Index)]
    [ValidateApiCode(ApiValidationCodes.UsersModuleCode.ViewAll)]
    [AuthorizeResource(AuthorizePermissions.UsersPermissions.Controller, AuthorizePermissions.UsersPermissions.Actions.ViewAll)]
    public async Task<IActionResult> GetAll([FromBody] FilterRequest request)
    {
        var query = new GetUsersListQuery { FilterRequest = request };
        return await ExecuteRequest(query);
    }

    [HttpPut(ApiRoutes.Users.UpdateProfile)]
    [ValidateApiCode(ApiValidationCodes.UsersModuleCode.Update)]
    [AuthorizeResource(AuthorizePermissions.UsersPermissions.Controller, AuthorizePermissions.UsersPermissions.Actions.Update)]
    public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateUserProfileCommand command)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        command.Id = id;
        return await ExecuteRequest(command);
    }

    [HttpPost(ApiRoutes.Users.ChangePassword)]
    [ValidateApiCode(ApiValidationCodes.UsersModuleCode.ChangePassword)]
    [AuthorizeResource(AuthorizePermissions.UsersPermissions.Controller, AuthorizePermissions.UsersPermissions.Actions.Update)]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordCommand command)
    {
        command.UserId = id;
        return await ExecuteRequest(command);
    }

    [HttpPatch(ApiRoutes.Users.Activate)]
    [ValidateApiCode(ApiValidationCodes.UsersModuleCode.UpdateStatus)]
    [AuthorizeResource(AuthorizePermissions.UsersPermissions.Controller, AuthorizePermissions.UsersPermissions.Actions.Update)]
    public async Task<IActionResult> Activate(Guid id)
    {
        return await ExecuteRequest(new ActivateUserCommand { Id = id });
    }

    [HttpPatch(ApiRoutes.Users.Deactivate)]
    [ValidateApiCode(ApiValidationCodes.UsersModuleCode.UpdateStatus)]
    [AuthorizeResource(AuthorizePermissions.UsersPermissions.Controller, AuthorizePermissions.UsersPermissions.Actions.Update)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        return await ExecuteRequest(new DeactivateUserCommand { Id = id });
    }

    [HttpPost(ApiRoutes.Users.UploadAvatar)]
    [ValidateApiCode(ApiValidationCodes.UsersModuleCode.UploadAvatar)]
    [AuthorizeResource(AuthorizePermissions.UsersPermissions.Controller, AuthorizePermissions.UsersPermissions.Actions.Update)]
    public async Task<IActionResult> UploadAvatar(Guid id, IFormFile file)
    {
        return await ExecuteRequest(new UploadUserAvatarCommand { UserId = id, File = file });
    }
}

