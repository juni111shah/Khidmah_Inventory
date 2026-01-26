import { Injectable } from '@angular/core';
import { HttpClient, HttpEvent, HttpEventType } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { ApiConfigService } from './api-config.service';

export interface ImageUploadResult {
  success: boolean;
  imageUrl?: string;
  message?: string;
  error?: string;
}

export interface UploadProgress {
  loaded: number;
  total: number;
  percentage: number;
}

@Injectable({
  providedIn: 'root'
})
export class ImageUploadService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('');
  }

  // User Avatar Upload
  uploadUserAvatar(userId: string, file: File): Observable<ImageUploadResult> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<ImageUploadResult>(`${this.apiUrl}/users/${userId}/avatar`, formData);
  }

  uploadUserAvatarWithProgress(userId: string, file: File): Observable<UploadProgress | ImageUploadResult> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post(`${this.apiUrl}/users/${userId}/avatar`, formData, {
      reportProgress: true,
      observe: 'events'
    }).pipe(
      map(event => {
        switch (event.type) {
          case HttpEventType.UploadProgress:
            const progress = event.total ? Math.round(100 * event.loaded / event.total) : 0;
            return {
              loaded: event.loaded,
              total: event.total || 0,
              percentage: progress
            } as UploadProgress;

          case HttpEventType.Response:
            return event.body as ImageUploadResult;

          default:
            return {} as UploadProgress;
        }
      })
    );
  }

  // Customer Image Upload
  uploadCustomerImage(customerId: string, file: File): Observable<ImageUploadResult> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<ImageUploadResult>(`${this.apiUrl}/customers/${customerId}/image`, formData);
  }

  // Supplier Image Upload
  uploadSupplierImage(supplierId: string, file: File): Observable<ImageUploadResult> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<ImageUploadResult>(`${this.apiUrl}/suppliers/${supplierId}/image`, formData);
  }

  // Product Image Upload
  uploadProductImage(productId: string, file: File, altText?: string, isPrimary: boolean = false): Observable<ImageUploadResult> {
    const formData = new FormData();
    formData.append('file', file);
    if (altText) {
      formData.append('altText', altText);
    }
    formData.append('isPrimary', isPrimary.toString());

    return this.http.post<ImageUploadResult>(`${this.apiUrl}/products/${productId}/image`, formData);
  }

  // Category Image Upload
  uploadCategoryImage(categoryId: string, file: File): Observable<ImageUploadResult> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<ImageUploadResult>(`${this.apiUrl}/categories/${categoryId}/image`, formData);
  }

  // Brand Logo Upload
  uploadBrandLogo(brandId: string, file: File): Observable<ImageUploadResult> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<ImageUploadResult>(`${this.apiUrl}/brands/${brandId}/logo`, formData);
  }

  // Company Logo Upload
  uploadCompanyLogo(companyId: string, file: File): Observable<ImageUploadResult> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<ImageUploadResult>(`${this.apiUrl}/companies/${companyId}/logo`, formData);
  }

  // Theme Logo Upload (existing)
  uploadThemeLogo(file: File): Observable<ImageUploadResult> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<ImageUploadResult>(`${this.apiUrl.replace('/api', '')}/api/theme/logo`, formData);
  }

  // Utility methods
  validateImageFile(file: File, maxSizeInMB: number = 5): { valid: boolean; error?: string } {
    // Check file type
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp', 'image/svg+xml'];
    if (!allowedTypes.includes(file.type)) {
      return {
        valid: false,
        error: 'Please select a valid image file (JPEG, PNG, GIF, WebP, or SVG)'
      };
    }

    // Check file size
    const maxSizeInBytes = maxSizeInMB * 1024 * 1024;
    if (file.size > maxSizeInBytes) {
      return {
        valid: false,
        error: `File size must be less than ${maxSizeInMB}MB`
      };
    }

    return { valid: true };
  }

  createPreview(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => resolve(reader.result as string);
      reader.onerror = reject;
      reader.readAsDataURL(file);
    });
  }
}