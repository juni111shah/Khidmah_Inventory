# Frontend Authentication Integration

## Feature Overview

This module implements frontend authentication integration for the Khidmah Inventory Management System. It provides login, registration, token management, and protected route functionality.

## Business Requirements

1. **Authentication Features**
   - User login
   - User registration (admin only)
   - Token storage and management
   - Token refresh
   - Logout
   - Protected routes
   - Auto-logout on token expiry

2. **User Session**
   - Store JWT token securely
   - Store refresh token
   - Store user information
   - Track token expiry

3. **Route Protection**
   - Guard protected routes
   - Redirect to login if not authenticated
   - Redirect to appropriate page after login

## Implementation Steps

### 1. Auth Service

**File**: `khidmah_inventory.client/src/app/core/services/auth.service.ts`

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  userName: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  companyId: string;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  expiresAt: string;
  user: UserDto;
}

export interface UserDto {
  id: string;
  email: string;
  userName: string;
  firstName: string;
  lastName: string;
  companyId: string;
  roles: string[];
  permissions: string[];
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = '/api/auth';
  private tokenKey = 'auth_token';
  private refreshTokenKey = 'refresh_token';
  private userKey = 'user_data';
  
  private currentUserSubject = new BehaviorSubject<UserDto | null>(this.getStoredUser());
  public currentUser$ = this.currentUserSubject.asObservable();
  
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasValidToken());
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.checkTokenExpiry();
  }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        this.setAuthData(response);
        this.currentUserSubject.next(response.user);
        this.isAuthenticatedSubject.next(true);
      }),
      catchError(error => {
        this.clearAuthData();
        return throwError(() => error);
      })
    );
  }

  register(data: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, data).pipe(
      tap(response => {
        this.setAuthData(response);
        this.currentUserSubject.next(response.user);
        this.isAuthenticatedSubject.next(true);
      })
    );
  }

  logout(): void {
    this.clearAuthData();
    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
    this.router.navigate(['/login']);
  }

  refreshToken(): Observable<AuthResponse> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      this.logout();
      return throwError(() => new Error('No refresh token'));
    }

    return this.http.post<AuthResponse>(`${this.apiUrl}/refresh`, { refreshToken }).pipe(
      tap(response => {
        this.setAuthData(response);
        this.currentUserSubject.next(response.user);
      }),
      catchError(error => {
        this.logout();
        return throwError(() => error);
      })
    );
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.refreshTokenKey);
  }

  getCurrentUser(): UserDto | null {
    return this.currentUserSubject.value;
  }

  isAuthenticated(): boolean {
    return this.isAuthenticatedSubject.value;
  }

  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user?.roles.includes(role) ?? false;
  }

  hasPermission(permission: string): boolean {
    const user = this.getCurrentUser();
    return user?.permissions.includes(permission) ?? false;
  }

  private setAuthData(response: AuthResponse): void {
    localStorage.setItem(this.tokenKey, response.token);
    localStorage.setItem(this.refreshTokenKey, response.refreshToken);
    localStorage.setItem(this.userKey, JSON.stringify(response.user));
  }

  private clearAuthData(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.refreshTokenKey);
    localStorage.removeItem(this.userKey);
  }

  private getStoredUser(): UserDto | null {
    const userData = localStorage.getItem(this.userKey);
    return userData ? JSON.parse(userData) : null;
  }

  private hasValidToken(): boolean {
    const token = this.getToken();
    if (!token) return false;
    
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expiry = payload.exp * 1000;
      return Date.now() < expiry;
    } catch {
      return false;
    }
  }

  private checkTokenExpiry(): void {
    if (!this.hasValidToken()) {
      this.logout();
    } else {
      // Set up automatic token refresh before expiry
      const token = this.getToken();
      if (token) {
        try {
          const payload = JSON.parse(atob(token.split('.')[1]));
          const expiry = payload.exp * 1000;
          const timeUntilExpiry = expiry - Date.now();
          const refreshTime = timeUntilExpiry - (5 * 60 * 1000); // Refresh 5 minutes before expiry
          
          if (refreshTime > 0) {
            setTimeout(() => this.refreshToken().subscribe(), refreshTime);
          }
        } catch {
          // Invalid token format
        }
      }
    }
  }
}
```

### 2. HTTP Interceptor

**File**: `khidmah_inventory.client/src/app/core/interceptors/auth.interceptor.ts`

```typescript
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<any> {
    const token = this.authService.getToken();
    
    if (token) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }

    // Add company ID header if available
    const user = this.authService.getCurrentUser();
    if (user?.companyId) {
      req = req.clone({
        setHeaders: {
          'X-Company-Id': user.companyId
        }
      });
    }

    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          // Try to refresh token
          return this.authService.refreshToken().pipe(
            switchMap(() => {
              const newToken = this.authService.getToken();
              const clonedReq = req.clone({
                setHeaders: {
                  Authorization: `Bearer ${newToken}`
                }
              });
              return next.handle(clonedReq);
            }),
            catchError(() => {
              this.authService.logout();
              this.router.navigate(['/login']);
              return throwError(() => error);
            })
          );
        }
        return throwError(() => error);
      })
    );
  }
}
```

### 3. Auth Guard

**File**: `khidmah_inventory.client/src/app/core/guards/auth.guard.ts`

```typescript
import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    if (this.authService.isAuthenticated()) {
      // Check role requirement if specified
      const requiredRole = route.data['role'];
      if (requiredRole && !this.authService.hasRole(requiredRole)) {
        this.router.navigate(['/unauthorized']);
        return false;
      }
      
      // Check permission requirement if specified
      const requiredPermission = route.data['permission'];
      if (requiredPermission && !this.authService.hasPermission(requiredPermission)) {
        this.router.navigate(['/unauthorized']);
        return false;
      }
      
      return true;
    }

    this.router.navigate(['/login'], { queryParams: { returnUrl: route.url } });
    return false;
  }
}
```

### 4. Login Component

**File**: `khidmah_inventory.client/src/app/features/auth/login/login.component.ts`

```typescript
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  loginForm: FormGroup;
  errorMessage: string = '';
  loading: boolean = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.loading = true;
      this.errorMessage = '';
      
      this.authService.login(this.loginForm.value).subscribe({
        next: () => {
          const returnUrl = this.router.parseUrl(this.router.url).queryParams['returnUrl'] || '/';
          this.router.navigateByUrl(returnUrl);
        },
        error: (error) => {
          this.loading = false;
          this.errorMessage = error.error?.errors?.[0] || 'Login failed. Please check your credentials.';
        }
      });
    }
  }
}
```

### 5. App Module Configuration

**File**: `khidmah_inventory.client/src/app/app.module.ts`

```typescript
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './core/interceptors/auth.interceptor';
import { AuthGuard } from './core/guards/auth.guard';

