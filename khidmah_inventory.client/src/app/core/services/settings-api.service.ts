import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import {
  CompanySettings,
  UserSettings,
  SystemSettings,
  NotificationSettings,
  UISettings,
  ReportSettings
} from '../models/app-settings.model';
import { ApiConfigService } from './api-config.service';

@Injectable({
  providedIn: 'root'
})
export class SettingsApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('settings');
  }

  // Company Settings
  getCompanySettings(): Observable<ApiResponse<CompanySettings>> {
    return this.http.get<ApiResponse<CompanySettings>>(`${this.apiUrl}/company`);
  }

  saveCompanySettings(settings: CompanySettings): Observable<ApiResponse<CompanySettings>> {
    return this.http.post<ApiResponse<CompanySettings>>(`${this.apiUrl}/company`, { settings });
  }

  // User Settings
  getUserSettings(): Observable<ApiResponse<UserSettings>> {
    return this.http.get<ApiResponse<UserSettings>>(`${this.apiUrl}/user`);
  }

  saveUserSettings(settings: UserSettings): Observable<ApiResponse<UserSettings>> {
    return this.http.post<ApiResponse<UserSettings>>(`${this.apiUrl}/user`, { settings });
  }

  // System Settings
  getSystemSettings(): Observable<ApiResponse<SystemSettings>> {
    return this.http.get<ApiResponse<SystemSettings>>(`${this.apiUrl}/system`);
  }

  saveSystemSettings(settings: SystemSettings): Observable<ApiResponse<SystemSettings>> {
    return this.http.post<ApiResponse<SystemSettings>>(`${this.apiUrl}/system`, { settings });
  }

  // Notification Settings
  getNotificationSettings(): Observable<ApiResponse<NotificationSettings>> {
    return this.http.get<ApiResponse<NotificationSettings>>(`${this.apiUrl}/notifications`);
  }

  saveNotificationSettings(settings: NotificationSettings): Observable<ApiResponse<NotificationSettings>> {
    return this.http.post<ApiResponse<NotificationSettings>>(`${this.apiUrl}/notifications`, { settings });
  }

  // UI Settings
  getUISettings(): Observable<ApiResponse<UISettings>> {
    return this.http.get<ApiResponse<UISettings>>(`${this.apiUrl}/ui`);
  }

  saveUISettings(settings: UISettings): Observable<ApiResponse<UISettings>> {
    return this.http.post<ApiResponse<UISettings>>(`${this.apiUrl}/ui`, { settings });
  }

  // Report Settings
  getReportSettings(): Observable<ApiResponse<ReportSettings>> {
    return this.http.get<ApiResponse<ReportSettings>>(`${this.apiUrl}/reports`);
  }

  saveReportSettings(settings: ReportSettings): Observable<ApiResponse<ReportSettings>> {
    return this.http.post<ApiResponse<ReportSettings>>(`${this.apiUrl}/reports`, { settings });
  }

  // Appearance Settings (using UI settings endpoint for now)
  // TODO: Create dedicated appearance settings endpoint
  getAppearanceSettings(): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/ui`);
  }

  saveAppearanceSettings(settings: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/ui`, { settings });
  }
}

