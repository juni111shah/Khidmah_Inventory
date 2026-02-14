import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, PagedResult } from '../models/api-response.model';
import { Company, CreateCompanyRequest, UpdateCompanyRequest } from '../models/company.model';
import { ApiConfigService } from './api-config.service';

@Injectable({ providedIn: 'root' })
export class CompanyApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('companies');
  }

  getList(filterRequest?: unknown): Observable<ApiResponse<PagedResult<Company>>> {
    return this.http.post<ApiResponse<PagedResult<Company>>>(`${this.apiUrl}/list`, filterRequest || {});
  }

  getById(id: string): Observable<ApiResponse<Company>> {
    return this.http.get<ApiResponse<Company>>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateCompanyRequest): Observable<ApiResponse<Company>> {
    return this.http.post<ApiResponse<Company>>(this.apiUrl, request);
  }

  update(id: string, request: UpdateCompanyRequest): Observable<ApiResponse<Company>> {
    return this.http.put<ApiResponse<Company>>(`${this.apiUrl}/${id}`, request);
  }

  uploadLogo(companyId: string, file: File): Observable<ApiResponse<{ logoUrl: string }>> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<ApiResponse<{ logoUrl: string }>>(`${this.apiUrl}/${companyId}/logo`, formData);
  }

  activate(id: string): Observable<ApiResponse<Company>> {
    return this.http.patch<ApiResponse<Company>>(`${this.apiUrl}/${id}/activate`, null);
  }

  deactivate(id: string): Observable<ApiResponse<Company>> {
    return this.http.patch<ApiResponse<Company>>(`${this.apiUrl}/${id}/deactivate`, null);
  }
}
