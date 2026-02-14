import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { RoleApiService } from '../../../core/services/role-api.service';
import { PermissionApiService } from '../../../core/services/permission-api.service';
import { Role, Permission, CreateRoleRequest, UpdateRoleRequest } from '../../../core/models/role.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { PermissionService } from '../../../core/services/permission.service';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { HeaderService } from '../../../core/services/header.service';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { FormFieldComponent } from '../../../shared/components/form-field/form-field.component';
import { UnifiedCheckboxComponent } from '../../../shared/components/unified-checkbox/unified-checkbox.component';
import { BadgeComponent } from '../../../shared/components/badge/badge.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonDetailHeaderComponent } from '../../../shared/components/skeleton-detail-header/skeleton-detail-header.component';
import { SkeletonFormComponent } from '../../../shared/components/skeleton-form/skeleton-form.component';
import { SkeletonListComponent } from '../../../shared/components/skeleton-list/skeleton-list.component';

@Component({
  selector: 'app-role-form',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ToastComponent,
    LoadingSpinnerComponent,
    IconComponent,
    HasPermissionDirective,
    UnifiedButtonComponent,
    UnifiedCardComponent,
    FormFieldComponent,
    UnifiedCheckboxComponent,
    BadgeComponent,
    ContentLoaderComponent,
    SkeletonDetailHeaderComponent,
    SkeletonFormComponent,
    SkeletonListComponent
  ],
  templateUrl: './role-form.component.html'
})
export class RoleFormComponent implements OnInit {
  role: Role | null = null;
  permissions: Permission[] = [];
  groupedPermissions: { [module: string]: Permission[] } = {};
  permissionsByModuleAndAction: { [module: string]: { [action: string]: Permission[] } } = {};
  selectedPermissionIds: Set<string> = new Set();
  permissionStates: { [permissionId: string]: boolean } = {};

  formData = {
    name: '',
    description: ''
  };

  loading = false;
  saving = false;
  isEditMode = false;

  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';

  constructor(
    private roleApiService: RoleApiService,
    private permissionApiService: PermissionApiService,
    private route: ActivatedRoute,
    public router: Router,
    public permissionService: PermissionService,
    private headerService: HeaderService
  ) {}

  ngOnInit(): void {
    const roleId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!roleId;

    this.headerService.setHeaderInfo({
      title: this.isEditMode ? 'Edit Role' : 'Create Role',
      description: this.isEditMode ? 'Modify existing role permissions' : 'Add a new system role'
    });

    this.loadPermissions();

    if (this.isEditMode && roleId) {
      this.loadRole(roleId);
    }
  }

  loadRole(id: string): void {
    this.loading = true;
    this.roleApiService.getRole(id).subscribe({
      next: (response: ApiResponse<Role>) => {
        if (response.success && response.data) {
          this.role = response.data;
          this.formData = {
            name: response.data.name,
            description: response.data.description || ''
          };
          this.selectedPermissionIds = new Set(response.data.permissions.map((p: Permission) => p.id));
          // Initialize permission states
          this.permissions.forEach(permission => {
            this.permissionStates[permission.id] = this.selectedPermissionIds.has(permission.id);
          });
        } else {
          this.showToastMessage('error', response.message || 'Failed to load role');
        }
        this.loading = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error loading role');
        this.loading = false;
      }
    });
  }

  loadPermissions(): void {
    this.permissionApiService.getPermissions().subscribe({
      next: (response: ApiResponse<Permission[]>) => {
        if (response.success && response.data) {
          this.permissions = response.data;
          this.groupPermissions();
          // Initialize permission states
          this.permissions.forEach(permission => {
            this.permissionStates[permission.id] = this.selectedPermissionIds.has(permission.id);
          });
        } else {
          this.showToastMessage('error', response.message || 'Failed to load permissions');
        }
      },
      error: () => {
        this.showToastMessage('error', 'Error loading permissions');
      }
    });
  }

  groupPermissions(): void {
    this.groupedPermissions = {};
    this.permissionsByModuleAndAction = {};

    this.permissions.forEach(permission => {
      // Group by module
      if (!this.groupedPermissions[permission.module]) {
        this.groupedPermissions[permission.module] = [];
      }
      this.groupedPermissions[permission.module].push(permission);

      // Group by module and action
      if (!this.permissionsByModuleAndAction[permission.module]) {
        this.permissionsByModuleAndAction[permission.module] = {};
      }

      const mainAction = permission.action.split(':')[0];
      if (!this.permissionsByModuleAndAction[permission.module][mainAction]) {
        this.permissionsByModuleAndAction[permission.module][mainAction] = [];
      }
      this.permissionsByModuleAndAction[permission.module][mainAction].push(permission);
    });
  }

  onPermissionChange(permissionId: string, checked: boolean): void {
    if (checked) {
      this.selectedPermissionIds.add(permissionId);
    } else {
      this.selectedPermissionIds.delete(permissionId);
    }
    this.permissionStates[permissionId] = checked;
  }

  isPermissionSelected(permissionId: string): boolean {
    return this.selectedPermissionIds.has(permissionId);
  }

  toggleModule(module: string): void {
    const modulePermissions = this.groupedPermissions[module];
    const allSelected = modulePermissions.every(p => this.permissionStates[p.id]);

    if (allSelected) {
      // Deselect all
      modulePermissions.forEach(p => {
        this.selectedPermissionIds.delete(p.id);
        this.permissionStates[p.id] = false;
      });
    } else {
      // Select all
      modulePermissions.forEach(p => {
        this.selectedPermissionIds.add(p.id);
        this.permissionStates[p.id] = true;
      });
    }
  }

