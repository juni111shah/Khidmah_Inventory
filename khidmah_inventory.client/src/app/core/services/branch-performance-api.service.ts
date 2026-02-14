import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { ApiResponse } from '../models/api-response.model';
import { BranchComparisonData } from '../models/branch-performance.model';
import { ApiConfigService } from './api-config.service';

@Injectable({ providedIn: 'root' })
export class BranchPerformanceApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('analytics');
  }

  getBranchComparison(periodFrom?: string, periodTo?: string): Observable<ApiResponse<BranchComparisonData>> {
    let params = new HttpParams();
    if (periodFrom) params = params.set('from', periodFrom);
    if (periodTo) params = params.set('to', periodTo);
    return this.http.get<ApiResponse<BranchComparisonData>>(`${this.apiUrl}/branch-comparison`, { params }).pipe(
      delay(0)
    );
  }

  getBranchComparisonMock(): Observable<ApiResponse<BranchComparisonData>> {
    const now = new Date();
    const to = now.toISOString().split('T')[0];
    const from = new Date(now.getFullYear(), now.getMonth() - 1, 1).toISOString().split('T')[0];
    const mock: BranchComparisonData = {
      periodFrom: from,
      periodTo: to,
      branches: [
        { id: 'w1', name: 'Main Warehouse', type: 'warehouse', revenue: 85000, revenueGrowth: 12, orderCount: 420, stockHealthScore: 88, lowStockCount: 5, rank: 1, trend: 'up' },
        { id: 'w2', name: 'North Branch', type: 'warehouse', revenue: 42000, revenueGrowth: 5, orderCount: 210, stockHealthScore: 72, lowStockCount: 12, rank: 2, trend: 'flat' },
        { id: 'w3', name: 'South Branch', type: 'warehouse', revenue: 38000, revenueGrowth: -2, orderCount: 180, stockHealthScore: 65, lowStockCount: 18, rank: 3, trend: 'down' }
      ]
    };
    return of({ success: true, data: mock, message: '', statusCode: 200, errors: [], timestamp: new Date().toISOString() }).pipe(delay(400));
  }
}
