import { Injectable } from '@angular/core';
import { Observable, forkJoin, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { ApiResponse } from '../models/api-response.model';
import {
  DailyBriefing,
  BriefingStatus,
  LowStockRiskItem,
  ExpiringBatchItem,
  PendingApprovalItem,
  BriefingProductItem,
  UnusualActivityItem,
  AiSuggestionItem
} from '../models/daily-briefing.model';
import { ApiConfigService } from './api-config.service';
import { DashboardApiService } from './dashboard-api.service';
import { ReorderApiService } from './reorder-api.service';
import { WorkflowApiService } from './workflow-api.service';
import { PredictiveRiskApiService } from './predictive-risk-api.service';

@Injectable({ providedIn: 'root' })
export class DailyBriefingApiService {
  constructor(
    private apiConfig: ApiConfigService,
    private dashboardApi: DashboardApiService,
    private reorderApi: ReorderApiService,
    private workflowApi: WorkflowApiService,
    private predictiveRiskApi: PredictiveRiskApiService
  ) {}

  getBriefing(): Observable<ApiResponse<DailyBriefing>> {
    const now = new Date();
    const startOfMonth = new Date(now.getFullYear(), now.getMonth(), 1);
    const endOfMonth = new Date(now.getFullYear(), now.getMonth() + 1, 0);

    return forkJoin({
      dashboard: this.dashboardApi.getDashboardData(startOfMonth, endOfMonth).pipe(
        catchError(() => of({ success: false, data: undefined } as ApiResponse<any>))
      ),
      reorder: this.reorderApi.getSuggestions({}).pipe(
        catchError(() => of({ success: true, data: [] } as ApiResponse<any>))
      ),
      pending: this.workflowApi.getPendingApprovals().pipe(
        catchError(() => of({ success: true, data: [] } as ApiResponse<any>))
      ),
      risks: this.predictiveRiskApi.getRisks().pipe(
        catchError(() => of({ success: true, data: { risks: [], summary: { high: 0, medium: 0, low: 0 } } } as ApiResponse<any>))
      )
    }).pipe(
      map(({ dashboard, reorder, pending, risks }) => {
        const d = dashboard.data;
        const suggestions: any[] = (reorder as any).data || [];
        const pendingList: any[] = (pending as any).data || [];
        const riskItems: any[] = (risks?.data as any)?.risks || [];

        const yesterday = new Date(now);
        yesterday.setDate(yesterday.getDate() - 1);
        const daysInMonth = new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate();
        const daysElapsed = now.getDate();
        const monthlySales = d?.summary?.monthlySales ?? 0;
        const todaySales = d?.summary?.todaySales ?? 0;
        const monthlyPurchases = d?.summary?.monthlyPurchases ?? 0;
        const yesterdaySalesEst = daysElapsed > 1
          ? Math.max(0, (monthlySales - todaySales) / Math.max(1, daysElapsed - 1))
          : monthlySales / Math.max(1, daysInMonth);
        const prevDayAvg = daysElapsed > 1 ? (monthlySales - todaySales) / (daysElapsed - 1) : 0;
        const trendPercent = prevDayAvg > 0
          ? Math.round(((todaySales - prevDayAvg) / prevDayAvg) * 1000) / 10
          : (todaySales > 0 ? 10 : 0);
        const salesTrendDirection: 'up' | 'down' | 'flat' = trendPercent > 1 ? 'up' : trendPercent < -1 ? 'down' : 'flat';

        const criticalCount = suggestions.filter((s: any) => s.priority === 'Critical').length;
        const highCount = suggestions.filter((s: any) => s.priority === 'High').length;
        const lowStockStatus: BriefingStatus = criticalCount > 0 ? 'CRITICAL' : highCount > 0 ? 'WATCH' : 'GOOD';
        const lowStockRisks: LowStockRiskItem[] = suggestions.slice(0, 10).map((s: any) => ({
          productId: s.productId,
          productName: s.productName,
          warehouseName: s.warehouseName,
          currentStock: s.currentStock,
          minStockLevel: s.minStockLevel ?? 0,
          priority: s.priority,
          link: `/products/${s.productId}`
        }));

        const expiringRisks = riskItems.filter((r: any) => r.type === 'Expiry' || (r.title && r.title.toLowerCase().includes('expir')));
        const expiringBatches: ExpiringBatchItem[] = expiringRisks.slice(0, 5).map((r: any) => ({
          productName: r.productName || r.title || 'Product',
          batchNumber: (r.description && r.description.match(/#[\w-]+/)?.[0]) || '—',
          quantity: 0,
          expiryDate: '',
          daysUntilExpiry: 14,
          link: r.link
        }));
        const expiringStatus: BriefingStatus = expiringBatches.length > 3 ? 'CRITICAL' : expiringBatches.length > 0 ? 'WATCH' : 'GOOD';

        const pendingApprovals: PendingApprovalItem[] = pendingList.slice(0, 10).map((w: any) => ({
          id: w.id,
          entityType: w.entityType,
          entityId: w.entityId,
          currentStep: w.currentStep,
          description: `${w.entityType} - ${w.currentStep}`,
          link: '/workflows/inbox'
        }));

        const topProducts: BriefingProductItem[] = (d?.topProducts || []).slice(0, 5).map((p: any, i: number) => ({
          productId: p.productId,
          productName: p.productName,
          value: p.totalSales ?? 0,
          trend: i % 3 === 0 ? 'up' : i % 3 === 1 ? 'down' : 'flat',
          unit: 'sales',
          link: `/products/${p.productId}`
        }));

        const profitEst = (monthlySales ?? 0) - (monthlyPurchases ?? 0);
        const profitTrendPercent = monthlyPurchases > 0 ? Math.round(((profitEst / monthlyPurchases) * 100) * 10) / 10 : 0;
        const profitTrendDirection: 'up' | 'down' | 'flat' = profitTrendPercent > 5 ? 'up' : profitTrendPercent < -5 ? 'down' : 'flat';

        const unusualActivity: UnusualActivityItem[] = riskItems
          .filter((r: any) => r.severity === 'high' || r.type === 'AbnormalSales')
          .slice(0, 5)
          .map((r: any) => ({
            id: r.id,
            type: r.type || 'Alert',
            description: r.description || r.title,
            severity: (r.severity === 'high' ? 'high' : 'medium') as 'high' | 'medium' | 'low',
            link: r.link
          }));

        const aiSuggestions: AiSuggestionItem[] = [];
        if (criticalCount > 0) {
          aiSuggestions.push({
            id: 'reorder',
            title: 'Reorder critical items',
            description: `${criticalCount} product(s) at or below minimum stock.`,
            actionLabel: 'View reorder list',
            actionLink: '/reorder',
            priority: 'CRITICAL'
          });
        }
        if (pendingList.length > 0) {
          aiSuggestions.push({
            id: 'approvals',
            title: 'Pending approvals',
            description: `${pendingList.length} workflow(s) waiting for your action.`,
            actionLabel: 'Open inbox',
            actionLink: '/workflows/inbox',
            priority: pendingList.length > 5 ? 'CRITICAL' : 'WATCH'
          });
        }
        if (expiringBatches.length > 0) {
          aiSuggestions.push({
            id: 'expiry',
            title: 'Batches expiring soon',
            description: `${expiringBatches.length} batch(es) need attention.`,
            actionLabel: 'View batches',
            actionLink: '/inventory/batches',
            priority: expiringStatus
          });
        }
        if (aiSuggestions.length === 0) {
          aiSuggestions.push({
            id: 'ok',
            title: 'All clear',
            description: 'No critical actions right now. Check dashboard for details.',
            actionLabel: 'Go to dashboard',
            actionLink: '/dashboard',
            priority: 'GOOD'
          });
        }

        const slowFromRisks = riskItems.filter((r: any) => r.type === 'Overstock' || (r.title && r.title.toLowerCase().includes('overstock')));
        const slowMovingItems: BriefingProductItem[] = slowFromRisks.slice(0, 3).map((r: any) => ({
          productId: r.entityId || '',
          productName: r.productName || r.title || 'Product',
          value: r.metric ?? 0,
          trend: 'down' as const,
          link: r.link
        }));

        const data: DailyBriefing = {
          generatedAt: now.toISOString(),
          yesterdaySales: yesterdaySalesEst,
          salesTrendPercent: Math.abs(trendPercent),
          salesTrendDirection,
          lowStockRisks,
          lowStockStatus,
          expiringBatches,
          expiringStatus,
          pendingApprovals,
          pendingApprovalsCount: pendingList.length,
          topSellingProducts: topProducts,
          slowMovingItems: slowMovingItems.length ? slowMovingItems : [{ productId: '', productName: 'No slow movers flagged', value: 0, link: '/reports' }] as BriefingProductItem[],
          estimatedProfit: profitEst,
          profitTrendPercent: Math.abs(profitTrendPercent),
          profitTrendDirection,
          unusualActivity,
          aiSuggestions
        };

        return { success: true, data, message: '', statusCode: 200, errors: [], timestamp: new Date().toISOString() } as ApiResponse<DailyBriefing>;
      })
    );
  }
}
