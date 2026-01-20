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
    return this.http.post<ApiResponse<StockTransaction>>(`${this.apiUrl}/transactions`, request);
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
    return this.http.post<ApiResponse<StockTransaction[]>>(`${this.apiUrl}/transfer`, request);
  }
}

