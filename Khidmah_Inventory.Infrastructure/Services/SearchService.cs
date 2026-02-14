using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Search.Models;

namespace Khidmah_Inventory.Infrastructure.Services;

public class SearchService : ISearchService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public SearchService(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<GlobalSearchResultDto> SearchGroupedAsync(string searchTerm, int limitPerGroup = 10)
    {
        var result = new GlobalSearchResultDto();
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue || string.IsNullOrWhiteSpace(searchTerm))
            return result;

        var term = searchTerm.Trim();
        var like = $"%{term}%";

        // Products: Name, SKU, Barcode
        result.Products = await _context.Products
            .AsNoTracking()
            .Where(p => p.CompanyId == companyId.Value && !p.IsDeleted &&
                (EF.Functions.Like(p.Name, like) ||
                 EF.Functions.Like(p.SKU, like) ||
                 (p.Barcode != null && EF.Functions.Like(p.Barcode, like))))
            .OrderBy(p => p.Name)
            .Take(limitPerGroup)
            .Select(p => new GlobalSearchItemDto
            {
                Id = p.Id,
                NameOrNumber = p.Name,
                Route = $"/products/{p.Id}",
                ExtraInfo = $"SKU: {p.SKU}"
            })
            .ToListAsync();

        // Customers: Name, Email, Phone
        result.Customers = await _context.Customers
            .AsNoTracking()
            .Where(c => c.CompanyId == companyId.Value && !c.IsDeleted &&
                (EF.Functions.Like(c.Name, like) ||
                 (c.Email != null && EF.Functions.Like(c.Email, like)) ||
                 (c.PhoneNumber != null && c.PhoneNumber.Contains(term))))
            .OrderBy(c => c.Name)
            .Take(limitPerGroup)
            .Select(c => new GlobalSearchItemDto
            {
                Id = c.Id,
                NameOrNumber = c.Name,
                Route = $"/customers/{c.Id}",
                ExtraInfo = c.Email ?? c.PhoneNumber
            })
            .ToListAsync();

        // Suppliers: Name, Email, Phone
        result.Suppliers = await _context.Suppliers
            .AsNoTracking()
            .Where(s => s.CompanyId == companyId.Value && !s.IsDeleted &&
                (EF.Functions.Like(s.Name, like) ||
                 (s.Email != null && EF.Functions.Like(s.Email, like)) ||
                 (s.PhoneNumber != null && s.PhoneNumber.Contains(term))))
            .OrderBy(s => s.Name)
            .Take(limitPerGroup)
            .Select(s => new GlobalSearchItemDto
            {
                Id = s.Id,
                NameOrNumber = s.Name,
                Route = $"/suppliers/{s.Id}",
                ExtraInfo = s.Email ?? s.PhoneNumber
            })
            .ToListAsync();

        // PurchaseOrders: OrderNumber, Supplier name
        result.PurchaseOrders = await _context.PurchaseOrders
            .AsNoTracking()
            .Include(po => po.Supplier)
            .Where(po => po.CompanyId == companyId.Value && !po.IsDeleted &&
                (EF.Functions.Like(po.OrderNumber, like) ||
                 EF.Functions.Like(po.Supplier.Name, like)))
            .OrderByDescending(po => po.OrderDate)
            .Take(limitPerGroup)
            .Select(po => new GlobalSearchItemDto
            {
                Id = po.Id,
                NameOrNumber = po.OrderNumber,
                Route = $"/purchase-orders/{po.Id}",
                ExtraInfo = po.Supplier.Name
            })
            .ToListAsync();

        // SalesOrders: OrderNumber, Customer name
        result.SalesOrders = await _context.SalesOrders
            .AsNoTracking()
            .Include(so => so.Customer)
            .Where(so => so.CompanyId == companyId.Value && !so.IsDeleted &&
                (EF.Functions.Like(so.OrderNumber, like) ||
                 EF.Functions.Like(so.Customer.Name, like)))
            .OrderByDescending(so => so.OrderDate)
            .Take(limitPerGroup)
            .Select(so => new GlobalSearchItemDto
            {
                Id = so.Id,
                NameOrNumber = so.OrderNumber,
                Route = $"/sales-orders/{so.Id}",
                ExtraInfo = so.Customer.Name
            })
            .ToListAsync();

        return result;
    }

    public async Task<SearchResult> SearchAsync(string searchTerm, List<string> entityTypes, int limit = 50)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return new SearchResult();

        var results = new List<SearchResultItem>();
        var searchLower = searchTerm.ToLower();

        // Search Products
        if (entityTypes.Count == 0 || entityTypes.Contains("Product"))
        {
            var products = await _context.Products
                .Where(p => p.CompanyId == companyId.Value &&
                    !p.IsDeleted &&
                    (p.Name.ToLower().Contains(searchLower) ||
                     p.SKU.ToLower().Contains(searchLower) ||
                     (p.Barcode != null && p.Barcode.ToLower().Contains(searchLower)) ||
                     (p.Description != null && p.Description.ToLower().Contains(searchLower))))
                .Take(limit)
                .Select(p => new SearchResultItem
                {
                    EntityType = "Product",
                    EntityId = p.Id,
                    Title = p.Name,
                    Description = $"SKU: {p.SKU}",
                    Url = $"/products/{p.Id}",
                    Metadata = new Dictionary<string, object>
                    {
                        { "SKU", p.SKU },
                        { "Category", p.Category != null ? p.Category.Name : "N/A" }
                    }
                })
                .ToListAsync();

            results.AddRange(products);
        }

        // Search Sales Orders
        if (entityTypes.Count == 0 || entityTypes.Contains("SalesOrder"))
        {
            var salesOrders = await _context.SalesOrders
                .Include(so => so.Customer)
                .Where(so => so.CompanyId == companyId.Value &&
                    !so.IsDeleted &&
                    (so.OrderNumber.ToLower().Contains(searchLower) ||
                     so.Customer.Name.ToLower().Contains(searchLower)))
                .Take(limit)
                .Select(so => new SearchResultItem
                {
                    EntityType = "SalesOrder",
                    EntityId = so.Id,
                    Title = so.OrderNumber,
                    Description = $"Customer: {so.Customer.Name}",
                    Url = $"/sales-orders/{so.Id}",
                    Metadata = new Dictionary<string, object>
                    {
                        { "Status", so.Status },
                        { "TotalAmount", so.TotalAmount }
                    }
                })
                .ToListAsync();

            results.AddRange(salesOrders);
        }

        // Search Purchase Orders
        if (entityTypes.Count == 0 || entityTypes.Contains("PurchaseOrder"))
        {
            var purchaseOrders = await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Where(po => po.CompanyId == companyId.Value &&
                    !po.IsDeleted &&
                    (po.OrderNumber.ToLower().Contains(searchLower) ||
                     po.Supplier.Name.ToLower().Contains(searchLower)))
                .Take(limit)
                .Select(po => new SearchResultItem
                {
                    EntityType = "PurchaseOrder",
                    EntityId = po.Id,
                    Title = po.OrderNumber,
                    Description = $"Supplier: {po.Supplier.Name}",
                    Url = $"/purchase-orders/{po.Id}",
                    Metadata = new Dictionary<string, object>
                    {
                        { "Status", po.Status },
                        { "TotalAmount", po.TotalAmount }
                    }
                })
                .ToListAsync();

            results.AddRange(purchaseOrders);
        }

        // Search Customers
        if (entityTypes.Count == 0 || entityTypes.Contains("Customer"))
        {
            var customers = await _context.Customers
                .Where(c => c.CompanyId == companyId.Value &&
                    !c.IsDeleted &&
                    (c.Name.ToLower().Contains(searchLower) ||
                     (c.Email != null && c.Email.ToLower().Contains(searchLower)) ||
                     (c.PhoneNumber != null && c.PhoneNumber.Contains(searchTerm))))
                .Take(limit)
                .Select(c => new SearchResultItem
                {
                    EntityType = "Customer",
                    EntityId = c.Id,
                    Title = c.Name,
                    Description = c.Email ?? c.PhoneNumber ?? "",
                    Url = $"/customers/{c.Id}",
                    Metadata = new Dictionary<string, object>
                    {
                        { "Email", c.Email ?? "" },
                        { "Phone", c.PhoneNumber ?? "" }
                    }
                })
                .ToListAsync();

            results.AddRange(customers);
        }

        // Search Suppliers
        if (entityTypes.Count == 0 || entityTypes.Contains("Supplier"))
        {
            var suppliers = await _context.Suppliers
                .Where(s => s.CompanyId == companyId.Value &&
                    !s.IsDeleted &&
                    (s.Name.ToLower().Contains(searchLower) ||
                     (s.Email != null && s.Email.ToLower().Contains(searchLower)) ||
                     (s.PhoneNumber != null && s.PhoneNumber.Contains(searchTerm))))
                .Take(limit)
                .Select(s => new SearchResultItem
                {
                    EntityType = "Supplier",
                    EntityId = s.Id,
                    Title = s.Name,
                    Description = s.Email ?? s.PhoneNumber ?? "",
                    Url = $"/suppliers/{s.Id}",
                    Metadata = new Dictionary<string, object>
                    {
                        { "Email", s.Email ?? "" },
                        { "Phone", s.PhoneNumber ?? "" }
                    }
                })
                .ToListAsync();

            results.AddRange(suppliers);
        }

        return new SearchResult
        {
            Items = results.Take(limit).ToList(),
            TotalCount = results.Count
        };
    }

    public async Task<List<SavedSearch>> GetSavedSearchesAsync()
    {
        // Implementation for saved searches (can use a SavedSearch entity)
        // For now, return empty list
        return new List<SavedSearch>();
    }

    public async Task<SavedSearch> SaveSearchAsync(string name, string searchTerm, List<string> entityTypes, Dictionary<string, object> filters)
    {
        // Implementation for saving searches
        // For now, return a mock saved search
        return new SavedSearch
        {
            Id = Guid.NewGuid(),
            Name = name,
            SearchTerm = searchTerm,
            EntityTypes = entityTypes,
            Filters = filters,
            CreatedAt = DateTime.UtcNow
        };
    }

    public async Task DeleteSavedSearchAsync(Guid searchId)
    {
        // Implementation for deleting saved searches
        await Task.CompletedTask;
    }
}

