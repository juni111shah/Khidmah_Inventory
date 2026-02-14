export interface ApiKeyDto {
  id: string;
  name: string;
  keyPrefix: string;
  permissions: string;
  expiresAt?: string;
  isActive: boolean;
  lastUsedAt?: string;
  requestCount: number;
  errorCount: number;
  createdAt: string;
}

export interface CreateApiKeyRequest {
  name: string;
  permissions: string;
  expiresAt?: string;
}

export interface CreateApiKeyResult {
  apiKey: ApiKeyDto;
  plainKey: string;
}

export interface ApiKeyUsageDto {
  apiKeyId: string;
  name: string;
  totalCalls: number;
  errorCalls: number;
  lastAccessAt?: string;
  recentLogs?: ApiKeyUsageLogDto[];
}

export interface ApiKeyUsageLogDto {
  method: string;
  path: string;
  statusCode: number;
  success: boolean;
  elapsedMs: number;
  createdAt: string;
}

export interface WebhookDto {
  id: string;
  name: string;
  url: string;
  events: string;
  isActive: boolean;
  description?: string;
  createdAt: string;
}

export interface WebhookDeliveryLogDto {
  id: string;
  eventName: string;
  httpStatusCode: number;
  success: boolean;
  retryCount: number;
  errorMessage?: string;
  deliveredAt: string;
}

export interface IntegrationDto {
  integrationType: string;
  displayName: string;
  description?: string;
  isEnabled: boolean;
  isConfigured: boolean;
}

export interface ScheduledReportDto {
  id: string;
  name: string;
  reportType: string;
  frequency: string;
  cronExpression?: string;
  recipientsJson: string;
  lastRunAt?: string;
  nextRunAt?: string;
  isActive: boolean;
  createdAt: string;
}

export const WEBHOOK_EVENTS = ['OrderCreated', 'SaleCompleted', 'StockLow', 'ApprovalDone'];
export const REPORT_TYPES = ['SalesSummary', 'Inventory', 'LowStock'];
export const FREQUENCIES = ['Daily', 'Weekly', 'Monthly'];
