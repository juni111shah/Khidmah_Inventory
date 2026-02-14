import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { ApiConfigService } from './api-config.service';
import {
  HandsFreeTasksResponse,
  CompleteHandsFreeTaskRequest,
  HandsFreeSupervisorWorkerDto
} from '../models/hands-free.model';

@Injectable({
  providedIn: 'root'
})
export class HandsFreeApiService {
  private baseUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.baseUrl = this.apiConfig.getApiUrl('warehouse/handsfree');
  }

  getTasks(warehouseId: string, maxTasks = 50): Observable<ApiResponse<HandsFreeTasksResponse>> {
    const params = new HttpParams()
      .set('warehouseId', warehouseId)
      .set('maxTasks', maxTasks.toString());
    return this.http.get<ApiResponse<HandsFreeTasksResponse>>(`${this.baseUrl}/tasks`, { params });
  }

  completeTask(request: CompleteHandsFreeTaskRequest): Observable<ApiResponse<unknown>> {
    return this.http.post<ApiResponse<unknown>>(`${this.baseUrl}/complete`, request);
  }

  validateBarcode(code: string): Observable<ApiResponse<{ productId: string; productName: string; barcode: string | null; sku: string }>> {
    const params = new HttpParams().set('code', code);
    return this.http.get<ApiResponse<{ productId: string; productName: string; barcode: string | null; sku: string }>>(
      `${this.baseUrl}/validate-barcode`,
      { params }
    );
  }

  getSessions(activeWithinMinutes = 60): Observable<ApiResponse<HandsFreeSupervisorWorkerDto[]>> {
    const params = new HttpParams().set('activeWithinMinutes', activeWithinMinutes.toString());
    return this.http.get<ApiResponse<HandsFreeSupervisorWorkerDto[]>>(`${this.baseUrl}/sessions`, { params });
  }
}
