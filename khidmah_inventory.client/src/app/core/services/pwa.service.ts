import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PwaService {
  private promptEvent: any;
  private installable$ = new BehaviorSubject<boolean>(false);
  private installed$ = new BehaviorSubject<boolean>(false);

  constructor() {
    this.checkInstallStatus();
    this.setupInstallPrompt();
  }

  private checkInstallStatus(): void {
    // Check if app is already installed
    if (window.matchMedia('(display-mode: standalone)').matches) {
      this.installed$.next(true);
    }
  }

  private setupInstallPrompt(): void {
    window.addEventListener('beforeinstallprompt', (e: Event) => {
      e.preventDefault();
      this.promptEvent = e;
      this.installable$.next(true);
    });

    window.addEventListener('appinstalled', () => {
      this.installed$.next(true);
      this.installable$.next(false);
      this.promptEvent = null;
    });
  }

  isInstallable(): Observable<boolean> {
    return this.installable$.asObservable();
  }

  isInstalled(): Observable<boolean> {
    return this.installed$.asObservable();
  }

  async install(): Promise<boolean> {
    if (!this.promptEvent) {
      return false;
    }

    this.promptEvent.prompt();
    const { outcome } = await this.promptEvent.userChoice;

    if (outcome === 'accepted') {
      this.installable$.next(false);
      this.promptEvent = null;
      return true;
    }

    return false;
  }

  async requestNotificationPermission(): Promise<NotificationPermission> {
    if (!('Notification' in window)) {
      return 'denied';
    }

    if (Notification.permission === 'default') {
      return await Notification.requestPermission();
    }

    return Notification.permission;
  }

  async showNotification(title: string, options?: NotificationOptions): Promise<void> {
    const permission = await this.requestNotificationPermission();

    if (permission === 'granted') {
      if ('serviceWorker' in navigator) {
        const registration = await navigator.serviceWorker.ready;
        await registration.showNotification(title, options);
      } else {
        new Notification(title, options);
      }
    }
  }
}

