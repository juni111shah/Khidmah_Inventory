import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { WhatIfRequest, WhatIfResult } from '../../../core/models/decision-support.model';
import { DecisionSupportApiService } from '../../../core/services/decision-support-api.service';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { UnifiedInputComponent } from '../../../shared/components/unified-input/unified-input.component';

@Component({
  selector: 'app-what-if-simulator',
  standalone: true,
  imports: [CommonModule, FormsModule, UnifiedCardComponent, UnifiedButtonComponent, UnifiedInputComponent],
  template: `
    <app-unified-card header="What-If Simulator" customClass="border-0 shadow-sm">
      <div class="mb-3">
        <label class="form-label">Scenario</label>
        <select class="form-select" [(ngModel)]="request.type" (ngModelChange)="onTypeChange()">
          <option value="price">Price change</option>
          <option value="demand">Demand change</option>
          <option value="supplier_delay">Supplier delay</option>
        </select>
      </div>
      <div class="mb-3" *ngIf="request.type === 'price'">
        <label class="form-label">Price change %</label>
        <input type="number" class="form-control" [(ngModel)]="request.priceChangePercent" step="1" placeholder="e.g. 10 or -5" />
      </div>
      <div class="mb-3" *ngIf="request.type === 'demand'">
        <label class="form-label">Demand change %</label>
        <input type="number" class="form-control" [(ngModel)]="request.demandChangePercent" step="1" placeholder="e.g. 20 or -10" />
      </div>
      <div class="mb-3" *ngIf="request.type === 'supplier_delay'">
        <label class="form-label">Delay (days)</label>
        <input type="number" class="form-control" [(ngModel)]="request.delayDays" min="0" />
      </div>
      <button type="button" class="btn btn-primary" (click)="run()" [disabled]="running">Run simulation</button>

      <div *ngIf="result" class="mt-4 p-3 bg-light rounded">
        <h6>{{ result.scenario }}</h6>
        <dl class="row mb-0 small">
          <dt class="col-sm-5">Projected revenue</dt>
          <dd class="col-sm-7">{{ result.projections.revenue | currency }}</dd>
          <dt class="col-sm-5">Margin</dt>
          <dd class="col-sm-7">{{ result.projections.margin }}%</dd>
          <dt class="col-sm-5" *ngIf="result.projections.stockoutDate">Stockout date</dt>
          <dd class="col-sm-7" *ngIf="result.projections.stockoutDate">{{ result.projections.stockoutDate }}</dd>
        </dl>
        <p class="mb-0 mt-2 small">{{ result.summary }}</p>
      </div>
    </app-unified-card>
  `
})
export class WhatIfSimulatorComponent {
  @Input() productId: string | null = null;
  @Input() currentPrice: number = 0;
  @Input() currentStock: number = 0;
  @Input() dailyDemand: number = 0;
  @Output() resultChange = new EventEmitter<WhatIfResult | null>();

  request: WhatIfRequest = { type: 'price', priceChangePercent: 10 };
  result: WhatIfResult | null = null;
  running = false;

  constructor(private api: DecisionSupportApiService) {}

  onTypeChange(): void {
    this.result = null;
    if (this.request.type === 'price') this.request.priceChangePercent = 10;
    if (this.request.type === 'demand') this.request.demandChangePercent = 20;
    if (this.request.type === 'supplier_delay') this.request.delayDays = 7;
  }

  run(): void {
    this.request.productId = this.productId || undefined;
    this.request.currentPrice = this.currentPrice || 100;
    this.request.currentStock = this.currentStock || 50;
    this.request.dailyDemand = this.dailyDemand || 5;
    this.running = true;
    this.api.runWhatIfSimulation(this.request).subscribe({
      next: res => {
        this.running = false;
        if (res.success && res.data) {
          this.result = res.data;
          this.resultChange.emit(res.data);
        }
      },
      error: () => { this.running = false; }
    });
  }
}
