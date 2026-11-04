import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkeletonLoaderComponent } from '../skeleton-loader/skeleton-loader.component';

@Component({
  selector: 'app-skeleton-stat-cards',
  standalone: true,
  imports: [CommonModule, SkeletonLoaderComponent],
  template: `
    <div class="row g-3 skeleton-stat-cards dashboard-kpi-grid" [class.equal-height-cards]="true">
      <div *ngFor="let i of cardCountArray" class="col-6 col-md-3">
        <div class="skeleton-stat-card h-100">
          <div class="d-flex align-items-center gap-2 mb-2">
            <app-skeleton-loader [width]="'40px'" [height]="'40px'" shape="circle" [animation]="animation"></app-skeleton-loader>
            <app-skeleton-loader [width]="'72px'" [height]="'14px'" shape="rounded" [animation]="animation"></app-skeleton-loader>
          </div>
          <app-skeleton-loader [width]="'52%'" [height]="'32px'" shape="rounded" [animation]="animation" class="mb-2"></app-skeleton-loader>
          <app-skeleton-loader [width]="'42%'" [height]="'12px'" shape="rounded" [animation]="animation"></app-skeleton-loader>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-kpi-grid {
      --bs-gutter-x: 1.15rem;
      --bs-gutter-y: 1.15rem;
    }

    .skeleton-stat-card {
      background: var(--surface-color, #fff);
      border: 1px solid var(--border-color, #e5e7eb);
      border-radius: var(--card-border-radius, 0.75rem);
      box-shadow: var(--elevation-1, 0 1px 3px rgba(0, 0, 0, 0.08));
      min-height: 140px;
      padding: 1rem 1.25rem;
    }
  `]
})
export class SkeletonStatCardsComponent {
  @Input() cardCount = 4;
  @Input() animation: 'pulse' | 'shimmer' = 'shimmer';

  get cardCountArray(): number[] {
    return Array.from({ length: this.cardCount }, (_, i) => i);
  }
}
