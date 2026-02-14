import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { KpiApiService } from '../../../core/services/kpi-api.service';
import { InventoryKpisDto, KpiValueDto, KpiFilters } from '../../../core/models/kpi.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { KpiStatCardComponent, KpiStatCardTheme } from '../../../shared/components/kpi-stat-card/kpi-stat-card.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonStatCardsComponent } from '../../../shared/components/skeleton-stat-cards/skeleton-stat-cards.component';
import { ChartComponent, ChartData } from '../../../shared/components/chart/chart.component';
import { HeaderService } from '../../../core/services/header.service';

@Component({
  selector: 'app-inventory-health',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    KpiStatCardComponent,
    UnifiedCardComponent,
    ContentLoaderComponent,
    SkeletonStatCardsComponent,
    ChartComponent
  ],
  templateUrl: './inventory-health.component.html'
})
export class InventoryHealthComponent implements OnInit {
  loading = false;
  data: InventoryKpisDto | null = null;
  dateFrom: Date = new Date(new Date().setDate(new Date().getDate() - 30));
  dateTo: Date = new Date();

  constructor(
    private kpiApi: KpiApiService,
    private header: HeaderService
  ) {}

  ngOnInit(): void {
    this.header.setHeaderInfo({ title: 'Inventory health', description: 'Stock value, turnover, aging, and dead stock' });
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
    this.kpiApi.getInventoryKpis(this.filters).subscribe({
      next: (res: ApiResponse<InventoryKpisDto>) => {
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

  get agingChartLabels(): string[] {
    return ['0–30 days', '30–60 days', '60–90 days', '90+ days'];
  }

  get agingChartValues(): number[] {
    if (!this.data?.agingBuckets) return [];
    const b = this.data.agingBuckets;
    return [b.days0To30, b.days30To60, b.days60To90, b.days90Plus];
  }

  get agingChartData(): ChartData | null {
    const labels = this.agingChartLabels;
    const values = this.agingChartValues;
    if (values.every(v => v === 0)) return null;
    return {
      labels,
      datasets: [{ label: 'Value', data: values }]
    };
  }
}
