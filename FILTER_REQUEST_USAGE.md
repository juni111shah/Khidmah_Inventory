# Filter Request Usage Guide

This document explains how to use the common filter request system for list APIs across the application.

## Overview

The filter request system provides a standardized way to handle:
- **Pagination**: Page number, page size, sorting
- **Filtering**: Column-based filters with various operators
- **Search**: Full-text search across multiple fields

## Request Structure

```json
{
    "pagination": {
        "pageNo": 1,
        "pageSize": 10,
        "sortBy": "CreatedOn",
        "sortOrder": "ascending"
    },
    "filters": [
        {
            "column": "Email",
            "operator": "=",
            "value": "junaid"
        }
    ],
    "Search": {
        "Term": "a",
        "SearchFields": [
            "Email",
            "FullName"
        ],
        "Mode": "Contains",
        "IsCaseSensitive": false
    }
}
```

## Filter Operators

| Operator | Description | Example |
|----------|-------------|---------|
| `=` or `equals` | Equal to | `{"column": "Status", "operator": "=", "value": "Active"}` |
| `!=` or `notequals` | Not equal to | `{"column": "Status", "operator": "!=", "value": "Deleted"}` |
| `>` | Greater than | `{"column": "Price", "operator": ">", "value": "100"}` |
| `>=` | Greater than or equal | `{"column": "Price", "operator": ">=", "value": "100"}` |
| `<` | Less than | `{"column": "Price", "operator": "<", "value": "1000"}` |
| `<=` | Less than or equal | `{"column": "Price", "operator": "<=", "value": "1000"}` |
| `in` | Value in list | `{"column": "Status", "operator": "in", "value": "Active,Inactive"}` |
| `equalsOrNull` | Equal to value OR null | `{"column": "DeletedAt", "operator": "equalsOrNull", "value": null}` |

## Search Modes

| Mode | Description |
|------|-------------|
| `Contains` | Search term appears anywhere in the field |
| `StartsWith` | Field starts with search term |
| `EndsWith` | Field ends with search term |
| `ExactMatch` | Exact match (case sensitive or not) |

## Implementation Example

### 1. Create Query

```csharp
// Application/Features/Users/Queries/GetUserList/GetUserListQuery.cs
using Khidmah_Inventory.Application.Common.Models;
using MediatR;

namespace Khidmah_Inventory.Application.Features.Users.Queries.GetUserList;

public class GetUserListQuery : IRequest<Result<PagedResult<UserDto>>>
{
    public FilterRequest? FilterRequest { get; set; }
}
```

### 2. Create Handler

```csharp
// Application/Features/Users/Queries/GetUserList/GetUserListQueryHandler.cs
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Domain.Entities;
using Khidmah_Inventory.Infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khidmah_Inventory.Application.Features.Users.Queries.GetUserList;

public class GetUserListQueryHandler : IRequestHandler<GetUserListQuery, Result<PagedResult<UserDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<GetUserListQueryHandler> _logger;

    public GetUserListQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        ILogger<GetUserListQueryHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<PagedResult<UserDto>>> Handle(
        GetUserListQuery request,
        CancellationToken cancellationToken)
    {
        // Start with base query - always filter by CompanyId for multi-tenancy
        var query = _context.Users
            .Where(u => u.CompanyId == _currentUser.CompanyId && !u.IsDeleted)
            .AsQueryable();

        // Apply filter request (filters, search, sorting)
        var filterRequest = request.FilterRequest ?? new FilterRequest
        {
            Pagination = new PaginationDto { PageNo = 1, PageSize = 10 }
        };

        // Apply filters, search, and sorting
        query = query.ApplyFilterRequest(filterRequest);

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var pagination = filterRequest.Pagination ?? new PaginationDto { PageNo = 1, PageSize = 10 };
        var users = await query
            .Skip((pagination.PageNo - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var result = new PagedResult<UserDto>
        {
            Items = users,
            TotalCount = totalCount,
            PageNo = pagination.PageNo,
            PageSize = pagination.PageSize
        };

        _logger.LogInformation("Retrieved {Count} users", users.Count);
        return Result<PagedResult<UserDto>>.Success(result);
    }
}
```

### 3. Create Controller

```csharp
// API/Controllers/UsersController.cs
using Khidmah_Inventory.Application.Features.Users.Queries.GetUserList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khidmah_Inventory.API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    [HttpPost("list")]
    public async Task<IActionResult> GetUserList([FromBody] GetUserListQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Users retrieved successfully");
    }

    // Alternative: GET with query parameters
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] FilterRequest? filterRequest)
    {
        var query = new GetUserListQuery { FilterRequest = filterRequest };
        var result = await Mediator.Send(query);
        return HandleResult(result, "Users retrieved successfully");
    }
}
```

## Usage Examples

