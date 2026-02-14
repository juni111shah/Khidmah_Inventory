import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkeletonLoaderComponent } from '../skeleton-loader/skeleton-loader.component';

@Component({
  selector: 'app-skeleton-detail-header',
  standalone: true,
  imports: [CommonModule, SkeletonLoaderComponent],
  template: `
    <div class="skeleton-detail-header d-flex justify-content-between align-items-center flex-wrap gap-3 mb-4">
      <div class="d-flex align-items-center gap-2">
        <app-skeleton-loader [width]="'100px'" [height]="'36px'" shape="rounded" [animation]="animation"></app-skeleton-loader>
        <app-skeleton-loader *ngIf="showTitle" [width]="titleWidth" [height]="'28px'" shape="rounded" [animation]="animation" class="ms-3"></app-skeleton-loader>
      </div>
      <div class="d-flex gap-2">
        <app-skeleton-loader *ngFor="let a of actionCountArray" [width]="'100px'" [height]="'36px'" shape="rounded" [animation]="animation"></app-skeleton-loader>
      </div>
    </div>
  `,
  styles: [`
    .skeleton-detail-header { min-height: 48px; }
  `]
})
export class SkeletonDetailHeaderComponent {
  @Input() showTitle = true;
  @Input() titleWidth = '200px';
  @Input() actionCount = 2;
  @Input() animation: 'pulse' | 'shimmer' = 'shimmer';

  get actionCountArray(): number[] {
    return Array.from({ length: this.actionCount }, (_, i) => i);
  }
}
