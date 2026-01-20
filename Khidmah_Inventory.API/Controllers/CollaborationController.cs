using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.Application.Features.Collaboration.Queries.GetActivityFeed;
using Khidmah_Inventory.Application.Features.Collaboration.Queries.GetComments;
using Khidmah_Inventory.Application.Features.Collaboration.Commands.CreateComment;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CollaborationController : BaseApiController
{
    [HttpGet("activity-feed")]
    [AuthorizePermission("Collaboration:ActivityFeed:Read")]
    public async Task<IActionResult> GetActivityFeed([FromQuery] GetActivityFeedQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Activity feed retrieved successfully");
    }

    [HttpGet("comments")]
    [AuthorizePermission("Collaboration:Comments:Read")]
    public async Task<IActionResult> GetComments([FromQuery] GetCommentsQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Comments retrieved successfully");
    }

    [HttpPost("comments")]
    [AuthorizePermission("Collaboration:Comments:Create")]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Comment created successfully");
    }
}

