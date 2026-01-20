import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class PermissionService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$: Observable<User | null> = this.currentUserSubject.asObservable();

  private permissions: string[] = [];
  private roles: string[] = [];

  constructor() {
    this.loadUserFromStorage();
  }

  setCurrentUser(user: User): void {
    this.currentUserSubject.next(user);
    this.permissions = user.permissions || [];
    this.roles = user.roles || [];
    this.saveUserToStorage(user);
  }

  clearUser(): void {
    this.currentUserSubject.next(null);
    this.permissions = [];
    this.roles = [];
    localStorage.removeItem('currentUser');
  }

  hasPermission(permission: string): boolean {
    if (!permission) return true; // If no permission specified, allow
    return this.permissions.includes(permission);
  }

  hasAnyPermission(permissions: string[]): boolean {
    if (!permissions || permissions.length === 0) return true;
    return permissions.some(p => this.hasPermission(p));
  }

  hasAllPermissions(permissions: string[]): boolean {
    if (!permissions || permissions.length === 0) return true;
    return permissions.every(p => this.hasPermission(p));
  }

  hasRole(role: string): boolean {
    return this.roles.includes(role);
  }

  hasAnyRole(roles: string[]): boolean {
    if (!roles || roles.length === 0) return true;
    return roles.some(r => this.hasRole(r));
  }

  getPermissions(): string[] {
    return [...this.permissions];
  }

  getRoles(): string[] {
    return [...this.roles];
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  private loadUserFromStorage(): void {
    const userStr = localStorage.getItem('currentUser');
    if (userStr) {
      try {
        const user: User = JSON.parse(userStr);
        this.setCurrentUser(user);
      } catch (e) {
        console.error('Error loading user from storage', e);
      }
    }
  }

  private saveUserToStorage(user: User): void {
    localStorage.setItem('currentUser', JSON.stringify(user));
  }
}

