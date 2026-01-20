import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkeletonLoaderComponent } from '../skeleton-loader/skeleton-loader.component';

@Component({
  selector: 'app-skeleton-field',
  standalone: true,
  imports: [CommonModule, SkeletonLoaderComponent],
  template: `
    <div class="skeleton-field" [class.skeleton-field-with-label]="showLabel">
      <app-skeleton-loader
        *ngIf="showLabel"
        [width]="labelWidth"
        [height]="'16px'"
        [shape]="'rounded'"
        [animation]="animation"
        class="skeleton-label">
      </app-skeleton-loader>
      <app-skeleton-loader
        [width]="fieldWidth"
        [height]="fieldHeight"
        [shape]="'rounded'"
        [animation]="animation"
        class="skeleton-input">
      </app-skeleton-loader>
    </div>
  `,
  styles: [`
    .skeleton-field {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }

    .skeleton-field-with-label {
      gap: 6px;
    }

    .skeleton-label {
      margin-bottom: 4px;
    }

    .skeleton-input {
      width: 100%;
    }
  `]
})
export class SkeletonFieldComponent {
  @Input() showLabel: boolean = true;
  @Input() labelWidth: string = '80px';
  @Input() fieldWidth: string = '100%';
  @Input() fieldHeight: string = '40px';
  @Input() animation: 'pulse' | 'wave' | 'shimmer' = 'shimmer';
}

