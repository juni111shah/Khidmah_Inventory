using Khidmah_Inventory.Domain.Enums;

namespace Khidmah_Inventory.Application.Common.Interfaces;

/// <summary>
/// Abstraction for any agent that can execute warehouse operations (human worker or robot).
/// Future hardware can connect via API by implementing this contract.
/// </summary>
public interface IOperationAgent
{
    Guid AgentId { get; }
    OperationAgentType Type { get; }
    string DisplayName { get; }
    bool IsAvailable { get; }
    Guid? CurrentWarehouseId { get; }
    /// <summary>Current position (x,y) for routing; null if unknown.</summary>
    (decimal X, decimal Y)? CurrentPosition { get; }
}
