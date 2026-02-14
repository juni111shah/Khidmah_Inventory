import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { ApiResponse } from '../models/api-response.model';
import { AskResponse } from '../models/ai-assistant.model';
import { ApiConfigService } from './api-config.service';

@Injectable({ providedIn: 'root' })
export class AiAssistantApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('ai');
  }

  ask(request: { question: string }): Observable<ApiResponse<AskResponse>> {
    return this.http.post<ApiResponse<AskResponse>>(`${this.apiUrl}/assistant`, request).pipe(delay(0));
  }

  /** Mock response for when backend does not yet expose /ai/assistant */
  askMock(question: string): Observable<ApiResponse<AskResponse>> {
    const q = question.toLowerCase();
    let answer = '';
    let metrics: AskResponse['metrics'] = [];
    let links: AskResponse['links'] = [];

    if (q.includes('sales') || q.includes('revenue')) {
      answer = 'Sales this month are trending up by 8% compared to last month. Top drivers are categories Electronics and FMCG. Consider promoting slow-moving items to balance mix.';
      metrics = [
        { label: 'Monthly sales', value: '$42,500', trend: 'up', change: '+8%' },
        { label: 'Orders', value: '312', trend: 'up', change: '+12' }
      ];
      links = [{ label: 'Sales analytics', url: '/analytics/sales', icon: 'bi-graph-up' }];
    } else if (q.includes('reorder') || q.includes('stock')) {
      answer = 'You have 5 critical reorder suggestions and 12 high-priority items. Recommended: generate a purchase order from the Reorder list for the top 3 suppliers to cover the next 2 weeks.';
      metrics = [
        { label: 'Critical items', value: 5, trend: 'up' },
        { label: 'Suggested PO lines', value: 18 }
      ];
      links = [{ label: 'Reorder list', url: '/reorder/list', icon: 'bi-arrow-repeat' }];
    } else if (q.includes('supplier')) {
      answer = 'Best performing suppliers by delivery and quality: Supplier A (score 92), Supplier B (89). Two suppliers have delayed deliveries this week — check Purchase Orders for details.';
      metrics = [
        { label: 'On-time delivery', value: '87%' },
        { label: 'Active suppliers', value: 24 }
      ];
      links = [{ label: 'Suppliers', url: '/suppliers', icon: 'bi-truck' }];
    } else if (q.includes('profit') || q.includes('margin')) {
      answer = 'Gross margin is 34% this month. Main risks: rising cost in category "Beverages" and high dead stock in "Seasonal". Use Profit Intelligence to drill down by category.';
      metrics = [
        { label: 'Gross margin', value: '34%', trend: 'flat' },
        { label: 'Dead stock value', value: '$2,100' }
      ];
      links = [{ label: 'Profit intelligence', url: '/intelligence/profit', icon: 'bi-currency-dollar' }];
    } else {
      answer = 'I can help with sales trends, reorder suggestions, supplier performance, and profit risks. Try asking: "Why did sales change?" or "What should I reorder?"';
      links = [
        { label: 'Command center', url: '/command-center', icon: 'bi-speedometer2' },
        { label: 'Reports', url: '/reports', icon: 'bi-file-earmark-bar-graph' }
      ];
    }

    return of({
      success: true,
      data: { answer, metrics, links },
      message: '',
      statusCode: 200,
      errors: [],
      timestamp: new Date().toISOString()
    }).pipe(delay(600));
  }
}
