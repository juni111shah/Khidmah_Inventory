import { Component, Input, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkeletonLoaderComponent } from '../skeleton-loader/skeleton-loader.component';

@Component({
  selector: 'app-skeleton-table',
  standalone: true,
  imports: [CommonModule, SkeletonLoaderComponent],
  template: `
    <div class="skeleton-table-wrapper">
      <div class="table-responsive border shadow-sm mb-0">
        <table class="skeleton-table">
          <thead class="table-light" *ngIf="showHeader">
            <tr>
              <th *ngFor="let h of headerWidths" [style.width]="h" [style.minWidth]="h">
                <app-skeleton-loader
                  [width]="'70%'"
                  [height]="'14px'"
                  shape="rounded"
                  [animation]="animation">
                </app-skeleton-loader>
              </th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let row of builtRows">
              <td *ngFor="let cell of row" [style.width]="cell.width">
                <app-skeleton-loader
                  [width]="cell.contentWidth"
                  [height]="'16px'"
                  shape="rounded"
                  [animation]="animation">
                </app-skeleton-loader>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: [`
    .skeleton-table-wrapper { width: 100%; }
    .skeleton-table { width: 100%; border-collapse: collapse; table-layout: fixed; }
    .skeleton-table thead th {
      padding: 12px 16px;
      text-align: left;
      border-bottom: 2px solid var(--skeleton-border, #e2e8f0);
      vertical-align: middle;
    }
    .skeleton-table td {
      padding: 12px 16px;
      border-bottom: 1px solid var(--skeleton-border, #e2e8f0);
      vertical-align: middle;
    }
    .skeleton-table tbody tr:last-child td { border-bottom: none; }
  `]
})
export class SkeletonTableComponent implements OnInit, OnChanges {
  @Input() showHeader = true;
  /** Number of columns (used when building from rows/columns) */
  @Input() columns: number = 4;
  /** Number of data rows */
  @Input() rows: number = 10;
  /** Optional: explicit header widths (e.g. from data-table columns) */
  @Input() headerWidths: string[] = [];
  @Input() animation: 'pulse' | 'shimmer' = 'shimmer';

  builtRows: Array<Array<{ width: string; contentWidth: string }>> = [];

  ngOnInit(): void {
    this.build();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['rows'] || changes['columns'] || changes['headerWidths']) {
      this.build();
    }
  }

  private build(): void {
    const colCount = this.headerWidths?.length > 0 ? this.headerWidths.length : this.columns;
    if (this.headerWidths?.length === 0) {
      const pct = 100 / colCount;
      this.headerWidths = Array.from({ length: colCount }, () => `${pct}%`);
    }
    const widths = this.headerWidths.length >= colCount
      ? this.headerWidths.slice(0, colCount)
      : Array.from({ length: colCount }, () => `${100 / colCount}%`);
    this.builtRows = Array.from({ length: this.rows }, (_, rowIdx) =>
      widths.map((w, i) => ({
        width: w,
        contentWidth: `${60 + (rowIdx + i) % 35}%`
      }))
    );
  }
}
