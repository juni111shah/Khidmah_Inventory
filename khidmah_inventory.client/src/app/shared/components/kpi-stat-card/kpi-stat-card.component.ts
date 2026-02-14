import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SparklineChartComponent } from '../sparkline-chart/sparkline-chart.component';

export type KpiStatCardTheme = 'primary' | 'success' | 'warning' | 'danger' | 'info';

@Component({
  selector: 'app-kpi-stat-card',
  standalone: true,
  imports: [CommonModule, SparklineChartComponent],
  templateUrl: './kpi-stat-card.component.html'
})
export class KpiStatCardComponent {
  /** Card title (e.g. "Total Companies", "Products") */
  @Input() title: string = '';
  /** Main value (number or string, e.g. 61, "$18K") */
  @Input() value: string | number = '';
  /** Bootstrap icon class (e.g. "bi-building", "bi-box-seam") */
  @Input() icon: string = 'bi-graph-up';
  /** Theme for icon circle and sparkline: primary, success, warning, danger, info */
  @Input() theme: KpiStatCardTheme = 'primary';
  /** Optional trend/sparkline data. If empty, a subtle default curve is shown. */
  @Input() trendData: number[] = [];
  /** Whether to show the sparkline at the bottom (default true) */
  @Input() showSparkline: boolean = true;
  /** Optional trend percentage badge (e.g. "+8%"). If set, shows a small badge. */
  @Input() trendPercent: string | null = null;
  /** Optional link/button slot (e.g. "View") - projected via ng-content or we skip for "perfect" card */
  @Input() hoverable: boolean = true;

  get chartColor(): string {
    const map: Record<KpiStatCardTheme, string> = {
      primary: 'var(--primary-color, #6366f1)',
      success: 'var(--success-color, #22c55e)',
      warning: 'var(--warning-color, #f59e0b)',
      danger: 'var(--danger-color, #ef4444)',
      info: 'var(--info-color, #0ea5e9)'
    };
    return map[this.theme] || map.primary;
  }

  /** Default wavy data when no trendData provided (matches second attachment look) */
  get effectiveTrendData(): number[] {
    if (this.trendData && this.trendData.length > 0) return this.trendData;
    return [30, 45, 35, 55, 42, 60, 52];
  }
}
