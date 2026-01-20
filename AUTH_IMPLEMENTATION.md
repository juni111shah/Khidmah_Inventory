# Complete Authentication Flow Implementation

This document describes the complete authentication flow from backend to frontend with JWT tokens, refresh tokens, and permission-based access control.

## Overview

The authentication system provides:
- **Backend**: Login, Register, RefreshToken, Logout commands
- **Frontend**: Login/Register components, AuthService, HTTP Interceptor, Guards
- **Security**: JWT tokens with refresh token mechanism
- **Integration**: Automatic token injection, token refresh, permission management

## Backend Implementation

### 1. Domain Layer

#### User Entity
- **Location**: `Khidmah_Inventory.Domain/Entities/User.cs`
- **Methods**:
  - `SetRefreshToken()` - Set refresh token and expiry
  - `ClearRefreshToken()` - Clear refresh token on logout
  - `RecordLogin()` - Record last login time

### 2. Application Layer

#### Commands

1. **LoginCommand**
   - **Location**: `Khidmah_Inventory.Application/Features/Auth/Commands/Login/`
   - **Request**: Email, Password
   - **Response**: Token, RefreshToken, ExpiresAt, User (with roles and permissions)
   - **Handler**: 
     - Validates credentials
     - Gets user roles and permissions for default company
     - Generates JWT token with claims
     - Generates refresh token
     - Records login time

2. **RegisterCommand**
   - **Location**: `Khidmah_Inventory.Application/Features/Auth/Commands/Register/`
   - **Request**: Email, UserName, Password, FirstName, LastName, PhoneNumber, CompanyId
   - **Response**: UserId, Email, UserName
   - **Handler**:
     - Validates email/username uniqueness
     - Validates company exists
     - Hashes password
     - Creates user and user-company relationship

3. **RefreshTokenCommand**
   - **Location**: `Khidmah_Inventory.Application/Features/Auth/Commands/RefreshToken/`
   - **Request**: Token, RefreshToken
   - **Response**: New Token, New RefreshToken, ExpiresAt
   - **Handler**:
     - Validates refresh token
     - Checks expiry
     - Generates new tokens
     - Updates refresh token in database

4. **LogoutCommand**
   - **Location**: `Khidmah_Inventory.Application/Features/Auth/Commands/Logout/`
   - **Request**: None (uses current user from context)
   - **Response**: Success/Failure
   - **Handler**:
     - Clears refresh token from database

### 3. API Layer

#### AuthController
- **Location**: `Khidmah_Inventory.API/Controllers/AuthController.cs`
- **Endpoints**:
  - `POST /api/auth/login` - Public (AllowAnonymous)
  - `POST /api/auth/register` - Requires `Auth:Create` permission
  - `POST /api/auth/refresh-token` - Public (AllowAnonymous)
  - `POST /api/auth/logout` - Requires authentication

## Frontend Implementation

### 1. Models

#### Auth Models
- **Location**: `khidmah_inventory.client/src/app/core/models/auth.model.ts`
- **Interfaces**:
  - `LoginRequest`, `LoginResponse`
  - `RegisterRequest`, `RegisterResponse`
  - `RefreshTokenRequest`, `RefreshTokenResponse`
  - `AuthUser` - User data from login response

### 2. Services

#### AuthService
- **Location**: `khidmah_inventory.client/src/app/core/services/auth.service.ts`
- **Features**:
  - `login()` - Authenticate user
  - `register()` - Register new user
  - `logout()` - Logout user
  - `refreshToken()` - Refresh JWT token
  - `getToken()` - Get current JWT token
  - `isAuthenticated()` - Check if user is authenticated
  - `getCurrentUser()` - Get current user
  - Token storage in localStorage
  - Automatic token expiry checking
  - Integration with PermissionService

#### PermissionService Integration
- On login success, user data is stored in PermissionService
- Permissions and roles are available throughout the app
- User data persists in localStorage

### 3. HTTP Interceptor

#### AuthInterceptor
- **Location**: `khidmah_inventory.client/src/app/core/interceptors/auth.interceptor.ts`
- **Features**:
  - Automatically adds `Authorization: Bearer {token}` header to all requests
  - Handles 401 errors by attempting token refresh
  - Handles 403 errors (permission denied)
  - Prevents multiple simultaneous refresh attempts
  - Logs out user if refresh fails

### 4. Guards

#### AuthGuard
- **Location**: `khidmah_inventory.client/src/app/core/guards/auth.guard.ts`
- **Purpose**: Protect routes that require authentication
- **Usage**: `canActivate: [AuthGuard]`
- **Behavior**: Redirects to login if not authenticated

