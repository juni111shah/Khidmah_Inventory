using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.AutonomousWarehouse.Commands.CompleteWorkTask;

public class CompleteWorkTaskCommand : IRequest<Result>
{
    public Guid TaskId { get; set; }
    public string? Notes { get; set; }
}
