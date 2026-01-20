using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Inventory.Models;

namespace Khidmah_Inventory.Application.Features.Inventory.Commands.RecallBatch;

public class RecallBatchCommand : IRequest<Result<BatchDto>>
{
    public Guid BatchId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

