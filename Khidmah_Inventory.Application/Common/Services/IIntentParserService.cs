using Khidmah_Inventory.Application.Features.Copilot.Models;

namespace Khidmah_Inventory.Application.Common.Services;

/// <summary>
/// Parses natural language intent into commands/queries (rule-based; replaceable with AI later).
/// </summary>
public interface IIntentParserService
{
    /// <summary>
    /// Parses user input (voice or text) and returns intent + suggested action.
    /// </summary>
    Task<CopilotIntentResult> ParseAsync(string input, CancellationToken cancellationToken = default);
}
