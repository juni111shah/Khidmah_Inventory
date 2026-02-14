import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import {
  ExecutiveKpisDto,
  SalesKpisDto,
  InventoryKpisDto,
  CustomerKpisDto,
  KpiFilters
} from '../models/kpi.model';
import { ApiConfigService } from './api-config.service';

@Injectable({
  providedIn: 'root'
})
export class KpiApiService {
  private baseUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.baseUrl = this.apiConfig.getApiUrl('kpi');
  }

  private buildParams(filters: KpiFilters): HttpParams {
    let params = new HttpParams();
    if (filters.dateFrom) params = params.set('dateFrom', filters.dateFrom);
    if (filters.dateTo) params = params.set('dateTo', filters.dateTo);
    if (filters.warehouseId) params = params.set('warehouseId', filters.warehouseId);
    if (filters.productId) params = params.set('productId', filters.productId);
    if (filters.categoryId) params = params.set('categoryId', filters.categoryId);
    if (filters.deadStockDays != null) params = params.set('deadStockDays', filters.deadStockDays.toString());
    return params;
  }

  getExecutiveKpis(filters?: KpiFilters): Observable<ApiResponse<ExecutiveKpisDto>> {
    const params = filters ? this.buildParams(filters) : undefined;
    return this.http.get<ApiResponse<ExecutiveKpisDto>>(`${this.baseUrl}/executive`, { params });
  }

  getSalesKpis(filters?: KpiFilters): Observable<ApiResponse<SalesKpisDto>> {
    const params = filters ? this.buildParams(filters) : undefined;
    return this.http.get<ApiResponse<SalesKpisDto>>(`${this.baseUrl}/sales`, { params });
  }

  getInventoryKpis(filters?: KpiFilters): Observable<ApiResponse<InventoryKpisDto>> {
    const params = filters ? this.buildParams(filters) : undefined;
    return this.http.get<ApiResponse<InventoryKpisDto>>(`${this.baseUrl}/inventory`, { params });
  }

  getCustomerKpis(filters?: KpiFilters): Observable<ApiResponse<CustomerKpisDto>> {
    const params = filters ? this.buildParams(filters) : undefined;
    return this.http.get<ApiResponse<CustomerKpisDto>>(`${this.baseUrl}/customers`, { params });
  }
}
