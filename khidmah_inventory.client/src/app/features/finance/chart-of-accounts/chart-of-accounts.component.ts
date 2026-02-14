import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FinanceApiService } from '../../../core/services/finance-api.service';
import { AccountDto } from '../../../core/models/finance.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { HeaderService } from '../../../core/services/header.service';

@Component({
  selector: 'app-chart-of-accounts',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    UnifiedCardComponent,
    UnifiedButtonComponent,
    ContentLoaderComponent,
    IconComponent
  ],
  templateUrl: './chart-of-accounts.component.html'
})
export class ChartOfAccountsComponent implements OnInit {
  loading = false;
  accounts: AccountDto[] = [];
  includeInactive = false;

  constructor(
    private financeApi: FinanceApiService,
    private header: HeaderService
  ) {}

  ngOnInit(): void {
    this.header.setHeaderInfo({ title: 'Chart of accounts', description: 'Manage your accounts and import a standard template' });
    this.load();
  }

  load(): void {
    this.loading = true;
    this.financeApi.getAccountsTree(this.includeInactive).subscribe({
      next: (res: ApiResponse<AccountDto[]>) => {
        this.loading = false;
        if (res.success && res.data) this.accounts = res.data;
      },
      error: () => { this.loading = false; }
    });
  }

  importStandard(): void {
    this.loading = true;
    this.financeApi.importStandardChart().subscribe({
      next: (res) => {
        this.loading = false;
        if (res.success && res.data) {
          this.accounts = res.data.accounts;
        }
      },
      error: () => { this.loading = false; }
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', minimumFractionDigits: 0 }).format(value);
  }
}
