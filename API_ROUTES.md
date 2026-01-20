# API Routes Documentation

Complete list of all API endpoints in the Khidmah Inventory Management System.

## Base URL
```
/api
```

## Response Format
All endpoints return standardized `ApiResponse<T>` structure:
```json
{
  "success": boolean,
  "message": string,
  "statusCode": number,
  "data": T,
  "errors": string[],
  "timestamp": string
}
```

---

## Authentication Endpoints

### POST /api/auth/login
**Description**: User login  
**Authentication**: Not required (`[AllowAnonymous]`)  
**Request Body**:
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```
**Response**: `ApiResponse<LoginResponseDto>`

### POST /api/auth/register
**Description**: Register new user  
**Authentication**: Required - Admin role only  
**Request Body**:
```json
{
  "email": "user@example.com",
  "password": "password123",
  "firstName": "John",
  "lastName": "Doe",
  "userName": "johndoe"
}
```
**Response**: `ApiResponse<UserDto>`

---

## Theme Endpoints

### GET /api/theme/user
**Description**: Get user-specific theme  
**Authentication**: Required  
**Response**: `ApiResponse<ThemeDto>`

### GET /api/theme/global
**Description**: Get global theme  
**Authentication**: Required  
**Response**: `ApiResponse<ThemeDto>`

### POST /api/theme/user
**Description**: Save user-specific theme  
**Authentication**: Required  
**Request Body**: `ThemeDto`  
**Response**: `ApiResponse<ThemeDto>`

### POST /api/theme/global
**Description**: Save global theme  
**Authentication**: Required  
**Request Body**: `ThemeDto`  
**Response**: `ApiResponse<ThemeDto>`

### POST /api/theme/logo
**Description**: Upload logo  
**Authentication**: Required  
**Request**: `multipart/form-data` with `file`  
**Response**: `ApiResponse<{ logoUrl: string }>`

---

## Settings Endpoints

### Company Settings

#### GET /api/settings/company
**Description**: Get company settings  
**Authentication**: Required  
**Response**: `ApiResponse<CompanySettingsDto>`

#### POST /api/settings/company
**Description**: Save company settings  
**Authentication**: Required  
**Request Body**: `SaveCompanySettingsCommand`  
**Response**: `ApiResponse<CompanySettingsDto>`

### User Settings

#### GET /api/settings/user
**Description**: Get current user settings  
**Authentication**: Required  
**Response**: `ApiResponse<UserSettingsDto>`

#### POST /api/settings/user
**Description**: Save current user settings  
**Authentication**: Required  
**Request Body**: `SaveUserSettingsCommand`  
**Response**: `ApiResponse<UserSettingsDto>`

### System Settings

#### GET /api/settings/system
**Description**: Get system settings  
**Authentication**: Required  
**Response**: `ApiResponse<SystemSettingsDto>`

#### POST /api/settings/system
**Description**: Save system settings  
**Authentication**: Required  
**Request Body**: `SaveSystemSettingsCommand`  
**Response**: `ApiResponse<SystemSettingsDto>`

### Notification Settings

#### GET /api/settings/notifications
**Description**: Get notification settings  
**Authentication**: Required  
**Response**: `ApiResponse<NotificationSettingsDto>`

#### POST /api/settings/notifications
**Description**: Save notification settings  
**Authentication**: Required  
**Request Body**: `SaveNotificationSettingsCommand`  
**Response**: `ApiResponse<NotificationSettingsDto>`

### UI Settings

#### GET /api/settings/ui
**Description**: Get UI settings  
**Authentication**: Required  
**Response**: `ApiResponse<UISettingsDto>`

#### POST /api/settings/ui
**Description**: Save UI settings  
**Authentication**: Required  
**Request Body**: `SaveUISettingsCommand`  
**Response**: `ApiResponse<UISettingsDto>`

### Report Settings

#### GET /api/settings/reports
**Description**: Get report settings  
**Authentication**: Required  
**Response**: `ApiResponse<ReportSettingsDto>`

#### POST /api/settings/reports
**Description**: Save report settings  
**Authentication**: Required  
**Request Body**: `SaveReportSettingsCommand`  
**Response**: `ApiResponse<ReportSettingsDto>`

---

## Products Endpoints (Placeholder)

### GET /api/products
**Description**: Get products list  
**Authentication**: Required  
**Status**: To be implemented

### GET /api/products/{id}
**Description**: Get product by ID  
**Authentication**: Required  
**Status**: To be implemented

### POST /api/products
**Description**: Create product  
**Authentication**: Required  
**Status**: To be implemented

### PUT /api/products/{id}
**Description**: Update product  
**Authentication**: Required  
**Status**: To be implemented

### DELETE /api/products/{id}
**Description**: Delete product  
**Authentication**: Required  
**Status**: To be implemented

---

## Roles Endpoints

### GET /api/roles
**Description**: Get roles list  
**Authentication**: Required  
**Permission**: `Roles:List`  
**Response**: `ApiResponse<RoleDto[]>`

### GET /api/roles/{id}
**Description**: Get role by ID  
**Authentication**: Required  
**Permission**: `Roles:Read`  
**Response**: `ApiResponse<RoleDto>`

### POST /api/roles
**Description**: Create role  
**Authentication**: Required  
**Permission**: `Roles:Create`  
**Request Body**:
```json
{
  "name": "Manager",
  "description": "Manager role",
  "permissionIds": ["guid1", "guid2"]
}
```
**Response**: `ApiResponse<RoleDto>`

### PUT /api/roles/{id}
**Description**: Update role  
**Authentication**: Required  
**Permission**: `Roles:Update`  
**Request Body**: `UpdateRoleRequest`  
**Response**: `ApiResponse<RoleDto>`

### DELETE /api/roles/{id}
**Description**: Delete role  
**Authentication**: Required  
**Permission**: `Roles:Delete`  
**Response**: `ApiResponse<void>`

### POST /api/roles/{roleId}/assign-user/{userId}
**Description**: Assign role to user  
**Authentication**: Required  
**Permission**: `Roles:Assign`  
**Response**: `ApiResponse<void>`

### DELETE /api/roles/{roleId}/remove-user/{userId}
**Description**: Remove role from user  
**Authentication**: Required  
**Permission**: `Roles:Assign`  
**Response**: `ApiResponse<void>`

---

## Permissions Endpoints

### GET /api/permissions
**Description**: Get permissions list  
**Authentication**: Required  
**Permission**: `Permissions:Read`  
**Query Parameters**: `?module=Users` (optional)  
**Response**: `ApiResponse<PermissionDto[]>`

---

## Companies Endpoints (Placeholder)

### GET /api/companies
**Description**: Get companies list  
**Authentication**: Required  
**Status**: To be implemented

### GET /api/companies/{id}
**Description**: Get company by ID  
**Authentication**: Required  
**Status**: To be implemented

### POST /api/companies
**Description**: Create company  
**Authentication**: Required  
**Status**: To be implemented

### PUT /api/companies/{id}
**Description**: Update company  
**Authentication**: Required  
**Status**: To be implemented

---

## HTTP Status Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 201 | Created |
| 400 | Bad Request (validation errors, business logic errors) |
| 401 | Unauthorized (authentication required) |
| 403 | Forbidden (insufficient permissions) |
| 404 | Not Found |
| 500 | Internal Server Error |

---

## Authentication

Most endpoints require JWT authentication. Include the token in the Authorization header:
```
Authorization: Bearer {token}
```

Public endpoints (no authentication):
- `POST /api/auth/login`

---

## Multi-Tenancy

All authenticated endpoints automatically filter data by `CompanyId` from:
1. JWT token claim (`CompanyId`)
2. HTTP header (`X-Company-Id`)

---

## Filtering & Pagination

List endpoints support filtering, searching, sorting, and pagination via `FilterRequest`:

**Request Body**:
```json
{
  "pagination": {
    "pageNo": 1,
    "pageSize": 10,
    "sortBy": "CreatedAt",
    "sortOrder": "ascending"
  },
  "filters": [
    {
      "column": "Email",
      "operator": "=",
      "value": "user@example.com"
    }
  ],
  "Search": {
    "Term": "search term",
    "SearchFields": ["Name", "Email"],
    "Mode": "Contains",
    "IsCaseSensitive": false
  }
}
```

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0.0 | 2024-01-15 | Initial API routes documentation |

---

## Notes

- All endpoints use CQRS pattern via MediatR
- All endpoints return standardized `ApiResponse<T>` structure
- All endpoints support multi-tenancy
- All endpoints (except login) require authentication
- Endpoints marked as "Placeholder" are to be implemented

