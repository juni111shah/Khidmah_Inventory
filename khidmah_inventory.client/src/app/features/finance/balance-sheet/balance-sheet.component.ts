import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FinanceApiService } from '../../../core/services/finance-api.service';
import { BalanceSheetDto } from '../../../core/models/finance.model';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';

@Component({
  selector: 'app-balance-sheet',
  standalone: true,
  imports: [CommonModule, FormsModule, UnifiedCardComponent, ContentLoaderComponent],
  templateUrl: './balance-sheet.component.html'
})
export class BalanceSheetComponent implements OnInit {
  loading = false;
  report: BalanceSheetDto | null = null;
  asOfDate = new Date().toISOString().split('T')[0];
  expandedSection: 'assets' | 'liabilities' | 'equity' | null = null;

  constructor(private financeApi: FinanceApiService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.financeApi.getBalanceSheet(this.asOfDate).subscribe({
      next: (res) => {
        this.loading = false;
        this.report = res.success && res.data ? res.data : null;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  toggleDrill(section: 'assets' | 'liabilities' | 'equity'): void {
    this.expandedSection = this.expandedSection === section ? null : section;
  }

  exportCsv(): void {
    if (!this.report) return;
    const rows = [
      'Section,Code,Name,Amount',
      ...this.report.assetLines.map(l => `Asset,${this.escape(l.code)},${this.escape(l.name)},${l.amount.toFixed(2)}`),
      ...this.report.liabilityLines.map(l => `Liability,${this.escape(l.code)},${this.escape(l.name)},${l.amount.toFixed(2)}`),
      ...this.report.equityLines.map(l => `Equity,${this.escape(l.code)},${this.escape(l.name)},${l.amount.toFixed(2)}`)
    ];
    const blob = new Blob([rows.join('\n')], { type: 'text/csv;charset=utf-8;' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `balance-sheet-${this.asOfDate}.csv`;
    a.click();
    window.URL.revokeObjectURL(url);
  }

  private escape(v: string): string {
    return `"${(v || '').replace(/"/g, '""')}"`;
  }
}
