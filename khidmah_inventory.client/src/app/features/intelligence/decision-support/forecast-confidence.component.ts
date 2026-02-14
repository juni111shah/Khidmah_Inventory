import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DecisionSupportApiService } from '../../../core/services/decision-support-api.service';

@Component({
  selector: 'app-forecast-confidence',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="forecast-confidence border rounded p-2">
      <span class="small text-muted d-block">Forecast confidence</span>
      <div class="d-flex align-items-center">
        <div class="progress flex-grow-1 me-2" style="height: 8px;">
          <div class="progress-bar bg-{{ confidenceClass }}" [style.width.%]="confidencePercent" role="progressbar"></div>
        </div>
        <span class="small fw-bold">{{ confidencePercent }}%</span>
      </div>
      <span class="small text-muted">{{ confidenceLabel }}</span>
      <div *ngIf="trends?.length" class="small mt-1">Trends: {{ trends.join(', ') }}</div>
    </div>
  `
})
export class ForecastConfidenceComponent implements OnInit {
  @Input() productId: string | null = null;

  confidencePercent = 0;
  confidenceLabel = 'Medium';
  trends: string[] = [];

  get confidenceClass(): string {
    return this.confidencePercent >= 70 ? 'success' : this.confidencePercent >= 40 ? 'warning' : 'danger';
  }

  constructor(private api: DecisionSupportApiService) {}

  ngOnInit(): void {
    if (this.productId) {
      this.api.getForecastConfidence(this.productId).subscribe({
        next: res => {
          if (res.success && res.data) {
            this.confidencePercent = res.data.confidencePercent;
            this.confidenceLabel = res.data.confidenceLabel;
            this.trends = res.data.trends || [];
          }
        }
      });
    }
  }
}
