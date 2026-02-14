import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkeletonLoaderComponent } from '../skeleton-loader/skeleton-loader.component';

@Component({
  selector: 'app-skeleton-activity-feed',
  standalone: true,
  imports: [CommonModule, SkeletonLoaderComponent],
  template: `
    <div class="skeleton-activity-feed">
      <div class="mb-4">
        <app-skeleton-loader [width]="'80px'" [height]="'12px'" shape="rounded" [animation]="animation" class="mb-3"></app-skeleton-loader>
        <div class="activity-items">
          <div *ngFor="let i of activityCountArray" class="skeleton-activity-item border-start border-2 ps-3 mb-3">
            <app-skeleton-loader [width]="'60px'" [height]="'10px'" shape="rounded" [animation]="animation" class="mb-2"></app-skeleton-loader>
            <app-skeleton-loader [width]="'90%'" [height]="'14px'" shape="rounded" [animation]="animation" class="mb-1"></app-skeleton-loader>
            <app-skeleton-loader [width]="'50%'" [height]="'10px'" shape="rounded" [animation]="animation"></app-skeleton-loader>
          </div>
        </div>
      </div>
      <div>
        <app-skeleton-loader [width]="'70px'" [height]="'12px'" shape="rounded" [animation]="animation" class="mb-3"></app-skeleton-loader>
        <app-skeleton-loader [width]="'100%'" [height]="'56px'" shape="rounded" [animation]="animation" class="mb-2"></app-skeleton-loader>
        <app-skeleton-loader [width]="'80px'" [height]="'36px'" shape="rounded" [animation]="animation"></app-skeleton-loader>
      </div>
    </div>
  `,
  styles: [`
    .skeleton-activity-feed { min-height: 200px; }
    .skeleton-activity-item { border-color: var(--skeleton-border, #e2e8f0) !important; }
  `]
})
export class SkeletonActivityFeedComponent {
  @Input() activityCount = 4;
  @Input() animation: 'pulse' | 'shimmer' = 'shimmer';

  get activityCountArray(): number[] {
    return Array.from({ length: this.activityCount }, (_, i) => i);
  }
}
