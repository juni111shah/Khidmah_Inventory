import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { GlobalSearchResultDto, GlobalSearchQuery } from '../models/search.model';
import { ApiConfigService } from './api-config.service';

@Injectable({ providedIn: 'root' })
export class SearchApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('search');
  }

  globalSearch(query: GlobalSearchQuery): Observable<ApiResponse<GlobalSearchResultDto>> {
    const params = new HttpParams()
      .set('searchTerm', query.searchTerm)
      .set('limitPerGroup', String(query.limitPerGroup ?? 10));
    return this.http.get<ApiResponse<GlobalSearchResultDto>>(`${this.apiUrl}/global`, { params });
  }
}
