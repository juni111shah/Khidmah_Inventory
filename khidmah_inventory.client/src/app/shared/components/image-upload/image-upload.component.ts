import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UnifiedFileUploadComponent, FileUploadResult } from '../unified-file-upload/unified-file-upload.component';
import { ImageUploadService, ImageUploadResult as UploadResult, UploadProgress } from '../../../core/services/image-upload.service';
import { ToastComponent } from '../toast/toast.component';
import { LoadingSpinnerComponent } from '../loading-spinner/loading-spinner.component';

export type ImageUploadType =
  | 'user-avatar'
  | 'customer-image'
  | 'supplier-image'
  | 'product-image'
  | 'category-image'
  | 'brand-logo'
  | 'company-logo'
  | 'theme-logo';

@Component({
  selector: 'app-image-upload',
  standalone: true,
  imports: [
    CommonModule,
    UnifiedFileUploadComponent,
    ToastComponent,
    LoadingSpinnerComponent
  ],
  templateUrl: './image-upload.component.html'
})
export class ImageUploadComponent implements OnInit {
  @Input() uploadType: ImageUploadType = 'user-avatar';
  @Input() entityId: string = '';
  @Input() label: string = 'Upload Image';
  @Input() hint: string = '';
  @Input() maxSizeMB: number = 5;
  @Input() showProgress: boolean = true;
  @Input() allowMultiple: boolean = false;

  @Output() uploadSuccess = new EventEmitter<UploadResult>();
  @Output() uploadError = new EventEmitter<string>();

  currentImageUrl: string | null = null;
  isUploading = false;
  uploadProgress: number = 0;
  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';

  selectedFile: File | null = null;
  private altText: string = '';
  private isPrimary: boolean = false;

  constructor(private imageUploadService: ImageUploadService) {}

  ngOnInit(): void {
    // Load current image if available
    this.loadCurrentImage();
  }

  private loadCurrentImage(): void {
    // This would be implemented to load current images from the entities
    // For now, we'll handle this through input properties
  }

  onFileSelected(results: FileUploadResult[]): void {
    if (results.length === 0) return;

    const result = results[0];
    if (result.error) {
      this.showToastMessage('error', result.error);
      return;
    }

    if (result.file) {
      const validation = this.imageUploadService.validateImageFile(result.file, this.maxSizeMB);
      if (!validation.valid) {
        this.showToastMessage('error', validation.error!);
        return;
      }

      this.selectedFile = result.file;
      if (result.dataUrl) {
        this.currentImageUrl = result.dataUrl;
      }
    }
  }

  onFileRemoved(file: File): void {
    this.selectedFile = null;
    this.loadCurrentImage();
  }

  async uploadImage(): Promise<void> {
    if (!this.selectedFile) {
      this.showToastMessage('warning', 'Please select an image first');
      return;
    }

    if (!this.entityId && this.uploadType !== 'theme-logo') {
      this.showToastMessage('error', 'Entity ID is required');
      return;
    }

    this.isUploading = true;
    this.uploadProgress = 0;

    try {
      let uploadObservable: any;

      switch (this.uploadType) {
        case 'user-avatar':
          uploadObservable = this.imageUploadService.uploadUserAvatar(this.entityId, this.selectedFile);
          break;
        case 'customer-image':
          uploadObservable = this.imageUploadService.uploadCustomerImage(this.entityId, this.selectedFile);
          break;
        case 'supplier-image':
          uploadObservable = this.imageUploadService.uploadSupplierImage(this.entityId, this.selectedFile);
          break;
        case 'product-image':
          uploadObservable = this.imageUploadService.uploadProductImage(this.entityId, this.selectedFile, this.altText, this.isPrimary);
          break;
        case 'category-image':
          uploadObservable = this.imageUploadService.uploadCategoryImage(this.entityId, this.selectedFile);
          break;
        case 'brand-logo':
          uploadObservable = this.imageUploadService.uploadBrandLogo(this.entityId, this.selectedFile);
          break;
        case 'company-logo':
          uploadObservable = this.imageUploadService.uploadCompanyLogo(this.entityId, this.selectedFile);
          break;
        case 'theme-logo':
          uploadObservable = this.imageUploadService.uploadThemeLogo(this.selectedFile);
          break;
        default:
          throw new Error('Invalid upload type');
      }

      const result: UploadResult = await uploadObservable.toPromise();

      if (result && result.success) {
        this.showToastMessage('success', result.message || 'Image uploaded successfully');
        this.uploadSuccess.emit(result);
        this.selectedFile = null;
        if (result.imageUrl) {
          this.currentImageUrl = result.imageUrl;
        }
      } else {
        throw new Error(result?.error || 'Upload failed');
      }

    } catch (error: any) {
      const errorMessage = error.error?.message || error.message || 'Upload failed';
      this.showToastMessage('error', errorMessage);
      this.uploadError.emit(errorMessage);
    } finally {
      this.isUploading = false;
      this.uploadProgress = 0;
    }
  }

  setAltText(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.altText = target?.value || '';
  }

  setIsPrimary(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.isPrimary = target?.checked ?? false;
  }

  setCurrentImage(url: string | null): void {
    this.currentImageUrl = url;
  }

  private showToastMessage(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.toastType = type;
    this.toastMessage = message;
    this.showToast = true;
    setTimeout(() => { this.showToast = false; }, 3000);
  }

  getAcceptTypes(): string {
    return 'image/jpeg,image/jpg,image/png,image/gif,image/webp,image/svg+xml';
  }
}