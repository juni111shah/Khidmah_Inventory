import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, PagedResult } from '../models/api-response.model';
import {
  Supplier,
  CreateSupplierRequest,
  GetSuppliersListQuery
} from '../models/supplier.model';
import { ApiConfigService } from './api-config.service';

@Injectable({
  providedIn: 'root'
})
export class SupplierApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('suppliers');
  }

  getSuppliers(query?: GetSuppliersListQuery): Observable<ApiResponse<PagedResult<Supplier>>> {
    return this.http.post<ApiResponse<PagedResult<Supplier>>>(`${this.apiUrl}/list`, query || {});
  }

  createSupplier(request: CreateSupplierRequest): Observable<ApiResponse<Supplier>> {
    return this.http.post<ApiResponse<Supplier>>(this.apiUrl, request);
  }
}

