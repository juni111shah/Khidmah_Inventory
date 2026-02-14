import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ApiResponse } from '../models/api-response.model';
import { ProfitIntelligenceData } from '../models/profit-intelligence.model';
import { ApiConfigService } from './api-config.service';
import { AnalyticsApiService } from './analytics-api.service';

@Injectable({ providedIn: 'root' })
export class ProfitIntelligenceApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService,
    private analyticsApi: AnalyticsApiService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('analytics');
  }

  getProfitIntelligence(warehouseId?: string): Observable<ApiResponse<ProfitIntelligenceData>> {
    return this.http.get<ApiResponse<ProfitIntelligenceData>>(`${this.apiUrl}/profit-intelligence`, {
      params: warehouseId ? new HttpParams().set('warehouseId', warehouseId) : undefined
    }).pipe(
      catchError(() => this.getProfitIntelligenceMock())
    );
  }

  getProfitIntelligenceMock(): Observable<ApiResponse<ProfitIntelligenceData>> {
    const data: ProfitIntelligenceData = {
      marginByProduct: [
        { productId: '1', productName: 'Product A', categoryName: 'Electronics', revenue: 5000, cost: 3200, profit: 1800, marginPercent: 36 },
        { productId: '2', productName: 'Product B', categoryName: 'Electronics', revenue: 3200, cost: 2400, profit: 800, marginPercent: 25 }
      ],
      marginByCategory: [
        { categoryId: 'c1', categoryName: 'Electronics', revenue: 25000, cost: 16000, profit: 9000, marginPercent: 36, productCount: 12 },
        { categoryId: 'c2', categoryName: 'FMCG', revenue: 18000, cost: 12000, profit: 6000, marginPercent: 33, productCount: 8 }
      ],
      deadStock: [
        { productId: 'd1', productName: 'Old SKU', sku: 'SKU-OLD', quantity: 50, value: 500, daysInStock: 120 }
      ],
      agingInventory: [
        { range: '0-30 days', count: 80, value: 12000, percent: 60 },
        { range: '31-90 days', count: 35, value: 5000, percent: 25 },
        { range: '90+ days', count: 15, value: 2000, percent: 15 }
      ],
      capitalLocked: 19000,
      fastMovers: [],
      slowMovers: [],
      summary: { totalMarginPercent: 32, totalDeadStockValue: 500, totalCapitalLocked: 19000 }
    };
    return of({ success: true, data, message: '', statusCode: 200, errors: [], timestamp: new Date().toISOString() });
  }
}
