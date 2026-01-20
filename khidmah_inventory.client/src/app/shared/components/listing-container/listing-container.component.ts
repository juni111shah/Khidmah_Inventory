import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListingHeaderComponent } from '../listing-header/listing-header.component';
import { GenericFilterComponent, FilterField, AppliedFilter } from '../generic-filter/generic-filter.component';
import { DataTableColumn } from '../../models/data-table.model';
import { FilterRequest } from '../../../core/models/user.model';

@Component({
  selector: 'app-listing-container',
  standalone: true,
  imports: [CommonModule, ListingHeaderComponent, GenericFilterComponent],
  template: `
    <div class="listing-container py-4">
      <div class="listing-card card border-0 shadow-sm p-4">
        <app-listing-header
          [searchPlaceholder]="searchPlaceholder"
          [searchTerm]="searchTerm"
          (searchTermChange)="searchTerm = $event; searchTermChange.emit($event)"
          [showFilters]="showFilters"
          [filterCount]="filterCount"
          [hasActiveFilters]="hasActiveFilters"
          [showColumnToggle]="showColumnToggle"
          [totalColumnsCount]="totalColumnsCount"
          [visibleColumnsCount]="visibleColumnsCount"
          [columns]="columns"
          [(showColumnDropdown)]="showColumnDropdown"
          (showColumnDropdownChange)="showColumnDropdownChange.emit($event)"
          (search)="search.emit($event)"
          (filterClicked)="filterClicked.emit()"
          (toggleColumns)="toggleColumns.emit()"
          (closeColumns)="closeColumns.emit()"
          (columnToggle)="columnToggle.emit($event)"
          [showGenericFilter]="useGenericFilter"
          [isFilterOpen]="isFilterOpen"
          (isFilterOpenChange)="isFilterOpen = $event; isFilterOpenChange.emit($event)">
          <ng-content select="[listing-actions]"></ng-content>
          <ng-content select="[listing-export]"></ng-content>
        </app-listing-header>

        <app-generic-filter
          *ngIf="useGenericFilter"
          [fields]="filterFields"
          [isOpen]="isFilterOpen"
          [appliedFilters]="appliedFilters"
          (isOpenChange)="isFilterOpen = $event; isFilterOpenChange.emit($event)"
          (filterApplied)="filterApplied.emit($event)"
          (filterReset)="filterReset.emit()"
          (filterRemoved)="filterRemoved.emit($event)">
        </app-generic-filter>

        <div class="filter-panel-wrapper mb-4" *ngIf="showFilterPanel">
          <ng-content select="[filter-panel]"></ng-content>
        </div>

        <div class="listing-content">
          <ng-content></ng-content>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .listing-container {
      max-width: 100%;
      position: relative;
    }
    .page-title {
      color: var(--text-color);
      font-size: 1.5rem;
    }
.listing-card {
  border-radius: 20px !important;
  background-color: #ffffff;
  padding: 1.5rem !important;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.05) !important;
}

.listing-header {
  margin-bottom: 2rem;
}

.filter-panel-wrapper {
  background-color: #f8f9fa;
  border-radius: 12px;
  padding: 1.5rem;
  border: 1px solid #e9ecef;
  animation: fadeIn 0.3s ease-out;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(-10px); }
  to { opacity: 1; transform: translateY(0); }
}
  `]
})
export class ListingContainerComponent {
  @Input() title: string = '';
  @Input() subtitle: string = '';
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
  @Input() showFilterPanel: boolean = false;

  // Generic filter inputs
  @Input() useGenericFilter = false;
  @Input() filterFields: FilterField[] = [];
  @Input() appliedFilters: AppliedFilter[] = [];
  @Input() isFilterOpen = false;
  @Output() isFilterOpenChange = new EventEmitter<boolean>();
  @Output() filterApplied = new EventEmitter<{ [key: string]: any }>();
  @Output() filterReset = new EventEmitter<void>();
  @Output() filterRemoved = new EventEmitter<string>();

  @Output() searchTermChange = new EventEmitter<string>();
  @Output() search = new EventEmitter<string>();
  @Output() filterClicked = new EventEmitter<void>();
  @Output() toggleColumns = new EventEmitter<void>();
  @Output() closeColumns = new EventEmitter<void>();
  @Output() columnToggle = new EventEmitter<DataTableColumn>();
}
