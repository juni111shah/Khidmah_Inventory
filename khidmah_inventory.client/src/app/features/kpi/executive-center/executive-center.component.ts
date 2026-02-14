import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { KpiApiService } from '../../../core/services/kpi-api.service';
import { ExecutiveKpisDto, KpiValueDto, KpiFilters } from '../../../core/models/kpi.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { KpiStatCardComponent, KpiStatCardTheme } from '../../../shared/components/kpi-stat-card/kpi-stat-card.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonStatCardsComponent } from '../../../shared/components/skeleton-stat-cards/skeleton-stat-cards.component';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { HeaderService } from '../../../core/services/header.service';

@Component({
  selector: 'app-executive-center',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    KpiStatCardComponent,
    UnifiedCardComponent,
    ContentLoaderComponent,
    SkeletonStatCardsComponent,
    IconComponent
  ],
  templateUrl: './executive-center.component.html'
})
export class ExecutiveCenterComponent implements OnInit {
  loading = false;
  data: ExecutiveKpisDto | null = null;
  dateFrom: Date = new Date(new Date().setDate(new Date().getDate() - 30));
  dateTo: Date = new Date();

  constructor(
    private kpiApi: KpiApiService,
    private header: HeaderService
  ) {}

  ngOnInit(): void {
    this.header.setHeaderInfo({ title: 'Executive Center', description: 'Key metrics and top products at a glance' });
    this.load();
  }

  get filters(): KpiFilters {
    return {
      dateFrom: this.dateFrom?.toISOString(),
      dateTo: this.dateTo?.toISOString()
    };
  }

  load(): void {
    this.loading = true;
    this.kpiApi.getExecutiveKpis(this.filters).subscribe({
      next: (res: ApiResponse<ExecutiveKpisDto>) => {
        this.loading = false;
        if (res.success && res.data) this.data = res.data;
      },
      error: () => { this.loading = false; }
    });
  }

  formatKpiValue(k: KpiValueDto): string {
    if (k.format === 'currency') return this.formatCurrency(k.currentValue);
    if (k.format === 'percent') return Number(k.currentValue).toFixed(1) + '%';
    return Number(k.currentValue).toFixed(0);
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', minimumFractionDigits: 0, maximumFractionDigits: 0 }).format(value);
  }

  trendPercent(k: KpiValueDto): string | null {
    if (k.percentageChange == null) return null;
    const sign = k.percentageChange >= 0 ? '+' : '';
    return sign + Number(k.percentageChange).toFixed(1) + '%';
  }

  themeForKpi(k: KpiValueDto, higherIsBetter: boolean): KpiStatCardTheme {
    if (k.trendIndicator === 'neutral') return 'primary';
    const up = k.trendIndicator === 'up';
    if (higherIsBetter) return up ? 'success' : 'danger';
    return up ? 'danger' : 'success';
  }
}
