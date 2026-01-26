import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, filter, take, switchMap } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';
import { ApiCodeService } from '../services/api-code.service';
import { Router } from '@angular/router';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(
    private authService: AuthService,
    private apiCodeService: ApiCodeService,
    private router: Router
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Get API code first
    const apiCode = this.apiCodeService.getApiCode(request.method, request.url);
    
    // Build headers object
    const headers: { [key: string]: string } = {};
    
    // Add token to request
    const token = this.authService.getToken();
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }
    
    // Add API code to request header
    if (apiCode) {
      headers['X-Api-Code'] = apiCode;
      // Debug logging (remove in production)
      if (!request.url.includes('/auth/')) {
        console.log(`[API Code] ${request.method} ${request.url} -> ${apiCode}`);
      }
    } else {
      // Debug logging for missing API codes (remove in production)
      if (!request.url.includes('/auth/')) {
        console.warn(`[API Code] No API code found for ${request.method} ${request.url}`);
      }
    }
    
    // Clone request with all headers at once
    if (Object.keys(headers).length > 0) {
      request = request.clone({
        setHeaders: headers
      });
    }

    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        // If 401 and we have a refresh token, try to refresh
        if (error.status === 401 && this.authService.getRefreshToken()) {
          return this.handle401Error(request, next);
        }

        // If 403, redirect to unauthorized page or show message
        if (error.status === 403) {
          // Handle permission denied
          console.error('Permission denied');
        }

        return throwError(() => error);
      })
    );
  }

  private addTokenToRequest(request: HttpRequest<unknown>, token: string): HttpRequest<unknown> {
    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  private handle401Error(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      return this.authService.refreshToken().pipe(
        switchMap((response) => {
          this.isRefreshing = false;
          if (response.success && response.data) {
            this.refreshTokenSubject.next(response.data.token);
            // Re-add API code when retrying with new token
            const apiCode = this.apiCodeService.getApiCode(request.method, request.url);
            const headers: { [key: string]: string } = {
              'Authorization': `Bearer ${response.data.token}`
            };
            if (apiCode) {
              headers['X-Api-Code'] = apiCode;
            }
            return next.handle(request.clone({ setHeaders: headers }));
          }
          return throwError(() => new Error('Token refresh failed'));
        }),
        catchError((error) => {
          this.isRefreshing = false;
          this.authService.logout().subscribe();
          return throwError(() => error);
        })
      );
    } else {
      // Wait for token refresh to complete
      return this.refreshTokenSubject.pipe(
        filter(token => token !== null),
        take(1),
        switchMap((token) => {
          // Re-add API code when retrying with refreshed token
          const apiCode = this.apiCodeService.getApiCode(request.method, request.url);
          const headers: { [key: string]: string } = {
            'Authorization': `Bearer ${token}`
          };
          if (apiCode) {
            headers['X-Api-Code'] = apiCode;
          }
          return next.handle(request.clone({ setHeaders: headers }));
        })
      );
    }
  }
}

