import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ThemeConfig } from '../models/theme.model';
import { ApiResponse } from '../models/api-response.model';
import { ApiConfigService } from './api-config.service';

@Injectable({
  providedIn: 'root'
})
export class ThemeApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('theme');
  }

  getUserTheme(): Observable<ApiResponse<ThemeConfig>> {
    return this.http.get<ApiResponse<ThemeConfig>>(`${this.apiUrl}/user`);
  }

  getGlobalTheme(): Observable<ApiResponse<ThemeConfig>> {
    return this.http.get<ApiResponse<ThemeConfig>>(`${this.apiUrl}/global`);
  }

  saveUserTheme(theme: ThemeConfig): Observable<ApiResponse<ThemeConfig>> {
    return this.http.post<ApiResponse<ThemeConfig>>(`${this.apiUrl}/user`, { Theme: theme });
  }

  saveGlobalTheme(theme: ThemeConfig): Observable<ApiResponse<ThemeConfig>> {
    return this.http.post<ApiResponse<ThemeConfig>>(`${this.apiUrl}/global`, { Theme: theme });
  }

  uploadLogo(file: File): Observable<ApiResponse<{ logoUrl: string }>> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<ApiResponse<{ logoUrl: string }>>(`${this.apiUrl}/logo`, formData);
  }
}

