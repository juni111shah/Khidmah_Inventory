import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { ApiResponse } from '../models/api-response.model';
import {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  RegisterResponse,
  RefreshTokenRequest,
  RefreshTokenResponse
} from '../models/auth.model';
import { PermissionService } from './permission.service';
import { User } from '../models/user.model';
import { ApiConfigService } from './api-config.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly TOKEN_KEY = 'auth_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';
  private readonly TOKEN_EXPIRY_KEY = 'token_expiry';
  private readonly USER_KEY = 'current_user';

  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private router: Router,
    private permissionService: PermissionService,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('auth');
    this.loadUserFromStorage();
    this.checkTokenExpiry();
  }

  login(credentials: LoginRequest): Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(this.apiConfig.getApiUrl('auth/login'), credentials).pipe(
      tap(response => {
        if (response.success && response.data) {
          this.handleLoginSuccess(response.data);
        }
      }),
      catchError(error => {
        return throwError(() => error);
      })
    );
  }

  register(userData: RegisterRequest): Observable<ApiResponse<RegisterResponse>> {
    return this.http.post<ApiResponse<RegisterResponse>>(this.apiConfig.getApiUrl('auth/register'), userData);
  }

  logout(): Observable<ApiResponse<void>> {
    return this.http.post<ApiResponse<void>>(this.apiConfig.getApiUrl('auth/logout'), {}).pipe(
      tap(() => {
        this.handleLogout();
      }),
      catchError(() => {
        // Even if API call fails, clear local storage
        this.handleLogout();
        return throwError(() => new Error('Logout failed'));
      })
    );
  }

  refreshToken(): Observable<ApiResponse<RefreshTokenResponse>> {
    const token = this.getToken();
    const refreshToken = this.getRefreshToken();

    if (!token || !refreshToken) {
      return throwError(() => new Error('No tokens available'));
    }

    const request: RefreshTokenRequest = {
      token,
      refreshToken
    };

    return this.http.post<ApiResponse<RefreshTokenResponse>>(this.apiConfig.getApiUrl('auth/refresh-token'), request).pipe(
      tap(response => {
        if (response.success && response.data) {
          this.setTokens(
            response.data.token,
            response.data.refreshToken,
            response.data.expiresAt
          );
        }
      }),
      catchError(error => {
        // If refresh fails, logout user
        this.handleLogout();
        return throwError(() => error);
      })
    );
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) {
      return false;
    }

    // Check if token is expired
    const expiry = localStorage.getItem(this.TOKEN_EXPIRY_KEY);
    if (expiry) {
      const expiryDate = new Date(expiry);
      if (expiryDate < new Date()) {
        // Try to refresh token
        this.refreshToken().subscribe({
          next: () => {},
          error: () => {
            this.handleLogout();
          }
        });
        return false;
      }
    }

    return true;
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  private handleLoginSuccess(data: LoginResponse): void {
    // Store tokens
    this.setTokens(data.token, data.refreshToken, data.expiresAt);

    // Convert AuthUser to User format
    const user: User = {
      id: data.user.id,
      email: data.user.email,
      userName: data.user.userName,
      firstName: data.user.firstName,
      lastName: data.user.lastName,
      phoneNumber: undefined,
      isActive: true,
      emailConfirmed: true,
      lastLoginAt: new Date().toISOString(),
      roles: data.user.roles,
      permissions: data.user.permissions,
      companies: [],
      defaultCompanyId: data.user.companyId,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    };

    // Store user
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    this.currentUserSubject.next(user);

    // Update permission service
    this.permissionService.setCurrentUser(user);
  }

  private handleLogout(): void {
    // Clear tokens
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.TOKEN_EXPIRY_KEY);
    localStorage.removeItem(this.USER_KEY);

    // Clear user
    this.currentUserSubject.next(null);
    this.permissionService.clearUser();

    // Redirect to login
    this.router.navigate(['/login']);
  }

  private setTokens(token: string, refreshToken: string, expiresAt: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, refreshToken);
    localStorage.setItem(this.TOKEN_EXPIRY_KEY, expiresAt);
  }

  private loadUserFromStorage(): void {
    const userStr = localStorage.getItem(this.USER_KEY);
    if (userStr) {
      try {
        const user: User = JSON.parse(userStr);
        this.currentUserSubject.next(user);
        this.permissionService.setCurrentUser(user);
      } catch (e) {
        console.error('Error loading user from storage', e);
      }
    }
  }

  private checkTokenExpiry(): void {
    const expiry = localStorage.getItem(this.TOKEN_EXPIRY_KEY);
    if (expiry) {
      const expiryDate = new Date(expiry);
      const now = new Date();
      const timeUntilExpiry = expiryDate.getTime() - now.getTime();

      // If token expires in less than 5 minutes, refresh it
      if (timeUntilExpiry > 0 && timeUntilExpiry < 5 * 60 * 1000) {
        this.refreshToken().subscribe({
          next: () => {},
          error: () => {
            // If refresh fails, user will be logged out on next request
          }
        });
      } else if (timeUntilExpiry <= 0) {
        // Token already expired
        this.handleLogout();
      }
    }
  }
}

