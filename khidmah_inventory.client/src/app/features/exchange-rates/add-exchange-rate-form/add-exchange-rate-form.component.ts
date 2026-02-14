import { Component, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CurrencyApiService } from '../../../core/services/currency-api.service';
import { CurrencyDto } from '../../../core/models/currency.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';

@Component({
  selector: 'app-add-exchange-rate-form',
  standalone: true,
  imports: [CommonModule, FormsModule, UnifiedButtonComponent, ContentLoaderComponent],
  templateUrl: './add-exchange-rate-form.component.html'
})
export class AddExchangeRateFormComponent implements OnInit {
  @Output() saved = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();

  currencies: CurrencyDto[] = [];
  loadingCurrencies = false;
  fromCurrencyId = '';
  toCurrencyId = '';
  rate: number | null = null;
  date: string = new Date().toISOString().slice(0, 10);
  saving = false;
  errorMessage = '';

  constructor(private currencyApi: CurrencyApiService) {}

  ngOnInit(): void {
    this.loadCurrencies();
  }

  loadCurrencies(): void {
    this.loadingCurrencies = true;
    this.currencyApi.getCurrencies(true).subscribe({
      next: (res: ApiResponse<{ items: CurrencyDto[] }>) => {
        this.loadingCurrencies = false;
        if (res.success && res.data?.items) this.currencies = res.data.items;
      },
      error: () => { this.loadingCurrencies = false; }
    });
  }

  submit(): void {
    this.errorMessage = '';
    if (!this.fromCurrencyId || !this.toCurrencyId) {
      this.errorMessage = 'Select from and to currency.';
      return;
    }
    if (this.fromCurrencyId === this.toCurrencyId) {
      this.errorMessage = 'From and to currency must be different.';
      return;
    }
    const r = this.rate != null && this.rate > 0 ? this.rate : null;
    if (r === null) {
      this.errorMessage = 'Enter a valid rate (e.g. 1 from = X to).';
      return;
    }
    if (!this.date) {
      this.errorMessage = 'Select date.';
      return;
    }
    this.saving = true;
    this.currencyApi.createExchangeRate({
      fromCurrencyId: this.fromCurrencyId,
      toCurrencyId: this.toCurrencyId,
      rate: r,
      date: this.date
    }).subscribe({
      next: (res: ApiResponse<unknown>) => {
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
