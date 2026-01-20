import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkeletonLoaderComponent } from '../skeleton-loader/skeleton-loader.component';

@Component({
  selector: 'app-skeleton-button',
  standalone: true,
  imports: [CommonModule, SkeletonLoaderComponent],
  template: `
    <div class="skeleton-button" [class.skeleton-button-block]="block">
      <app-skeleton-loader
        [width]="width"
        [height]="height"
        [shape]="'rounded'"
        [animation]="animation">
      </app-skeleton-loader>
    </div>
  `,
  styles: [`
    .skeleton-button {
      display: inline-block;
    }

    .skeleton-button-block {
      display: block;
      width: 100%;
    }

    .skeleton-button-block app-skeleton-loader {
      width: 100%;
    }
  `]
})
export class SkeletonButtonComponent {
  @Input() width: string = '120px';
  @Input() height: string = '36px';
  @Input() block: boolean = false;
  @Input() animation: 'pulse' | 'wave' | 'shimmer' = 'shimmer';
}

