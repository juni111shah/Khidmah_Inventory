import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, PagedResult } from '../models/api-response.model';
import {
  SalesOrder,
  CreateSalesOrderRequest,
  GetSalesOrdersListQuery
} from '../models/sales-order.model';
import { ApiConfigService } from './api-config.service';

@Injectable({
  providedIn: 'root'
})
export class SalesOrderApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('salesorders');
  }

  getSalesOrders(query?: GetSalesOrdersListQuery): Observable<ApiResponse<PagedResult<SalesOrder>>> {
    return this.http.post<ApiResponse<PagedResult<SalesOrder>>>(`${this.apiUrl}/list`, query || {});
  }

  createSalesOrder(request: CreateSalesOrderRequest): Observable<ApiResponse<SalesOrder>> {
    return this.http.post<ApiResponse<SalesOrder>>(this.apiUrl, request);
  }
}

