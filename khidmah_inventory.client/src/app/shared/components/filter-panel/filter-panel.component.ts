import { Component, Input, Output, EventEmitter, HostListener, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FilterFieldComponent, FilterOption } from '../filter-field/filter-field.component';
import { UnifiedButtonComponent } from '../unified-button/unified-button.component';
import { IconComponent } from '../icon/icon.component';

export interface FilterPanelField {
  key: string;
  label: string;
  type: 'text' | 'select' | 'date' | 'number' | 'boolean';
  placeholder?: string;
  options?: FilterOption[];
  defaultValue?: any;
  required?: boolean;
  disabled?: boolean;
  colSize?: string; // Bootstrap column class like 'col-md-3', 'col-md-4', etc.
}

export interface FilterPanelConfig {
  fields: FilterPanelField[];
  showResetButton?: boolean;
  showApplyButton?: boolean;
  resetButtonText?: string;
  applyButtonText?: string;
  layout?: 'row' | 'grid'; // 'row' for horizontal layout, 'grid' for responsive grid
}

@Component({
  selector: 'app-filter-panel',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    FilterFieldComponent,
    UnifiedButtonComponent,
    IconComponent
  ],
  templateUrl: './filter-panel.component.html',
  styleUrls: ['./filter-panel.component.scss']
})
export class FilterPanelComponent {
  @Input() config: FilterPanelConfig = { fields: [] };
  @Input() filterValues: { [key: string]: any } = {};

  constructor(private elementRef: ElementRef) {}

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event): void {
    const target = event.target as HTMLElement;

    // Check if the click is on a filter button or filter-related element
    const isFilterButton = target.closest('[data-filter-button]') ||
                          target.closest('.btn')?.textContent?.includes('Filters') ||
                          target.closest('[icon="bi-filter"]');

    // If it's a filter button click, don't close the panel
    if (isFilterButton) {
      return;
    }

    // Use setTimeout to allow other click events to be processed first
    setTimeout(() => {
      if (!this.elementRef.nativeElement.contains(event.target as Node)) {
        this.filterClose.emit();
      }
    }, 10);
  }

  @Output() filterApplied = new EventEmitter<{ [key: string]: any }>();
  @Output() filterReset = new EventEmitter<void>();
  @Output() filterValueChanged = new EventEmitter<{ key: string; value: any }>();
  @Output() filterClose = new EventEmitter<void>();

  get showResetButton(): boolean {
    return this.config.showResetButton ?? true;
  }

  get showApplyButton(): boolean {
    return this.config.showApplyButton ?? true;
  }

  get resetButtonText(): string {
    return this.config.resetButtonText ?? 'Reset';
  }

  get applyButtonText(): string {
    return this.config.applyButtonText ?? 'Apply Filters';
  }

  get layout(): string {
    return this.config.layout ?? 'row';
  }

  onFilterValueChange(fieldKey: string, value: any): void {
    this.filterValues[fieldKey] = value;
    this.filterValueChanged.emit({ key: fieldKey, value });
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
    this.filterClose.emit();
  }

  resetFilters(): void {
    // Reset all filter values to their defaults
    this.config.fields.forEach(field => {
      this.filterValues[field.key] = field.defaultValue !== undefined
        ? field.defaultValue
        : this.getDefaultValueForType(field.type);
    });

    this.filterReset.emit();
    this.filterClose.emit();
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

  getFieldColumnClass(field: FilterPanelField): string {
    return field.colSize || 'col-md-3';
  }
}