### Example 1: Simple Pagination

**Request:**
```json
POST /api/users/list
{
    "filterRequest": {
        "pagination": {
            "pageNo": 1,
            "pageSize": 20,
            "sortBy": "CreatedAt",
            "sortOrder": "descending"
        }
    }
}
```

### Example 2: Filtering

**Request:**
```json
POST /api/users/list
{
    "filterRequest": {
        "pagination": {
            "pageNo": 1,
            "pageSize": 10
        },
        "filters": [
            {
                "column": "IsActive",
                "operator": "=",
                "value": "true"
            },
            {
                "column": "CreatedAt",
                "operator": ">=",
                "value": "2024-01-01"
            }
        ]
    }
}
```

### Example 3: Search

**Request:**
```json
POST /api/users/list
{
    "filterRequest": {
        "pagination": {
            "pageNo": 1,
            "pageSize": 10
        },
        "Search": {
            "Term": "john",
            "SearchFields": ["Email", "FirstName", "LastName"],
            "Mode": "Contains",
            "IsCaseSensitive": false
        }
    }
}
```

### Example 4: Combined Filters and Search

**Request:**
```json
POST /api/users/list
{
    "filterRequest": {
        "pagination": {
            "pageNo": 1,
            "pageSize": 10,
            "sortBy": "Email",
            "sortOrder": "ascending"
        },
        "filters": [
            {
                "column": "IsActive",
                "operator": "=",
                "value": "true"
            }
        ],
        "Search": {
            "Term": "admin",
            "SearchFields": ["Email", "FirstName"],
            "Mode": "Contains",
            "IsCaseSensitive": false
        }
    }
}
```

### Example 5: IN Operator

**Request:**
```json
POST /api/users/list
{
    "filterRequest": {
        "pagination": {
            "pageNo": 1,
            "pageSize": 10
        },
        "filters": [
            {
                "column": "Status",
                "operator": "in",
                "value": "Active,Inactive,Pending"
            }
        ]
    }
}
```

### Example 6: Nested Properties

**Request:**
```json
POST /api/orders/list
{
    "filterRequest": {
        "pagination": {
            "pageNo": 1,
            "pageSize": 10
        },
        "filters": [
            {
                "column": "Customer.Email",
                "operator": "=",
                "value": "customer@example.com"
            }
        ]
    }
}
```

## Response Format

```json
{
    "success": true,
    "message": "Users retrieved successfully",
    "statusCode": 200,
    "data": {
        "items": [
            {
                "id": "123e4567-e89b-12d3-a456-426614174000",
                "email": "user@example.com",
                "firstName": "John",
                "lastName": "Doe",
                "isActive": true,
                "createdAt": "2024-01-15T10:30:00Z"
            }
        ],
        "totalCount": 100,
        "pageNo": 1,
        "pageSize": 10,
        "totalPages": 10,
        "hasPreviousPage": false,
        "hasNextPage": true
    },
    "errors": [],
    "timestamp": "2024-01-15T10:30:00Z"
}
```

## Best Practices

1. **Always filter by CompanyId**: For multi-tenant applications, always filter by CompanyId first
2. **Always check IsDeleted**: Filter out soft-deleted records
3. **Use Select for DTOs**: Use `.Select()` to map to DTOs before pagination for better performance
4. **Get total count before pagination**: Count before applying Skip/Take
5. **Default pagination**: Always provide default pagination if not specified
6. **Validate filter columns**: Consider validating that filter columns exist (optional)

## Performance Considerations

- **Indexes**: Ensure database indexes on frequently filtered/sorted columns
- **Select projection**: Use `.Select()` to only fetch needed fields
- **Count optimization**: For large datasets, consider caching or approximate counts
- **Search fields**: Limit search to indexed string columns

## TypeScript Interface

```typescript
interface FilterRequest {
  pagination?: {
    pageNo: number;
    pageSize: number;
    sortBy?: string;
    sortOrder?: "ascending" | "descending";
  };
  filters?: Array<{
    column: string;
    operator: "=" | "!=" | ">" | ">=" | "<" | "<=" | "in" | "equalsOrNull";
    value: any;
  }>;
  Search?: {
    Term: string;
    SearchFields: string[];
    Mode: "Contains" | "StartsWith" | "EndsWith" | "ExactMatch";
    IsCaseSensitive: boolean;
  };
}

interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNo: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
```

## Summary

✅ **Standardized filtering**: Consistent filter structure across all list APIs  
✅ **Flexible operators**: Support for various comparison operators  
✅ **Full-text search**: Search across multiple fields with different modes  
✅ **Pagination**: Built-in pagination with metadata  
✅ **Sorting**: Sort by any column in ascending/descending order  
✅ **Nested properties**: Support for filtering on nested properties  
✅ **Type-safe**: Strongly typed DTOs and extensions  

