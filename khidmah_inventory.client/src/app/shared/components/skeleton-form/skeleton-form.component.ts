import { Component, Input, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkeletonFieldComponent } from '../skeleton-field/skeleton-field.component';
import { SkeletonLoaderComponent } from '../skeleton-loader/skeleton-loader.component';

@Component({
  selector: 'app-skeleton-form',
  standalone: true,
  imports: [CommonModule, SkeletonFieldComponent, SkeletonLoaderComponent],
  template: `
    <div class="skeleton-form">
      <div class="row g-3">
        <div *ngFor="let field of builtFields" class="col-12" [class.col-md-6]="field.half" [class.col-md-4]="field.third">
          <app-skeleton-field
            [showLabel]="showLabels"
            [labelWidth]="field.labelWidth"
            [fieldWidth]="'100%'"
            [fieldHeight]="fieldHeight"
            [animation]="animation">
          </app-skeleton-field>
        </div>
      </div>
      <div *ngIf="showActions" class="skeleton-form-actions mt-4">
        <app-skeleton-loader [width]="'120px'" [height]="'40px'" shape="rounded" [animation]="animation"></app-skeleton-loader>
        <app-skeleton-loader [width]="'100px'" [height]="'40px'" shape="rounded" [animation]="animation"></app-skeleton-loader>
      </div>
    </div>
  `,
  styles: [`
    .skeleton-form { display: flex; flex-direction: column; }
    .skeleton-form-actions { display: flex; gap: 12px; flex-wrap: wrap; }
  `]
})
export class SkeletonFormComponent implements OnInit, OnChanges {
  @Input() fieldCount: number = 6;
  @Input() showLabels = true;
  @Input() showActions = true;
  @Input() fieldHeight: string = '40px';
  @Input() animation: 'pulse' | 'shimmer' = 'shimmer';

  builtFields: Array<{ labelWidth: string; half?: boolean; third?: boolean }> = [];

  ngOnInit(): void {
    this.build();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['fieldCount']) this.build();
  }

  private build(): void {
    const widths = ['90px', '110px', '100px', '120px', '80px', '95px'];
    this.builtFields = Array.from({ length: this.fieldCount }, (_, i) => ({
      labelWidth: widths[i % widths.length],
      half: i % 3 !== 0,
      third: i % 4 === 0
    }));
  }
}
