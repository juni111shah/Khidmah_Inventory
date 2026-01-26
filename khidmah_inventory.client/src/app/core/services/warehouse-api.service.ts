import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, PagedResult } from '../models/api-response.model';
import {
  Warehouse,
  CreateWarehouseRequest,
  UpdateWarehouseRequest,
  GetWarehousesListQuery
} from '../models/warehouse.model';
import { ApiConfigService } from './api-config.service';

@Injectable({
  providedIn: 'root'
})
export class WarehouseApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('warehouses');
  }

  getWarehouse(id: string): Observable<ApiResponse<Warehouse>> {
    return this.http.get<ApiResponse<Warehouse>>(`${this.apiUrl}/${id}`);
  }

  getWarehouses(query?: GetWarehousesListQuery): Observable<ApiResponse<PagedResult<Warehouse>>> {
    return this.http.post<ApiResponse<PagedResult<Warehouse>>>(`${this.apiUrl}/list`, query || {});
  }

  createWarehouse(request: CreateWarehouseRequest): Observable<ApiResponse<Warehouse>> {
    return this.http.post<ApiResponse<Warehouse>>(this.apiUrl, request);
  }

  updateWarehouse(id: string, request: UpdateWarehouseRequest): Observable<ApiResponse<Warehouse>> {
    return this.http.put<ApiResponse<Warehouse>>(`${this.apiUrl}/${id}`, request);
  }

  deleteWarehouse(id: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/${id}`);
  }

  activateWarehouse(id: string): Observable<ApiResponse<Warehouse>> {
    return this.http.patch<ApiResponse<Warehouse>>(`${this.apiUrl}/${id}/activate`, null);
  }

  deactivateWarehouse(id: string): Observable<ApiResponse<Warehouse>> {
    return this.http.patch<ApiResponse<Warehouse>>(`${this.apiUrl}/${id}/deactivate`, null);
  }
}

