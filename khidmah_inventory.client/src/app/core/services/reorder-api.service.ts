import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import {
  ReorderSuggestion,
  GetReorderSuggestionsQuery,
  GeneratePOFromSuggestionsRequest
} from '../models/reorder.model';
import { ApiConfigService } from './api-config.service';
import { PurchaseOrder } from '../models/purchase-order.model';

@Injectable({ providedIn: 'root' })
export class ReorderApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('reordering');
  }

  getSuggestions(query?: GetReorderSuggestionsQuery): Observable<ApiResponse<ReorderSuggestion[]>> {
    let params = new HttpParams();
    if (query?.warehouseId) params = params.set('warehouseId', query.warehouseId);
    if (query?.priority) params = params.set('priority', query.priority);
    if (query?.includeInStock !== undefined) params = params.set('includeInStock', String(query.includeInStock));
    return this.http.get<ApiResponse<ReorderSuggestion[]>>(`${this.apiUrl}/suggestions`, { params });
  }

  generatePurchaseOrder(request: GeneratePOFromSuggestionsRequest): Observable<ApiResponse<PurchaseOrder>> {
    return this.http.post<ApiResponse<PurchaseOrder>>(`${this.apiUrl}/generate-po`, request);
  }
}
