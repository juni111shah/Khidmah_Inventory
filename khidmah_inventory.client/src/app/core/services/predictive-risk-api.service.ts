import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, forkJoin } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { ApiResponse } from '../models/api-response.model';
import { PredictiveRiskData, RiskItem } from '../models/predictive-risk.model';
import { ApiConfigService } from './api-config.service';
import { ReorderApiService } from './reorder-api.service';

@Injectable({ providedIn: 'root' })
export class PredictiveRiskApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService,
    private reorderApi: ReorderApiService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('analytics');
  }

  getRisks(): Observable<ApiResponse<PredictiveRiskData>> {
    return this.http.get<ApiResponse<PredictiveRiskData>>(`${this.apiUrl}/predictive-risks`).pipe(
      catchError(() => this.getRisksFromReorderAndMock())
    );
  }

  getRisksFromReorderAndMock(): Observable<ApiResponse<PredictiveRiskData>> {
    return this.reorderApi.getSuggestions({}).pipe(
      map(reorderRes => {
        const suggestions = (reorderRes as any).data || [];
        const risks: RiskItem[] = suggestions
          .filter((s: any) => s.priority === 'Critical' || s.priority === 'High')
          .slice(0, 15)
          .map((s: any) => ({
            id: `risk-${s.productId}-${s.warehouseId}`,
            type: 'OutOfStockSoon' as const,
            severity: s.priority === 'Critical' ? 'high' : 'medium',
            title: `Low stock: ${s.productName}`,
            description: `Current: ${s.currentStock}, days left: ${s.daysOfStockRemaining}. Reorder point: ${s.reorderPoint ?? 'N/A'}`,
            entityType: 'Product',
            entityId: s.productId,
            productName: s.productName,
            warehouseName: s.warehouseName,
            metric: s.daysOfStockRemaining,
            unit: 'days',
            suggestedAction: 'Create purchase order from reorder list',
            link: '/reorder/list',
            createdAt: new Date().toISOString()
          }));

        risks.push({
          id: 'risk-expiry-1',
          type: 'Expiry',
          severity: 'medium',
          title: 'Batch expiring in 14 days',
          description: 'Beverages batch #B-2024-001, 120 units',
          entityType: 'Batch',
          entityId: 'b1',
          suggestedAction: 'Run promotion or move to clearance',
          link: '/inventory/batches',
          createdAt: new Date().toISOString()
        });
        risks.push({
          id: 'risk-overstock-1',
          type: 'Overstock',
          severity: 'low',
          title: 'Overstock: Seasonal item',
          description: 'Product "Winter Pack" 45 days supply',
          entityType: 'Product',
          entityId: 'p2',
          productName: 'Winter Pack',
          metric: 45,
          unit: 'days supply',
          suggestedAction: 'Review demand forecast',
          link: '/products/p2',
          createdAt: new Date().toISOString()
        });

        const high = risks.filter(r => r.severity === 'high').length;
        const medium = risks.filter(r => r.severity === 'medium').length;
        const low = risks.filter(r => r.severity === 'low').length;

        const data: PredictiveRiskData = {
          risks,
          summary: { high, medium, low }
        };
        return { success: true, data, message: '', statusCode: 200, errors: [], timestamp: new Date().toISOString() };
      }),
      catchError(() => of({
        success: true,
        data: { risks: [], summary: { high: 0, medium: 0, low: 0 } },
        message: '', statusCode: 200, errors: [], timestamp: new Date().toISOString()
      }))
    );
  }
}
