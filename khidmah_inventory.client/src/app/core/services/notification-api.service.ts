import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import {
  NotificationDto,
  GetNotificationsRequest,
  GetNotificationsResponse
} from '../models/notification.model';
import { ApiConfigService } from './api-config.service';

@Injectable({
  providedIn: 'root'
})
export class NotificationApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('notifications');
  }

  getList(request?: GetNotificationsRequest): Observable<ApiResponse<GetNotificationsResponse>> {
    return this.http.post<ApiResponse<GetNotificationsResponse>>(`${this.apiUrl}/list`, request ?? {});
  }

  getUnreadCount(): Observable<ApiResponse<number>> {
    return this.http.get<ApiResponse<number>>(`${this.apiUrl}/unread-count`);
  }

  markRead(id: string): Observable<ApiResponse<unknown>> {
    return this.http.post<ApiResponse<unknown>>(`${this.apiUrl}/mark-read/${id}`, {});
  }

  markAllRead(): Observable<ApiResponse<unknown>> {
    return this.http.post<ApiResponse<unknown>>(`${this.apiUrl}/mark-all-read`, {});
  }
}
