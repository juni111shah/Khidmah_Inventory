import { Component, Input, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkeletonLoaderComponent } from '../skeleton-loader/skeleton-loader.component';

@Component({
  selector: 'app-skeleton-list',
  standalone: true,
  imports: [CommonModule, SkeletonLoaderComponent],
  template: `
    <div class="skeleton-list" [class.skeleton-list-grid]="layout === 'grid'" [style.grid-template-columns]="layout === 'grid' ? gridColumns : null">
      <div *ngFor="let item of builtItems" class="skeleton-list-item" [class.skeleton-list-item-horizontal]="direction === 'horizontal'">
        <app-skeleton-loader
          *ngIf="showAvatar"
          [width]="avatarSize"
          [height]="avatarSize"
          shape="circle"
          [animation]="animation"
          class="skeleton-avatar">
        </app-skeleton-loader>
        <div class="skeleton-list-content flex-grow-1">
          <app-skeleton-loader
            *ngFor="let line of item.lines"
            [width]="line.width"
            [height]="line.height"
            shape="rounded"
            [animation]="animation"
            [style.margin-bottom]="line.marginBottom || '6px'">
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
    .skeleton-list-grid {
      display: grid;
      gap: 16px;
    }
    .skeleton-list-item {
      display: flex;
      gap: 12px;
      align-items: flex-start;
    }
    .skeleton-list-item-horizontal { flex-direction: row; }
    .skeleton-avatar { flex-shrink: 0; }
    .skeleton-list-content { min-width: 0; }
  `]
})
export class SkeletonListComponent implements OnInit, OnChanges {
  @Input() itemCount = 5;
  @Input() showAvatar = false;
  @Input() avatarSize = '40px';
  @Input() direction: 'vertical' | 'horizontal' = 'vertical';
  /** 'list' | 'grid' – grid uses gridColumns */
  @Input() layout: 'list' | 'grid' = 'list';
  @Input() gridColumns = 'repeat(auto-fill, minmax(200px, 1fr))';
  @Input() animation: 'pulse' | 'shimmer' = 'shimmer';

  builtItems: Array<{ lines: Array<{ width: string; height: string; marginBottom?: string }> }> = [];

  ngOnInit(): void {
    this.build();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['itemCount']) this.build();
  }

  private build(): void {
    const lineWidths = ['80%', '75%', '85%', '70%', '90%', '65%'];
    this.builtItems = Array.from({ length: this.itemCount }, (_, i) => ({
      lines: [
        { width: lineWidths[i % lineWidths.length], height: '16px', marginBottom: '6px' },
        { width: `${50 + (i % 4) * 10}%`, height: '12px', marginBottom: '0' }
      ]
    }));
  }
}
