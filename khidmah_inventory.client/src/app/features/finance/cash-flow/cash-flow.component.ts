import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FinanceApiService } from '../../../core/services/finance-api.service';
import { CashFlowDto } from '../../../core/models/finance.model';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';

@Component({
  selector: 'app-cash-flow',
  standalone: true,
  imports: [CommonModule, FormsModule, UnifiedCardComponent, ContentLoaderComponent],
  templateUrl: './cash-flow.component.html'
})
export class CashFlowComponent implements OnInit {
  loading = false;
  report: CashFlowDto | null = null;

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
    this.financeApi.getCashFlow(this.filter.fromDate, this.filter.toDate).subscribe({
      next: (res) => {
        this.loading = false;
        this.report = res.success && res.data ? res.data : null;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  exportCsv(): void {
    if (!this.report) return;
    const rows = [
      'Metric,Amount',
      `Operating Inflow,${this.report.operatingInflow.toFixed(2)}`,
      `Operating Outflow,${this.report.operatingOutflow.toFixed(2)}`,
      `Net Operating,${this.report.netOperating.toFixed(2)}`,
      `Investing Inflow,${this.report.investingInflow.toFixed(2)}`,
      `Investing Outflow,${this.report.investingOutflow.toFixed(2)}`,
      `Net Investing,${this.report.netInvesting.toFixed(2)}`,
      `Financing Inflow,${this.report.financingInflow.toFixed(2)}`,
      `Financing Outflow,${this.report.financingOutflow.toFixed(2)}`,
      `Net Financing,${this.report.netFinancing.toFixed(2)}`,
      `Net Cash Change,${this.report.netCashChange.toFixed(2)}`
    ];
    const blob = new Blob([rows.join('\n')], { type: 'text/csv;charset=utf-8;' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `cash-flow-${this.filter.fromDate}-${this.filter.toDate}.csv`;
    a.click();
    window.URL.revokeObjectURL(url);
  }

  private getDateOffset(offsetDays: number): string {
    const d = new Date();
    d.setDate(d.getDate() + offsetDays);
    return d.toISOString().split('T')[0];
  }
}
