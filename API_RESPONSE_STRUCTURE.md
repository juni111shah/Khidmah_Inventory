# API Response Structure

This document defines the standardized API response structure used across all endpoints in the Khidmah Inventory Management System.

## Standard Response Format

All API endpoints return a consistent response structure to ensure predictable client-side handling and better error management.

### Success Response with Data

```json
{
  "success": true,
  "message": "Permission retrieved successfully.",
  "statusCode": 200,
  "data": {
    "id": 211,
    "name": "Analytics:Export",
    "description": "Export analytics",
    "module": "Analytics",
    "permissionType": "Controller",
    "isActive": false,
    "isSystemPermission": false,
    "order": 0,
    "isDeleted": false,
    "createdOn": "2025-11-14T11:02:17.000Z"
  },
  "errors": [],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Success Response without Data

```json
{
  "success": true,
  "message": "Operation completed successfully",
  "statusCode": 200,
  "errors": [],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Error Response

```json
{
  "success": false,
  "message": "Operation failed",
  "statusCode": 400,
  "data": null,
  "errors": [
    "Error message 1",
    "Error message 2"
  ],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Not Found Response

```json
{
  "success": false,
  "message": "Resource not found",
  "statusCode": 404,
  "data": null,
  "errors": [
    "The requested resource was not found"
  ],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## Response Properties

| Property | Type | Description |
|----------|------|-------------|
| `success` | `boolean` | Indicates whether the operation was successful |
| `message` | `string` | Human-readable message describing the result |
| `statusCode` | `integer` | HTTP status code (200, 201, 400, 404, etc.) |
| `data` | `T` or `null` | Response data (null if operation failed) |
| `errors` | `string[]` | Array of error messages (empty if operation succeeded) |
| `timestamp` | `DateTime` | UTC timestamp of the response |

## HTTP Status Codes

The API uses standard HTTP status codes in combination with the response structure:

- **200 OK**: Operation succeeded
- **201 Created**: Resource created successfully (with `HandleResult(result, 201)`)
- **400 Bad Request**: Operation failed (validation errors, business logic errors)
- **404 Not Found**: Resource not found
- **401 Unauthorized**: Authentication required
- **403 Forbidden**: Insufficient permissions
- **500 Internal Server Error**: Server error (handled by exception middleware)

## Usage in Controllers

### Using BaseApiController

All controllers should inherit from `BaseApiController` which provides helper methods:

```csharp
public class ProductController : BaseApiController
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var query = new GetProductQuery { Id = id };
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Product created successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var command = new DeleteProductCommand { Id = id };
        var result = await Mediator.Send(command);
        return HandleResult(result, "Product deleted successfully");
    }
}
```

### Custom Messages

You can provide custom success messages:

```csharp
return HandleResult(result, "Product created successfully");
return HandleResult(result, 201, "Product created successfully"); // With custom status code
```

### Direct Response Methods

For special cases, you can use direct response methods:

```csharp
// Success with data
return SuccessResponse(productDto, "Product retrieved successfully");

// Success without data
return SuccessResponse("Product deleted successfully");

// Failure
return FailureResponse("Validation failed", "Name is required", "SKU is required");

// Not found
return NotFoundResponse("Product not found");
```

## Examples

### Example 1: Get Product (Success)

**Request:**
```
GET /api/products/123e4567-e89b-12d3-a456-426614174000
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Product retrieved successfully.",
  "statusCode": 200,
  "data": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "name": "Sample Product",
    "sku": "PROD-001",
    "price": 99.99
  },
  "errors": [],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Example 2: Get Product (Not Found)

**Request:**
```
GET /api/products/00000000-0000-0000-0000-000000000000
```

**Response (404 Not Found):**
```json
{
  "success": false,
  "message": "Resource not found",
  "statusCode": 404,
  "data": null,
  "errors": [
    "The requested resource was not found"
  ],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Example 3: Create Product (Validation Error)

**Request:**
```
POST /api/products
Content-Type: application/json

{
  "name": "",
  "sku": "",
  "price": -10
}
```

**Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "Operation failed",
  "statusCode": 400,
  "data": null,
  "errors": [
    "Product name is required",
    "SKU is required",
    "Price must be greater than 0"
  ],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Example 4: Create Product (Success)

**Request:**
```
POST /api/products
Content-Type: application/json

{
  "name": "New Product",
  "sku": "PROD-002",
  "price": 149.99
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Product created successfully",
  "statusCode": 200,
  "data": {
    "id": "456e7890-e89b-12d3-a456-426614174001",
    "name": "New Product",
    "sku": "PROD-002",
    "price": 149.99,
    "createdAt": "2024-01-15T10:30:00Z"
  },
  "errors": [],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Example 5: Delete Product (Success)

**Request:**
```
DELETE /api/products/123e4567-e89b-12d3-a456-426614174000
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Product deleted successfully",
  "statusCode": 200,
  "errors": [],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## Frontend Integration

### TypeScript Interface

```typescript
interface ApiResponse<T> {
  success: boolean;
  message: string;
  statusCode: number;
  data?: T;
  errors: string[];
  timestamp: string;
}
```

### Handling Responses

```typescript
// Success response
if (response.success) {
  const data = response.data;
  // Handle success
} else {
  // Handle errors
  const errors = response.errors;
  console.error('Errors:', errors);
}

// Example service method
async getProduct(id: string): Promise<Product> {
  const response = await this.http.get<ApiResponse<Product>>(`/api/products/${id}`).toPromise();
  
  if (response?.success && response.data) {
    return response.data;
  }
  
  throw new Error(response?.errors.join(', ') || 'Failed to get product');
}
```

## Benefits

1. **Consistency**: All endpoints return the same structure
2. **Predictability**: Clients know what to expect
3. **Error Handling**: Centralized error information
4. **Debugging**: Timestamp helps with debugging and logging
5. **Type Safety**: Strongly typed responses in TypeScript
6. **Documentation**: Clear structure for API documentation

## Implementation Details

### ApiResponse Model

Located in `Khidmah_Inventory.API/Models/ApiResponse.cs`:

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; }
    public DateTime Timestamp { get; set; }
}
```

### BaseApiController Methods

- `HandleResult<T>(Result<T> result, string? successMessage = null)`: Handles Result<T> from MediatR
- `HandleResult(Result result, string? successMessage = null)`: Handles Result from MediatR
- `HandleResult<T>(Result<T> result, int successStatusCode, string? successMessage = null)`: With custom status code
- `SuccessResponse<T>(T data, string message)`: Direct success response
- `SuccessResponse(string message)`: Success without data
- `FailureResponse(string message, params string[] errors)`: Failure response
- `NotFoundResponse(string message)`: Not found response

## Migration Guide

If you have existing endpoints that don't use the standard response structure:

1. **Inherit from BaseApiController**:
   ```csharp
   public class YourController : BaseApiController
   ```

2. **Replace direct returns**:
   ```csharp
   // Before
   return Ok(result.Data);
   return BadRequest(new { errors = result.Errors });
   
   // After
   return HandleResult(result);
   ```

3. **Update frontend code** to handle the new response structure

## Summary

- ✅ All endpoints return consistent `ApiResponse<T>` structure
- ✅ Success and error responses follow the same format
- ✅ HTTP status codes used appropriately
- ✅ Timestamp included for debugging
- ✅ Easy to use via `BaseApiController` helper methods
- ✅ Type-safe responses for frontend integration

