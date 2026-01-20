import { Directive, Input, TemplateRef, ViewContainerRef, OnInit, OnDestroy } from '@angular/core';
import { PermissionService } from '../../core/services/permission.service';
import { Subscription } from 'rxjs';

@Directive({
  selector: '[appHasPermission]',
  standalone: true
})
export class HasPermissionDirective implements OnInit, OnDestroy {
  @Input() appHasPermission: string | string[] = '';
  @Input() appHasPermissionMode: 'any' | 'all' = 'any';
  
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
    const hasPermission = this.checkPermission();

    if (hasPermission && !this.hasView) {
      this.viewContainer.createEmbeddedView(this.templateRef);
      this.hasView = true;
    } else if (!hasPermission && this.hasView) {
      this.viewContainer.clear();
      this.hasView = false;
    }
  }

  private checkPermission(): boolean {
    if (!this.appHasPermission) {
      return true;
    }

    if (Array.isArray(this.appHasPermission)) {
      if (this.appHasPermission.length === 0) {
        return true;
      }
      return this.appHasPermissionMode === 'all'
        ? this.permissionService.hasAllPermissions(this.appHasPermission)
        : this.permissionService.hasAnyPermission(this.appHasPermission);
    }

    return this.permissionService.hasPermission(this.appHasPermission);
  }
}

