# User Flow Implementation - Complete Backend to Frontend

This document describes the complete implementation of the user management flow from backend to frontend.

## Overview

The user flow includes:
- **Backend**: CQRS commands/queries, repository pattern, API controllers
- **Frontend**: Angular components, services, models, and routing

## Backend Implementation

### 1. Domain Layer
- **User Entity**: Already exists in `Khidmah_Inventory.Domain/Entities/User.cs`
  - Properties: Email, UserName, FirstName, LastName, PhoneNumber, IsActive, etc.
  - Methods: UpdateProfile, ChangePassword, Activate, Deactivate, etc.

### 2. Application Layer

#### DTOs
- **Location**: `Khidmah_Inventory.Application/Features/Users/Models/UserDto.cs`
- **UserDto**: Complete user information including roles, permissions, companies
- **CompanyDto**: Company information for user-company relationships

#### Repository Interface
- **Location**: `Khidmah_Inventory.Application/Common/Interfaces/IUserRepository.cs`
- Methods: GetByIdAsync, GetByEmailAsync, GetUsersByCompanyIdAsync, AddAsync, UpdateAsync

#### CQRS Queries

1. **GetUserQuery**
   - **Location**: `Khidmah_Inventory.Application/Features/Users/Queries/GetUser/`
   - Gets a specific user by ID
   - Includes roles, permissions, and companies
   - Filters by company context

2. **GetUsersListQuery**
   - **Location**: `Khidmah_Inventory.Application/Features/Users/Queries/GetUsersList/`
   - Gets paginated list of users
   - Supports filtering, searching, and sorting via `FilterRequest`
   - Filters by company context

3. **GetCurrentUserQuery**
   - **Location**: `Khidmah_Inventory.Application/Features/Users/Queries/GetCurrentUser/`
   - Gets the currently authenticated user
   - Includes all roles, permissions, and companies

#### CQRS Commands

1. **UpdateUserProfileCommand**
   - **Location**: `Khidmah_Inventory.Application/Features/Users/Commands/UpdateUserProfile/`
   - Updates user profile (FirstName, LastName, PhoneNumber)
   - Users can update their own profile, admins can update any user in their company
   - Includes FluentValidation

2. **ChangePasswordCommand**
   - **Location**: `Khidmah_Inventory.Application/Features/Users/Commands/ChangePassword/`
   - Changes user password
   - Verifies current password (unless admin)
   - Includes FluentValidation

3. **ActivateUserCommand**
   - **Location**: `Khidmah_Inventory.Application/Features/Users/Commands/ActivateUser/`
   - Activates a user account
   - Admin only

4. **DeactivateUserCommand**
   - **Location**: `Khidmah_Inventory.Application/Features/Users/Commands/DeactivateUser/`
   - Deactivates a user account
   - Admin only
   - Prevents deactivating own account

### 3. Infrastructure Layer

#### Repository Implementation
- **Location**: `Khidmah_Inventory.Infrastructure/Repositories/UserRepository.cs`
- Implements `IUserRepository`
- Uses EF Core with includes for related entities
- Registered in `DependencyInjection.cs`

### 4. API Layer

#### UsersController
- **Location**: `Khidmah_Inventory.API/Controllers/UsersController.cs`
- **Endpoints**:
  - `GET /api/users/current` - Get current user
  - `GET /api/users/{id}` - Get user by ID
  - `POST /api/users/list` - Get users list (with filtering)
  - `PUT /api/users/{id}/profile` - Update user profile
  - `POST /api/users/{id}/change-password` - Change password
  - `POST /api/users/{id}/activate` - Activate user (Admin only)
  - `POST /api/users/{id}/deactivate` - Deactivate user (Admin only)

All endpoints:
- Use `BaseApiController` for consistent responses
- Return standardized `ApiResponse<T>` structure
- Support multi-tenancy (filter by CompanyId)
- Require authentication (except login)

## Frontend Implementation

### 1. Models
- **Location**: `khidmah_inventory.client/src/app/core/models/user.model.ts`
- **Interfaces**:
  - `User`: Complete user model
  - `Company`: Company model
  - `UpdateUserProfileRequest`: Profile update request
  - `ChangePasswordRequest`: Password change request
  - `PagedResult<T>`: Paginated results
  - `FilterRequest`: Filtering, searching, sorting

### 2. Services
- **Location**: `khidmah_inventory.client/src/app/core/services/user-api.service.ts`
- **UserApiService**: Angular service for all user API calls
- Methods:
  - `getCurrentUser()`: Get current authenticated user
  - `getUser(id)`: Get user by ID
  - `getUsers(filterRequest)`: Get paginated users list
  - `updateProfile(id, profile)`: Update user profile
  - `changePassword(id, passwords)`: Change password
  - `activateUser(id)`: Activate user
  - `deactivateUser(id)`: Deactivate user

### 3. Components

#### UsersListComponent
- **Location**: `khidmah_inventory.client/src/app/features/users/users-list/`
- **Features**:
  - Display users in a table
  - Search functionality
  - Pagination
  - View/Edit user actions
  - Activate/Deactivate user actions
  - Loading states
  - Toast notifications

