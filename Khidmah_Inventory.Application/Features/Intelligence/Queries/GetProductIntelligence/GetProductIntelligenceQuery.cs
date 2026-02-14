using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Intelligence.Models;

namespace Khidmah_Inventory.Application.Features.Intelligence.Queries.GetProductIntelligence;

public class GetProductIntelligenceQuery : IRequest<Result<ProductIntelligenceDto>>
{
    public Guid ProductId { get; set; }
    public int? DaysForVelocity { get; set; } = 30;
}
