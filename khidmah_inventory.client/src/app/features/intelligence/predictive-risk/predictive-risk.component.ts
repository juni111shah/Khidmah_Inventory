import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { PredictiveRiskApiService } from '../../../core/services/predictive-risk-api.service';
import { PredictiveRiskData, RiskItem } from '../../../core/models/predictive-risk.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { KpiStatCardComponent } from '../../../shared/components/kpi-stat-card/kpi-stat-card.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonStatCardsComponent } from '../../../shared/components/skeleton-stat-cards/skeleton-stat-cards.component';
import { SkeletonActivityFeedComponent } from '../../../shared/components/skeleton-activity-feed/skeleton-activity-feed.component';

@Component({
  selector: 'app-predictive-risk',
  standalone: true,
  imports: [CommonModule, RouterModule, ToastComponent, LoadingSpinnerComponent, UnifiedCardComponent, KpiStatCardComponent, ContentLoaderComponent, SkeletonStatCardsComponent, SkeletonActivityFeedComponent],
  templateUrl: './predictive-risk.component.html'
})
export class PredictiveRiskComponent implements OnInit {
  data: PredictiveRiskData | null = null;
  loading = true;

  constructor(private riskApi: PredictiveRiskApiService) {}

  ngOnInit(): void {
    this.riskApi.getRisks().subscribe({
      next: (res: ApiResponse<PredictiveRiskData>) => {
        this.loading = false;
        if (res.success && res.data) this.data = res.data;
      },
      error: () => { this.loading = false; }
    });
  }

  severityClass(severity: string): string {
    return severity === 'high' ? 'danger' : severity === 'medium' ? 'warning' : 'secondary';
  }

  typeIcon(type: string): string {
    const m: Record<string, string> = {
      OutOfStockSoon: 'bi-exclamation-triangle',
      Overstock: 'bi-arrow-up-circle',
      Expiry: 'bi-calendar-x',
      AbnormalSales: 'bi-graph-down'
    };
    return m[type] || 'bi-info-circle';
  }
}
