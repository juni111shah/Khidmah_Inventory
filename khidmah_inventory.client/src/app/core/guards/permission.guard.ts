import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { PermissionService } from '../services/permission.service';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class PermissionGuard implements CanActivate {
  constructor(
    private permissionService: PermissionService,
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    // First check if user is authenticated
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
      return false;
    }

    // Get required permission from route data
    const requiredPermission = route.data['permission'] as string | string[];
    const permissionMode = route.data['permissionMode'] as 'any' | 'all' || 'any';

    if (!requiredPermission) {
      // No permission required, allow access
      return true;
    }

    // Check permission
    let hasPermission = false;
    if (Array.isArray(requiredPermission)) {
      hasPermission = permissionMode === 'all'
        ? this.permissionService.hasAllPermissions(requiredPermission)
        : this.permissionService.hasAnyPermission(requiredPermission);
    } else {
      hasPermission = this.permissionService.hasPermission(requiredPermission);
    }

    if (!hasPermission) {
      // Redirect to unauthorized page or show message
      this.router.navigate(['/unauthorized']);
      return false;
    }

    return true;
  }
}

