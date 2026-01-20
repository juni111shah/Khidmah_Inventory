using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Common.Interfaces;

public interface ISearchService
{
    Task<SearchResult> SearchAsync(string searchTerm, List<string> entityTypes, int limit = 50);
    Task<List<SavedSearch>> GetSavedSearchesAsync();
    Task<SavedSearch> SaveSearchAsync(string name, string searchTerm, List<string> entityTypes, Dictionary<string, object> filters);
    Task DeleteSavedSearchAsync(Guid searchId);
}

public class SearchResult
{
    public List<SearchResultItem> Items { get; set; } = new();
    public int TotalCount { get; set; }
}

public class SearchResultItem
{
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Url { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class SavedSearch
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SearchTerm { get; set; } = string.Empty;
    public List<string> EntityTypes { get; set; } = new();
    public Dictionary<string, object> Filters { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

