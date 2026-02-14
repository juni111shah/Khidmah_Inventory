using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Copilot.Models;

namespace Khidmah_Inventory.Application.Features.Copilot.Commands.ExecuteCopilot;

public class ExecuteCopilotCommand : IRequest<Result<CopilotExecuteResult>>
{
    public string Input { get; set; } = string.Empty;
    public bool Confirmed { get; set; }
    public CopilotConversationState? SessionState { get; set; }
}
