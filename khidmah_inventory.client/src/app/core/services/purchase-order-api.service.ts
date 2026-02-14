import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, PagedResult } from '../models/api-response.model';
import {
  PurchaseOrder,
  CreatePurchaseOrderRequest,
  UpdatePurchaseOrderRequest,
  GetPurchaseOrdersListQuery
} from '../models/purchase-order.model';
import { ApiConfigService } from './api-config.service';

@Injectable({
  providedIn: 'root'
})
export class PurchaseOrderApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('purchaseorders');
  }

  getPurchaseOrder(id: string): Observable<ApiResponse<PurchaseOrder>> {
    return this.http.get<ApiResponse<PurchaseOrder>>(`${this.apiUrl}/${id}`);
  }

  getPurchaseOrders(query?: GetPurchaseOrdersListQuery): Observable<ApiResponse<PagedResult<PurchaseOrder>>> {
    return this.http.post<ApiResponse<PagedResult<PurchaseOrder>>>(`${this.apiUrl}/list`, query || {});
  }

  updatePurchaseOrder(id: string, request: UpdatePurchaseOrderRequest): Observable<ApiResponse<PurchaseOrder>> {
    return this.http.put<ApiResponse<PurchaseOrder>>(`${this.apiUrl}/${id}`, request);
  }

  createPurchaseOrder(request: CreatePurchaseOrderRequest): Observable<ApiResponse<PurchaseOrder>> {
    return this.http.post<ApiResponse<PurchaseOrder>>(this.apiUrl, request);
  }
}