#### PermissionGuard
- **Location**: `khidmah_inventory.client/src/app/core/guards/permission.guard.ts`
- **Purpose**: Protect routes that require specific permissions
- **Usage**: `canActivate: [AuthGuard, PermissionGuard], data: { permission: 'Users:Read' }`
- **Behavior**: 
  - Checks authentication first
  - Checks permission from route data
  - Supports single permission or array (any/all modes)
  - Redirects to unauthorized page if permission denied

### 5. Components

#### LoginComponent
- **Location**: `khidmah_inventory.client/src/app/features/auth/login/`
- **Features**:
  - Email/password form with validation
  - Password visibility toggle
  - Loading state
  - Error handling
  - Redirect to returnUrl after login
  - Redirects if already logged in

#### RegisterComponent
- **Location**: `khidmah_inventory.client/src/app/features/auth/register/`
- **Features**:
  - Complete registration form
  - Password confirmation validation
  - Form validation
  - Loading state
  - Error handling
  - Redirect to login after successful registration

### 6. Routing

#### Route Configuration
- **Location**: `khidmah_inventory.client/src/app/app-routing.module.ts`
- **Routes**:
  - `/login` - LoginComponent (public)
  - `/register` - RegisterComponent (public)
  - All other routes protected with `AuthGuard`
  - Permission-based routes use `PermissionGuard`

## Authentication Flow

### Login Flow

```
1. User enters email/password
2. Frontend sends POST /api/auth/login
3. Backend validates credentials
4. Backend generates JWT token with:
   - UserId, Email, CompanyId
   - Roles (as Role claims)
   - Permissions (as Permission claims)
5. Backend generates refresh token
6. Backend stores refresh token in database
7. Frontend receives tokens and user data
8. Frontend stores tokens in localStorage
9. Frontend stores user in PermissionService
10. Frontend redirects to returnUrl or dashboard
```

### Token Refresh Flow

```
1. HTTP request fails with 401
2. Interceptor catches error
3. Interceptor calls refreshToken()
4. Frontend sends POST /api/auth/refresh-token
5. Backend validates refresh token
6. Backend generates new tokens
7. Backend updates refresh token in database
8. Frontend stores new tokens
9. Interceptor retries original request with new token
```

### Logout Flow

```
1. User clicks logout
2. Frontend calls logout()
3. Frontend sends POST /api/auth/logout
4. Backend clears refresh token from database
5. Frontend clears tokens from localStorage
6. Frontend clears user from PermissionService
7. Frontend redirects to login
```

## Token Management

### JWT Token Structure

```json
{
  "sub": "userId",
  "email": "user@example.com",
  "CompanyId": "companyId",
  "role": ["Admin", "Manager"],
  "Permission": ["Users:Create", "Users:Read", ...],
  "exp": 1234567890,
  "iat": 1234567890
}
```

### Token Storage

- **JWT Token**: `localStorage.getItem('auth_token')`
- **Refresh Token**: `localStorage.getItem('refresh_token')`
- **Token Expiry**: `localStorage.getItem('token_expiry')`
- **User Data**: `localStorage.getItem('current_user')`

### Token Expiry Handling

- Token expires after 24 hours (configurable)
- Refresh token expires after 7 days
- Interceptor checks token expiry before requests
- Automatic refresh if token expires in < 5 minutes
- User logged out if refresh fails

## Security Features

### Backend
- ✅ Password hashing with BCrypt
- ✅ JWT token signing
- ✅ Refresh token validation
- ✅ Token expiry checking
- ✅ Permission-based authorization
- ✅ Multi-tenant isolation (CompanyId)

### Frontend
- ✅ Token storage in localStorage
- ✅ Automatic token injection
- ✅ Token refresh on 401
- ✅ Route guards
- ✅ Permission-based UI control
- ✅ Secure logout

## Usage Examples

### Login

```typescript
// In component
this.authService.login({ email, password }).subscribe({
  next: (response) => {
    if (response.success) {
      // User is logged in, tokens stored automatically
      this.router.navigate(['/dashboard']);
    }
  }
});
```

### Check Authentication

```typescript
if (this.authService.isAuthenticated()) {
  // User is logged in
}
```

### Get Current User

```typescript
const user = this.authService.getCurrentUser();
console.log(user?.email, user?.permissions);
```

### Logout

```typescript
this.authService.logout().subscribe({
  next: () => {
    // User logged out, redirected to login
  }
});
```

