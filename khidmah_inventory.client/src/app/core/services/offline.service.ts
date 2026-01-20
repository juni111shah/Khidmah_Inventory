import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OfflineService {
  private onlineStatus$ = new BehaviorSubject<boolean>(navigator.onLine);
  private dbName = 'KhidmahInventoryDB';
  private dbVersion = 1;
  private db: IDBDatabase | null = null;

  constructor() {
    window.addEventListener('online', () => this.onlineStatus$.next(true));
    window.addEventListener('offline', () => this.onlineStatus$.next(false));
    this.initDatabase();
  }

  isOnline(): boolean {
    return navigator.onLine;
  }

  getOnlineStatus(): Observable<boolean> {
    return this.onlineStatus$.asObservable();
  }

  private async initDatabase(): Promise<void> {
    return new Promise((resolve, reject) => {
      const request = indexedDB.open(this.dbName, this.dbVersion);

      request.onerror = () => reject(request.error);
      request.onsuccess = () => {
        this.db = request.result;
        resolve();
      };

      request.onupgradeneeded = (event) => {
        const db = (event.target as IDBOpenDBRequest).result;

        // Create object stores for offline data
        if (!db.objectStoreNames.contains('stockTransactions')) {
          db.createObjectStore('stockTransactions', { keyPath: 'id', autoIncrement: true });
        }

        if (!db.objectStoreNames.contains('salesOrders')) {
          db.createObjectStore('salesOrders', { keyPath: 'id', autoIncrement: true });
        }

        if (!db.objectStoreNames.contains('purchaseOrders')) {
          db.createObjectStore('purchaseOrders', { keyPath: 'id', autoIncrement: true });
        }

        if (!db.objectStoreNames.contains('syncQueue')) {
          const syncStore = db.createObjectStore('syncQueue', { keyPath: 'id', autoIncrement: true });
          syncStore.createIndex('type', 'type', { unique: false });
          syncStore.createIndex('status', 'status', { unique: false });
        }
      };
    });
  }

  async saveOfflineData(storeName: string, data: any): Promise<void> {
    if (!this.db) {
      await this.initDatabase();
    }

    return new Promise((resolve, reject) => {
      if (!this.db) {
        reject(new Error('Database not initialized'));
        return;
      }

      const transaction = this.db.transaction([storeName], 'readwrite');
      const store = transaction.objectStore(storeName);
      const request = store.add(data);

      request.onsuccess = () => resolve();
      request.onerror = () => reject(request.error);
    });
  }

  async getOfflineData(storeName: string): Promise<any[]> {
    if (!this.db) {
      await this.initDatabase();
    }

    return new Promise((resolve, reject) => {
      if (!this.db) {
        reject(new Error('Database not initialized'));
        return;
      }

      const transaction = this.db.transaction([storeName], 'readonly');
      const store = transaction.objectStore(storeName);
      const request = store.getAll();

      request.onsuccess = () => resolve(request.result);
      request.onerror = () => reject(request.error);
    });
  }

  async addToSyncQueue(type: string, action: string, data: any): Promise<void> {
    if (!this.db) {
      await this.initDatabase();
    }

    return new Promise((resolve, reject) => {
      if (!this.db) {
        reject(new Error('Database not initialized'));
        return;
      }

      const transaction = this.db.transaction(['syncQueue'], 'readwrite');
      const store = transaction.objectStore('syncQueue');
      const syncItem = {
        type,
        action,
        data,
        status: 'pending',
        createdAt: new Date().toISOString()
      };
      const request = store.add(syncItem);

      request.onsuccess = () => resolve();
      request.onerror = () => reject(request.error);
    });
  }

  async getSyncQueue(): Promise<any[]> {
    if (!this.db) {
      await this.initDatabase();
    }

    return new Promise((resolve, reject) => {
      if (!this.db) {
        reject(new Error('Database not initialized'));
        return;
      }

      const transaction = this.db.transaction(['syncQueue'], 'readonly');
      const store = transaction.objectStore('syncQueue');
      const index = store.index('status');
      const request = index.getAll('pending');

      request.onsuccess = () => resolve(request.result);
      request.onerror = () => reject(request.error);
    });
  }

  async removeFromSyncQueue(id: number): Promise<void> {
    if (!this.db) {
      await this.initDatabase();
    }

    return new Promise((resolve, reject) => {
      if (!this.db) {
        reject(new Error('Database not initialized'));
        return;
      }

      const transaction = this.db.transaction(['syncQueue'], 'readwrite');
      const store = transaction.objectStore('syncQueue');
      const request = store.delete(id);

      request.onsuccess = () => resolve();
      request.onerror = () => reject(request.error);
    });
  }
}

