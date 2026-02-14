import { Component, OnInit, OnDestroy, HostListener, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { NotificationApiService } from '../../../core/services/notification-api.service';
import { SignalRService } from '../../../core/services/signalr.service';
import { ToastNotificationService } from '../../../core/services/toast-notification.service';
import { NotificationDto } from '../../../core/models/notification.model';
import { getRouteForNotification } from '../../../core/utils/notification-routes.util';
import { OperationsEventPayload } from '../../../core/models/operations-event.model';

@Component({
  selector: 'app-notification-center',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification-center.component.html',
  styles: [`
    .notification-btn {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      width: 2.25rem;
      height: 2.25rem;
      padding: 0;
      border: none;
      border-radius: 0.5rem;
      background: transparent;
      color: var(--text-color);
      cursor: pointer;
      transition: background-color 0.2s ease, color 0.2s ease;
    }
    .notification-btn:hover {
      background-color: var(--background-color);
      color: var(--primary-color);
    }
    .notification-btn:focus-visible {
      outline: 2px solid var(--primary-color);
      outline-offset: 2px;
    }
    .notification-badge {
      position: absolute;
      top: 2px;
      right: 2px;
      min-width: 1rem;
      height: 1rem;
      padding: 0 0.25rem;
      font-size: 0.625rem;
      font-weight: 700;
      line-height: 1rem;
      border-radius: 50px;
      background: var(--error-color, #ef4444);
      color: #fff;
      display: inline-flex;
      align-items: center;
      justify-content: center;
    }
    .notification-item.unread { background-color: var(--bs-light, #f8f9fa); }
  `]
})
export class NotificationCenterComponent implements OnInit, OnDestroy {
  open = false;
  loading = false;
  unreadCount = 0;
  notifications: NotificationDto[] = [];
  private subs = new Subscription();

  constructor(
    private notificationApi: NotificationApiService,
    private signalR: SignalRService,
    private toast: ToastNotificationService,
    private router: Router,
    private elementRef: ElementRef<HTMLElement>
  ) {}

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.open) return;
    const el = this.elementRef.nativeElement;
    if (el && !el.contains(event.target as Node)) {
      this.close();
    }
  }

  ngOnInit(): void {
    this.loadUnreadCount();
    this.loadPreview();
    this.subs.add(
      this.signalR.getNotificationRaised().subscribe((payload: OperationsEventPayload) => {
        this.loadUnreadCount();
        this.loadPreview();
        const p = payload?.payload as Record<string, unknown> | undefined;
        const title = (p?.['title'] as string) ?? payload?.entityType ?? 'Notification';
        const message = (p?.['message'] as string) ?? 'You have a new notification.';
        const type = this.toastTypeFrom((p?.['type'] as string) ?? 'Info');
        this.toast.show(`${title}: ${message}`, type);
      })
    );
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  private toastTypeFrom(t: string): 'success' | 'error' | 'warning' | 'info' {
    switch (t?.toLowerCase()) {
      case 'success': return 'success';
      case 'error': return 'error';
      case 'warning': return 'warning';
      default: return 'info';
    }
  }

  loadUnreadCount(): void {
    this.notificationApi.getUnreadCount().subscribe({
      next: (res) => {
        if (res.success && res.data !== undefined) this.unreadCount = res.data;
      }
    });
  }

  loadPreview(): void {
    this.loading = true;
    this.notificationApi.getList({
      filterRequest: { pagination: { pageNo: 1, pageSize: 10 } },
      unreadOnly: false
    }).subscribe({
      next: (res) => {
        this.loading = false;
        if (res.success && res.data?.items) {
          this.notifications = res.data.items;
          if (this.unreadCount === 0 && res.data.totalCount > 0) {
            this.unreadCount = res.data.items.filter(n => !n.isRead).length;
          }
        } else {
          this.notifications = [];
        }
      },
      error: () => { this.loading = false; this.notifications = []; }
    });
  }

  toggle(): void {
    this.open = !this.open;
    if (this.open) {
      this.loadPreview();
      this.loadUnreadCount();
    }
  }

  close(): void {
    this.open = false;
  }

  goToNotificationsPage(): void {
    this.router.navigate(['/notifications']);
    this.close();
  }

  getRoute(n: NotificationDto): string | null {
    return getRouteForNotification(n.entityType, n.entityId);
  }

  go(n: NotificationDto): void {
    const route = this.getRoute(n);
    if (route) this.router.navigateByUrl(route);
    if (!n.isRead) {
      this.notificationApi.markRead(n.id).subscribe(() => {
        this.loadUnreadCount();
        this.loadPreview();
      });
    }
    this.close();
  }

  getIcon(type: string): string {
    switch (type?.toLowerCase()) {
      case 'success': return 'bi-check-circle text-success';
      case 'warning': return 'bi-exclamation-triangle text-warning';
      case 'error': return 'bi-x-circle text-danger';
      default: return 'bi-info-circle text-info';
    }
  }

  get badgeCount(): number {
    return this.unreadCount;
  }
}
