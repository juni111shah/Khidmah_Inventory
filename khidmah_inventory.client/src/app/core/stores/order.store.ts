import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { ApiResponse, PagedResult } from '../models/api-response.model';
import { GetSalesOrdersListQuery, SalesOrder } from '../models/sales-order.model';
import { GetPurchaseOrdersListQuery, PurchaseOrder } from '../models/purchase-order.model';
import { SalesOrderApiService } from '../services/sales-order-api.service';
import { PurchaseOrderApiService } from '../services/purchase-order-api.service';
import { RealtimeSyncService } from '../services/realtime-sync.service';

@Injectable({ providedIn: 'root' })
export class OrderStore {
  private readonly salesItemsSubject = new BehaviorSubject<SalesOrder[]>([]);
  private readonly purchaseItemsSubject = new BehaviorSubject<PurchaseOrder[]>([]);
  private readonly salesPageSubject = new BehaviorSubject<PagedResult<SalesOrder> | null>(null);
  private readonly purchasePageSubject = new BehaviorSubject<PagedResult<PurchaseOrder> | null>(null);
  private readonly touchedAtSubject = new BehaviorSubject<Map<string, string>>(new Map());

  readonly salesItems$ = this.salesItemsSubject.asObservable();
  readonly purchaseItems$ = this.purchaseItemsSubject.asObservable();
  readonly salesPage$ = this.salesPageSubject.asObservable();
  readonly purchasePage$ = this.purchasePageSubject.asObservable();
  readonly touchedAt$ = this.touchedAtSubject.asObservable();

  constructor(
    private salesApi: SalesOrderApiService,
    private purchaseApi: PurchaseOrderApiService,
    realtimeSync: RealtimeSyncService
  ) {
    realtimeSync.watchEntityType('SalesOrder').subscribe(changes => {
      this.markTouched(changes.map(c => c.entityId).filter((x): x is string => !!x));
    });
    realtimeSync.watchEntityType('PurchaseOrder').subscribe(changes => {
      this.markTouched(changes.map(c => c.entityId).filter((x): x is string => !!x));
    });
  }

  fetchSales(query?: GetSalesOrdersListQuery): Observable<ApiResponse<PagedResult<SalesOrder>>> {
    return this.salesApi.getSalesOrders(query).pipe(
      tap(response => {
        if (!response.success || !response.data) return;
        this.salesPageSubject.next(response.data);
        this.salesItemsSubject.next(response.data.items);
        this.markTouched(response.data.items.map(x => x.id));
      })
    );
  }

  fetchPurchases(query?: GetPurchaseOrdersListQuery): Observable<ApiResponse<PagedResult<PurchaseOrder>>> {
    return this.purchaseApi.getPurchaseOrders(query).pipe(
      tap(response => {
        if (!response.success || !response.data) return;
        this.purchasePageSubject.next(response.data);
        this.purchaseItemsSubject.next(response.data.items);
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
