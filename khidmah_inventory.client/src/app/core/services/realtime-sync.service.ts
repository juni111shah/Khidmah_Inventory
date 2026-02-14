import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { bufferTime, filter, map, shareReplay } from 'rxjs/operators';
import { SignalRService, OperationsEvent } from './signalr.service';
import { OperationsEventPayload } from '../models/operations-event.model';

export interface RealtimeEntityChange {
  eventName: string;
  entityType: string;
  entityId?: string;
  timestampUtc: string;
  payload?: Record<string, unknown>;
}

@Injectable({ providedIn: 'root' })
export class RealtimeSyncService {
  private readonly rawChanges$ = new Subject<RealtimeEntityChange>();
  private readonly batchedChanges$: Observable<RealtimeEntityChange[]>;

  constructor(private signalR: SignalRService) {
    this.signalR.getAnyOperationEvent().subscribe(evt => this.handleOperationEvent(evt));

    this.batchedChanges$ = this.rawChanges$.pipe(
      bufferTime(250),
      filter(batch => batch.length > 0),
      map(batch => this.dedupe(batch)),
      shareReplay({ bufferSize: 1, refCount: true })
    );
  }

  getChanges(): Observable<RealtimeEntityChange[]> {
    return this.batchedChanges$;
  }

  watchEntityType(entityType: string): Observable<RealtimeEntityChange[]> {
    const normalized = entityType.toLowerCase();
    return this.batchedChanges$.pipe(
      map(batch => batch.filter(x => x.entityType.toLowerCase() === normalized)),
      filter(batch => batch.length > 0)
    );
  }

  watchEntity(entityType: string, entityId: string): Observable<RealtimeEntityChange[]> {
    const normalizedType = entityType.toLowerCase();
    const normalizedId = entityId.toLowerCase();
    return this.batchedChanges$.pipe(
      map(batch => batch.filter(x =>
        x.entityType.toLowerCase() === normalizedType &&
        (x.entityId ?? '').toLowerCase() === normalizedId)),
      filter(batch => batch.length > 0)
    );
  }

  private handleOperationEvent(event: OperationsEvent): void {
    const payload = event.payload;
    this.rawChanges$.next({
      eventName: event.eventName,
      entityType: this.resolveEntityType(payload),
      entityId: payload.entityId,
      timestampUtc: payload.timestampUtc ?? new Date().toISOString(),
      payload: payload.payload
    });
  }

  private resolveEntityType(payload: OperationsEventPayload): string {
    if (payload.entityType && payload.entityType.trim().length > 0) {
      return payload.entityType;
    }
    return 'Unknown';
  }

  private dedupe(batch: RealtimeEntityChange[]): RealtimeEntityChange[] {
    const mapRef = new Map<string, RealtimeEntityChange>();
    for (const change of batch) {
      const key = `${change.eventName}|${change.entityType}|${change.entityId ?? ''}`;
      mapRef.set(key, change);
    }
    return Array.from(mapRef.values());
  }
}
