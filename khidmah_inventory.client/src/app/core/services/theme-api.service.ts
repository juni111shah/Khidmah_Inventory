import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ThemeConfig } from '../models/theme.model';
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

  getUserTheme(): Observable<ThemeConfig> {
    return this.http.get<ThemeConfig>(`${this.apiUrl}/user`);
  }

  getGlobalTheme(): Observable<ThemeConfig> {
    return this.http.get<ThemeConfig>(`${this.apiUrl}/global`);
  }

  saveUserTheme(theme: ThemeConfig): Observable<ThemeConfig> {
    return this.http.post<ThemeConfig>(`${this.apiUrl}/user`, theme);
  }

  saveGlobalTheme(theme: ThemeConfig): Observable<ThemeConfig> {
    return this.http.post<ThemeConfig>(`${this.apiUrl}/global`, theme);
  }

  uploadLogo(file: File): Observable<{ logoUrl: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ logoUrl: string }>(`${this.apiUrl}/logo`, formData);
  }
}

