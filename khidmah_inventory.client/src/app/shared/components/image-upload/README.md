# Image Upload Component

A comprehensive image upload component that handles all types of image uploads throughout the Khidmah Inventory application.

## Features

- ✅ Multiple upload types (avatars, logos, product images, etc.)
- ✅ File validation (type, size, format)
- ✅ Image preview
- ✅ Progress tracking
- ✅ Error handling
- ✅ Current image display and removal
- ✅ Responsive design

## Supported Upload Types

| Type | Description | Endpoint | Permissions |
|------|-------------|----------|-------------|
| `user-avatar` | User profile pictures | `POST /api/users/{id}/avatar` | Users:Update |
| `customer-image` | Customer profile images | `POST /api/customers/{id}/image` | Customers:Update |
| `supplier-image` | Supplier profile images | `POST /api/suppliers/{id}/image` | Suppliers:Update |
| `product-image` | Product gallery images | `POST /api/products/{id}/image` | Products:Update |
| `category-image` | Category display images | `POST /api/categories/{id}/image` | Categories:Update |
| `brand-logo` | Brand logo images | `POST /api/brands/{id}/logo` | Brands:Update |
| `company-logo` | Company logo | `POST /api/companies/{id}/logo` | Companies:Update |
| `theme-logo` | Application theme logo | `POST /api/theme/logo` | Theme:Update |

## Usage Examples

### Basic User Avatar Upload

```html
<app-image-upload
  uploadType="user-avatar"
  [entityId]="userId"
  label="Upload Avatar"
  hint="Select a profile picture (max 2MB)"
  [maxSizeMB]="2"
  (uploadSuccess)="onUploadSuccess($event)"
  (uploadError)="onUploadError($event)">
</app-image-upload>
```

### Product Image Upload with Options

```html
<app-image-upload
  uploadType="product-image"
  [entityId]="productId"
  label="Add Product Image"
  hint="Upload product photos (JPEG, PNG, max 5MB)"
  [maxSizeMB]="5"
  (uploadSuccess)="onProductImageUploaded($event)">
</app-image-upload>
```

### Brand Logo Upload

```html
<app-image-upload
  uploadType="brand-logo"
  [entityId]="brandId"
  label="Upload Brand Logo"
  hint="Upload brand logo (PNG, SVG recommended)"
  (uploadSuccess)="onLogoUploaded($event)">
</app-image-upload>
```

## Component Properties

### Input Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `uploadType` | `ImageUploadType` | `'user-avatar'` | Type of upload (determines endpoint) |
| `entityId` | `string` | `''` | ID of the entity being updated |
| `label` | `string` | `'Upload Image'` | Label text for the upload area |
| `hint` | `string` | `''` | Helper text below the upload area |
| `maxSizeMB` | `number` | `5` | Maximum file size in MB |
| `showProgress` | `boolean` | `true` | Show upload progress bar |
| `allowMultiple` | `boolean` | `false` | Allow multiple file selection |

### Output Events

| Event | Payload | Description |
|-------|---------|-------------|
| `uploadSuccess` | `ImageUploadResult` | Fired when upload completes successfully |
| `uploadError` | `string` | Fired when upload fails |

## ImageUploadResult Interface

```typescript
interface ImageUploadResult {
  success: boolean;
  imageUrl?: string;
  message?: string;
  error?: string;
}
```

## File Validation

The component automatically validates:

- **File Types**: JPEG, PNG, GIF, WebP, SVG
- **File Size**: Configurable maximum (default 5MB)
- **Image Format**: Only image files accepted

## Integration Examples

### In User Profile Component

```typescript
export class UserProfileComponent {
  user: User | null = null;

  onAvatarUploadSuccess(result: ImageUploadResult): void {
    if (this.user && result.imageUrl) {
      this.user.avatarUrl = result.imageUrl;
      this.showToast('Avatar updated successfully');
    }
  }

  onAvatarUploadError(error: string): void {
    this.showToast(error, 'error');
  }
}
```

### In Product Form Component

```html
<div class="product-images">
  <h5>Product Images</h5>
  <app-image-upload
    uploadType="product-image"
    [entityId]="productId"
    label="Add Product Image"
    [allowMultiple]="true"
    (uploadSuccess)="onImageAdded($event)">
  </app-image-upload>

  <div class="existing-images">
    <div *ngFor="let image of productImages" class="image-item">
      <img [src]="image.imageUrl" [alt]="image.altText" />
      <button (click)="removeImage(image.id)">Remove</button>
    </div>
  </div>
</div>
```

## Backend Integration

The component uses the `ImageUploadService` which handles:

- File validation
- HTTP requests to appropriate endpoints
- Progress tracking
- Error handling
- Response parsing

## Error Handling

The component provides comprehensive error handling:

- File validation errors
- Network errors
- Server errors
- Permission errors

All errors are emitted via the `uploadError` event and can be displayed to users.

## Styling

The component uses Bootstrap classes and custom CSS for consistent styling across the application. The upload area is fully responsive and works on all device sizes.

## Dependencies

- Angular CommonModule
- Angular FormsModule
- UnifiedFileUploadComponent
- ImageUploadService
- ToastComponent
- LoadingSpinnerComponent