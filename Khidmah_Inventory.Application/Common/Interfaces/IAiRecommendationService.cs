using Khidmah_Inventory.Application.Features.Intelligence.Models;

namespace Khidmah_Inventory.Application.Common.Interfaces;

public interface IAiRecommendationService
{
    Task<List<AiRecommendationDto>> GetRecommendationsAsync(Guid companyId, Guid? productId = null, int horizonDays = 14, CancellationToken cancellationToken = default);
}