  isModuleSelected(module: string): boolean {
    const modulePermissions = this.groupedPermissions[module];
    return modulePermissions.length > 0 && modulePermissions.every(p => this.permissionStates[p.id]);
  }

  isModulePartiallySelected(module: string): boolean {
    const modulePermissions = this.groupedPermissions[module];
    const selectedCount = modulePermissions.filter(p => this.permissionStates[p.id]).length;
    return selectedCount > 0 && selectedCount < modulePermissions.length;
  }

  save(): void {
    if (!this.formData.name.trim()) {
      this.showToastMessage('error', 'Role name is required');
      return;
    }

    this.saving = true;
    const permissionIds = Array.from(this.selectedPermissionIds);

    if (this.isEditMode && this.role) {
      const updateRequest: UpdateRoleRequest = {
        name: this.formData.name,
        description: this.formData.description,
        permissionIds: permissionIds
      };

      this.roleApiService.updateRole(this.role.id, updateRequest).subscribe({
        next: (response: ApiResponse<Role>) => {
          if (response.success) {
            this.showToastMessage('success', 'Role updated successfully');
            setTimeout(() => {
              this.router.navigate(['/roles']);
            }, 1500);
          } else {
            this.showToastMessage('error', response.message || 'Failed to update role');
          }
          this.saving = false;
        },
        error: () => {
          this.showToastMessage('error', 'Error updating role');
          this.saving = false;
        }
      });
    } else {
      const createRequest: CreateRoleRequest = {
        name: this.formData.name,
        description: this.formData.description,
        permissionIds: permissionIds
      };

      this.roleApiService.createRole(createRequest).subscribe({
        next: (response: ApiResponse<Role>) => {
          if (response.success) {
            this.showToastMessage('success', 'Role created successfully');
            setTimeout(() => {
              this.router.navigate(['/roles']);
            }, 1500);
          } else {
            this.showToastMessage('error', response.message || 'Failed to create role');
          }
          this.saving = false;
        },
        error: () => {
          this.showToastMessage('error', 'Error creating role');
          this.saving = false;
        }
      });
    }
  }

  showToastMessage(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.toastType = type;
    this.toastMessage = message;
    this.showToast = true;
    setTimeout(() => {
      this.showToast = false;
    }, 3000);
  }

  get modules(): string[] {
    return Object.keys(this.groupedPermissions).sort();
  }

  getActionsForModule(module: string): string[] {
    if (!this.permissionsByModuleAndAction[module]) {
      return [];
    }

    // Define action priority order for consistent display
    const actionOrder = ['List', 'Read', 'Create', 'Update', 'Delete', 'Assign', 'Approve', 'Activate', 'Deactivate', 'ChangePassword', 'Generate', 'Execute', 'Admin', 'Audit', 'Backup', 'Start'];

    return Object.keys(this.permissionsByModuleAndAction[module]).sort((a, b) => {
      const indexA = actionOrder.indexOf(a);
      const indexB = actionOrder.indexOf(b);

      // If both actions are in the priority list, sort by priority
      if (indexA !== -1 && indexB !== -1) {
        return indexA - indexB;
      }

      // If only one is in the priority list, prioritize it
      if (indexA !== -1) return -1;
      if (indexB !== -1) return 1;

      // Otherwise, sort alphabetically
      return a.localeCompare(b);
    });
  }

  getActionDisplayName(action: string): string {
    // Parse action to get the main action type
    const actionParts = action.split(':');
    const mainAction = actionParts[0];

    const actionMap: { [key: string]: string } = {
      'Create': 'CREATE',
      'Read': 'VIEW',
      'Update': 'EDIT',
      'Delete': 'DELETE',
      'List': 'LIST',
      'Assign': 'ASSIGN',
      'Approve': 'APPROVE',
      'Generate': 'GENERATE',
      'Execute': 'EXECUTE',
      'Activate': 'ACTIVATE',
      'Deactivate': 'DEACTIVATE',
      'ChangePassword': 'PASSWORD',
      'Admin': 'ADMIN',
      'Audit': 'AUDIT',
      'Backup': 'BACKUP',
      'Start': 'START'
    };

    return actionMap[mainAction] || mainAction.toUpperCase();
  }

  getActionBadgeColor(action: string): string {
    const mainAction = action.split(':')[0];

    const colorMap: { [key: string]: string } = {
      'Create': 'success',
      'Read': 'info',
      'Update': 'warning',
      'Delete': 'error',
      'List': 'secondary',
      'Assign': 'primary',
      'Approve': 'success',
      'Generate': 'info',
      'Execute': 'primary',
      'Activate': 'success',
      'Deactivate': 'warning',
      'ChangePassword': 'warning',
      'Admin': 'error',
      'Audit': 'secondary',
      'Backup': 'info',
      'Start': 'primary'
    };

    return colorMap[mainAction] || 'secondary';
  }

  getPermissionDisplayName(permission: Permission): string {
    // For permissions like "Module:SubModule:Action", show "SubModule Action"
    // For permissions like "Module:Action", show just the action part
    const parts = permission.name.split(':');

    if (parts.length === 3) {
      // Module:SubModule:Action -> "SubModule Action"
      return `${parts[1]} ${parts[2]}`;
    } else if (parts.length === 2) {
      // Module:Action -> "Action"
      return parts[1];
    }

    return permission.name;
  }

  getModulePermissionCount(module: string): number {
    return this.groupedPermissions[module]?.length || 0;
  }

  getActionPermissionsCount(module: string, action: string): number {
    return this.permissionsByModuleAndAction[module]?.[action]?.length || 0;
  }
}


