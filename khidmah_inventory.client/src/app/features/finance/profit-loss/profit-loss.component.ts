import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FinanceApiService } from '../../../core/services/finance-api.service';
import { ProfitLossDto } from '../../../core/models/finance.model';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';

@Component({
  selector: 'app-profit-loss',
  standalone: true,
  imports: [CommonModule, FormsModule, UnifiedCardComponent, ContentLoaderComponent],
  templateUrl: './profit-loss.component.html'
})
export class ProfitLossComponent implements OnInit {
  loading = false;
  report: ProfitLossDto | null = null;
  expandedSection: 'revenue' | 'expense' | null = null;

  filter = {
    fromDate: this.getDateOffset(-30),
    toDate: this.getDateOffset(0)
  };

  constructor(private financeApi: FinanceApiService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.financeApi.getProfitLoss(this.filter.fromDate, this.filter.toDate).subscribe({
      next: (res) => {
        this.loading = false;
        this.report = res.success && res.data ? res.data : null;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  toggleDrill(section: 'revenue' | 'expense'): void {
    this.expandedSection = this.expandedSection === section ? null : section;
  }

  exportCsv(): void {
    if (!this.report) return;

    const rows = [
      'Section,Code,Name,Amount',
      ...this.report.revenueLines.map(l => `Revenue,${this.escape(l.code)},${this.escape(l.name)},${l.amount.toFixed(2)}`),
      ...this.report.expenseLines.map(l => `Expense,${this.escape(l.code)},${this.escape(l.name)},${l.amount.toFixed(2)}`),
      `Summary,,Revenue,${this.report.revenue.toFixed(2)}`,
      `Summary,,COGS,${this.report.cogs.toFixed(2)}`,
      `Summary,,GrossProfit,${this.report.grossProfit.toFixed(2)}`,
      `Summary,,Expenses,${this.report.expenses.toFixed(2)}`,
      `Summary,,NetProfit,${this.report.netProfit.toFixed(2)}`
    ];

    const blob = new Blob([rows.join('\n')], { type: 'text/csv;charset=utf-8;' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `profit-loss-${this.filter.fromDate}-${this.filter.toDate}.csv`;
    a.click();
    window.URL.revokeObjectURL(url);
  }

  private escape(v: string): string {
    return `"${(v || '').replace(/"/g, '""')}"`;
  }

  private getDateOffset(offsetDays: number): string {
    const d = new Date();
    d.setDate(d.getDate() + offsetDays);
    return d.toISOString().split('T')[0];
  }
}