#### UserProfileComponent
- **Location**: `khidmah_inventory.client/src/app/features/users/user-profile/`
- **Features**:
  - Display user information
  - Edit profile form (FirstName, LastName, PhoneNumber)
  - Change password form
  - Tab-based navigation
  - Loading states
  - Toast notifications
  - Form validation

### 4. Routing
- **Location**: `khidmah_inventory.client/src/app/app-routing.module.ts`
- **Routes**:
  - `/users` - Users list
  - `/users/:id` - User profile
  - `/users/:id/edit` - User profile (edit mode)

## Features

### Multi-Tenancy
- All queries filter by `CompanyId` from JWT token
- Users can only see users in their company
- Admins can manage users in their company

### Authorization
- Most endpoints require authentication
- Activate/Deactivate endpoints require Admin role
- Users can update their own profile
- Admins can update any user in their company

### Validation
- FluentValidation on backend commands
- Form validation on frontend
- Password strength requirements
- Phone number format validation

### Error Handling
- Consistent `Result<T>` pattern on backend
- Standardized `ApiResponse<T>` on API
- Toast notifications on frontend
- Loading states for async operations

## API Response Format

All endpoints return standardized `ApiResponse<T>`:

```json
{
  "success": true,
  "message": "Operation completed successfully",
  "statusCode": 200,
  "data": { ... },
  "errors": [],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## Usage Examples

### Get Current User
```typescript
this.userApiService.getCurrentUser().subscribe({
  next: (response) => {
    if (response.success && response.data) {
      this.user = response.data;
    }
  }
});
```

### Get Users List with Filtering
```typescript
const filterRequest: FilterRequest = {
  pagination: {
    pageNo: 1,
    pageSize: 10,
    sortBy: 'FirstName',
    sortOrder: 'ascending'
  },
  search: {
    term: 'john',
    searchFields: ['FirstName', 'LastName', 'Email'],
    mode: SearchMode.Contains,
    isCaseSensitive: false
  }
};

this.userApiService.getUsers(filterRequest).subscribe({
  next: (response) => {
    if (response.success && response.data) {
      this.users = response.data.items;
      this.pagedResult = response.data;
    }
  }
});
```

### Update Profile
```typescript
const profile: UpdateUserProfileRequest = {
  firstName: 'John',
  lastName: 'Doe',
  phoneNumber: '+1234567890'
};

this.userApiService.updateProfile(userId, profile).subscribe({
  next: (response) => {
    if (response.success) {
      // Show success message
    }
  }
});
```

## Testing Checklist

- [x] Get current user
- [x] Get user by ID
- [x] Get users list with pagination
- [x] Search users
- [x] Filter users
- [x] Update user profile
- [x] Change password
- [x] Activate user (Admin only)
- [x] Deactivate user (Admin only)
- [x] Prevent deactivating own account
- [x] Multi-tenancy: Users can only see users in their company
- [x] Frontend: Users list displays correctly
- [x] Frontend: User profile form works
- [x] Frontend: Password change form works
- [x] Frontend: Toast notifications work
- [x] Frontend: Loading states work

## Next Steps

1. Add user creation (if needed)
2. Add user deletion (soft delete)
3. Add role assignment UI
4. Add company assignment UI
5. Add email confirmation flow
6. Add user activity logs
7. Add export functionality

## Files Created/Modified

### Backend
- `Khidmah_Inventory.Application/Features/Users/Models/UserDto.cs`
- `Khidmah_Inventory.Application/Common/Interfaces/IUserRepository.cs`
- `Khidmah_Inventory.Infrastructure/Repositories/UserRepository.cs`
- `Khidmah_Inventory.Application/Features/Users/Queries/GetUser/`
- `Khidmah_Inventory.Application/Features/Users/Queries/GetUsersList/`
- `Khidmah_Inventory.Application/Features/Users/Queries/GetCurrentUser/`
- `Khidmah_Inventory.Application/Features/Users/Commands/UpdateUserProfile/`
- `Khidmah_Inventory.Application/Features/Users/Commands/ChangePassword/`
- `Khidmah_Inventory.Application/Features/Users/Commands/ActivateUser/`
- `Khidmah_Inventory.Application/Features/Users/Commands/DeactivateUser/`
- `Khidmah_Inventory.API/Controllers/UsersController.cs`
- `Khidmah_Inventory.Infrastructure/DependencyInjection.cs` (modified)

### Frontend
- `khidmah_inventory.client/src/app/core/models/user.model.ts`
- `khidmah_inventory.client/src/app/core/services/user-api.service.ts`
- `khidmah_inventory.client/src/app/features/users/users-list/`
- `khidmah_inventory.client/src/app/features/users/user-profile/`
- `khidmah_inventory.client/src/app/app-routing.module.ts` (modified)

## Summary

The complete user flow has been implemented following Clean Architecture, CQRS, and Repository patterns. The implementation includes:

✅ **Backend**: Complete CQRS implementation with queries and commands
✅ **Frontend**: Angular components with proper service layer
✅ **Multi-Tenancy**: Company-based filtering
✅ **Authorization**: Role-based access control
✅ **Validation**: Both backend and frontend validation
✅ **Error Handling**: Consistent error handling across layers
✅ **UI/UX**: Modern, responsive components with loading states and notifications

The implementation is production-ready and follows all coding standards and best practices.

