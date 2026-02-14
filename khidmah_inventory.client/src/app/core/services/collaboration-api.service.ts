import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { ActivityLog, Comment, CreateCommentRequest } from '../models/collaboration.model';
import { ApiConfigService } from './api-config.service';

@Injectable({ providedIn: 'root' })
export class CollaborationApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('collaboration');
  }

  getActivityFeed(entityType?: string, entityId?: string, limit: number = 50): Observable<ApiResponse<ActivityLog[]>> {
    let params = new HttpParams().set('limit', String(limit));
    if (entityType) params = params.set('entityType', entityType);
    if (entityId) params = params.set('entityId', entityId);
    return this.http.get<ApiResponse<ActivityLog[]>>(`${this.apiUrl}/activity-feed`, { params });
  }

  getComments(entityType: string, entityId: string): Observable<ApiResponse<Comment[]>> {
    const params = new HttpParams()
      .set('entityType', entityType)
      .set('entityId', entityId);
    return this.http.get<ApiResponse<Comment[]>>(`${this.apiUrl}/comments`, { params });
  }

  createComment(request: CreateCommentRequest): Observable<ApiResponse<Comment>> {
    return this.http.post<ApiResponse<Comment>>(`${this.apiUrl}/comments`, request);
  }
}
