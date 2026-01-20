import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { AuthService } from './auth.service';
import { ApiConfigService } from './api-config.service';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: HubConnection | null = null;
  private connectionState$ = new BehaviorSubject<HubConnectionState>(HubConnectionState.Disconnected);
  private baseUrl: string;

  constructor(
    private authService: AuthService,
    private apiConfig: ApiConfigService
  ) {
    // SignalR hubs are typically at the base URL, not under /api
    this.baseUrl = `${this.apiConfig.getBaseUrl()}/hubs/analytics`;
  }

  async startConnection(): Promise<void> {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      return;
    }

    const token = this.authService.getToken();
    if (!token) {
      console.warn('No token available for SignalR connection');
      return;
    }

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.baseUrl, {
        accessTokenFactory: () => token,
        skipNegotiation: false, // Allow negotiation fallback
        transport: 1 // WebSockets, but will fallback if needed
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: retryContext => {
          // Exponential backoff with max 30 seconds
          return Math.min(1000 * Math.pow(2, retryContext.previousRetryCount), 30000);
        }
      })
      .build();

    this.hubConnection.onreconnecting(() => {
      this.connectionState$.next(HubConnectionState.Reconnecting);
    });

    this.hubConnection.onreconnected(() => {
      this.connectionState$.next(HubConnectionState.Connected);
    });

    this.hubConnection.onclose(() => {
      this.connectionState$.next(HubConnectionState.Disconnected);
    });

    try {
      // Add timeout to prevent hanging indefinitely
      const timeoutPromise = new Promise<void>((_, reject) => {
        setTimeout(() => reject(new Error('SignalR connection timeout')), 10000); // 10 second timeout
      });

      await Promise.race([
        this.hubConnection.start(),
        timeoutPromise
      ]);
      
      this.connectionState$.next(HubConnectionState.Connected);
      console.log('SignalR connection established');
    } catch (error) {
      console.error('Error starting SignalR connection:', error);
      this.connectionState$.next(HubConnectionState.Disconnected);
      // Don't throw, just log the error so the app can continue
    }
  }

  async stopConnection(): Promise<void> {
    if (this.hubConnection) {
      await this.hubConnection.stop();
      this.hubConnection = null;
      this.connectionState$.next(HubConnectionState.Disconnected);
    }
  }

  async subscribeToAnalytics(analyticsType: string): Promise<void> {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      await this.hubConnection.invoke('SubscribeToAnalytics', analyticsType);
    }
  }

  async unsubscribeFromAnalytics(analyticsType: string): Promise<void> {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      await this.hubConnection.invoke('UnsubscribeFromAnalytics', analyticsType);
    }
  }

  onDashboardUpdate(callback: (data: any) => void): void {
    if (this.hubConnection) {
      this.hubConnection.on('DashboardUpdated', callback);
    }
  }

  onAnalyticsUpdate(callback: (analyticsType: string, data: any) => void): void {
    if (this.hubConnection) {
      this.hubConnection.on('AnalyticsUpdated', callback);
    }
  }

  offDashboardUpdate(callback: (data: any) => void): void {
    if (this.hubConnection) {
      this.hubConnection.off('DashboardUpdated', callback);
    }
  }

  offAnalyticsUpdate(callback: (analyticsType: string, data: any) => void): void {
    if (this.hubConnection) {
      this.hubConnection.off('AnalyticsUpdated', callback);
    }
  }

  getConnectionState(): Observable<HubConnectionState> {
    return this.connectionState$.asObservable();
  }

  isConnected(): boolean {
    return this.hubConnection?.state === HubConnectionState.Connected;
  }
}

