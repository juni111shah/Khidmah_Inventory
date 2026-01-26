import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UserApiService } from '../../../core/services/user-api.service';
import { RoleApiService } from '../../../core/services/role-api.service';
import { User, UpdateUserProfileRequest } from '../../../core/models/user.model';
import { Role } from '../../../core/models/role.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { PermissionService } from '../../../core/services/permission.service';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { FormFieldComponent } from '../../../shared/components/form-field/form-field.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { HeaderService } from '../../../core/services/header.service';

interface CreateUserRequest {
  email: string;
  userName: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  password: string;
  confirmPassword: string;
  roles: string[];
  companyId?: string;
}

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ToastComponent,
    LoadingSpinnerComponent,
    IconComponent,
    HasPermissionDirective,
    FormFieldComponent,
    UnifiedButtonComponent,
    UnifiedCardComponent
  ],
  templateUrl: './user-form.component.html'
})
export class UserFormComponent implements OnInit {
  user: User | null = null;
  availableRoles: Role[] = [];
  loading = false;
  saving = false;
  isEditMode = false;

  formData: CreateUserRequest = {
    email: '',
    userName: '',
    firstName: '',
    lastName: '',
    phoneNumber: '',
    password: '',
    confirmPassword: '',
    roles: [],
    companyId: ''
  };

  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';

  constructor(
    private userApiService: UserApiService,
    private roleApiService: RoleApiService,
    private route: ActivatedRoute,
    public router: Router,
    public permissionService: PermissionService,
    private headerService: HeaderService
  ) {}

  ngOnInit(): void {
    const userId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!userId;

    this.headerService.setHeaderInfo({
      title: this.isEditMode ? 'Edit User' : 'Create User',
      description: this.isEditMode ? 'Modify user information' : 'Add a new user to the system'
    });

    this.loadAvailableRoles();

    if (this.isEditMode && userId) {
      this.loadUser(userId);
    } else {
      // For new user, we could load a template, but for now just initialize empty form
    }
  }

  loadUser(id: string): void {
    this.loading = true;
    this.userApiService.getUser(id).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.user = response.data;
          this.formData = {
            email: response.data.email,
            userName: response.data.userName,
            firstName: response.data.firstName,
            lastName: response.data.lastName,
            phoneNumber: response.data.phoneNumber || '',
            password: '',
            confirmPassword: '',
            roles: response.data.roles || [],
            companyId: response.data.defaultCompanyId || ''
          };
        } else {
          this.showToastMessage('error', response.message || 'Failed to load user');
        }
        this.loading = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error loading user');
        this.loading = false;
      }
    });
  }

  loadAvailableRoles(): void {
    this.roleApiService.getRoles().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.availableRoles = response.data;
        }
      },
      error: () => {
        this.showToastMessage('error', 'Error loading roles');
      }
    });
  }

  save(): void {
    if (!this.validateForm()) {
      return;
    }

    this.saving = true;

    if (this.isEditMode && this.user) {
      // For editing, only update profile fields that are allowed
      const updateRequest: UpdateUserProfileRequest = {
        firstName: this.formData.firstName,
        lastName: this.formData.lastName,
        phoneNumber: this.formData.phoneNumber || undefined
      };

      this.userApiService.updateProfile(this.user.id, updateRequest).subscribe({
        next: (response) => {
          if (response.success) {
            this.showToastMessage('success', 'User updated successfully');
            setTimeout(() => {
              this.router.navigate(['/users']);
            }, 1500);
          } else {
            this.showToastMessage('error', response.message || 'Failed to update user');
          }
          this.saving = false;
        },
        error: () => {
          this.showToastMessage('error', 'Error updating user');
          this.saving = false;
        }
      });
    } else {
      // For creating new user
      const createRequest = {
        email: this.formData.email,
        userName: this.formData.userName,
        firstName: this.formData.firstName,
        lastName: this.formData.lastName,
        phoneNumber: this.formData.phoneNumber || undefined,
        password: this.formData.password,
        roles: this.formData.roles,
        companyId: this.formData.companyId || undefined
      };

      this.userApiService.createUser(createRequest).subscribe({
        next: (response) => {
          if (response.success) {
            this.showToastMessage('success', 'User created successfully');
            setTimeout(() => {
              this.router.navigate(['/users']);
            }, 1500);
          } else {
            this.showToastMessage('error', response.message || 'Failed to create user');
          }
          this.saving = false;
        },
        error: () => {
          this.showToastMessage('error', 'Error creating user');
          this.saving = false;
        }
      });
    }
  }

  private validateForm(): boolean {
    if (!this.formData.firstName.trim()) {
      this.showToastMessage('error', 'First name is required');
      return false;
    }

    if (!this.formData.lastName.trim()) {
      this.showToastMessage('error', 'Last name is required');
      return false;
    }

    if (!this.isEditMode) {
      // Validation for create mode
      if (!this.formData.email.trim()) {
        this.showToastMessage('error', 'Email is required');
        return false;
      }

      if (!this.formData.userName.trim()) {
        this.showToastMessage('error', 'Username is required');
        return false;
      }

      if (!this.formData.password) {
        this.showToastMessage('error', 'Password is required');
        return false;
      }

      if (this.formData.password !== this.formData.confirmPassword) {
        this.showToastMessage('error', 'Passwords do not match');
        return false;
      }

      if (this.formData.password.length < 6) {
        this.showToastMessage('error', 'Password must be at least 6 characters');
        return false;
      }

      if (!this.formData.roles || this.formData.roles.length === 0) {
        this.showToastMessage('error', 'At least one role must be selected');
        return false;
      }
    }

    return true;
  }

  onRoleChange(roleName: string, checked: boolean): void {
    if (checked) {
      if (!this.formData.roles.includes(roleName)) {
        this.formData.roles.push(roleName);
      }
    } else {
      this.formData.roles = this.formData.roles.filter(r => r !== roleName);
    }
  }

  isRoleSelected(roleName: string): boolean {
    return this.formData.roles.includes(roleName);
  }

  showToastMessage(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.toastType = type;
    this.toastMessage = message;
    this.showToast = true;
    setTimeout(() => {
      this.showToast = false;
    }, 3000);
  }

  get roleOptions(): { value: string; label: string }[] {
    return this.availableRoles.map(r => ({ value: r.name, label: r.name }));
  }
}