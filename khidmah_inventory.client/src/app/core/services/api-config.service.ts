import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiConfigService {
  /**
   * Get the full API URL for a given endpoint
   * @param endpoint - The API endpoint (e.g., '/auth/login' or 'auth/login')
   * @returns Full URL (e.g., 'https://localhost:5001/api/auth/login')
   */
  getApiUrl(endpoint: string): string {
    // Remove leading slash if present to avoid double slashes
    const cleanEndpoint = endpoint.startsWith('/') ? endpoint.substring(1) : endpoint;
    return `${environment.apiUrl}/${cleanEndpoint}`;
  }

  /**
   * Get the base API URL
   * @returns Base API URL (e.g., 'https://localhost:5001/api')
   */
  getBaseApiUrl(): string {
    return environment.apiUrl;
  }

  /**
   * Get the API base URL without /api
   * @returns Base URL (e.g., 'https://localhost:5001')
   */
  getBaseUrl(): string {
    return environment.apiBaseUrl;
  }

  /**
   * Check if running in production
   */
  isProduction(): boolean {
    return environment.production;
  }

  /**
   * Check if debug is enabled
   */
  isDebugEnabled(): boolean {
    return environment.enableDebug;
  }
}

