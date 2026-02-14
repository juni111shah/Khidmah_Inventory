import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { ApiResponse, PagedResult } from '../models/api-response.model';
import { GetStockLevelsListQuery, StockLevel } from '../models/inventory.model';
import { InventoryApiService } from '../services/inventory-api.service';
import { RealtimeSyncService } from '../services/realtime-sync.service';

@Injectable({ providedIn: 'root' })
export class InventoryStore {
  private readonly levelsSubject = new BehaviorSubject<StockLevel[]>([]);
  private readonly pageSubject = new BehaviorSubject<PagedResult<StockLevel> | null>(null);
  private readonly touchedAtSubject = new BehaviorSubject<Map<string, string>>(new Map());

  readonly levels$ = this.levelsSubject.asObservable();
  readonly page$ = this.pageSubject.asObservable();
  readonly touchedAt$ = this.touchedAtSubject.asObservable();

  constructor(
    private api: InventoryApiService,
    realtimeSync: RealtimeSyncService
  ) {
    realtimeSync.watchEntityType('StockLevel').subscribe(() => this.markTouched(this.levelsSubject.value.map(x => x.id)));
    realtimeSync.watchEntityType('StockTransaction').subscribe(() => this.markTouched(this.levelsSubject.value.map(x => x.id)));
  }

  fetchLevels(query?: GetStockLevelsListQuery): Observable<ApiResponse<PagedResult<StockLevel>>> {
    return this.api.getStockLevels(query).pipe(
      tap(response => {
        if (!response.success || !response.data) return;
        this.pageSubject.next(response.data);
        this.levelsSubject.next(response.data.items);
        this.markTouched(response.data.items.map(x => x.id));
      })
    );
  }

  private markTouched(ids: string[]): void {
    if (!ids.length) return;
    const next = new Map(this.touchedAtSubject.value);
    const now = new Date().toISOString();
    ids.forEach(id => next.set(id, now));
    this.touchedAtSubject.next(next);
  }
}
