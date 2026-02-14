import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { AuthService } from './auth.service';
import { ApiConfigService } from './api-config.service';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { OperationsEventPayload, OperationsEventNames } from '../models/operations-event.model';

export type OperationsEvent = { eventName: string; payload: OperationsEventPayload };

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private analyticsHub: HubConnection | null = null;
  private operationsHub: HubConnection | null = null;
  private connectionState$ = new BehaviorSubject<HubConnectionState>(HubConnectionState.Disconnected);
  private analyticsBaseUrl: string;
  private operationsBaseUrl: string;

  /** Generic stream of all operations events (eventName + payload). */
  private anyOperationEvent$ = new Subject<OperationsEvent>();

  /** Typed streams per event (convenience). */
  private stockChanged$ = new Subject<OperationsEventPayload>();
  private productUpdated$ = new Subject<OperationsEventPayload>();
  private orderCreated$ = new Subject<OperationsEventPayload>();
  private orderApproved$ = new Subject<OperationsEventPayload>();
  private purchaseCreated$ = new Subject<OperationsEventPayload>();
  private saleCompleted$ = new Subject<OperationsEventPayload>();
  private lowStockDetected$ = new Subject<OperationsEventPayload>();
  private batchExpiring$ = new Subject<OperationsEventPayload>();
  private commentAdded$ = new Subject<OperationsEventPayload>();
  private activityCreated$ = new Subject<OperationsEventPayload>();
  private notificationRaised$ = new Subject<OperationsEventPayload>();
  private dashboardUpdated$ = new Subject<OperationsEventPayload>();
  private handsFreeTaskPushed$ = new Subject<OperationsEventPayload>();

  constructor(
    private authService: AuthService,
    private apiConfig: ApiConfigService
  ) {
    this.analyticsBaseUrl = `${this.apiConfig.getBaseUrl()}/hubs/analytics`;
    this.operationsBaseUrl = `${this.apiConfig.getBaseUrl()}/hubs/operations`;
  }

  /**
   * Start both Analytics and Operations hub connections.
   * Call after login; reconnects automatically via SignalR client.
   */
  async startConnection(): Promise<void> {
    const token = this.authService.getToken();
    if (!token) {
      console.warn('No token available for SignalR connection');
      return;
    }

    const tokenFactory = () => token;
    const reconnectPolicy = {
      nextRetryDelayInMilliseconds: (retryContext: { previousRetryCount: number }) =>
        Math.min(1000 * Math.pow(2, retryContext.previousRetryCount), 30000)
    };

    // ---- Analytics hub (existing) ----
    if (!this.analyticsHub || this.analyticsHub.state === HubConnectionState.Disconnected) {
      this.analyticsHub = new HubConnectionBuilder()
        .withUrl(this.analyticsBaseUrl, { accessTokenFactory: tokenFactory, skipNegotiation: false, transport: 1 })
        .withAutomaticReconnect(reconnectPolicy)
        .build();
      this.analyticsHub.onreconnecting(() => this.connectionState$.next(HubConnectionState.Reconnecting));
      this.analyticsHub.onreconnected(() => this.connectionState$.next(HubConnectionState.Connected));
      this.analyticsHub.onclose(() => this.connectionState$.next(HubConnectionState.Disconnected));
      try {
        const timeout = new Promise<never>((_, reject) => setTimeout(() => reject(new Error('SignalR timeout')), 10000));
        await Promise.race([this.analyticsHub.start(), timeout]);
      } catch (err) {
        console.warn('Analytics hub connection failed:', err);
      }
    }

    // ---- Operations hub (real-time events) ----
    if (!this.operationsHub || this.operationsHub.state === HubConnectionState.Disconnected) {
      this.operationsHub = new HubConnectionBuilder()
        .withUrl(this.operationsBaseUrl, { accessTokenFactory: tokenFactory, skipNegotiation: false, transport: 1 })
        .withAutomaticReconnect(reconnectPolicy)
        .build();
      this.operationsHub.onreconnecting(() => this.connectionState$.next(HubConnectionState.Reconnecting));
      this.operationsHub.onreconnected(() => this.connectionState$.next(HubConnectionState.Connected));
      this.operationsHub.onclose(() => this.connectionState$.next(HubConnectionState.Disconnected));

      const eventNames: (keyof typeof OperationsEventNames)[] = [
        'EntityChanged', 'EntityDeleted', 'ProductCreated', 'ProductDeleted', 'OrderUpdated', 'OrderStatusChanged',
        'CustomerUpdated', 'SupplierUpdated', 'FinancePosted',
        'StockChanged', 'ProductUpdated', 'OrderCreated', 'OrderApproved', 'PurchaseCreated',
        'SaleCompleted', 'LowStockDetected', 'BatchExpiring', 'CommentAdded', 'ActivityCreated',
        'NotificationRaised', 'DashboardUpdated', 'HandsFreeTaskPushed'
      ];
      eventNames.forEach(name => {
        this.operationsHub!.on(OperationsEventNames[name], (payload: OperationsEventPayload) => {
          this.anyOperationEvent$.next({ eventName: OperationsEventNames[name], payload });
          switch (name) {
            case 'StockChanged': this.stockChanged$.next(payload); break;
            case 'ProductUpdated': this.productUpdated$.next(payload); break;
            case 'OrderCreated': this.orderCreated$.next(payload); break;
            case 'OrderApproved': this.orderApproved$.next(payload); break;
            case 'PurchaseCreated': this.purchaseCreated$.next(payload); break;
            case 'SaleCompleted': this.saleCompleted$.next(payload); break;
            case 'LowStockDetected': this.lowStockDetected$.next(payload); break;
            case 'BatchExpiring': this.batchExpiring$.next(payload); break;
            case 'CommentAdded': this.commentAdded$.next(payload); break;
            case 'ActivityCreated': this.activityCreated$.next(payload); break;
            case 'NotificationRaised': this.notificationRaised$.next(payload); break;
            case 'DashboardUpdated': this.dashboardUpdated$.next(payload); break;
            case 'HandsFreeTaskPushed': this.handsFreeTaskPushed$.next(payload); break;
            default: break;
          }
        });
      });

      try {
        const timeout = new Promise<never>((_, reject) => setTimeout(() => reject(new Error('SignalR timeout')), 10000));
        await Promise.race([this.operationsHub.start(), timeout]);
        this.connectionState$.next(HubConnectionState.Connected);
      } catch (err) {
        console.warn('Operations hub connection failed:', err);
        this.connectionState$.next(HubConnectionState.Disconnected);
      }
    }
  }

  async stopConnection(): Promise<void> {
    if (this.analyticsHub) {
      await this.analyticsHub.stop().catch(() => {});
      this.analyticsHub = null;
    }
    if (this.operationsHub) {
      await this.operationsHub.stop().catch(() => {});
      this.operationsHub = null;
    }
    this.connectionState$.next(HubConnectionState.Disconnected);
  }

  // ---------- Analytics hub (existing) ----------
  async subscribeToAnalytics(analyticsType: string): Promise<void> {
    if (this.analyticsHub?.state === HubConnectionState.Connected) {
      await this.analyticsHub.invoke('SubscribeToAnalytics', analyticsType);
    }
  }

  async unsubscribeFromAnalytics(analyticsType: string): Promise<void> {
    if (this.analyticsHub?.state === HubConnectionState.Connected) {
      await this.analyticsHub.invoke('UnsubscribeFromAnalytics', analyticsType);
    }
  }

  onDashboardUpdate(callback: (data: unknown) => void): void {
    this.analyticsHub?.on('DashboardUpdated', callback);
  }

  offDashboardUpdate(callback: (data: unknown) => void): void {
    this.analyticsHub?.off('DashboardUpdated', callback);
  }

  onAnalyticsUpdate(callback: (analyticsType: string, data: unknown) => void): void {
    this.analyticsHub?.on('AnalyticsUpdated', callback);
  }

  offAnalyticsUpdate(callback: (analyticsType: string, data: unknown) => void): void {
    this.analyticsHub?.off('AnalyticsUpdated', callback);
  }

  // ---------- Operations hub observables ----------
  getConnectionState(): Observable<HubConnectionState> {
    return this.connectionState$.asObservable();
  }

  isConnected(): boolean {
    return this.operationsHub?.state === HubConnectionState.Connected ||
           this.analyticsHub?.state === HubConnectionState.Connected;
  }

  /** All operations events (eventName + payload). */
  getAnyOperationEvent(): Observable<OperationsEvent> {
    return this.anyOperationEvent$.asObservable();
  }

  getStockChanged(): Observable<OperationsEventPayload> { return this.stockChanged$.asObservable(); }
  getProductUpdated(): Observable<OperationsEventPayload> { return this.productUpdated$.asObservable(); }
  getOrderCreated(): Observable<OperationsEventPayload> { return this.orderCreated$.asObservable(); }
  getOrderApproved(): Observable<OperationsEventPayload> { return this.orderApproved$.asObservable(); }
  getPurchaseCreated(): Observable<OperationsEventPayload> { return this.purchaseCreated$.asObservable(); }
  getSaleCompleted(): Observable<OperationsEventPayload> { return this.saleCompleted$.asObservable(); }
  getLowStockDetected(): Observable<OperationsEventPayload> { return this.lowStockDetected$.asObservable(); }
  getBatchExpiring(): Observable<OperationsEventPayload> { return this.batchExpiring$.asObservable(); }
  getCommentAdded(): Observable<OperationsEventPayload> { return this.commentAdded$.asObservable(); }
  getActivityCreated(): Observable<OperationsEventPayload> { return this.activityCreated$.asObservable(); }
  getNotificationRaised(): Observable<OperationsEventPayload> { return this.notificationRaised$.asObservable(); }
  getDashboardUpdated(): Observable<OperationsEventPayload> { return this.dashboardUpdated$.asObservable(); }
  getHandsFreeTaskPushed(): Observable<OperationsEventPayload> { return this.handsFreeTaskPushed$.asObservable(); }
}
