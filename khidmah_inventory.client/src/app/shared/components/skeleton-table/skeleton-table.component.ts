import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkeletonLoaderComponent } from '../skeleton-loader/skeleton-loader.component';

@Component({
  selector: 'app-skeleton-table',
  standalone: true,
  imports: [CommonModule, SkeletonLoaderComponent],
  template: `
    <div class="skeleton-table-container">
      <table class="skeleton-table">
        <thead *ngIf="showHeader">
          <tr>
            <th *ngFor="let header of headers" [style.width]="header.width">
              <app-skeleton-loader
                [width]="'80%'"
                [height]="'20px'"
                [shape]="'rounded'"
                [animation]="animation">
              </app-skeleton-loader>
            </th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let row of rows">
            <td *ngFor="let cell of row.cells" [style.width]="cell.width">
              <app-skeleton-loader
                [width]="cell.contentWidth"
                [height]="cell.height"
                [shape]="'rounded'"
                [animation]="animation">
              </app-skeleton-loader>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  `,
  styles: [`
    .skeleton-table-container {
      width: 100%;
      overflow-x: auto;
    }

    .skeleton-table {
      width: 100%;
      border-collapse: collapse;
    }

    .skeleton-table thead {
      background-color: var(--color-background-light, #f5f5f5);
    }

    .skeleton-table th {
      padding: 12px 15px;
      text-align: left;
      border-bottom: 2px solid var(--color-border, #e0e0e0);
    }

    .skeleton-table td {
      padding: 12px 15px;
      border-bottom: 1px solid var(--color-border, #e0e0e0);
    }

    .skeleton-table tbody tr:last-child td {
      border-bottom: none;
    }
  `]
})
export class SkeletonTableComponent implements OnInit {
  @Input() showHeader: boolean = true;
  @Input() headers: Array<{ width?: string }> = [
    { width: '25%' },
    { width: '25%' },
    { width: '25%' },
    { width: '25%' }
  ];
  @Input() rows: Array<{ cells: Array<{ width?: string; contentWidth: string; height: string }> }> = [
    {
      cells: [
        { width: '25%', contentWidth: '90%', height: '16px' },
        { width: '25%', contentWidth: '80%', height: '16px' },
        { width: '25%', contentWidth: '70%', height: '16px' },
        { width: '25%', contentWidth: '60%', height: '16px' }
      ]
    },
    {
      cells: [
        { width: '25%', contentWidth: '85%', height: '16px' },
        { width: '25%', contentWidth: '75%', height: '16px' },
        { width: '25%', contentWidth: '65%', height: '16px' },
        { width: '25%', contentWidth: '55%', height: '16px' }
      ]
    },
    {
      cells: [
        { width: '25%', contentWidth: '95%', height: '16px' },
        { width: '25%', contentWidth: '85%', height: '16px' },
        { width: '25%', contentWidth: '75%', height: '16px' },
        { width: '25%', contentWidth: '65%', height: '16px' }
      ]
    },
    {
      cells: [
        { width: '25%', contentWidth: '88%', height: '16px' },
        { width: '25%', contentWidth: '78%', height: '16px' },
        { width: '25%', contentWidth: '68%', height: '16px' },
        { width: '25%', contentWidth: '58%', height: '16px' }
      ]
    },
    {
      cells: [
        { width: '25%', contentWidth: '92%', height: '16px' },
        { width: '25%', contentWidth: '82%', height: '16px' },
        { width: '25%', contentWidth: '72%', height: '16px' },
        { width: '25%', contentWidth: '62%', height: '16px' }
      ]
    }
  ];
  @Input() rowCount: number = 5;
  @Input() animation: 'pulse' | 'wave' | 'shimmer' = 'shimmer';

  ngOnInit() {
    if (this.rowCount > 0 && this.rows.length === 0) {
      // Generate rows based on rowCount
      this.rows = Array.from({ length: this.rowCount }, () => ({
        cells: this.headers.map(() => ({
          contentWidth: `${Math.floor(Math.random() * 30) + 60}%`,
          height: '16px'
        }))
      }));
    }
  }
}

