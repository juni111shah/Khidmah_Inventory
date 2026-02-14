import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkeletonLoaderComponent } from '../skeleton-loader/skeleton-loader.component';

@Component({
  selector: 'app-skeleton-stat-cards',
  standalone: true,
  imports: [CommonModule, SkeletonLoaderComponent],
  template: `
    <div class="row g-3 skeleton-stat-cards" [class.equal-height-cards]="true">
      <div *ngFor="let i of cardCountArray" class="col-6 col-md-3">
        <div class="skeleton-stat-card h-100 rounded-3 border-0 shadow-sm p-3">
          <div class="d-flex align-items-center gap-2 mb-2">
            <app-skeleton-loader [width]="'40px'" [height]="'40px'" shape="circle" [animation]="animation"></app-skeleton-loader>
            <app-skeleton-loader [width]="'70px'" [height]="'14px'" shape="rounded" [animation]="animation"></app-skeleton-loader>
          </div>
          <app-skeleton-loader [width]="'60%'" [height]="'28px'" shape="rounded" [animation]="animation" class="mb-1"></app-skeleton-loader>
          <app-skeleton-loader [width]="'40%'" [height]="'12px'" shape="rounded" [animation]="animation"></app-skeleton-loader>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .skeleton-stat-card {
      background: var(--surface-color, #fff);
      min-height: 120px;
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
