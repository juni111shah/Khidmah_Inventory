import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { Permission } from '../models/role.model';
import { ApiConfigService } from './api-config.service';

@Injectable({
  providedIn: 'root'
})
export class PermissionApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('permissions');
  }

  getPermissions(module?: string): Observable<ApiResponse<Permission[]>> {
    let params = new HttpParams();
    if (module) {
      params = params.set('module', module);
    }
    return this.http.get<ApiResponse<Permission[]>>(this.apiUrl, { params });
  }
}

