import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CurrencyApiService } from '../../../core/services/currency-api.service';
import { ExchangeRateDto, CurrencyDto } from '../../../core/models/currency.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { HeaderService } from '../../../core/services/header.service';
import { DrawerComponent } from '../../../shared/components/drawer/drawer.component';
import { AddExchangeRateFormComponent } from '../add-exchange-rate-form/add-exchange-rate-form.component';

@Component({
  selector: 'app-exchange-rates-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    UnifiedCardComponent,
    UnifiedButtonComponent,
    ContentLoaderComponent,
    DrawerComponent,
    AddExchangeRateFormComponent
  ],
  templateUrl: './exchange-rates-list.component.html'
})
export class ExchangeRatesListComponent implements OnInit {
  loading = false;
  rates: ExchangeRateDto[] = [];
  drawerOpen = false;
  fromDate: string | null = null;
  toDate: string | null = null;

  constructor(
    private currencyApi: CurrencyApiService,
    private header: HeaderService
  ) {}

  ngOnInit(): void {
    this.header.setHeaderInfo({ title: 'Exchange rates', description: 'Manage FX rates for multi-currency reporting' });
    this.load();
  }

  load(): void {
    this.loading = true;
    const params: { fromDate?: string; toDate?: string } = {};
    if (this.fromDate) params.fromDate = this.fromDate;
    if (this.toDate) params.toDate = this.toDate;
    this.currencyApi.getExchangeRates(params).subscribe({
      next: (res: ApiResponse<{ items: ExchangeRateDto[] }>) => {
        this.loading = false;
        if (res.success && res.data?.items) this.rates = res.data.items;
      },
      error: () => { this.loading = false; }
    });
  }

  openAdd(): void {
    this.drawerOpen = true;
  }

  onDrawerClose(): void {
    this.drawerOpen = false;
    this.load();
  }

  onSaved(): void {
    this.onDrawerClose();
  }
}
