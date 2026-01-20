namespace Khidmah_Inventory.Application.Common.Models;

/// <summary>
/// Common filter request for list APIs
/// </summary>
public class FilterRequest
{
    /// <summary>
    /// Pagination settings
    /// </summary>
    public PaginationDto? Pagination { get; set; }

    /// <summary>
    /// List of filters to apply
    /// </summary>
    public List<FilterDto>? Filters { get; set; }

    /// <summary>
    /// Search configuration
    /// </summary>
    public SearchRequest? Search { get; set; }
}

/// <summary>
/// Pagination settings
/// </summary>
public class PaginationDto
{
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNo { get; set; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Column name to sort by
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort order: "ascending" or "descending"
    /// </summary>
    public string? SortOrder { get; set; } = "ascending";
}

/// <summary>
/// Filter criteria
/// </summary>
public class FilterDto
{
    /// <summary>
    /// Column name to filter (supports nested properties with dot notation, e.g., "User.Email")
    /// </summary>
    public string Column { get; set; } = string.Empty;

    /// <summary>
    /// Filter operator: "=", "!=", ">", ">=", "<", "<=", "in", "equalsOrNull"
    /// </summary>
    public string Operator { get; set; } = "=";

    /// <summary>
    /// Filter value (can be string, number, boolean, or array for "in" operator)
    /// </summary>
    public object? Value { get; set; }
}

/// <summary>
/// Search configuration
/// </summary>
public class SearchRequest
{
    /// <summary>
    /// Search term
    /// </summary>
    public string Term { get; set; } = string.Empty;

    /// <summary>
    /// Fields to search in
    /// </summary>
    public List<string> SearchFields { get; set; } = new();

    /// <summary>
    /// Search mode
    /// </summary>
    public SearchMode Mode { get; set; } = SearchMode.Contains;

    /// <summary>
    /// Whether search is case sensitive
    /// </summary>
    public bool IsCaseSensitive { get; set; } = false;
}

/// <summary>
/// Search mode enumeration
/// </summary>
public enum SearchMode
{
    /// <summary>
    /// Search term appears anywhere in the field
    /// </summary>
    Contains,

    /// <summary>
    /// Field starts with search term
    /// </summary>
    StartsWith,

    /// <summary>
    /// Field ends with search term
    /// </summary>
    EndsWith,

    /// <summary>
    /// Exact match
    /// </summary>
    ExactMatch
}

