import { Component, Input, Output, EventEmitter, HostListener, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ExportService, ExportFormat, ExportOptions } from '../../../core/services/export.service';
import { UnifiedButtonComponent } from '../unified-button/unified-button.component';
import { DropdownComponent } from '../dropdown/dropdown.component';
import { IconComponent } from '../icon/icon.component';
import { DataTableColumn } from '../../models/data-table.model';

@Component({
  selector: 'app-export',
  standalone: true,
  imports: [
    CommonModule,
    UnifiedButtonComponent,
    DropdownComponent,
    IconComponent
  ],
  templateUrl: './export.component.html'
})
export class ExportComponent<T = any> {
  @Input() data: T[] = [];
  @Input() columns: DataTableColumn<T>[] = [];
  @Input() title: string = 'Data Export';
  @Input() filename: string = '';
  @Input() includeFilters: boolean = false;
  @Input() filters: any[] = [];
  @Input() disabled: boolean = false;
  @Input() showPdf: boolean = true;
  @Input() showExcel: boolean = true;
  @Input() showCsv: boolean = true;

  @Output() export = new EventEmitter<{ format: ExportFormat; options: ExportOptions }>();

  showDropdown = false;
  isExporting = false;

  constructor(private exportService: ExportService, private cdr: ChangeDetectorRef) {}

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    const container = target.closest('.export-dropdown-container');
    if (!container) {
      this.showDropdown = false;
      this.cdr.detectChanges();
    }
  }

  toggleDropdown(): void {
    if (!this.disabled && !this.isExporting) {
      this.showDropdown = !this.showDropdown;
      this.cdr.detectChanges();
    }
  }

  async exportData(format: ExportFormat): Promise<void> {
    if (this.disabled || this.isExporting || this.data.length === 0) {
      return;
    }

    this.isExporting = true;
    this.showDropdown = false; // Close dropdown immediately when starting export

    try {
      const options: ExportOptions = {
        filename: this.filename || `export_${new Date().getTime()}`,
        title: this.title,
        includeFilters: this.includeFilters,
        filters: this.filters
      };

      await this.exportService.exportData(this.data, this.columns, format, options);
      this.export.emit({ format, options });
    } catch (error) {
      console.error('Export failed:', error);
      // You could emit an error event here if needed
    } finally {
      this.isExporting = false;
    }
  }

  get exportOptions(): Array<{ label: string; format: ExportFormat; icon: string; disabled: boolean }> {
    const options = [];

    if (this.showPdf) {
      options.push({
        label: 'Export as PDF',
        format: 'pdf' as ExportFormat,
        icon: 'file-earmark-pdf',
        disabled: this.isExporting || this.data.length === 0
      });
    }

    if (this.showExcel) {
      options.push({
        label: 'Export as Excel',
        format: 'excel' as ExportFormat,
        icon: 'file-earmark-excel',
        disabled: this.isExporting || this.data.length === 0
      });
    }

    if (this.showCsv) {
      options.push({
        label: 'Export as CSV',
        format: 'csv' as ExportFormat,
        icon: 'file-earmark-spreadsheet',
        disabled: this.isExporting || this.data.length === 0
      });
    }

    return options;
  }

  get buttonText(): string {
    if (this.isExporting) {
      return 'Exporting...';
    }
    return 'Export';
  }

  get hasData(): boolean {
    return this.data && this.data.length > 0;
  }
}