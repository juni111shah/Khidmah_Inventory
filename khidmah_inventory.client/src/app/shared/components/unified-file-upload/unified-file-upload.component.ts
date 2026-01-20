import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UnifiedButtonComponent } from '../unified-button/unified-button.component';

export interface FileUploadResult {
  file: File;
  dataUrl?: string;
  error?: string;
}

@Component({
  selector: 'app-unified-file-upload',
  standalone: true,
  imports: [
    CommonModule,
    UnifiedButtonComponent
  ],
  templateUrl: './unified-file-upload.component.html'
})
export class UnifiedFileUploadComponent {
  @Input() label: string = 'Upload File';
  @Input() accept: string = '*/*';
  @Input() multiple: boolean = false;
  @Input() maxSize: number = 0; // in bytes, 0 = no limit
  @Input() disabled: boolean = false;
  @Input() required: boolean = false;
  @Input() error: string = '';
  @Input() hint: string = '';
  @Input() customClass: string = '';
  @Input() buttonVariant: 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info' = 'primary';
  @Input() buttonSize: 'xs' | 'sm' | 'md' | 'lg' | 'xl' = 'md';
  @Input() showPreview: boolean = true;
  @Input() previewType: 'image' | 'file' = 'image';

  @Output() fileSelected = new EventEmitter<FileUploadResult[]>();
  @Output() fileRemoved = new EventEmitter<File>();

  selectedFiles: File[] = [];
  uploadProgress: number = 0;
  isUploading: boolean = false;
  previewUrls: string[] = [];

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const files = Array.from(input.files);
      const results: FileUploadResult[] = [];
      
      files.forEach(file => {
        // Check file size
        if (this.maxSize > 0 && file.size > this.maxSize) {
          results.push({
            file,
            error: `File size exceeds maximum allowed size of ${this.formatFileSize(this.maxSize)}`
          });
          return;
        }
        
        // Generate preview for images
        if (this.showPreview && this.previewType === 'image' && file.type.startsWith('image/')) {
          const reader = new FileReader();
          reader.onload = (e: any) => {
            this.previewUrls.push(e.target.result);
          };
          reader.readAsDataURL(file);
        }
        
        results.push({ file });
        this.selectedFiles.push(file);
      });
      
      this.fileSelected.emit(results);
    }
  }

  removeFile(index: number): void {
    const file = this.selectedFiles[index];
    this.selectedFiles.splice(index, 1);
    if (this.previewUrls[index]) {
      this.previewUrls.splice(index, 1);
    }
    this.fileRemoved.emit(file);
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  }

  get fileUploadClasses(): string {
    const classes: string[] = ['unified-file-upload'];
    
    if (this.customClass) {
      classes.push(this.customClass);
    }
    
    return classes.join(' ');
  }
}

