import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProfitIntelligenceApiService } from '../../../core/services/profit-intelligence-api.service';
import { ProfitIntelligenceData } from '../../../core/models/profit-intelligence.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { KpiStatCardComponent } from '../../../shared/components/kpi-stat-card/kpi-stat-card.component';
import { ChartComponent, ChartData } from '../../../shared/components/chart/chart.component';
import { getChartColors } from '../../../shared/constants/chart-colors';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonStatCardsComponent } from '../../../shared/components/skeleton-stat-cards/skeleton-stat-cards.component';
import { SkeletonChartComponent } from '../../../shared/components/skeleton-chart/skeleton-chart.component';
import { SkeletonTableComponent } from '../../../shared/components/skeleton-table/skeleton-table.component';

@Component({
  selector: 'app-profit-intelligence',
  standalone: true,
  imports: [CommonModule, RouterModule, ToastComponent, LoadingSpinnerComponent, UnifiedCardComponent, KpiStatCardComponent, ChartComponent, ContentLoaderComponent, SkeletonStatCardsComponent, SkeletonChartComponent, SkeletonTableComponent],
  templateUrl: './profit-intelligence.component.html'
})
export class ProfitIntelligenceComponent implements OnInit {
  data: ProfitIntelligenceData | null = null;
  loading = true;
  /** Cached chart data – set only when data loads to avoid new references every CD and chart update loops */
  marginChartData: ChartData | null = null;
  agingChartData: ChartData | null = null;
  formatCurrency = (n: number) => new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', minimumFractionDigits: 0 }).format(n);

  constructor(private profitApi: ProfitIntelligenceApiService) {}

  ngOnInit(): void {
    this.profitApi.getProfitIntelligence().subscribe({
      next: (res: ApiResponse<ProfitIntelligenceData>) => {
        this.loading = false;
        if (res.success && res.data) {
          this.data = res.data;
          this.updateChartData();
        }
      },
      error: () => { this.loading = false; }
    });
  }

  private updateChartData(): void {
    if (!this.data) {
      this.marginChartData = null;
      this.agingChartData = null;
      return;
    }
    if (this.data.marginByCategory?.length) {
      const labels = this.data.marginByCategory.map(c => c.categoryName);
      this.marginChartData = {
        labels,
        datasets: [{ label: 'Margin %', data: this.data.marginByCategory.map(c => c.marginPercent), backgroundColor: getChartColors(labels.length) }]
      };
    } else {
      this.marginChartData = null;
    }
    if (this.data.agingInventory?.length) {
      const labels = this.data.agingInventory.map(a => a.range);
      this.agingChartData = {
        labels,
        datasets: [{ label: 'Value', data: this.data.agingInventory.map(a => a.value), backgroundColor: getChartColors(labels.length) }]
      };
    } else {
      this.agingChartData = null;
    }
  }
}
