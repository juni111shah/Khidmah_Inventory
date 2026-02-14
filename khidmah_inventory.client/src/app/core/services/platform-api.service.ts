import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, PagedResult } from '../models/api-response.model';
import {
  ApiKeyDto,
  CreateApiKeyRequest,
  CreateApiKeyResult,
  ApiKeyUsageDto,
  WebhookDto,
  WebhookDeliveryLogDto,
  IntegrationDto,
  ScheduledReportDto
} from '../models/platform.model';
import { ApiConfigService } from './api-config.service';

@Injectable({
  providedIn: 'root'
})
export class PlatformApiService {
  private baseUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.baseUrl = this.apiConfig.getApiUrl('platform');
  }

  // API Keys
  getApiKeys(body?: { filterRequest?: any; isActive?: boolean }): Observable<ApiResponse<PagedResult<ApiKeyDto>>> {
    return this.http.post<ApiResponse<PagedResult<ApiKeyDto>>>(`${this.baseUrl}/api-keys/list`, body ?? {});
  }

  createApiKey(request: CreateApiKeyRequest): Observable<ApiResponse<CreateApiKeyResult>> {
    return this.http.post<ApiResponse<CreateApiKeyResult>>(`${this.baseUrl}/api-keys`, request);
  }

  revokeApiKey(id: string): Observable<ApiResponse<void>> {
    return this.http.patch<ApiResponse<void>>(`${this.baseUrl}/api-keys/${id}/revoke`, {});
  }

  getApiKeyUsage(body?: { apiKeyId?: string; recentLogsCount?: number }): Observable<ApiResponse<ApiKeyUsageDto>> {
    return this.http.post<ApiResponse<ApiKeyUsageDto>>(`${this.baseUrl}/api-keys/usage`, body ?? {});
  }

  // Webhooks
  getWebhooks(body?: { isActive?: boolean }): Observable<ApiResponse<WebhookDto[]>> {
    return this.http.post<ApiResponse<WebhookDto[]>>(`${this.baseUrl}/webhooks/list`, body ?? {});
  }

  createWebhook(request: Partial<WebhookDto> & { name: string; url: string; events: string }): Observable<ApiResponse<WebhookDto>> {
    return this.http.post<ApiResponse<WebhookDto>>(`${this.baseUrl}/webhooks`, request);
  }

  updateWebhook(id: string, request: Partial<WebhookDto>): Observable<ApiResponse<WebhookDto>> {
    return this.http.put<ApiResponse<WebhookDto>>(`${this.baseUrl}/webhooks/${id}`, request);
  }

  deleteWebhook(id: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.baseUrl}/webhooks/${id}`);
  }

  getWebhookLogs(id: string, pageNo = 1, pageSize = 20): Observable<ApiResponse<PagedResult<WebhookDeliveryLogDto>>> {
    return this.http.post<ApiResponse<PagedResult<WebhookDeliveryLogDto>>>(`${this.baseUrl}/webhooks/${id}/logs`, { webhookId: id, pageNo, pageSize });
  }

  // Integrations
  getIntegrations(): Observable<ApiResponse<IntegrationDto[]>> {
    return this.http.post<ApiResponse<IntegrationDto[]>>(`${this.baseUrl}/integrations/list`, {});
  }

  toggleIntegration(type: string, isEnabled: boolean): Observable<ApiResponse<IntegrationDto>> {
    return this.http.patch<ApiResponse<IntegrationDto>>(`${this.baseUrl}/integrations/${encodeURIComponent(type)}/toggle`, { integrationType: type, isEnabled });
  }

  // Scheduled Reports
  getScheduledReports(body?: { isActive?: boolean }): Observable<ApiResponse<ScheduledReportDto[]>> {
    return this.http.post<ApiResponse<ScheduledReportDto[]>>(`${this.baseUrl}/scheduled-reports/list`, body ?? {});
  }

  createScheduledReport(request: Partial<ScheduledReportDto> & { name: string; reportType: string; frequency: string; recipientsJson: string }): Observable<ApiResponse<ScheduledReportDto>> {
    return this.http.post<ApiResponse<ScheduledReportDto>>(`${this.baseUrl}/scheduled-reports`, request);
  }

  updateScheduledReport(id: string, request: Partial<ScheduledReportDto>): Observable<ApiResponse<ScheduledReportDto>> {
    return this.http.put<ApiResponse<ScheduledReportDto>>(`${this.baseUrl}/scheduled-reports/${id}`, request);
  }

  deleteScheduledReport(id: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.baseUrl}/scheduled-reports/${id}`);
  }
}
