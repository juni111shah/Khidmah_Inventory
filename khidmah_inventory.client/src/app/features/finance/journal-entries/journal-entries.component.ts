import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FinanceApiService } from '../../../core/services/finance-api.service';
import { ApiResponse } from '../../../core/models/api-response.model';
import { GetJournalEntriesResult, JournalEntryDto } from '../../../core/models/finance.model';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';

@Component({
  selector: 'app-journal-entries',
  standalone: true,
  imports: [CommonModule, FormsModule, UnifiedCardComponent, ContentLoaderComponent],
  templateUrl: './journal-entries.component.html'
})
export class JournalEntriesComponent implements OnInit {
  loading = false;
  entries: JournalEntryDto[] = [];
  expandedEntryId: string | null = null;

  filter = {
    dateFrom: this.getDateOffset(-30),
    dateTo: this.getDateOffset(0),
    sourceModule: '',
    pageNo: 1,
    pageSize: 25
  };

  totalCount = 0;

  constructor(private financeApi: FinanceApiService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.financeApi.getJournalEntries(this.filter).subscribe({
      next: (res: ApiResponse<GetJournalEntriesResult>) => {
        this.loading = false;
        if (res.success && res.data) {
          this.entries = res.data.items;
          this.totalCount = res.data.totalCount;
          this.filter.pageNo = res.data.pageNo;
          this.filter.pageSize = res.data.pageSize;
        }
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  onPageChange(delta: number): void {
    const nextPage = this.filter.pageNo + delta;
    if (nextPage < 1) return;
    this.filter.pageNo = nextPage;
    this.load();
  }

  toggleEntry(entryId: string): void {
    this.expandedEntryId = this.expandedEntryId === entryId ? null : entryId;
  }

  exportCsv(): void {
    const rows: string[] = ['Date,Reference,Source,Description,TotalDebit,TotalCredit'];
    this.entries.forEach(entry => {
      rows.push([
        this.escapeCsv(entry.date),
        this.escapeCsv(entry.reference),
        this.escapeCsv(entry.sourceModule),
        this.escapeCsv(entry.description || ''),
        entry.totalDebit.toFixed(2),
        entry.totalCredit.toFixed(2)
      ].join(','));
    });

    this.downloadCsv(rows.join('\n'), `journal-entries-${this.filter.dateFrom}-${this.filter.dateTo}.csv`);
  }

  private downloadCsv(content: string, filename: string): void {
    const blob = new Blob([content], { type: 'text/csv;charset=utf-8;' });
    const url = window.URL.createObjectURL(blob);
    const anchor = document.createElement('a');
    anchor.href = url;
    anchor.download = filename;
    anchor.click();
    window.URL.revokeObjectURL(url);
  }

  private escapeCsv(value: string): string {
    const escaped = value.replace(/"/g, '""');
    return `"${escaped}"`;
  }

  private getDateOffset(offsetDays: number): string {
    const d = new Date();
    d.setDate(d.getDate() + offsetDays);
    return d.toISOString().split('T')[0];
  }
}
