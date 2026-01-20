import { Directive, Input, TemplateRef, ViewContainerRef, OnInit, OnDestroy } from '@angular/core';
import { PermissionService } from '../../core/services/permission.service';
import { Subscription } from 'rxjs';

@Directive({
  selector: '[appHasRole]',
  standalone: true
})
export class HasRoleDirective implements OnInit, OnDestroy {
  @Input() appHasRole: string | string[] = '';
  @Input() appHasRoleMode: 'any' | 'all' = 'any';
  
  private subscription?: Subscription;
  private hasView = false;

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private permissionService: PermissionService
  ) {}

  ngOnInit(): void {
    this.subscription = this.permissionService.currentUser$.subscribe(() => {
      this.updateView();
    });
    this.updateView();
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  private updateView(): void {
    const hasRole = this.checkRole();

    if (hasRole && !this.hasView) {
      this.viewContainer.createEmbeddedView(this.templateRef);
      this.hasView = true;
    } else if (!hasRole && this.hasView) {
      this.viewContainer.clear();
      this.hasView = false;
    }
  }

  private checkRole(): boolean {
    if (!this.appHasRole) {
      return true;
    }

    if (Array.isArray(this.appHasRole)) {
      if (this.appHasRole.length === 0) {
        return true;
      }
      return this.appHasRoleMode === 'all'
        ? this.appHasRole.every(r => this.permissionService.hasRole(r))
        : this.permissionService.hasAnyRole(this.appHasRole);
    }

    return this.permissionService.hasRole(this.appHasRole);
  }
}

