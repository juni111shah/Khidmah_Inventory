import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FilterFieldComponent, FilterOption } from '../filter-field/filter-field.component';
import { UnifiedButtonComponent } from '../unified-button/unified-button.component';
import { IconComponent } from '../icon/icon.component';
import { ClickOutsideDirective } from '../../directives/click-outside.directive';

export interface FilterField {
  key: string;
  label: string;
  type: 'text' | 'select' | 'date' | 'number' | 'boolean';
  placeholder?: string;
  options?: FilterOption[];
  defaultValue?: any;
  required?: boolean;
  disabled?: boolean;
}

export interface AppliedFilter {
  key: string;
  label: string;
  value: any;
  displayValue: string;
  field: FilterField;
}

@Component({
  selector: 'app-generic-filter',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    FilterFieldComponent,
    UnifiedButtonComponent,
    IconComponent,
    ClickOutsideDirective
  ],
  templateUrl: './generic-filter.component.html',
  styleUrls: ['./generic-filter.component.scss']
})
export class GenericFilterComponent implements OnInit, OnChanges {
  @Input() fields: FilterField[] = [];
  @Input() isOpen = false;
  @Input() appliedFilters: AppliedFilter[] = [];
  @Input() showResetButton = true;
  @Input() showApplyButton = true;
  @Input() resetButtonText = 'Reset';
  @Input() applyButtonText = 'Apply Filters';

  @Output() isOpenChange = new EventEmitter<boolean>();
  @Output() filterApplied = new EventEmitter<{ [key: string]: any }>();
  @Output() filterReset = new EventEmitter<void>();
  @Output() filterRemoved = new EventEmitter<string>();

  filterValues: { [key: string]: any } = {};

  ngOnInit(): void {
    this.initializeFilterValues();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['fields'] && !changes['fields'].firstChange) {
      this.initializeFilterValues();
    }
  }

  private initializeFilterValues(): void {
    this.fields.forEach(field => {
      if (field.defaultValue !== undefined) {
        this.filterValues[field.key] = field.defaultValue;
      } else {
        this.filterValues[field.key] = this.getDefaultValueForType(field.type);
      }
    });
  }

  private getDefaultValueForType(type: string): any {
    switch (type) {
      case 'select':
        return '';
      case 'boolean':
        return null;
      case 'number':
        return null;
      case 'date':
        return null;
      default:
        return '';
    }
  }

  onFilterValueChange(fieldKey: string, value: any): void {
    this.filterValues[fieldKey] = value;
  }

  applyFilters(): void {
    // Filter out empty/null values
    const activeFilters: { [key: string]: any } = {};
    Object.keys(this.filterValues).forEach(key => {
      const value = this.filterValues[key];
      if (value !== null && value !== undefined && value !== '' && !(Array.isArray(value) && value.length === 0)) {
        activeFilters[key] = value;
      }
    });

    this.filterApplied.emit(activeFilters);
  }

  resetFilters(): void {
    this.fields.forEach(field => {
      this.filterValues[field.key] = field.defaultValue !== undefined
        ? field.defaultValue
        : this.getDefaultValueForType(field.type);
    });
    this.filterReset.emit();
  }

  removeFilter(filterKey: string): void {
    // Reset the specific filter value
    const field = this.fields.find(f => f.key === filterKey);
    if (field) {
      this.filterValues[filterKey] = field.defaultValue !== undefined
        ? field.defaultValue
        : this.getDefaultValueForType(field.type);
    }

    // Emit the filter removal
    this.filterRemoved.emit(filterKey);

    // Auto-apply if there are still other filters
    if (Object.values(this.filterValues).some(v => v !== null && v !== undefined && v !== '')) {
      this.applyFilters();
    }
  }

  onClickOutside(): void {
    if (this.isOpen) {
      this.closeFilter();
    }
  }

  closeFilter(): void {
    this.isOpen = false;
    this.isOpenChange.emit(false);
  }

  toggleFilter(): void {
    this.isOpen = !this.isOpen;
    this.isOpenChange.emit(this.isOpen);
  }

  getDisplayValue(appliedFilter: AppliedFilter): string {
    const field = appliedFilter.field;
    const value = appliedFilter.value;

    if (field.type === 'select' && field.options) {
      const option = field.options.find(opt => opt.value === value || opt.value?.toString() === value?.toString());
      return option ? option.label : value?.toString() || '';
    }

    if (field.type === 'boolean') {
      return value ? 'Yes' : 'No';
    }

    if (field.type === 'date' && value) {
      return new Date(value).toLocaleDateString();
    }

    return value?.toString() || '';
  }

  getAppliedFiltersCount(): number {
    return this.appliedFilters.length;
  }

  hasActiveFilters(): boolean {
    return this.getAppliedFiltersCount() > 0;
  }

  getFieldColumnClass(field: FilterField): string {
    // You can customize column classes based on field type or other properties
    // For now, use consistent column sizing
    return 'col-md-6 col-lg-4';
  }
}