### Protected Route

```typescript
// In routing module
{
  path: 'users',
  component: UsersListComponent,
  canActivate: [AuthGuard, PermissionGuard],
  data: { permission: 'Users:List' }
}
```

### Permission-Based UI

```html
<!-- Show button only if user has permission -->
<button *appHasPermission="'Users:Create'" (click)="createUser()">
  Create User
</button>
```

## API Endpoints

### POST /api/auth/login
- **Auth**: Not required
- **Request**: `{ email: string, password: string }`
- **Response**: `ApiResponse<LoginResponse>`
- **Success**: Returns JWT token, refresh token, and user data

### POST /api/auth/register
- **Auth**: Required (`Auth:Create` permission)
- **Request**: `RegisterRequest`
- **Response**: `ApiResponse<RegisterResponse>`
- **Success**: Returns created user info

### POST /api/auth/refresh-token
- **Auth**: Not required
- **Request**: `{ token: string, refreshToken: string }`
- **Response**: `ApiResponse<RefreshTokenResponse>`
- **Success**: Returns new tokens

### POST /api/auth/logout
- **Auth**: Required
- **Request**: None
- **Response**: `ApiResponse<void>`
- **Success**: Clears refresh token

## Error Handling

### Login Errors
- Invalid email/password → 400 Bad Request
- User inactive → 400 Bad Request
- No active company → 400 Bad Request

### Refresh Token Errors
- Invalid refresh token → 400 Bad Request
- Refresh token expired → 400 Bad Request
- User not found → 400 Bad Request

### Interceptor Errors
- 401 → Attempts token refresh
- 403 → Permission denied (logged)
- Refresh fails → Logs out user

## Testing Checklist

### Backend
- [x] Login with valid credentials
- [x] Login with invalid credentials
- [x] Login with inactive user
- [x] Register new user
- [x] Register with existing email
- [x] Refresh token with valid token
- [x] Refresh token with expired token
- [x] Logout clears refresh token
- [x] JWT token includes roles and permissions
- [x] Multi-tenant isolation

### Frontend
- [x] Login form validation
- [x] Login success flow
- [x] Login error handling
- [x] Register form validation
- [x] Register success flow
- [x] Token storage
- [x] Token injection in requests
- [x] Token refresh on 401
- [x] AuthGuard protection
- [x] PermissionGuard protection
- [x] Logout flow
- [x] PermissionService integration

## Files Created/Modified

### Backend
- `Khidmah_Inventory.Application/Features/Auth/Commands/RefreshToken/`
- `Khidmah_Inventory.Application/Features/Auth/Commands/Logout/`
- `Khidmah_Inventory.API/Controllers/AuthController.cs` (updated)
- `Khidmah_Inventory.Domain/Entities/User.cs` (added ClearRefreshToken method)

### Frontend
- `khidmah_inventory.client/src/app/core/models/auth.model.ts`
- `khidmah_inventory.client/src/app/core/services/auth.service.ts`
- `khidmah_inventory.client/src/app/core/interceptors/auth.interceptor.ts`
- `khidmah_inventory.client/src/app/core/guards/auth.guard.ts`
- `khidmah_inventory.client/src/app/core/guards/permission.guard.ts`
- `khidmah_inventory.client/src/app/features/auth/login/`
- `khidmah_inventory.client/src/app/features/auth/register/`
- `khidmah_inventory.client/src/app/app-routing.module.ts` (updated)
- `khidmah_inventory.client/src/app/app.module.ts` (updated - added interceptor)

## Next Steps

1. **Email Verification**: Add email confirmation flow
2. **Password Reset**: Add forgot password/reset password flow
3. **Two-Factor Authentication**: Add 2FA support
4. **Session Management**: Add active sessions tracking
5. **Token Blacklisting**: Add token revocation mechanism
6. **Remember Me**: Add remember me functionality
7. **Social Login**: Add OAuth providers (Google, Microsoft, etc.)

## Summary

✅ **Complete Authentication Flow**: Login, Register, Logout, Token Refresh
✅ **JWT Token Management**: Automatic injection, refresh, expiry handling
✅ **Security**: Password hashing, token validation, permission-based access
✅ **Frontend Integration**: Services, guards, interceptors, components
✅ **Permission Integration**: Automatic permission loading on login
✅ **Route Protection**: AuthGuard and PermissionGuard
✅ **Error Handling**: Comprehensive error handling throughout

The authentication system is production-ready and provides secure, token-based authentication with automatic token management and permission-based access control.

