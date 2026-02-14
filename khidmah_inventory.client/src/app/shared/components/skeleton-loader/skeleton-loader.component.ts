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
      display: inline-block;
      background: var(--skeleton-base, #eef1f5);
      border-radius: var(--skeleton-radius, 8px);
    }

    .skeleton-loader.skeleton-shimmer {
      background: linear-gradient(
        90deg,
        var(--skeleton-base, #eef1f5) 0%,
        var(--skeleton-highlight, #f5f7fa) 40%,
        var(--skeleton-base, #eef1f5) 80%
      );
      background-size: 200% 100%;
      animation: skeleton-shimmer 1.8s ease-in-out infinite;
    }

    .skeleton-loader.skeleton-pulse {
      animation: skeleton-pulse 1.5s ease-in-out infinite;
    }

    .skeleton-circle {
      border-radius: 50%;
    }

    .skeleton-rounded {
      border-radius: var(--skeleton-radius, 8px);
    }

    .skeleton-rectangle {
      border-radius: 0;
    }

    @keyframes skeleton-shimmer {
      0% { background-position: -200% 0; }
      100% { background-position: 200% 0; }
    }

    @keyframes skeleton-pulse {
      0%, 100% { opacity: 1; }
      50% { opacity: 0.55; }
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

