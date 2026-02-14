import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CompanyApiService } from '../../../core/services/company-api.service';
import { ApiConfigService } from '../../../core/services/api-config.service';
import { Company } from '../../../core/models/company.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { ImageUploadComponent } from '../../../shared/components/image-upload/image-upload.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonDetailHeaderComponent } from '../../../shared/components/skeleton-detail-header/skeleton-detail-header.component';
import { SkeletonFormComponent } from '../../../shared/components/skeleton-form/skeleton-form.component';

@Component({
  selector: 'app-company-form',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    ToastComponent,
    LoadingSpinnerComponent,
    UnifiedCardComponent,
    UnifiedButtonComponent,
    ImageUploadComponent,
    ContentLoaderComponent,
    SkeletonDetailHeaderComponent,
    SkeletonFormComponent
  ],
  templateUrl: './company-form.component.html'
})
export class CompanyFormComponent implements OnInit {
  loading = true;
  saving = false;
  companyId: string | null = null;
  form: FormGroup;
  logoUrl: string | null = null;
  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'info' = 'success';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private companyApi: CompanyApiService,
    private apiConfig: ApiConfigService
  ) {
    this.form = this.fb.group({
      name: [''],
      legalName: [''],
      email: [''],
      phoneNumber: [''],
      address: [''],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id && id !== 'new') {
      this.companyId = id;
      this.companyApi.getById(id).subscribe({
        next: (res: ApiResponse<Company>) => {
          this.loading = false;
          if (res.success && res.data) {
            this.form.patchValue({
              name: res.data.name,
              legalName: res.data.legalName || '',
              email: res.data.email || '',
              phoneNumber: res.data.phoneNumber || '',
              address: res.data.address || '',
              isActive: res.data.isActive ?? true
            });
            this.logoUrl = res.data.logoUrl || null;
          }
        },
        error: () => { this.loading = false; }
      });
    } else {
      this.loading = false;
    }
  }

  onLogoUploaded(result: { imageUrl?: string }): void {
    if (result?.imageUrl) this.logoUrl = result.imageUrl;
  }

  /** Resolve logo URL for display (relative API paths become absolute). */
  get logoUrlResolved(): string | null {
    if (!this.logoUrl) return null;
    if (this.logoUrl.startsWith('/')) {
      return `${this.apiConfig.getBaseUrl()}${this.logoUrl}`;
    }
    return this.logoUrl;
  }

  save(): void {
    if (this.companyId) {
      this.saving = true;
      this.companyApi.update(this.companyId, this.form.value).subscribe({
        next: (res: ApiResponse<Company>) => {
          this.saving = false;
          if (res.success) this.showToastMsg('Saved', 'success');
          else this.showToastMsg(res.message || 'Update failed', 'error');
        },
        error: () => { this.saving = false; this.showToastMsg('Update failed', 'error'); }
      });
    } else {
      this.saving = true;
      this.companyApi.create(this.form.value).subscribe({
        next: (res: ApiResponse<Company>) => {
          this.saving = false;
          if (res.success && res.data) {
            this.showToastMsg('Company created', 'success');
            this.router.navigate(['/companies', res.data.id]);
          } else this.showToastMsg(res.message || 'Create failed', 'error');
        },
        error: () => { this.saving = false; this.showToastMsg('Create failed', 'error'); }
      });
    }
  }

  showToastMsg(msg: string, type: 'success' | 'error' | 'info'): void {
    this.toastMessage = msg;
    this.toastType = type;
    this.showToast = true;
  }
}
