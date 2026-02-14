import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';

export type ToastType = 'success' | 'error' | 'warning' | 'info';

export interface ToastMessage {
  message: string;
  type: ToastType;
}

/**
 * Global toast notifications (e.g. for real-time events).
 * Subscribe in app component to show a single global toast.
 */
@Injectable({ providedIn: 'root' })
export class ToastNotificationService {
  private toast$ = new Subject<ToastMessage>();

  show(message: string, type: ToastType = 'info'): void {
    this.toast$.next({ message, type });
  }

  getToast(): Observable<ToastMessage> {
    return this.toast$.asObservable();
  }
}
