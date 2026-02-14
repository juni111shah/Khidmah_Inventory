import { Injectable } from '@angular/core';
import { Observable, forkJoin, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { ApiResponse } from '../models/api-response.model';
import {
  ExplainableInsight,
  WhatIfResult,
  WhatIfRequest,
  OptimizationSuggestion,
  Opportunity,
  ManagementStory,
  BusinessHealthScore,
  HealthFactor,
  Anomaly,
  DecisionSupportSummary,
  InsightReason
} from '../models/decision-support.model';
import { DashboardApiService } from './dashboard-api.service';
import { ReorderApiService } from './reorder-api.service';
import { PredictiveRiskApiService } from './predictive-risk-api.service';
import { AiApiService } from './ai-api.service';
import { ProfitIntelligenceApiService } from './profit-intelligence-api.service';

@Injectable({ providedIn: 'root' })
export class DecisionSupportApiService {
  constructor(
    private dashboardApi: DashboardApiService,
    private reorderApi: ReorderApiService,
    private predictiveRiskApi: PredictiveRiskApiService,
    private aiApi: AiApiService,
    private profitIntelligenceApi: ProfitIntelligenceApiService
  ) {}

  getSummary(): Observable<ApiResponse<DecisionSupportSummary>> {
    const now = new Date();
    const startOfMonth = new Date(now.getFullYear(), now.getMonth(), 1);
    return forkJoin({
      dashboard: this.dashboardApi.getDashboardData(startOfMonth, now).pipe(catchError(() => of({ success: false, data: undefined } as ApiResponse<any>))),
      reorder: this.reorderApi.getSuggestions({}).pipe(catchError(() => of({ success: true, data: [] } as ApiResponse<any>))),
      risks: this.predictiveRiskApi.getRisks().pipe(catchError(() => of({ success: true, data: { risks: [], summary: { high: 0, medium: 0, low: 0 } } } as ApiResponse<any>))),
      profit: this.profitIntelligenceApi.getProfitIntelligence().pipe(catchError(() => of({ success: true, data: null } as ApiResponse<any>)))
    }).pipe(
      map(({ dashboard, reorder, risks, profit }) => {
        const d = dashboard.data;
        const suggestions: any[] = (reorder as any).data || [];
        const riskItems: any[] = (risks?.data as any)?.risks || [];
        const profitData = (profit as any)?.data;

        const explainableInsights = this.buildExplainableInsights(suggestions, riskItems);
        const optimizationSuggestions = this.buildOptimizationSuggestions(suggestions, profitData, riskItems);
        const opportunities = this.buildOpportunities(d, profitData);
        const managementStory = this.buildManagementStory(d, suggestions, riskItems);
        const businessHealth = this.buildBusinessHealthScore(d, suggestions, riskItems);
        const anomalies = this.buildAnomalies(riskItems);

        const summary: DecisionSupportSummary = {
          explainableInsights,
          optimizationSuggestions,
          opportunities,
          managementStory,
          businessHealth,
          anomalies
        };
        return { success: true, data: summary, message: '', statusCode: 200, errors: [], timestamp: new Date().toISOString() } as ApiResponse<DecisionSupportSummary>;
      })
    );
  }

  getExplainableInsight(productId: string, type: 'reorder' | 'price_change' | 'risk'): Observable<ApiResponse<ExplainableInsight>> {
    return this.reorderApi.getSuggestions({}).pipe(
      map(res => {
        const list = (res as any).data || [];
        const s = list.find((x: any) => x.productId === productId);
        if (!s) return { success: false, data: undefined, message: 'Not found', statusCode: 404, errors: [], timestamp: new Date().toISOString() } as ApiResponse<ExplainableInsight>;
        const reasons: InsightReason[] = [
          { label: 'Demand trend', value: s.averageDailySales ?? 0, unit: 'units/day', trend: (s.averageDailySales ?? 0) > 0 ? 'up' : 'stable' },
          { label: 'Sales velocity', value: s.averageDailySales ?? 0, unit: 'per day' },
          { label: 'Lead time', value: s.supplierSuggestions?.[0]?.averageDeliveryDays ?? 7, unit: 'days' },
          { label: 'Stock days remaining', value: s.daysOfStockRemaining, unit: 'days', trend: s.daysOfStockRemaining < 7 ? 'down' : 'stable' }
        ];
        const insight: ExplainableInsight = {
          type: 'reorder',
          entityId: productId,
          entityName: s.productName,
          title: `Why reorder ${s.productName}`,
          reasons,
          severity: s.priority === 'Critical' ? 'critical' : s.priority === 'High' ? 'warning' : 'info',
          suggestedAction: `Order ${s.suggestedQuantity} units`,
          link: '/reorder'
        };
        return { success: true, data: insight, message: '', statusCode: 200, errors: [], timestamp: new Date().toISOString() } as ApiResponse<ExplainableInsight>;
      }),
      catchError(() => of({ success: false, data: undefined, message: 'Error', statusCode: 500, errors: [], timestamp: new Date().toISOString() } as ApiResponse<ExplainableInsight>))
    );
  }

  runWhatIfSimulation(request: WhatIfRequest): Observable<ApiResponse<WhatIfResult>> {
    const currentPrice = request.currentPrice ?? 100;
    const currentStock = request.currentStock ?? 50;
    const dailyDemand = request.dailyDemand ?? 5;
    let scenario = '';
    const inputs: { label: string; value: string | number }[] = [];
    let revenue = currentPrice * currentStock;
    let margin = 0.25;
    let stockoutDays = dailyDemand > 0 ? currentStock / dailyDemand : 999;
    let stockoutDate: string | undefined;

    if (request.type === 'price') {
      const pct = request.priceChangePercent ?? 0;
      scenario = `Price ${pct >= 0 ? '+' : ''}${pct}%`;
      inputs.push({ label: 'Price change', value: `${pct}%` });
      const newPrice = currentPrice * (1 + pct / 100);
      revenue = newPrice * currentStock;
      stockoutDate = new Date(Date.now() + stockoutDays * 86400000).toISOString().slice(0, 10);
    } else if (request.type === 'demand') {
      const pct = request.demandChangePercent ?? 0;
      scenario = `Demand ${pct >= 0 ? '+' : ''}${pct}%`;
      inputs.push({ label: 'Demand change', value: `${pct}%` });
      const newDemand = dailyDemand * (1 + pct / 100);
      stockoutDays = newDemand > 0 ? currentStock / newDemand : 999;
      revenue = currentPrice * Math.min(currentStock, newDemand * 30);
      stockoutDate = new Date(Date.now() + stockoutDays * 86400000).toISOString().slice(0, 10);
    } else if (request.type === 'supplier_delay') {
      const days = request.delayDays ?? 0;
      scenario = `Supplier delay +${days} days`;
      inputs.push({ label: 'Delay', value: `${days} days` });
      stockoutDays = dailyDemand > 0 ? currentStock / dailyDemand : 999;
      stockoutDate = new Date(Date.now() + (stockoutDays - days) * 86400000).toISOString().slice(0, 10);
      revenue = currentPrice * currentStock;
    }

    const result: WhatIfResult = {
      scenario,
      inputs,
      projections: { revenue, margin: margin * 100, stockoutDate, profit: revenue * margin },
      summary: `Projected revenue ${this.formatCurrency(revenue)}. Stockout ${stockoutDate ? stockoutDate : 'N/A'}. Margin ${(margin * 100).toFixed(0)}%.`
    };
    return of({ success: true, data: result, message: '', statusCode: 200, errors: [], timestamp: new Date().toISOString() } as ApiResponse<WhatIfResult>);
  }

  getForecastConfidence(productId: string): Observable<ApiResponse<{ confidencePercent: number; confidenceLabel: string; trends?: string[] }>> {
    return this.aiApi.getDemandForecast(productId, 30).pipe(
      map(res => {
        const f = (res as any).data;
        const confidence = f?.confidence === 'High' ? 85 : f?.confidence === 'Medium' ? 65 : 45;
        return { success: true, data: { confidencePercent: confidence, confidenceLabel: f?.confidence || 'Medium', trends: f?.trends }, message: '', statusCode: 200, errors: [], timestamp: new Date().toISOString() };
      }),
      catchError(() => of({ success: true, data: { confidencePercent: 50, confidenceLabel: 'Medium', trends: [] }, message: '', statusCode: 200, errors: [], timestamp: new Date().toISOString() } as ApiResponse<any>))
    );
  }

  private formatCurrency(n: number): string {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', minimumFractionDigits: 0 }).format(n);
  }

  private buildExplainableInsights(suggestions: any[], riskItems: any[]): ExplainableInsight[] {
    const insights: ExplainableInsight[] = [];
    suggestions.filter((s: any) => s.priority === 'Critical' || s.priority === 'High').slice(0, 5).forEach((s: any) => {
      insights.push({
        type: 'reorder',
        entityId: s.productId,
        entityName: s.productName,
        title: `Low stock: ${s.productName}`,
        reasons: [
          { label: 'Demand trend', value: s.averageDailySales ?? 0, unit: 'units/day' },
          { label: 'Stock days remaining', value: s.daysOfStockRemaining, unit: 'days', trend: s.daysOfStockRemaining < 7 ? 'down' : 'stable' },
          { label: 'Lead time', value: s.supplierSuggestions?.[0]?.averageDeliveryDays ?? 7, unit: 'days' }
        ],
        severity: s.priority === 'Critical' ? 'critical' : 'warning',
        suggestedAction: `Reorder ${s.suggestedQuantity} units`,
        link: '/reorder'
      });
    });
    riskItems.slice(0, 3).forEach((r: any) => {
      insights.push({
        type: 'risk',
        entityId: r.entityId,
        entityName: r.productName || r.title,
        title: r.title,
        reasons: [{ label: 'Metric', value: r.metric ?? 0, unit: r.unit }],
        severity: r.severity === 'high' ? 'critical' : 'warning',
        suggestedAction: r.suggestedAction,
        link: r.link
      });
    });
    return insights;
  }

  private buildOptimizationSuggestions(suggestions: any[], profitData: any, riskItems: any[]): OptimizationSuggestion[] {
    const list: OptimizationSuggestion[] = [];
    suggestions.filter((s: any) => s.priority === 'Critical').slice(0, 3).forEach((s: any, i: number) => {
      list.push({
        id: `reorder-${i}`,
        type: 'reorder_quantity',
        title: `Optimize reorder: ${s.productName}`,
        description: `Suggested quantity ${s.suggestedQuantity} units to cover demand.`,
        entityId: s.productId,
        entityName: s.productName,
        impact: 'Avoid stockout',
        priority: 'high',
        actionLabel: 'Create PO',
        actionLink: '/reorder'
      });
    });
    const overstock = riskItems.filter((r: any) => r.type === 'Overstock');
    overstock.slice(0, 2).forEach((r: any, i: number) => {
      list.push({
        id: `discount-${i}`,
        type: 'slow_stock_discount',
        title: `Discount slow mover: ${r.productName || r.title}`,
        description: r.description || 'Consider promotion to clear stock.',
        priority: 'medium',
        actionLink: r.link
      });
    });
    if (profitData?.marginByProduct?.length) {
      const top = profitData.marginByProduct[0];
      list.push({
        id: 'price-1',
        type: 'price_improvement',
        title: `Price opportunity: ${top.productName}`,
        description: `Current margin ${top.marginPercent}%. Test small increase.`,
        entityId: top.productId,
        priority: 'low',
        actionLink: `/products/${top.productId}`
      });
    }
    return list;
  }

  private buildOpportunities(d: any, profitData: any): Opportunity[] {
    const list: Opportunity[] = [];
    (d?.topProducts || []).slice(0, 3).forEach((p: any, i: number) => {
      list.push({ id: `top-${i}`, type: 'high_margin', title: p.productName, description: `High sales: ${this.formatCurrency(p.totalSales)}`, metric: p.totalSales, metricLabel: 'Sales', link: `/products/${p.productId}` });
    });
    if (profitData?.marginByCategory?.length) {
      profitData.marginByCategory.slice(0, 2).forEach((c: any, i: number) => {
        list.push({ id: `cat-${i}`, type: 'upsell', title: c.categoryName, description: `Margin ${c.marginPercent}%`, metric: c.marginPercent, metricLabel: '%' });
      });
    }
    list.push({ id: 'cust-1', type: 'customer_target', title: 'Top customers', description: 'Target repeat buyers with offers', link: '/customers' });
    return list;
  }

  private buildManagementStory(d: any, suggestions: any[], riskItems: any[]): ManagementStory {
    const monthlySales = d?.summary?.monthlySales ?? 0;
    const todaySales = d?.summary?.todaySales ?? 0;
    const lowStock = d?.summary?.lowStockItems ?? suggestions.length;
    const critical = suggestions.filter((s: any) => s.priority === 'Critical').length;
    const highlights: string[] = [];
    if (monthlySales > 0) highlights.push(`Sales this month total ${this.formatCurrency(monthlySales)}.`);
    if (todaySales > 0) highlights.push(`Today's sales so far: ${this.formatCurrency(todaySales)}.`);
    const risks: string[] = [];
    if (critical > 0) risks.push(`${critical} product(s) at critical stock level need reorder.`);
    if (lowStock > 0) risks.push(`Low stock risk exists in ${lowStock} item(s).`);
    riskItems.filter((r: any) => r.severity === 'high').forEach((r: any) => risks.push(r.title));
    const recommendations: string[] = [];
    if (critical > 0) recommendations.push('Review reorder list and create purchase orders for critical items.');
    if (riskItems.some((r: any) => r.type === 'Expiry')) recommendations.push('Address expiring batches to avoid write-off.');
    return { generatedAt: new Date().toISOString(), summary: highlights.join(' ') || 'Business overview loaded.', highlights, risks, recommendations };
  }

  private buildBusinessHealthScore(d: any, suggestions: any[], riskItems: any[]): BusinessHealthScore {
    const totalProducts = d?.summary?.totalProducts || 1;
    const lowStockItems = d?.summary?.lowStockItems ?? suggestions.length;
    const stockHealth = Math.max(0, 100 - (lowStockItems / totalProducts) * 50);
    const monthlySales = d?.summary?.monthlySales ?? 0;
    const growth = monthlySales > 0 ? 100 : 50;
    const expiryRisk = riskItems.filter((r: any) => r.type === 'Expiry').length;
    const expiryScore = Math.max(0, 100 - expiryRisk * 15);
    const fulfillment = 100 - (suggestions.filter((s: any) => s.priority === 'Critical').length * 10);
    const score = Math.round((stockHealth * 0.3 + growth * 0.2 + expiryScore * 0.2 + Math.max(0, fulfillment) * 0.3));
    const finalScore = Math.min(100, Math.max(0, score));
    const grade = finalScore >= 80 ? 'A' : finalScore >= 60 ? 'B' : finalScore >= 40 ? 'C' : finalScore >= 20 ? 'D' : 'F';
    const factors: HealthFactor[] = [
      { name: 'Stock health', value: stockHealth, weight: 30, status: (stockHealth >= 70 ? 'good' : stockHealth >= 40 ? 'warning' : 'critical') as 'good' | 'warning' | 'critical', description: `${lowStockItems} low stock` },
      { name: 'Growth', value: growth, weight: 20, status: 'good' },
      { name: 'Expiry risk', value: expiryScore, weight: 20, status: (expiryScore >= 70 ? 'good' : 'warning') as 'good' | 'warning' | 'critical', description: `${expiryRisk} expiring` },
      { name: 'Fulfillment', value: Math.max(0, fulfillment), weight: 30, status: (fulfillment >= 70 ? 'good' : 'warning') as 'good' | 'warning' | 'critical' }
    ];
    return { score: finalScore, label: `${finalScore}/100`, grade, factors, updatedAt: new Date().toISOString() };
  }

  private buildAnomalies(riskItems: any[]): Anomaly[] {
    return riskItems.filter((r: any) => r.type === 'AbnormalSales' || r.severity === 'high').slice(0, 10).map((r: any, i: number) => ({
      id: r.id || `anom-${i}`,
      type: r.type || 'Anomaly',
      title: r.title,
      description: r.description,
      severity: r.severity === 'high' ? 'high' : 'medium',
      detectedAt: r.createdAt || new Date().toISOString(),
      entityId: r.entityId,
      entityType: r.entityType,
      link: r.link,
      metric: r.metric,
      expectedRange: r.unit
    }));
  }
}
