import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { delay, map } from 'rxjs/operators';
import { ApiResponse } from '../models/api-response.model';
import { AutomationRule, AutomationExecution } from '../models/automation.model';
import { ApiConfigService } from './api-config.service';

@Injectable({ providedIn: 'root' })
export class AutomationApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('automation');
  }

  getRules(): Observable<ApiResponse<AutomationRule[]>> {
    return this.http.get<ApiResponse<AutomationRule[]>>(`${this.apiUrl}/rules`).pipe(
      map(r => r),
      delay(0)
    );
  }

  getRulesMock(): Observable<ApiResponse<AutomationRule[]>> {
    const mock: AutomationRule[] = [
      {
        id: '1',
        name: 'Low stock → Create PO',
        description: 'When stock falls below reorder point',
        isActive: true,
        condition: { type: 'StockBelowThreshold', params: { threshold: 10 } },
        action: { type: 'CreatePO', params: {} },
        priority: 1,
        createdAt: new Date().toISOString(),
        runCount: 12,
        lastRunAt: new Date().toISOString()
      },
      {
        id: '2',
        name: 'Large order → Require approval',
        description: 'Orders above 5000',
        isActive: true,
        condition: { type: 'OrderAboveLimit', params: { limit: 5000 } },
        action: { type: 'RequireApproval', params: {} },
        priority: 2,
        createdAt: new Date().toISOString(),
        runCount: 3
      }
    ];
    return of({ success: true, data: mock, message: '', statusCode: 200, errors: [], timestamp: new Date().toISOString() }).pipe(delay(300));
  }

  getRule(id: string): Observable<ApiResponse<AutomationRule>> {
    return this.http.get<ApiResponse<AutomationRule>>(`${this.apiUrl}/rules/${id}`).pipe(delay(0));
  }

  createRule(rule: Partial<AutomationRule>): Observable<ApiResponse<AutomationRule>> {
    return this.http.post<ApiResponse<AutomationRule>>(`${this.apiUrl}/rules`, rule).pipe(delay(0));
  }

  createRuleMock(rule: Partial<AutomationRule>): Observable<ApiResponse<AutomationRule>> {
    const created: AutomationRule = {
      id: `mock-${Date.now()}`,
      name: rule.name!,
      isActive: rule.isActive ?? true,
      condition: rule.condition!,
      action: rule.action!,
      priority: rule.priority ?? 0,
      createdAt: new Date().toISOString(),
      runCount: 0,
      description: rule.description
    };
    return of({ success: true, data: created, message: '', statusCode: 200, errors: [], timestamp: new Date().toISOString() }).pipe(delay(400));
  }

  updateRule(id: string, rule: Partial<AutomationRule>): Observable<ApiResponse<AutomationRule>> {
    return this.http.put<ApiResponse<AutomationRule>>(`${this.apiUrl}/rules/${id}`, rule).pipe(delay(0));
  }

  deleteRule(id: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/rules/${id}`).pipe(delay(0));
  }

  getExecutionHistory(ruleId?: string, top: number = 50): Observable<ApiResponse<AutomationExecution[]>> {
    const url = ruleId ? `${this.apiUrl}/executions?ruleId=${ruleId}&top=${top}` : `${this.apiUrl}/executions?top=${top}`;
    return this.http.get<ApiResponse<AutomationExecution[]>>(url).pipe(delay(0));
  }

  getExecutionHistoryMock(): Observable<ApiResponse<AutomationExecution[]>> {
    const mock: AutomationExecution[] = [
      { id: 'e1', ruleId: '1', ruleName: 'Low stock → Create PO', triggeredAt: new Date().toISOString(), status: 'Success', entityType: 'Product', entityId: 'p1' },
      { id: 'e2', ruleId: '1', ruleName: 'Low stock → Create PO', triggeredAt: new Date(Date.now() - 3600000).toISOString(), status: 'Success', message: 'PO created' }
    ];
    return of({ success: true, data: mock, message: '', statusCode: 200, errors: [], timestamp: new Date().toISOString() }).pipe(delay(300));
  }
}
