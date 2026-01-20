import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UnifiedButtonComponent } from '../unified-button/unified-button.component';
import { UnifiedInputComponent } from '../unified-input/unified-input.component';
import { IconComponent } from '../icon/icon.component';
import { DataTableColumn } from '../../models/data-table.model';

import { ClickOutsideDirective } from '../../directives/click-outside.directive';

@Component({
  selector: 'app-listing-header',
  standalone: true,
  imports: [CommonModule, FormsModule, UnifiedButtonComponent, UnifiedInputComponent, IconComponent, ClickOutsideDirective],
  template: `
    <div class="listing-header d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
      <!-- Search Section -->
      <div class="search-section flex-grow-1" style="max-width: 450px;">
        <div class="input-group">
          <span class="input-group-text"><i class="bi bi-search"></i></span>
          <input 
            type="text" 
            class="form-control" 
            [placeholder]="searchPlaceholder" 
            [(ngModel)]="searchTerm" 
            (keyup.enter)="onSearch()"
            (input)="onSearchInput()">
          <button *ngIf="searchTerm" class="btn btn-link text-muted" (click)="clearSearch()">
            <i class="bi bi-x-circle"></i>
          </button>
        </div>
      </div>

      <!-- Actions Section -->
      <div class="actions-section d-flex align-items-center gap-2">
        <app-unified-button
          *ngIf="showFilters && !showGenericFilter"
          variant="primary"
          (clicked)="filterClicked.emit()"
          data-filter-button
          icon="bi-filter">
          Filters
          <span *ngIf="filterCount > 0" class="badge bg-white text-primary ms-1">{{ filterCount }}</span>
        </app-unified-button>

        <!-- Generic Filter Button -->
        <div *ngIf="showGenericFilter" class="position-relative">
          <app-unified-button
            variant="primary"
            (clicked)="onToggleGenericFilter()"
            data-filter-button>
            <app-icon name="filter" size="sm" customClass="me-2"></app-icon>
            Filters
            <span *ngIf="filterCount > 0" class="badge bg-white text-primary ms-1">{{ filterCount }}</span>
          </app-unified-button>
        </div>

        <div *ngIf="showColumnToggle" class="dropdown position-relative" (clickOutside)="onCloseColumns()">
          <app-unified-button
            variant="secondary"
            [outlined]="true"
            (clicked)="onToggleColumns()"
            icon="bi-columns-gap">
            Columns
            <span class="ms-1 small">({{ visibleColumnsCount }}/{{ totalColumnsCount }})</span>
          </app-unified-button>
          
          <div class="dropdown-menu dropdown-menu-end shadow-sm border-0 mt-2 p-3" 
               [class.show]="showColumnDropdown"
               style="min-width: 200px; z-index: 1050;">
            <h6 class="dropdown-header px-0 mb-2">Visible Columns</h6>
            <div class="column-list" style="max-height: 300px; overflow-y: auto;">
              <div *ngFor="let col of columns" class="form-check mb-2">
                <input 
                  class="form-check-input" 
                  type="checkbox" 
                  [id]="'col-' + col.key"
                  [checked]="col.visible !== false"
                  (change)="columnToggle.emit(col)">
                <label class="form-check-label small" [for]="'col-' + col.key">
                  {{ col.label }}
                </label>
              </div>
            </div>
          </div>
        </div>

        <ng-content></ng-content>
      </div>
    </div>
  `,
  styles: [`
    .listing-header {
      background: transparent;
    }
    .dropdown-menu.show {
      display: block;
    }
  `]
})
export class ListingHeaderComponent {
  @Input() searchPlaceholder: string = 'Search...';
  @Input() searchTerm: string = '';
  @Input() showFilters: boolean = true;
  @Input() filterCount: number = 0;
  @Input() hasActiveFilters: boolean = false;
  @Input() showColumnToggle: boolean = true;
  @Input() totalColumnsCount: number = 0;
  @Input() visibleColumnsCount: number = 0;
  @Input() columns: DataTableColumn[] = [];
  @Input() showColumnDropdown: boolean = false;
  @Output() showColumnDropdownChange = new EventEmitter<boolean>();

  @Output() searchTermChange = new EventEmitter<string>();
  @Output() search = new EventEmitter<string>();
  @Output() filterClicked = new EventEmitter<void>();
  @Output() toggleColumns = new EventEmitter<void>();
  @Output() closeColumns = new EventEmitter<void>();
  @Output() columnToggle = new EventEmitter<DataTableColumn>();

  // New inputs for generic filter support
  @Input() showGenericFilter = false;
  @Input() isFilterOpen = false;
  @Output() isFilterOpenChange = new EventEmitter<boolean>();

  onSearch(): void {
    this.searchTermChange.emit(this.searchTerm);
    this.search.emit(this.searchTerm);
  }

  onSearchInput(): void {
    this.searchTermChange.emit(this.searchTerm);
    if (!this.searchTerm) {
      this.search.emit('');
    }
  }

  clearSearch(): void {
    this.searchTerm = '';
    this.searchTermChange.emit('');
    this.search.emit('');
  }

  onCloseColumns(): void {
    if (this.showColumnDropdown) {
      this.showColumnDropdown = false;
      this.showColumnDropdownChange.emit(false);
      this.closeColumns.emit();
    }
  }

  onToggleColumns(): void {
    this.showColumnDropdown = !this.showColumnDropdown;
    this.showColumnDropdownChange.emit(this.showColumnDropdown);
    this.toggleColumns.emit();
  }

  onToggleGenericFilter(): void {
    this.isFilterOpenChange.emit(!this.isFilterOpen);
  }
}
