import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CurrencyApiService } from '../../../core/services/currency-api.service';
import { CurrencyDto } from '../../../core/models/currency.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';

@Component({
  selector: 'app-currency-form',
  standalone: true,
  imports: [CommonModule, FormsModule, UnifiedButtonComponent, ContentLoaderComponent],
  templateUrl: './currency-form.component.html'
})
export class CurrencyFormComponent implements OnInit {
  @Input() currencyId: string | null = null;
  @Output() saved = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();

  code = '';
  name = '';
  symbol = '';
  isBase = false;
  loading = false;
  saving = false;
  errorMessage = '';

  constructor(private currencyApi: CurrencyApiService) {}

  ngOnInit(): void {
    if (this.currencyId) {
      this.load();
    }
  }

  load(): void {
    if (!this.currencyId) return;
    this.loading = true;
    this.currencyApi.getCurrency(this.currencyId).subscribe({
      next: (res: ApiResponse<CurrencyDto>) => {
        this.loading = false;
        if (res.success && res.data) {
          this.code = res.data.code;
          this.name = res.data.name;
          this.symbol = res.data.symbol;
          this.isBase = res.data.isBase;
        }
      },
      error: () => { this.loading = false; }
    });
  }

  submit(): void {
    this.errorMessage = '';
    const code = this.code?.trim();
    const name = this.name?.trim();
    const symbol = this.symbol?.trim();
    if (!code || !name || !symbol) {
      this.errorMessage = 'Code, name and symbol are required.';
      return;
    }
    this.saving = true;
    const body = { code, name, symbol, isBase: this.isBase };
    const req = this.currencyId
      ? this.currencyApi.updateCurrency(this.currencyId, body)
      : this.currencyApi.createCurrency(body);
    req.subscribe({
      next: (res: ApiResponse<CurrencyDto>) => {
        this.saving = false;
        if (res.success) this.saved.emit();
        else this.errorMessage = (res.errors && res.errors.length) ? res.errors.join(' ') : 'Save failed.';
      },
      error: (err) => {
        this.saving = false;
        this.errorMessage = err?.error?.message || err?.error?.errors?.join(' ') || 'Save failed.';
      }
    });
  }
}
