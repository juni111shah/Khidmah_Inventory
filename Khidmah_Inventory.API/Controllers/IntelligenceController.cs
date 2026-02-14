using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.Intelligence.Queries.GetProductIntelligence;
using Khidmah_Inventory.Application.Features.Intelligence.Queries.GetDashboardIntelligence;
using Khidmah_Inventory.Application.Features.Intelligence.Models;
using Khidmah_Inventory.Application.Common.Interfaces;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Intelligence.Base)]
[Authorize]
public class IntelligenceController : BaseController
{
    private readonly ICurrentUserService _currentUser;
    private readonly IAiRecommendationService _aiRecommendationService;

    public IntelligenceController(
        IMediator mediator,
        ICurrentUserService currentUser,
        IAiRecommendationService aiRecommendationService) : base(mediator)
    {
        _currentUser = currentUser;
        _aiRecommendationService = aiRecommendationService;
    }

    [HttpGet(ApiRoutes.Intelligence.Product)]
    public async Task<IActionResult> GetProductIntelligence(Guid productId, [FromQuery] int? daysForVelocity = 30)
    {
        return await ExecuteRequest<GetProductIntelligenceQuery, ProductIntelligenceDto>(new GetProductIntelligenceQuery
        {
            ProductId = productId,
            DaysForVelocity = daysForVelocity
        });
    }

    [HttpGet(ApiRoutes.Intelligence.Dashboard)]
    public async Task<IActionResult> GetDashboardIntelligence([FromQuery] int? predictionDays = 7)
    {
        return await ExecuteRequest<GetDashboardIntelligenceQuery, DashboardIntelligenceDto>(new GetDashboardIntelligenceQuery
        {
            PredictionDays = predictionDays
        });
    }

    [HttpGet(ApiRoutes.Intelligence.Recommendations)]
    public async Task<IActionResult> GetRecommendations([FromQuery] Guid? productId = null, [FromQuery] int horizonDays = 14)
    {
        if (!_currentUser.CompanyId.HasValue)
        {
            return Unauthorized(API.Models.ApiResponse<List<AiRecommendationDto>>.FailureResponse("Company context is required", 401));
        }

        var recommendations = await _aiRecommendationService.GetRecommendationsAsync(
            _currentUser.CompanyId.Value,
            productId,
            horizonDays);

        return Ok(API.Models.ApiResponse<List<AiRecommendationDto>>.SuccessResponse(recommendations));
    }
}
