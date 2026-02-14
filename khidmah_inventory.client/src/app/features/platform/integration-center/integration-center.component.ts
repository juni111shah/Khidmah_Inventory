import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PlatformApiService } from '../../../core/services/platform-api.service';
import {
  ApiKeyDto,
  CreateApiKeyRequest,
  CreateApiKeyResult,
  ApiKeyUsageDto,
  WebhookDto,
  WebhookDeliveryLogDto,
  IntegrationDto,
  ScheduledReportDto,
  WEBHOOK_EVENTS,
  REPORT_TYPES,
  FREQUENCIES
} from '../../../core/models/platform.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';

type PlatformTab = 'api-keys' | 'webhooks' | 'integrations' | 'scheduled-reports' | 'usage';

@Component({
  selector: 'app-integration-center',
  standalone: true,
  imports: [CommonModule, FormsModule, ToastComponent, UnifiedButtonComponent, UnifiedCardComponent],
  templateUrl: './integration-center.component.html',
  styleUrls: ['./integration-center.component.scss']
})
export class IntegrationCenterComponent implements OnInit {
  activeTab: PlatformTab = 'api-keys';
  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';

  // API Keys
  apiKeys: ApiKeyDto[] = [];
  apiKeysLoading = false;
  createKeyModal = false;
  newKey: CreateApiKeyRequest = { name: '', permissions: '' };
  createdPlainKey: string | null = null;

  // Webhooks
  webhooks: WebhookDto[] = [];
  webhooksLoading = false;
  webhookModal = false;
  webhookForm: Partial<WebhookDto> & { name: string; url: string; events: string } = { name: '', url: '', events: '' };
  selectedWebhookLogs: WebhookDeliveryLogDto[] = [];
  webhookLogsModal = false;
  webhookLogsId: string | null = null;
  webhookEventsList = WEBHOOK_EVENTS;

  // Integrations
  integrations: IntegrationDto[] = [];
  integrationsLoading = false;
  integrationToggling: string | null = null;

  // Scheduled Reports
  scheduledReports: ScheduledReportDto[] = [];
  scheduledReportsLoading = false;
  reportModal = false;
  reportForm: Partial<ScheduledReportDto> & { name: string; reportType: string; frequency: string; recipientsJson: string } = {
    name: '', reportType: 'SalesSummary', frequency: 'Daily', recipientsJson: '[]'
  };
  reportTypes = REPORT_TYPES;
  frequencies = FREQUENCIES;

  // Usage
  usage: ApiKeyUsageDto | null = null;
  usageLoading = false;

  constructor(private platformApi: PlatformApiService) {}

  ngOnInit(): void {
    this.loadApiKeys();
  }

  setActiveTab(tab: PlatformTab): void {
    this.activeTab = tab;
    if (tab === 'api-keys') this.loadApiKeys();
    else if (tab === 'webhooks') this.loadWebhooks();
    else if (tab === 'integrations') this.loadIntegrations();
    else if (tab === 'scheduled-reports') this.loadScheduledReports();
    else if (tab === 'usage') this.loadUsage();
  }

  private showT(msg: string, type: 'success' | 'error' | 'warning' | 'info' = 'success'): void {
    this.toastMessage = msg;
    this.toastType = type;
    this.showToast = true;
  }

  loadApiKeys(): void {
    this.apiKeysLoading = true;
    this.platformApi.getApiKeys({}).subscribe({
      next: (r) => {
        if (r.success && r.data) this.apiKeys = r.data.items;
        this.apiKeysLoading = false;
      },
      error: () => { this.apiKeysLoading = false; this.showT('Failed to load API keys', 'error'); }
    });
  }

  openCreateKey(): void {
    this.newKey = { name: '', permissions: '' };
    this.createdPlainKey = null;
    this.createKeyModal = true;
  }

  createApiKey(): void {
    if (!this.newKey.name?.trim()) { this.showT('Name is required', 'error'); return; }
    this.platformApi.createApiKey(this.newKey).subscribe({
      next: (r) => {
        if (r.success && r.data) {
          this.createdPlainKey = r.data.plainKey;
          this.loadApiKeys();
          this.showT('API key created. Copy the key now; it won\'t be shown again.', 'success');
        } else this.showT(r.message || 'Create failed', 'error');
      },
      error: () => this.showT('Failed to create API key', 'error')
    });
  }

  revokeKey(id: string): void {
    if (!confirm('Revoke this API key? It will stop working immediately.')) return;
    this.platformApi.revokeApiKey(id).subscribe({
      next: (r) => { if (r.success) { this.loadApiKeys(); this.showT('Key revoked'); } else this.showT(r.message || 'Failed', 'error'); },
      error: () => this.showT('Failed to revoke', 'error')
    });
  }

  loadWebhooks(): void {
    this.webhooksLoading = true;
    this.platformApi.getWebhooks({}).subscribe({
      next: (r) => { if (r.success && r.data) this.webhooks = r.data; this.webhooksLoading = false; },
      error: () => { this.webhooksLoading = false; this.showT('Failed to load webhooks', 'error'); }
    });
  }

  openWebhookForm(webhook?: WebhookDto): void {
    if (webhook) {
      this.webhookForm = { ...webhook, name: webhook.name, url: webhook.url, events: webhook.events };
    } else {
      this.webhookForm = { name: '', url: '', events: '' };
    }
    this.webhookModal = true;
  }

