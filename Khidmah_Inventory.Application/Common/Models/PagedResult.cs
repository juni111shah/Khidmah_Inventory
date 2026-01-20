namespace Khidmah_Inventory.Application.Common.Models;

/// <summary>
/// Paginated result with metadata
/// </summary>
public class PagedResult<T>
{
    /// <summary>
    /// List of items
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Total number of items (before pagination)
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int PageNo { get; set; }

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageNo > 1;

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => PageNo < TotalPages;

    /// <summary>
    /// Creates a paged result from a queryable
    /// </summary>
    public static PagedResult<T> Create(IQueryable<T> query, FilterRequest filterRequest)
    {
        var pagination = filterRequest.Pagination ?? new PaginationDto { PageNo = 1, PageSize = 10 };

        // Get total count before pagination
        var totalCount = query.Count();

        // Apply pagination
        var items = query
            .Skip((pagination.PageNo - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToList();

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNo = pagination.PageNo,
            PageSize = pagination.PageSize
        };
    }
}
