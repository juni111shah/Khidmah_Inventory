import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-skeleton-loader',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div 
      class="skeleton-loader"
      [class.skeleton-pulse]="animation === 'pulse'"
      [class.skeleton-wave]="animation === 'wave'"
      [class.skeleton-shimmer]="animation === 'shimmer'"
      [style.width]="width"
      [style.height]="height"
      [style.border-radius]="borderRadius"
      [class.skeleton-circle]="shape === 'circle'"
      [class.skeleton-rounded]="shape === 'rounded'"
      [class.skeleton-rectangle]="shape === 'rectangle'">
    </div>
  `,
  styles: [`
    .skeleton-loader {
      background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
      background-size: 200% 100%;
      animation: skeleton-loading 1.5s ease-in-out infinite;
      display: inline-block;
    }

    .skeleton-pulse {
      animation: skeleton-pulse 1.5s ease-in-out infinite;
    }

    .skeleton-wave {
      animation: skeleton-wave 1.5s ease-in-out infinite;
    }

    .skeleton-shimmer {
      animation: skeleton-shimmer 2s ease-in-out infinite;
    }

    .skeleton-circle {
      border-radius: 50%;
    }

    .skeleton-rounded {
      border-radius: 8px;
    }

    .skeleton-rectangle {
      border-radius: 0;
    }

    @keyframes skeleton-loading {
      0% {
        background-position: 200% 0;
      }
      100% {
        background-position: -200% 0;
      }
    }

    @keyframes skeleton-pulse {
      0%, 100% {
        opacity: 1;
      }
      50% {
        opacity: 0.5;
      }
    }

    @keyframes skeleton-wave {
      0% {
        transform: translateX(-100%);
      }
      100% {
        transform: translateX(100%);
      }
    }

    @keyframes skeleton-shimmer {
      0% {
        background-position: -1000px 0;
      }
      100% {
        background-position: 1000px 0;
      }
    }
  `]
})
export class SkeletonLoaderComponent {
  @Input() width: string = '100%';
  @Input() height: string = '20px';
  @Input() shape: 'circle' | 'rounded' | 'rectangle' = 'rounded';
  @Input() animation: 'pulse' | 'wave' | 'shimmer' = 'shimmer';
  @Input() borderRadius: string = '';
}

