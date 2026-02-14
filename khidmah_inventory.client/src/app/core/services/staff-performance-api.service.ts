import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { ApiResponse } from '../models/api-response.model';
import { StaffPerformanceData } from '../models/staff-performance.model';
import { ApiConfigService } from './api-config.service';

@Injectable({ providedIn: 'root' })
export class StaffPerformanceApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('analytics');
  }

  getStaffPerformance(periodFrom?: string, periodTo?: string): Observable<ApiResponse<StaffPerformanceData>> {
    let params = new HttpParams();
    if (periodFrom) params = params.set('from', periodFrom);
    if (periodTo) params = params.set('to', periodTo);
    return this.http.get<ApiResponse<StaffPerformanceData>>(`${this.apiUrl}/staff-performance`, { params }).pipe(
      delay(0)
    );
  }

  getStaffPerformanceMock(): Observable<ApiResponse<StaffPerformanceData>> {
    const now = new Date();
    const to = now.toISOString().split('T')[0];
    const from = new Date(now.getFullYear(), now.getMonth(), 1).toISOString().split('T')[0];
    const mock: StaffPerformanceData = {
      periodFrom: from,
      periodTo: to,
      staff: [
        { userId: 'u1', userName: 'John Doe', salesTotal: 12500, transactionCount: 85, averageTransactionValue: 147, discountRate: 0.12, averageDiscountPercent: 5, averageTransactionSpeedSeconds: 90, rank: 1 },
        { userId: 'u2', userName: 'Jane Smith', salesTotal: 9800, transactionCount: 72, averageTransactionValue: 136, discountRate: 0.18, averageDiscountPercent: 7, averageTransactionSpeedSeconds: 110, rank: 2 },
        { userId: 'u3', userName: 'Bob Wilson', salesTotal: 8200, transactionCount: 95, averageTransactionValue: 86, discountRate: 0.25, averageDiscountPercent: 10, averageTransactionSpeedSeconds: 75, approvalDelayHours: 4, rank: 3 }
      ]
    };
    return of({ success: true, data: mock, message: '', statusCode: 200, errors: [], timestamp: new Date().toISOString() }).pipe(delay(400));
  }
}
