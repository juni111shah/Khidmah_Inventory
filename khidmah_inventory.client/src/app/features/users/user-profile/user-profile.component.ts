import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UserApiService } from '../../../core/services/user-api.service';
import { User, UpdateUserProfileRequest, ChangePasswordRequest } from '../../../core/models/user.model';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { PermissionService } from '../../../core/services/permission.service';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { AuthService } from '../../../core/services/auth.service';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { HeaderService } from '../../../core/services/header.service';

import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { ImageUploadComponent } from '../../../shared/components/image-upload/image-upload.component';
import { ExportService } from '../../../core/services/export.service';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonDetailHeaderComponent } from '../../../shared/components/skeleton-detail-header/skeleton-detail-header.component';
import { SkeletonLoaderComponent } from '../../../shared/components/skeleton-loader/skeleton-loader.component';
import { SkeletonFormComponent } from '../../../shared/components/skeleton-form/skeleton-form.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, IconComponent, ToastComponent, LoadingSpinnerComponent, HasPermissionDirective, UnifiedButtonComponent, ImageUploadComponent, ContentLoaderComponent, SkeletonDetailHeaderComponent, SkeletonLoaderComponent, SkeletonFormComponent, UnifiedCardComponent],
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss']
})
export class UserProfileComponent implements OnInit {
  user: User | null = null;
  loading = false;
  saving = false;
  changingPassword = false;
  isViewMode = false;

  // Profile form
  profileForm: UpdateUserProfileRequest = {
    firstName: '',
    lastName: '',
    phoneNumber: ''
  };

  // Password form
  passwordForm: ChangePasswordRequest = {
    currentPassword: '',
    newPassword: ''
  };
  confirmPassword = '';

  activeTab: 'profile' | 'password' = 'profile';

  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';

  constructor(
    private userApiService: UserApiService,
    private route: ActivatedRoute,
    public router: Router,
    public permissionService: PermissionService,
    public authService: AuthService,
    private headerService: HeaderService,
    private exportService: ExportService
  ) {}

  ngOnInit(): void {
    const url = this.router.url;
    // View mode if we are not on 'profile' (my profile) AND not editing a user
    this.isViewMode = !url.includes('/users/profile') && !url.includes('/edit');

    this.headerService.setHeaderInfo({
      title: 'User Profile',
      description: 'View and manage user profile details'
    });
    const userId = this.route.snapshot.paramMap.get('id');
    if (userId) {
      this.loadUser(userId);
    } else {
      this.loadCurrentUser();
    }
  }

  loadCurrentUser(): void {
    this.loading = true;
    this.userApiService.getCurrentUser().subscribe({
      next: (response: any) => {
        if (response.success && response.data) {
          this.user = response.data;
          this.profileForm = {
            firstName: response.data.firstName,
            lastName: response.data.lastName,
            phoneNumber: response.data.phoneNumber || ''
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

  loadUser(id: string): void {
    this.loading = true;
    this.userApiService.getUser(id).subscribe({
      next: (response: any) => {
        if (response.success && response.data) {
          this.user = response.data;
          this.profileForm = {
            firstName: response.data.firstName,
            lastName: response.data.lastName,
            phoneNumber: response.data.phoneNumber || ''
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

  saveProfile(): void {
    if (!this.user) return;

    if (!this.profileForm.firstName || !this.profileForm.lastName) {
      this.showToastMessage('error', 'First name and last name are required');
      return;
    }

    this.saving = true;
    this.userApiService.updateProfile(this.user.id, this.profileForm).subscribe({
      next: (response: any) => {
        if (response.success && response.data) {
          this.user = response.data;
          this.showToastMessage('success', 'Profile updated successfully');
        } else {
          this.showToastMessage('error', response.message || 'Failed to update profile');
        }
        this.saving = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error updating profile');
        this.saving = false;
      }
    });
  }

  changePassword(): void {
    if (!this.user) return;

    if (!this.passwordForm.newPassword || !this.confirmPassword) {
      this.showToastMessage('error', 'Please fill in all password fields');
      return;
    }

    if (this.passwordForm.newPassword !== this.confirmPassword) {
      this.showToastMessage('error', 'New passwords do not match');
      return;
    }

    if (this.passwordForm.newPassword.length < 6) {
      this.showToastMessage('error', 'Password must be at least 6 characters');
      return;
    }

    this.changingPassword = true;
    this.userApiService.changePassword(this.user.id, this.passwordForm).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.showToastMessage('success', 'Password changed successfully');
          this.passwordForm = { currentPassword: '', newPassword: '' };
          this.confirmPassword = '';
        } else {
          this.showToastMessage('error', response.message || 'Failed to change password');
        }
        this.changingPassword = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error changing password');
        this.changingPassword = false;
      }
    });
  }



  async exportToPdf(): Promise<void> {
    if (!this.user) return;

    const details = [
      { label: 'First Name', value: this.user.firstName },
      { label: 'Last Name', value: this.user.lastName },
      { label: 'Email', value: this.user.email },
      { label: 'Username', value: this.user.userName },
      { label: 'Phone', value: this.user.phoneNumber },
      { label: 'Status', value: this.user.isActive ? 'Active' : 'Inactive' },
      { label: 'Roles', value: this.user.roles.join(', ') }
    ];

    try {
      await this.exportService.exportEntityDetails(
        details,
        `User Details: ${this.user.firstName} ${this.user.lastName}`,
        `user_details_${this.user.userName}`
      );
      this.showToastMessage('success', 'PDF exported successfully');
    } catch (error) {
      console.error(error);
      this.showToastMessage('error', 'Failed to export PDF');
    }
  }

  onAvatarUploadSuccess(result: any): void {
    if (this.user && result.imageUrl) {
      this.user.avatarUrl = result.imageUrl;
      this.showToastMessage('success', 'Avatar updated successfully');
    }
  }

  onAvatarUploadError(error: string): void {
    this.showToastMessage('error', error);
  }

  showToastMessage(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.toastType = type;
    this.toastMessage = message;
    this.showToast = true;
    setTimeout(() => {
      this.showToast = false;
    }, 3000);
  }
}


