import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkeletonLoaderComponent } from '../skeleton-loader/skeleton-loader.component';

@Component({
  selector: 'app-skeleton-side-panel',
  standalone: true,
  imports: [CommonModule, SkeletonLoaderComponent],
  template: `
    <div class="skeleton-side-panel">
      <div class="skeleton-side-panel-header d-flex justify-content-between align-items-center p-3 border-bottom">
        <app-skeleton-loader [width]="'140px'" [height]="'24px'" shape="rounded" [animation]="animation"></app-skeleton-loader>
        <app-skeleton-loader [width]="'36px'" [height]="'36px'" shape="circle" [animation]="animation"></app-skeleton-loader>
      </div>
      <div class="skeleton-side-panel-body p-3">
        <div *ngFor="let i of lineCountArray" class="skeleton-side-panel-line mb-3">
          <app-skeleton-loader [width]="lineWidth(i)" [height]="'16px'" shape="rounded" [animation]="animation"></app-skeleton-loader>
        </div>
        <div *ngIf="showBlock" class="skeleton-side-panel-block mt-4">
          <app-skeleton-loader [width]="'100%'" [height]="'80px'" shape="rounded" [animation]="animation"></app-skeleton-loader>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .skeleton-side-panel {
      background: var(--surface-color, #fff);
      min-height: 200px;
      border-radius: var(--skeleton-radius, 8px);
      overflow: hidden;
    }
    .skeleton-side-panel-body { min-height: 160px; }
  `]
})
export class SkeletonSidePanelComponent {
  @Input() lineCount = 6;
  @Input() showBlock = true;
  @Input() animation: 'pulse' | 'shimmer' = 'shimmer';

  get lineCountArray(): number[] {
    return Array.from({ length: this.lineCount }, (_, i) => i);
  }

  lineWidth(i: number): string {
    return `${70 + (i % 4) * 8}%`;
  }
}