  saveWebhook(): void {
    if (!this.webhookForm.name?.trim() || !this.webhookForm.url?.trim()) {
      this.showT('Name and URL are required', 'error'); return;
    }
    const payload = {
      name: this.webhookForm.name,
      url: this.webhookForm.url,
      events: typeof this.webhookForm.events === 'string' ? this.webhookForm.events : (this.webhookForm.events as string[]).join(','),
      description: this.webhookForm.description,
      isActive: this.webhookForm.isActive ?? true
    };
    if (this.webhookForm.id) {
      this.platformApi.updateWebhook(this.webhookForm.id, payload).subscribe({
        next: (r) => { if (r.success) { this.webhookModal = false; this.loadWebhooks(); this.showT('Webhook updated'); } else this.showT(r.message || 'Failed', 'error'); },
        error: () => this.showT('Update failed', 'error')
      });
    } else {
      this.platformApi.createWebhook(payload).subscribe({
        next: (r) => { if (r.success) { this.webhookModal = false; this.loadWebhooks(); this.showT('Webhook created'); } else this.showT(r.message || 'Failed', 'error'); },
        error: () => this.showT('Create failed', 'error')
      });
    }
  }

  deleteWebhook(id: string): void {
    if (!confirm('Delete this webhook?')) return;
    this.platformApi.deleteWebhook(id).subscribe({
      next: (r) => { if (r.success) { this.loadWebhooks(); this.showT('Webhook deleted'); } else this.showT(r.message || 'Failed', 'error'); },
      error: () => this.showT('Delete failed', 'error')
    });
  }

  openWebhookLogs(id: string): void {
    this.webhookLogsId = id;
    this.platformApi.getWebhookLogs(id, 1, 50).subscribe({
      next: (r) => { if (r.success && r.data) this.selectedWebhookLogs = r.data.items; this.webhookLogsModal = true; },
      error: () => this.showT('Failed to load logs', 'error')
    });
  }

  loadIntegrations(): void {
    this.integrationsLoading = true;
    this.platformApi.getIntegrations().subscribe({
      next: (r) => { if (r.success && r.data) this.integrations = r.data; this.integrationsLoading = false; },
      error: () => { this.integrationsLoading = false; this.showT('Failed to load integrations', 'error'); }
    });
  }

  toggleIntegration(integration: IntegrationDto): void {
    this.integrationToggling = integration.integrationType;
    this.platformApi.toggleIntegration(integration.integrationType, !integration.isEnabled).subscribe({
      next: (r) => {
        if (r.success && r.data) {
          const idx = this.integrations.findIndex(i => i.integrationType === r.data!.integrationType);
          if (idx >= 0) this.integrations[idx] = r.data;
          this.showT(r.data.isEnabled ? 'Integration enabled' : 'Integration disabled');
        } else this.showT(r.message || 'Failed', 'error');
        this.integrationToggling = null;
      },
      error: () => { this.integrationToggling = null; this.showT('Failed to toggle', 'error'); }
    });
  }

  loadScheduledReports(): void {
    this.scheduledReportsLoading = true;
    this.platformApi.getScheduledReports({}).subscribe({
      next: (r) => { if (r.success && r.data) this.scheduledReports = r.data; this.scheduledReportsLoading = false; },
      error: () => { this.scheduledReportsLoading = false; this.showT('Failed to load scheduled reports', 'error'); }
    });
  }

  openReportForm(report?: ScheduledReportDto): void {
    if (report) {
      this.reportForm = { ...report, name: report.name, reportType: report.reportType, frequency: report.frequency, recipientsJson: report.recipientsJson };
    } else {
      this.reportForm = { name: '', reportType: 'SalesSummary', frequency: 'Daily', recipientsJson: '[]' };
    }
    this.reportModal = true;
  }

  saveScheduledReport(): void {
    if (!this.reportForm.name?.trim()) { this.showT('Name is required', 'error'); return; }
    const payload = {
      name: this.reportForm.name,
      reportType: this.reportForm.reportType,
      frequency: this.reportForm.frequency,
      recipientsJson: this.reportForm.recipientsJson || '[]',
      isActive: this.reportForm.isActive ?? true
    };
    if (this.reportForm.id) {
      this.platformApi.updateScheduledReport(this.reportForm.id, payload).subscribe({
        next: (r) => { if (r.success) { this.reportModal = false; this.loadScheduledReports(); this.showT('Schedule updated'); } else this.showT(r.message || 'Failed', 'error'); },
        error: () => this.showT('Update failed', 'error')
      });
    } else {
      this.platformApi.createScheduledReport(payload).subscribe({
        next: (r) => { if (r.success) { this.reportModal = false; this.loadScheduledReports(); this.showT('Schedule created'); } else this.showT(r.message || 'Failed', 'error'); },
        error: () => this.showT('Create failed', 'error')
      });
    }
  }

  deleteScheduledReport(id: string): void {
    if (!confirm('Delete this scheduled report?')) return;
    this.platformApi.deleteScheduledReport(id).subscribe({
      next: (r) => { if (r.success) { this.loadScheduledReports(); this.showT('Schedule deleted'); } else this.showT(r.message || 'Failed', 'error'); },
      error: () => this.showT('Delete failed', 'error')
    });
  }

  loadUsage(): void {
    this.usageLoading = true;
    this.platformApi.getApiKeyUsage({}).subscribe({
      next: (r) => { if (r.success && r.data) this.usage = r.data; this.usageLoading = false; },
      error: () => { this.usageLoading = false; this.showT('Failed to load usage', 'error'); }
    });
  }

  copyToClipboard(text: string): void {
    navigator.clipboard.writeText(text).then(() => this.showT('Copied to clipboard'));
  }
}
