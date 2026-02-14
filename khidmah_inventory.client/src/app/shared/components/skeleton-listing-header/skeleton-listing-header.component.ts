import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkeletonLoaderComponent } from '../skeleton-loader/skeleton-loader.component';

@Component({
  selector: 'app-skeleton-listing-header',
  standalone: true,
  imports: [CommonModule, SkeletonLoaderComponent],
  template: `
    <div class="skeleton-listing-header d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
      <div class="skeleton-search flex-grow-1" style="max-width: 450px;">
        <app-skeleton-loader [width]="'100%'" [height]="'40px'" shape="rounded" [animation]="animation"></app-skeleton-loader>
      </div>
      <div class="d-flex gap-2">
        <app-skeleton-loader *ngFor="let i of buttonCountArray" [width]="buttonWidth" [height]="'40px'" shape="rounded" [animation]="animation"></app-skeleton-loader>
      </div>
    </div>
  `,
  styles: [`
    .skeleton-listing-header { min-height: 48px; }
  `]
})
export class SkeletonListingHeaderComponent {
  @Input() buttonCount = 3;
  @Input() buttonWidth = '120px';
  @Input() animation: 'pulse' | 'shimmer' = 'shimmer';

  get buttonCountArray(): number[] {
    return Array.from({ length: this.buttonCount }, (_, i) => i);
  }
}
