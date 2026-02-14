import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { ApiResponse } from '../models/api-response.model';
import { CommandCenterData } from '../models/command-center.model';
import { Dashboard } from '../models/dashboard.model';
import { ApiConfigService } from './api-config.service';
import { DashboardApiService } from './dashboard-api.service';
import { ReorderApiService } from './reorder-api.service';
import { WorkflowApiService } from './workflow-api.service';

@Injectable({ providedIn: 'root' })
export class CommandCenterApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService,
    private dashboardApi: DashboardApiService,
    private reorderApi: ReorderApiService,
    private workflowApi: WorkflowApiService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('dashboard');
  }

  /** Aggregates dashboard, reorder suggestions, and pending approvals into executive view */
  getCommandCenterData(): Observable<ApiResponse<CommandCenterData>> {
    const now = new Date();
    const startOfMonth = new Date(now.getFullYear(), now.getMonth(), 1);
    const startOfWeek = new Date(now);
    startOfWeek.setDate(now.getDate() - 7);

    return forkJoin({
      dashboard: this.dashboardApi.getDashboardData(startOfMonth, now).pipe(
        catchError(() => of({ success: false, data: undefined, message: '', statusCode: 0, errors: [], timestamp: new Date().toISOString() } as ApiResponse<Dashboard>))
      ),
      reorder: this.reorderApi.getSuggestions({}).pipe(
        catchError(() => of({ success: true, data: [] }))
      ),
      pending: this.workflowApi.getPendingApprovals().pipe(
        catchError(() => of({ success: true, data: [] }))
      )
    }).pipe(
      map(({ dashboard, reorder, pending }) => {
        const d = dashboard.data;
        const suggestions = (reorder as any).data || [];
        const pendingList = (pending as any).data || [];
        const criticalCount = suggestions.filter((s: any) => s.priority === 'Critical').length;

        const data: CommandCenterData = {
          dashboard: d || null,
          salesToday: d?.summary?.todaySales ?? 0,
          salesWeek: (d?.summary?.monthlySales ?? 0) / 4,
          salesMonth: d?.summary?.monthlySales ?? 0,
          purchaseVolume: d?.summary?.monthlyPurchases ?? 0,
          profitEstimate: (d?.summary?.monthlySales ?? 0) - (d?.summary?.monthlyPurchases ?? 0),
          lowStockAlerts: d?.summary?.lowStockItems ?? criticalCount,
          expiringItemsCount: 0,
          pendingApprovalsCount: pendingList.length,
          topProducts: (d?.topProducts || []).slice(0, 5).map((p: any, i: number) => ({
            productName: p.productName,
            productId: p.productId,
            value: p.totalSales ?? 0,
            trend: i % 3 === 0 ? 'up' : i % 3 === 1 ? 'down' : 'flat'
          })),
          warehouseUtilization: [],
          recentActivities: (d?.recentOrders || []).slice(0, 8).map((o: any) => ({
            id: o.id,
            type: o.type || 'Order',
            description: `${o.orderNumber} - ${o.customerOrSupplierName}`,
            timeAgo: formatTimeAgo(o.orderDate),
            link: o.type === 'Sales' ? `/sales-orders/${o.id}` : `/purchase-orders/${o.id}`
          }))
        };
        return { success: true, data, message: '', statusCode: 200, errors: [], timestamp: new Date().toISOString() };
      })
    );
  }

  /** Optional: dedicated executive endpoint when backend adds it */
  getExecutiveDashboard(): Observable<ApiResponse<CommandCenterData>> {
    return this.http.get<ApiResponse<CommandCenterData>>(`${this.apiUrl}/executive`).pipe(
      catchError(() => this.getCommandCenterData())
    );
  }
}

function formatTimeAgo(dateStr: string): string {
  const d = new Date(dateStr);
  const diff = (Date.now() - d.getTime()) / 60000;
  if (diff < 60) return `${Math.round(diff)}m ago`;
  if (diff < 1440) return `${Math.round(diff / 60)}h ago`;
  return `${Math.round(diff / 1440)}d ago`;
}
