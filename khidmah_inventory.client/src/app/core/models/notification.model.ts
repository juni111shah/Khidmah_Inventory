import { PagedResult } from './api-response.model';

export type NotificationType = 'Info' | 'Success' | 'Warning' | 'Error';

export interface NotificationDto {
  id: string;
  companyId: string;
  userId?: string;
  title: string;
  message: string;
  type: NotificationType;
  entityType?: string;
  entityId?: string;
  isRead: boolean;
  createdAt: string;
}

export interface GetNotificationsRequest {
  filterRequest?: {
    pagination?: { pageNo: number; pageSize: number; sortBy?: string; sortOrder?: string };
    filters?: unknown[];
    search?: unknown;
  };
  unreadOnly?: boolean;
}

export type GetNotificationsResponse = PagedResult<NotificationDto>;
