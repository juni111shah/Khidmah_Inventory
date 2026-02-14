import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { NotificationApiService } from '../../../core/services/notification-api.service';
import { NotificationDto, GetNotificationsRequest } from '../../../core/models/notification.model';
import { getRouteForNotification } from '../../../core/utils/notification-routes.util';
import { ToastNotificationService } from '../../../core/services/toast-notification.service';
import { SignalRService } from '../../../core/services/signalr.service';

@Component({
  selector: 'app-notifications-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './notifications-list.component.html'
})
export class NotificationsListComponent implements OnInit, OnDestroy {
  notifications: NotificationDto[] = [];
  totalCount = 0;
  pageNo = 1;
  pageSize = 20;
  loading = false;
  unreadOnly = false;
  private subs = new Subscription();

  constructor(
    private notificationApi: NotificationApiService,
    private router: Router,
    private toast: ToastNotificationService,
    private signalR: SignalRService
  ) {}

  ngOnInit(): void {
    this.load();
    this.subs.add(
      this.signalR.getNotificationRaised().subscribe(() => this.load())
    );
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  load(): void {
    this.loading = true;
    const request: GetNotificationsRequest = {
      filterRequest: {
        pagination: { pageNo: this.pageNo, pageSize: this.pageSize, sortBy: 'CreatedAt', sortOrder: 'descending' }
      },
      unreadOnly: this.unreadOnly || undefined
    };
    this.notificationApi.getList(request).subscribe({
      next: (res) => {
        this.loading = false;
        if (res.success && res.data) {
          this.notifications = res.data.items;
          this.totalCount = res.data.totalCount;
        } else {
          this.notifications = [];
          this.totalCount = 0;
        }
      },
      error: () => {
        this.loading = false;
        this.notifications = [];
        this.totalCount = 0;
      }
    });
  }

  onPageChange(page: number): void {
    this.pageNo = page;
    this.load();
  }

  markRead(n: NotificationDto): void {
    if (n.isRead) return;
    this.notificationApi.markRead(n.id).subscribe({
      next: (res) => {
        if (res.success) {
          n.isRead = true;
          this.toast.show('Marked as read', 'success');
        }
      }
    });
  }

  markAllRead(): void {
    this.notificationApi.markAllRead().subscribe({
      next: (res) => {
        if (res.success) {
          this.toast.show('All marked as read', 'success');
          this.load();
        }
      },
      error: () => this.toast.show('Failed to mark all as read', 'error')
    });
  }

  getRoute(n: NotificationDto): string | null {
    return getRouteForNotification(n.entityType, n.entityId);
  }

  go(n: NotificationDto): void {
    const route = this.getRoute(n);
    if (route) this.router.navigateByUrl(route);
    this.markRead(n);
  }

  getIcon(type: string): string {
    switch (type?.toLowerCase()) {
      case 'success': return 'bi-check-circle text-success';
      case 'warning': return 'bi-exclamation-triangle text-warning';
      case 'error': return 'bi-x-circle text-danger';
      default: return 'bi-info-circle text-info';
    }
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.totalCount / this.pageSize));
  }
}
