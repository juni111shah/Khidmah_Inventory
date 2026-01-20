import { Component, Input, OnInit, TemplateRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableSettings } from '../../models/component-settings.model';
import { ComponentSettingsService } from '../../services/component-settings.service';
import { UnifiedCheckboxComponent } from '../unified-checkbox/unified-checkbox.component';

export interface TableColumn {
  key: string;
  label: string;
  sortable?: boolean;
  filterable?: boolean;
  width?: string;
  align?: 'left' | 'center' | 'right';
  template?: TemplateRef<any>;
}

@Component({
  selector: 'app-unified-table',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    UnifiedCheckboxComponent
  ],
  templateUrl: './unified-table.component.html'
})
export class UnifiedTableComponent implements OnInit {
  @Input() id: string = '';
  @Input() columns: TableColumn[] = [];
  @Input() data: any[] = [];
  @Input() style: 'bootstrap' | 'custom' = 'bootstrap';
  @Input() striped: boolean = true;
  @Input() bordered: boolean = false;
  @Input() hoverable: boolean = true;
  @Input() responsive: boolean = true;
  @Input() pagination: boolean = true;
  @Input() sorting: boolean = true;
  @Input() filtering: boolean = false;
  @Input() pageSize: number = 10;
  @Input() pageSizeOptions: number[] = [5, 10, 25, 50, 100];
  @Input() showCheckbox: boolean = false;
  @Input() showActions: boolean = false;
  @Input() stickyHeader: boolean = false;
  @Input() stickyFooter: boolean = false;
  @Input() customClass: string = '';

  displayedColumns: string[] = [];
  selectedRows = new Set<any>();
  currentPage: number = 1;

  constructor(private settingsService: ComponentSettingsService) {}

  ngOnInit(): void {
    if (this.id) {
      const settings = this.settingsService.getTableSettings(this.id);
      if (settings) {
        Object.assign(this, settings);
      }
    }

    this.initializeTable();
  }

  get Math() {
    return Math;
  }

  private initializeTable(): void {
    if (this.showCheckbox) {
      this.displayedColumns.push('select');
    }
    
    this.displayedColumns.push(...this.columns.map(col => col.key));
    
    if (this.showActions) {
      this.displayedColumns.push('actions');
    }
  }

  isAllSelected(): boolean {
    return this.selectedRows.size === this.data.length && this.data.length > 0;
  }

  toggleAllSelection(): void {
    if (this.isAllSelected()) {
      this.selectedRows.clear();
    } else {
      this.data.forEach(row => this.selectedRows.add(row));
    }
  }

  toggleRowSelection(row: any): void {
    if (this.selectedRows.has(row)) {
      this.selectedRows.delete(row);
    } else {
      this.selectedRows.add(row);
    }
  }

  isRowSelected(row: any): boolean {
    return this.selectedRows.has(row);
  }

  getColumnValue(row: any, column: TableColumn): any {
    return row[column.key];
  }

  get tableClasses(): string {
    const classes: string[] = ['table'];
    
    if (this.striped) classes.push('table-striped');
    if (this.bordered) classes.push('table-bordered');
    if (this.hoverable) classes.push('table-hover');
    
    if (this.style === 'custom') {
      classes.push('custom-table');
    }
    
    if (this.customClass) {
      classes.push(this.customClass);
    }
    
    return classes.join(' ');
  }

  get paginatedData(): any[] {
    if (!this.pagination) return this.data;
    const start = (this.currentPage - 1) * this.pageSize;
    const end = start + this.pageSize;
    return this.data.slice(start, end);
  }

  get totalPages(): number {
    return Math.ceil(this.data.length / this.pageSize);
  }

  setPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }
}