@NgModule({
  // ... other imports
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    AuthGuard
  ]
})
export class AppModule { }
```

### 6. Routing Configuration

**File**: `khidmah_inventory.client/src/app/app-routing.module.ts`

```typescript
import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { LoginComponent } from './features/auth/login/login.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: '',
    canActivate: [AuthGuard],
    loadChildren: () => import('./features/dashboard/dashboard.module').then(m => m.DashboardModule)
  },
  {
    path: 'products',
    canActivate: [AuthGuard],
    loadChildren: () => import('./features/products/products.module').then(m => m.ProductsModule)
  },
  // ... other routes
  { path: '**', redirectTo: '' }
];
```

## Frontend Components

### 1. Login Component
- Email and password form
- Validation
- Error display
- Loading state
- Redirect after login

### 2. Register Component (Admin only)
- Registration form
- Company selection
- Validation
- Success message

### 3. User Profile Component
- Display current user info
- Edit profile
- Change password
- Logout button

## Workflow

### Login Flow
```
User enters credentials → Submit → AuthService.login()
→ API call → Store token and user data → Update observables
→ Redirect to return URL or dashboard
```

### Protected Route Access
```
User navigates to protected route → AuthGuard checks authentication
→ If authenticated, check roles/permissions → Allow or redirect
→ If not authenticated, redirect to login with return URL
```

### Token Refresh Flow
```
API call returns 401 → Interceptor catches → Refresh token
→ Update stored tokens → Retry original request
→ If refresh fails, logout and redirect to login
```

## Testing Checklist

- [ ] Login with valid credentials
- [ ] Login with invalid credentials (shows error)
- [ ] Token stored in localStorage
- [ ] User data stored correctly
- [ ] Protected routes require authentication
- [ ] Redirect to login if not authenticated
- [ ] Redirect after login works
- [ ] Token included in API requests
- [ ] Company ID header included
- [ ] Token refresh works
- [ ] Logout clears data
- [ ] Role-based access control works
- [ ] Permission-based access control works

