import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, PagedResult } from '../models/api-response.model';
import {
  Customer,
  CreateCustomerRequest,
  GetCustomersListQuery
} from '../models/customer.model';
import { ApiConfigService } from './api-config.service';

@Injectable({
  providedIn: 'root'
})
export class CustomerApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('customers');
  }

  getCustomers(query?: GetCustomersListQuery): Observable<ApiResponse<PagedResult<Customer>>> {
    return this.http.post<ApiResponse<PagedResult<Customer>>>(`${this.apiUrl}/list`, query || {});
  }

  getCustomer(id: string): Observable<ApiResponse<Customer>> {
    return this.http.get<ApiResponse<Customer>>(`${this.apiUrl}/${id}`);
  }

  updateCustomer(id: string, request: any): Observable<ApiResponse<Customer>> {
    return this.http.put<ApiResponse<Customer>>(`${this.apiUrl}/${id}`, request);
  }

  createCustomer(request: CreateCustomerRequest): Observable<ApiResponse<Customer>> {
    return this.http.post<ApiResponse<Customer>>(this.apiUrl, request);
  }
}

