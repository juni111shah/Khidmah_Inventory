import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkeletonFieldComponent } from '../skeleton-field/skeleton-field.component';
import { SkeletonLoaderComponent } from '../skeleton-loader/skeleton-loader.component';

@Component({
  selector: 'app-skeleton-form',
  standalone: true,
  imports: [CommonModule, SkeletonFieldComponent, SkeletonLoaderComponent],
  template: `
    <div class="skeleton-form">
      <app-skeleton-field
        *ngFor="let field of fields"
        [showLabel]="showLabels"
        [labelWidth]="field.labelWidth || defaultLabelWidth"
        [fieldWidth]="field.fieldWidth || '100%'"
        [fieldHeight]="field.fieldHeight || defaultFieldHeight"
        [animation]="animation"
        [style.margin-bottom]="fieldSpacing">
      </app-skeleton-field>
      <div *ngIf="showActions" class="skeleton-form-actions">
        <app-skeleton-loader
          [width]="'120px'"
          [height]="'40px'"
          [shape]="'rounded'"
          [animation]="animation">
        </app-skeleton-loader>
        <app-skeleton-loader
          [width]="'100px'"
          [height]="'40px'"
          [shape]="'rounded'"
          [animation]="animation">
        </app-skeleton-loader>
      </div>
    </div>
  `,
  styles: [`
    .skeleton-form {
      display: flex;
      flex-direction: column;
      gap: 20px;
    }

    .skeleton-form-actions {
      display: flex;
      gap: 12px;
      margin-top: 8px;
    }
  `]
})
export class SkeletonFormComponent implements OnInit {
  @Input() fields: Array<{
    labelWidth?: string;
    fieldWidth?: string;
    fieldHeight?: string;
  }> = [
    { labelWidth: '100px', fieldWidth: '100%', fieldHeight: '40px' },
    { labelWidth: '120px', fieldWidth: '100%', fieldHeight: '40px' },
    { labelWidth: '90px', fieldWidth: '100%', fieldHeight: '40px' },
    { labelWidth: '110px', fieldWidth: '100%', fieldHeight: '40px' }
  ];
  @Input() fieldCount: number = 4;
  @Input() showLabels: boolean = true;
  @Input() showActions: boolean = true;
  @Input() defaultLabelWidth: string = '100px';
  @Input() defaultFieldHeight: string = '40px';
  @Input() fieldSpacing: string = '20px';
  @Input() animation: 'pulse' | 'wave' | 'shimmer' = 'shimmer';

  ngOnInit() {
    if (this.fieldCount > 0 && this.fields.length === 0) {
      this.fields = Array.from({ length: this.fieldCount }, () => ({
        labelWidth: `${Math.floor(Math.random() * 40) + 80}px`,
        fieldWidth: '100%',
        fieldHeight: this.defaultFieldHeight
      }));
    }
  }
}

