import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { ApiResponse, PagedResult } from '../models/api-response.model';
import { GetProductsListQuery, Product } from '../models/product.model';
import { ProductApiService } from '../services/product-api.service';
import { RealtimeSyncService } from '../services/realtime-sync.service';

@Injectable({ providedIn: 'root' })
export class ProductStore {
  private readonly itemsSubject = new BehaviorSubject<Product[]>([]);
  private readonly pageSubject = new BehaviorSubject<PagedResult<Product> | null>(null);
  private readonly touchedAtSubject = new BehaviorSubject<Map<string, string>>(new Map());

  readonly items$ = this.itemsSubject.asObservable();
  readonly page$ = this.pageSubject.asObservable();
  readonly touchedAt$ = this.touchedAtSubject.asObservable();

  constructor(
    private api: ProductApiService,
    realtimeSync: RealtimeSyncService
  ) {
    realtimeSync.watchEntityType('Product').subscribe(changes => {
      this.markTouched(changes.map(c => c.entityId).filter((x): x is string => !!x));
    });
  }

  fetch(query?: GetProductsListQuery): Observable<ApiResponse<PagedResult<Product>>> {
    return this.api.getProducts(query).pipe(
      tap(response => {
        if (!response.success || !response.data) return;
        this.pageSubject.next(response.data);
        this.itemsSubject.next(response.data.items);
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
