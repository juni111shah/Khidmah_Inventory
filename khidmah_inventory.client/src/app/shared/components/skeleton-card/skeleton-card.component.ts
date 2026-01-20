import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkeletonLoaderComponent } from '../skeleton-loader/skeleton-loader.component';

@Component({
  selector: 'app-skeleton-card',
  standalone: true,
  imports: [CommonModule, SkeletonLoaderComponent],
  template: `
    <div class="skeleton-card" [class.skeleton-card-with-header]="showHeader">
      <div *ngIf="showHeader" class="skeleton-card-header">
        <app-skeleton-loader
          [width]="headerTitleWidth"
          [height]="'24px'"
          [shape]="'rounded'"
          [animation]="animation">
        </app-skeleton-loader>
        <app-skeleton-loader
          *ngIf="showHeaderActions"
          [width]="'80px'"
          [height]="'24px'"
          [shape]="'rounded'"
          [animation]="animation">
        </app-skeleton-loader>
      </div>
      <div class="skeleton-card-body">
        <ng-content></ng-content>
        <div *ngIf="!hasContent" class="skeleton-card-content">
          <app-skeleton-loader
            *ngFor="let line of contentLines"
            [width]="line.width"
            [height]="line.height"
            [shape]="'rounded'"
            [animation]="animation"
            [style.margin-bottom]="'12px'">
          </app-skeleton-loader>
        </div>
      </div>
      <div *ngIf="showFooter" class="skeleton-card-footer">
        <app-skeleton-loader
          [width]="'100px'"
          [height]="'20px'"
          [shape]="'rounded'"
          [animation]="animation">
        </app-skeleton-loader>
      </div>
    </div>
  `,
  styles: [`
    .skeleton-card {
      background: var(--color-white, #ffffff);
      border-radius: var(--border-radius, 8px);
      box-shadow: var(--box-shadow-sm, 0 2px 4px rgba(0,0,0,0.1));
      padding: 20px;
      display: flex;
      flex-direction: column;
      gap: 16px;
    }

    .skeleton-card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding-bottom: 16px;
      border-bottom: 1px solid var(--color-border, #e0e0e0);
    }

    .skeleton-card-body {
      flex: 1;
    }

    .skeleton-card-content {
      display: flex;
      flex-direction: column;
    }

    .skeleton-card-footer {
      padding-top: 16px;
      border-top: 1px solid var(--color-border, #e0e0e0);
    }
  `]
})
export class SkeletonCardComponent {
  @Input() showHeader: boolean = true;
  @Input() showHeaderActions: boolean = false;
  @Input() headerTitleWidth: string = '200px';
  @Input() showFooter: boolean = false;
  @Input() hasContent: boolean = false;
  @Input() contentLines: Array<{ width: string; height: string }> = [
    { width: '100%', height: '16px' },
    { width: '90%', height: '16px' },
    { width: '80%', height: '16px' }
  ];
  @Input() animation: 'pulse' | 'wave' | 'shimmer' = 'shimmer';
}

