import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, PagedResult } from '../models/api-response.model';
import {
  Category,
  CategoryTree,
  CreateCategoryRequest,
  UpdateCategoryRequest,
  GetCategoriesListQuery
} from '../models/category.model';
import { ApiConfigService } from './api-config.service';

@Injectable({
  providedIn: 'root'
})
export class CategoryApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('categories');
  }

  getCategory(id: string): Observable<ApiResponse<Category>> {
    return this.http.get<ApiResponse<Category>>(`${this.apiUrl}/${id}`);
  }

  getCategories(query?: GetCategoriesListQuery): Observable<ApiResponse<PagedResult<Category>>> {
    return this.http.post<ApiResponse<PagedResult<Category>>>(`${this.apiUrl}/list`, query || {});
  }

  getCategoryTree(): Observable<ApiResponse<CategoryTree[]>> {
    return this.http.get<ApiResponse<CategoryTree[]>>(`${this.apiUrl}/tree`);
  }

  createCategory(request: CreateCategoryRequest): Observable<ApiResponse<Category>> {
    return this.http.post<ApiResponse<Category>>(this.apiUrl, request);
  }

  updateCategory(id: string, request: UpdateCategoryRequest): Observable<ApiResponse<Category>> {
    return this.http.put<ApiResponse<Category>>(`${this.apiUrl}/${id}`, request);
  }

  deleteCategory(id: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/${id}`);
  }
}

