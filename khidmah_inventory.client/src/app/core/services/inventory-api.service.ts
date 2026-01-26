import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, PagedResult } from '../models/api-response.model';
import {
  StockTransaction,
  StockLevel,
  CreateStockTransactionRequest,
  GetStockTransactionsListQuery,
  GetStockLevelsListQuery
} from '../models/inventory.model';
import { ApiConfigService } from './api-config.service';

@Injectable({
  providedIn: 'root'
})
export class InventoryApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('inventory');
  }

  createStockTransaction(request: CreateStockTransactionRequest): Observable<ApiResponse<StockTransaction>> {
    return this.http.post<ApiResponse<StockTransaction>>(`${this.apiUrl}/stock-transaction`, request);
  }

  getStockTransactions(query?: GetStockTransactionsListQuery): Observable<ApiResponse<PagedResult<StockTransaction>>> {
    return this.http.post<ApiResponse<PagedResult<StockTransaction>>>(`${this.apiUrl}/transactions/list`, query || {});
  }

  getStockLevels(query?: GetStockLevelsListQuery): Observable<ApiResponse<PagedResult<StockLevel>>> {
    return this.http.post<ApiResponse<PagedResult<StockLevel>>>(`${this.apiUrl}/stock-levels/list`, query || {});
  }

  transferStock(request: {
    productId: string;
    fromWarehouseId: string;
    toWarehouseId: string;
    quantity: number;
    notes?: string;
    batchNumber?: string;
    expiryDate?: string;
  }): Observable<ApiResponse<StockTransaction[]>> {
    return this.http.post<ApiResponse<StockTransaction[]>>(`${this.apiUrl}/adjust-stock`, request);
  }

  getBatches(query?: any): Observable<ApiResponse<PagedResult<any>>> {
    return this.http.post<ApiResponse<PagedResult<any>>>(`${this.apiUrl}/batches/list`, query || {});
  }

  recallBatch(id: string, request: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/batches/${id}/recall`, request);
  }

  getSerialNumbers(query?: any): Observable<ApiResponse<PagedResult<any>>> {
    return this.http.post<ApiResponse<PagedResult<any>>>(`${this.apiUrl}/serial-numbers/list`, query || {});
  }

  createBatch(command: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/batches`, command);
  }

  createSerialNumber(command: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/serial-numbers`, command);
  }
}

