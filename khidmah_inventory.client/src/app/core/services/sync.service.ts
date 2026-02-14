import { Injectable } from '@angular/core';
import { OfflineService } from './offline.service';
import { HttpClient } from '@angular/common/http';
import { Observable, from, of } from 'rxjs';
import { switchMap, catchError, tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class SyncService {
  private isSyncing = false;

  constructor(
    private offlineService: OfflineService,
    private http: HttpClient
  ) {
    // Auto-sync when coming back online
    this.offlineService.getOnlineStatus().subscribe(isOnline => {
      if (isOnline && !this.isSyncing) {
        this.syncPendingChanges();
      }
    });
  }

  async syncPendingChanges(): Promise<void> {
    if (this.isSyncing || !this.offlineService.isOnline()) {
      return;
    }

    this.isSyncing = true;

    try {
      const queue = await this.offlineService.getSyncQueue();

      for (const item of queue) {
        try {
          await this.processSyncItem(item);
          await this.offlineService.removeFromSyncQueue(item.id);
        } catch (error) {
          console.error('Error syncing item:', error);
          // Keep item in queue for retry
        }
      }
    } finally {
      this.isSyncing = false;
    }
  }

  private async processSyncItem(item: any): Promise<void> {
    const { type, action, data } = item;

    switch (type) {
      case 'stockTransaction':
        if (action === 'create') {
          await this.http.post('/api/inventory/transactions', data).toPromise();
        }
        break;

      case 'salesOrder':
        if (action === 'create') {
          await this.http.post('/api/sales-orders', data).toPromise();
        }
        break;

      case 'purchaseOrder':
        if (action === 'create') {
          await this.http.post('/api/purchase-orders', data).toPromise();
        }
        break;

      case 'handsFreeComplete':
        if (action === 'complete') {
          await this.http.post('/api/warehouse/handsfree/complete', data).toPromise();
        }
        break;

      default:
        console.warn('Unknown sync type:', type);
    }
  }

  async queueForSync(type: string, action: string, data: any): Promise<void> {
    if (this.offlineService.isOnline()) {
      // Try to sync immediately
      try {
        await this.processSyncItem({ type, action, data });
      } catch (error) {
        // If fails, add to queue
        await this.offlineService.addToSyncQueue(type, action, data);
      }
    } else {
      // Add to queue for later sync
      await this.offlineService.addToSyncQueue(type, action, data);
    }
  }

  getSyncStatus(): Observable<{ pending: number; syncing: boolean }> {
    return from(this.offlineService.getSyncQueue()).pipe(
      switchMap(queue => {
        return of({
          pending: queue.length,
          syncing: this.isSyncing
        });
      }),
      catchError(() => of({ pending: 0, syncing: false }))
    );
  }
}

