/**
 * Envelope for real-time operations events from SignalR OperationsHub.
 * Matches backend API.Models.OperationsEventPayload.
 */
export interface OperationsEventPayload {
  companyId: string;
  entityId?: string;
  entityType?: string;
  payload?: Record<string, unknown>;
  timestampUtc: string;
}

export const OperationsEventNames = {
  EntityChanged: 'EntityChanged',
  EntityDeleted: 'EntityDeleted',
  ProductCreated: 'ProductCreated',
  ProductDeleted: 'ProductDeleted',
  OrderUpdated: 'OrderUpdated',
  OrderStatusChanged: 'OrderStatusChanged',
  CustomerUpdated: 'CustomerUpdated',
  SupplierUpdated: 'SupplierUpdated',
  FinancePosted: 'FinancePosted',
  StockChanged: 'StockChanged',
  ProductUpdated: 'ProductUpdated',
  OrderCreated: 'OrderCreated',
  OrderApproved: 'OrderApproved',
  PurchaseCreated: 'PurchaseCreated',
  SaleCompleted: 'SaleCompleted',
  LowStockDetected: 'LowStockDetected',
  BatchExpiring: 'BatchExpiring',
  CommentAdded: 'CommentAdded',
  ActivityCreated: 'ActivityCreated',
  NotificationRaised: 'NotificationRaised',
  DashboardUpdated: 'DashboardUpdated',
  HandsFreeTaskPushed: 'HandsFreeTaskPushed'
} as const;

export type OperationsEventName = keyof typeof OperationsEventNames;
