import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkeletonLoaderComponent } from '../skeleton-loader/skeleton-loader.component';

@Component({
  selector: 'app-skeleton-list',
  standalone: true,
  imports: [CommonModule, SkeletonLoaderComponent],
  template: `
    <div class="skeleton-list" [class.skeleton-list-vertical]="direction === 'vertical'">
      <div 
        *ngFor="let item of items" 
        class="skeleton-list-item"
        [class.skeleton-list-item-horizontal]="direction === 'horizontal'">
        <app-skeleton-loader
          *ngIf="showAvatar"
          [width]="avatarSize"
          [height]="avatarSize"
          [shape]="'circle'"
          [animation]="animation"
          class="skeleton-avatar">
        </app-skeleton-loader>
        <div class="skeleton-list-content">
          <app-skeleton-loader
            *ngFor="let line of item.lines"
            [width]="line.width"
            [height]="line.height"
            [shape]="'rounded'"
            [animation]="animation"
            [style.margin-bottom]="line.marginBottom || '8px'">
          </app-skeleton-loader>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .skeleton-list {
      display: flex;
      flex-direction: column;
      gap: 16px;
    }

    .skeleton-list-vertical {
      flex-direction: column;
    }

    .skeleton-list-item {
      display: flex;
      gap: 12px;
      align-items: flex-start;
    }

    .skeleton-list-item-horizontal {
      flex-direction: row;
    }

    .skeleton-avatar {
      flex-shrink: 0;
    }

    .skeleton-list-content {
      flex: 1;
      display: flex;
      flex-direction: column;
    }
  `]
})
export class SkeletonListComponent {
  @Input() items: Array<{ lines: Array<{ width: string; height: string; marginBottom?: string }> }> = [
    {
      lines: [
        { width: '80%', height: '16px' },
        { width: '60%', height: '14px', marginBottom: '0' }
      ]
    },
    {
      lines: [
        { width: '75%', height: '16px' },
        { width: '55%', height: '14px', marginBottom: '0' }
      ]
    },
    {
      lines: [
        { width: '85%', height: '16px' },
        { width: '65%', height: '14px', marginBottom: '0' }
      ]
    }
  ];
  @Input() itemCount: number = 3;
  @Input() showAvatar: boolean = false;
  @Input() avatarSize: string = '40px';
  @Input() direction: 'vertical' | 'horizontal' = 'vertical';
  @Input() animation: 'pulse' | 'wave' | 'shimmer' = 'shimmer';

  ngOnInit() {
    if (this.itemCount > 0 && this.items.length === 0) {
      this.items = Array.from({ length: this.itemCount }, () => ({
        lines: [
          { width: `${Math.floor(Math.random() * 20) + 70}%`, height: '16px' },
          { width: `${Math.floor(Math.random() * 20) + 50}%`, height: '14px', marginBottom: '0' }
        ]
      }));
    }
  }
}

