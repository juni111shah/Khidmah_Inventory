import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { SalesAnalytics, InventoryAnalytics, ProfitAnalytics, AnalyticsRequest } from '../models/analytics.model';
import { ApiConfigService } from './api-config.service';

@Injectable({
  providedIn: 'root'
})
export class AnalyticsApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('analytics');
  }

  getSalesAnalytics(request: AnalyticsRequest): Observable<ApiResponse<SalesAnalytics>> {
    return this.http.post<ApiResponse<SalesAnalytics>>(`${this.apiUrl}/sales`, request);
  }

  getInventoryAnalytics(warehouseId?: string, categoryId?: string): Observable<ApiResponse<InventoryAnalytics>> {
    let params = new HttpParams();
    if (warehouseId) {
      params = params.set('warehouseId', warehouseId);
    }
    if (categoryId) {
      params = params.set('categoryId', categoryId);
    }
    return this.http.get<ApiResponse<InventoryAnalytics>>(`${this.apiUrl}/inventory`, { params });
  }

  getProfitAnalytics(request: AnalyticsRequest): Observable<ApiResponse<ProfitAnalytics>> {
    return this.http.post<ApiResponse<ProfitAnalytics>>(`${this.apiUrl}/profit`, request);
  }
}

