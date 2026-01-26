import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import {
  User,
  UpdateUserProfileRequest,
  ChangePasswordRequest,
  PagedResult,
  FilterRequest
} from '../models/user.model';

interface CreateUserRequest {
  email: string;
  userName: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  password: string;
  roles: string[];
  companyId?: string;
}
import { ApiConfigService } from './api-config.service';

@Injectable({
  providedIn: 'root'
})
export class UserApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('users');
  }

  getCurrentUser(): Observable<ApiResponse<User>> {
    return this.http.get<ApiResponse<User>>(`${this.apiUrl}/current`);
  }

  getUser(id: string): Observable<ApiResponse<User>> {
    return this.http.get<ApiResponse<User>>(`${this.apiUrl}/${id}`);
  }

  getUsers(filterRequest?: FilterRequest): Observable<ApiResponse<PagedResult<User>>> {
    return this.http.post<ApiResponse<PagedResult<User>>>(`${this.apiUrl}/list`, filterRequest || {});
  }

  createUser(user: CreateUserRequest): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(this.apiUrl, user);
  }

  updateProfile(id: string, profile: UpdateUserProfileRequest): Observable<ApiResponse<User>> {
    return this.http.put<ApiResponse<User>>(`${this.apiUrl}/${id}/profile`, profile);
  }

  changePassword(id: string, passwords: ChangePasswordRequest): Observable<ApiResponse<void>> {
    return this.http.post<ApiResponse<void>>(`${this.apiUrl}/${id}/change-password`, passwords);
  }

  activateUser(id: string): Observable<ApiResponse<User>> {
    return this.http.patch<ApiResponse<User>>(`${this.apiUrl}/${id}/activate`, null);
  }

  deactivateUser(id: string): Observable<ApiResponse<User>> {
    return this.http.patch<ApiResponse<User>>(`${this.apiUrl}/${id}/deactivate`, null);
  }
}

