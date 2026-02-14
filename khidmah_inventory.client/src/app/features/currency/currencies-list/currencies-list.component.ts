import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CurrencyApiService } from '../../../core/services/currency-api.service';
import { CurrencyDto } from '../../../core/models/currency.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { HeaderService } from '../../../core/services/header.service';
import { DrawerComponent } from '../../../shared/components/drawer/drawer.component';
import { CurrencyFormComponent } from '../currency-form/currency-form.component';

@Component({
  selector: 'app-currencies-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    UnifiedCardComponent,
    UnifiedButtonComponent,
    ContentLoaderComponent,
    IconComponent,
    DrawerComponent,
    CurrencyFormComponent
  ],
  templateUrl: './currencies-list.component.html'
})
export class CurrenciesListComponent implements OnInit {
  loading = false;
  currencies: CurrencyDto[] = [];
  drawerOpen = false;
  editingId: string | null = null;

  constructor(
    private currencyApi: CurrencyApiService,
    private header: HeaderService
  ) {}

  ngOnInit(): void {
    this.header.setHeaderInfo({ title: 'Currencies', description: 'Manage company currencies and base currency' });
    this.load();
  }

  load(): void {
    this.loading = true;
    this.currencyApi.getCurrencies(true).subscribe({
      next: (res: ApiResponse<{ items: CurrencyDto[] }>) => {
        this.loading = false;
        if (res.success && res.data?.items) this.currencies = res.data.items;
      },
      error: () => { this.loading = false; }
    });
  }

  openAdd(): void {
    this.editingId = null;
    this.drawerOpen = true;
  }

  openEdit(id: string): void {
    this.editingId = id;
    this.drawerOpen = true;
  }

  onDrawerClose(): void {
    this.drawerOpen = false;
    this.editingId = null;
    this.load();
  }

  onSaved(): void {
    this.onDrawerClose();
  }
}
