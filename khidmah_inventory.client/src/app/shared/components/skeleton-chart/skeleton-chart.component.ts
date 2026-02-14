import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkeletonLoaderComponent } from '../skeleton-loader/skeleton-loader.component';

@Component({
  selector: 'app-skeleton-chart',
  standalone: true,
  imports: [CommonModule, SkeletonLoaderComponent],
  template: `
    <div class="skeleton-chart-card rounded-3 border-0 shadow-sm overflow-hidden">
      <div class="skeleton-chart-header p-4 pb-0">
        <div class="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-2">
          <app-skeleton-loader [width]="titleWidth" [height]="'24px'" shape="rounded" [animation]="animation"></app-skeleton-loader>
          <app-skeleton-loader [width]="'120px'" [height]="'32px'" shape="rounded" [animation]="animation"></app-skeleton-loader>
        </div>
        <app-skeleton-loader *ngIf="showSubtitle" [width]="'50%'" [height]="'14px'" shape="rounded" [animation]="animation"></app-skeleton-loader>
      </div>
      <div class="skeleton-chart-body p-4" [style.min-height.px]="chartHeight">
        <app-skeleton-loader [width]="'100%'" [height]="'100%'" shape="rounded" [animation]="animation" class="skeleton-chart-area"></app-skeleton-loader>
      </div>
    </div>
  `,
  styles: [`
    .skeleton-chart-card { background: var(--surface-color, #fff); }
    .skeleton-chart-body { min-height: 280px; }
    .skeleton-chart-area { display: block; min-height: 240px; }
  `]
})
export class SkeletonChartComponent {
  @Input() titleWidth = '180px';
  @Input() showSubtitle = true;
  @Input() chartHeight = 280;
  @Input() animation: 'pulse' | 'shimmer' = 'shimmer';
}
