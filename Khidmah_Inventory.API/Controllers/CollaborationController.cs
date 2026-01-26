using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.Collaboration.Queries.GetActivityFeed;
using Khidmah_Inventory.Application.Features.Collaboration.Queries.GetComments;
using Khidmah_Inventory.Application.Features.Collaboration.Commands.CreateComment;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Collaboration.Base)]
[Authorize]
public class CollaborationController : BaseController
{
    public CollaborationController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet(ApiRoutes.Collaboration.ActivityFeed)]
    [ValidateApiCode(ApiValidationCodes.CollaborationModuleCode.ActivityFeed)]
    [AuthorizeResource(AuthorizePermissions.CollaborationPermissions.Controller, AuthorizePermissions.CollaborationPermissions.Actions.ActivityFeed)]
    public async Task<IActionResult> GetActivityFeed([FromQuery] GetActivityFeedQuery query)
    {
        return await ExecuteRequest(query);
    }

    [HttpGet(ApiRoutes.Collaboration.Comments)]
    [ValidateApiCode(ApiValidationCodes.CollaborationModuleCode.Comments)]
    [AuthorizeResource(AuthorizePermissions.CollaborationPermissions.Controller, AuthorizePermissions.CollaborationPermissions.Actions.Comments)]
    public async Task<IActionResult> GetComments([FromQuery] GetCommentsQuery query)
    {
        return await ExecuteRequest(query);
    }

    [HttpPost(ApiRoutes.Collaboration.Comments)]
    [ValidateApiCode(ApiValidationCodes.CollaborationModuleCode.Comments)]
    [AuthorizeResource(AuthorizePermissions.CollaborationPermissions.Controller, AuthorizePermissions.CollaborationPermissions.Actions.CommentsCreate)]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentCommand command)
    {
        return await ExecuteRequest(command);
    }
}

