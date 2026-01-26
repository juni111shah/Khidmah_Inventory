import { Injectable } from '@angular/core';
import * as XLSX from 'xlsx';
import * as Papa from 'papaparse';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { DataTableColumn } from '../../shared/models/data-table.model';

export type ExportFormat = 'pdf' | 'excel' | 'csv';

export interface ExportOptions {
  filename?: string;
  title?: string;
  includeFilters?: boolean;
  filters?: any[];
  customHeaders?: string[];
  selectedColumns?: string[];
}

export interface PdfExportOptions {
  filename: string;
  title?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ExportService {

  constructor() { }

  /**
   * Export PDF from backend blob response
   */
  exportBackendPdf(blob: Blob, filename: string): void {
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    link.style.display = 'none';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  }

  /**
   * Export data to the specified format
   */
  async exportData<T>(
    data: T[],
    columns: DataTableColumn<T>[],
    format: ExportFormat,
    options: ExportOptions = {}
  ): Promise<void> {
    const filename = options.filename || `export_${new Date().getTime()}`;
    const visibleColumns = columns.filter(col => col.visible !== false);

    switch (format) {
      case 'pdf':
        await this.exportToPDF(data, visibleColumns, options);
        break;
      case 'excel':
        this.exportToExcel(data, visibleColumns, filename, options);
        break;
      case 'csv':
        this.exportToCSV(data, visibleColumns, filename, options);
        break;
      default:
        throw new Error(`Unsupported export format: ${format}`);
    }
  }

  /**
   * Export data to PDF format
   */
  private async exportToPDF<T>(
    data: T[],
    columns: DataTableColumn<T>[],
    options: ExportOptions
  ): Promise<void> {
    const filename = options.filename || `export_${new Date().getTime()}`;

    // Create PDF document
    const pdf = new jsPDF('landscape');
    const pageWidth = pdf.internal.pageSize.getWidth();
    const pageHeight = pdf.internal.pageSize.getHeight();
    const margin = 10;
    let yPosition = margin + 15;

    // Add title
    pdf.setFontSize(18);
    pdf.text(options.title || 'Data Export', margin, yPosition);
    yPosition += 10;

    // Add filters info if provided
    if (options.includeFilters && options.filters && options.filters.length > 0) {
      pdf.setFontSize(10);
      pdf.text('Applied Filters:', margin, yPosition);
      yPosition += 5;

      options.filters.forEach(filter => {
        pdf.text(`• ${filter.column}: ${filter.value}`, margin + 5, yPosition);
        yPosition += 5;
      });
      yPosition += 5;
    }

    // Add timestamp
    pdf.setFontSize(8);
    pdf.text(`Generated on: ${new Date().toLocaleString()}`, margin, yPosition);
    yPosition += 10;

    // Calculate column widths
    const availableWidth = pageWidth - (margin * 2);
    const columnWidths = this.calculateColumnWidths(columns, availableWidth);

    // Add table headers
    pdf.setFontSize(10);
    let xPosition = margin;

    columns.forEach((column, index) => {
      pdf.text(column.label, xPosition, yPosition);
      xPosition += columnWidths[index];
    });

    yPosition += 8;

    // Draw header line
    pdf.line(margin, yPosition - 2, pageWidth - margin, yPosition - 2);
    yPosition += 5;

    // Add data rows
    pdf.setFontSize(8);
    const rowHeight = 6;
    const maxRowsPerPage = Math.floor((pageHeight - yPosition - margin) / rowHeight);

    for (let i = 0; i < data.length; i++) {
      // Check if we need a new page
      if (i > 0 && i % maxRowsPerPage === 0) {
        pdf.addPage();
        yPosition = margin + 15;

        // Re-add headers on new page
        xPosition = margin;
        columns.forEach((column, index) => {
          pdf.text(column.label, xPosition, yPosition);
          xPosition += columnWidths[index];
        });
        yPosition += 8;
        pdf.line(margin, yPosition - 2, pageWidth - margin, yPosition - 2);
        yPosition += 5;
      }

      xPosition = margin;
      columns.forEach((column, index) => {
        const cellValue = this.getCellExportValue(data[i], column);
        const truncatedValue = this.truncateText(cellValue, columnWidths[index] / 2); // Rough character estimation
        pdf.text(truncatedValue, xPosition, yPosition);
        xPosition += columnWidths[index];
      });

      yPosition += rowHeight;
    }

    pdf.save(`${filename}.pdf`);
  }

  /**
   * Export single entity details to PDF
   */
  async exportEntityDetails(
    details: { label: string; value: any }[],
    title: string,
    filename: string
  ): Promise<void> {
    const pdf = new jsPDF();
    const pageWidth = pdf.internal.pageSize.getWidth();
    const margin = 15;
    let yPosition = margin + 10;

    // Title
    pdf.setFontSize(18);
    pdf.setTextColor(0, 0, 0);
    pdf.text(title, margin, yPosition);
    yPosition += 10;

    // Generated Date
    pdf.setFontSize(10);
    pdf.setTextColor(100, 100, 100);
    pdf.text(`Generated on: ${new Date().toLocaleString()}`, margin, yPosition);
    yPosition += 15;

    // Draw Line
    pdf.setDrawColor(200, 200, 200);
    pdf.line(margin, yPosition - 5, pageWidth - margin, yPosition - 5);
    yPosition += 5;

    // Details Content
    pdf.setFontSize(11);

    details.forEach(item => {
      // Check for page break
      if (yPosition > pdf.internal.pageSize.getHeight() - margin) {
        pdf.addPage();
        yPosition = margin + 10;
      }

      // Label
      pdf.setFont('helvetica', 'bold');
      pdf.setTextColor(80, 80, 80);
      pdf.text(`${item.label}:`, margin, yPosition);

      // Value
      pdf.setFont('helvetica', 'normal');
      pdf.setTextColor(0, 0, 0);

      const labelWidth = 50; // Fixed width for labels
      const valueX = margin + labelWidth;

      let valueStr = item.value !== null && item.value !== undefined ? String(item.value) : '-';

      // Handle multiline values
      const splitValue = pdf.splitTextToSize(valueStr, pageWidth - valueX - margin);
      pdf.text(splitValue, valueX, yPosition);

      yPosition += (splitValue.length * 7) + 3; // Line height spacing
    });

    // Footer
    const pageCount = pdf.getNumberOfPages();
    for (let i = 1; i <= pageCount; i++) {
        pdf.setPage(i);
        pdf.setFontSize(8);
        pdf.setTextColor(150, 150, 150);
        pdf.text(
          `Page ${i} of ${pageCount}`,
          pageWidth / 2,
          pdf.internal.pageSize.getHeight() - 10,
          { align: 'center' }
        );
    }

    pdf.save(`${filename}.pdf`);
  }

  /**
   * Export data to Excel format
   */
  private exportToExcel<T>(
    data: T[],
    columns: DataTableColumn<T>[],
    filename: string,
    options: ExportOptions
  ): void {
    // Prepare data for Excel
    const excelData: any[][] = [];

    // Add title row if provided
    if (options.title) {
      excelData.push([options.title]);
      excelData.push([]); // Empty row
    }

    // Add filters info if provided
    if (options.includeFilters && options.filters && options.filters.length > 0) {
      excelData.push(['Applied Filters:']);
      options.filters.forEach(filter => {
        excelData.push([`${filter.column}: ${filter.value}`]);
      });
      excelData.push([]); // Empty row
    }

    // Add timestamp
    excelData.push([`Generated on: ${new Date().toLocaleString()}`]);
    excelData.push([]); // Empty row

    // Add headers
    const headers = columns.map(col => col.label);
    excelData.push(headers);

    // Add data rows
    data.forEach(item => {
      const row = columns.map(column => this.getCellExportValue(item, column));
      excelData.push(row);
    });

    // Create workbook
    const ws = XLSX.utils.aoa_to_sheet(excelData);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Data');

    // Auto-size columns
    const colWidths = columns.map(col => ({ wch: Math.max(col.label.length, 15) }));
    ws['!cols'] = colWidths;

    // Save file
    XLSX.writeFile(wb, `${filename}.xlsx`);
  }

  /**
   * Export data to CSV format
   */
  private exportToCSV<T>(
    data: T[],
    columns: DataTableColumn<T>[],
    filename: string,
    options: ExportOptions
  ): void {
    // Prepare data for CSV
    const csvData: any[] = [];

    // Add title if provided
    if (options.title) {
      csvData.push([options.title]);
      csvData.push([]); // Empty row
    }

    // Add filters info if provided
    if (options.includeFilters && options.filters && options.filters.length > 0) {
      csvData.push(['Applied Filters:']);
      options.filters.forEach(filter => {
        csvData.push([`${filter.column}: ${filter.value}`]);
      });
      csvData.push([]); // Empty row
    }

    // Add timestamp
    csvData.push([`Generated on: ${new Date().toLocaleString()}`]);
    csvData.push([]); // Empty row

    // Add headers
    const headers = columns.map(col => col.label);
    csvData.push(headers);

    // Add data rows
    data.forEach(item => {
      const row = columns.map(column => this.getCellExportValue(item, column));
      csvData.push(row);
    });

    // Convert to CSV string
    const csv = Papa.unparse(csvData);

    // Download CSV file
    this.downloadFile(csv, `${filename}.csv`, 'text/csv');
  }

  /**
   * Get the export value for a cell
   */
  private getCellExportValue<T>(row: T, column: DataTableColumn<T>): string {
    if (column.render) {
      return column.render(row, column);
    }

    const value = (row as any)[column.key];

    if (value === null || value === undefined) {
      return '';
    }

    if (column.type === 'date' && value) {
      return new Date(value).toLocaleDateString();
    }

    if (column.type === 'boolean') {
      return value ? 'Yes' : 'No';
    }

    return String(value);
  }

  /**
   * Calculate column widths for PDF export
   */
  private calculateColumnWidths(columns: DataTableColumn<any>[], availableWidth: number): number[] {
    // Use equal width distribution for simplicity
    const widthPerColumn = availableWidth / columns.length;
    return columns.map(() => widthPerColumn);
  }

  /**
   * Truncate text to fit in PDF cell
   */
  private truncateText(text: string, maxWidth: number): string {
    // Rough estimation: 2.5 characters per unit width
    const maxChars = Math.floor(maxWidth * 2.5);
    if (text.length <= maxChars) {
      return text;
    }
    return text.substring(0, maxChars - 3) + '...';
  }

  /**
   * Download a file with the given content
   */
  private downloadFile(content: string, filename: string, mimeType: string): void {
    const blob = new Blob([content], { type: mimeType });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    link.style.display = 'none';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  }
}