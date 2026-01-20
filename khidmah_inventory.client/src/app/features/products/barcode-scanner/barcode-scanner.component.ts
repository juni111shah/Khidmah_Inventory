import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ProductApiService } from '../../../core/services/product-api.service';
import { Product } from '../../../core/models/product.model';
import { SearchMode } from '../../../core/models/user.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { IconComponent } from '../../../shared/components/icon/icon.component';

@Component({
  selector: 'app-barcode-scanner',
  standalone: true,
  imports: [CommonModule, FormsModule, ToastComponent, LoadingSpinnerComponent, IconComponent],
  templateUrl: './barcode-scanner.component.html'
})
export class BarcodeScannerComponent implements OnInit {
  @ViewChild('barcodeInput', { static: false }) barcodeInput!: ElementRef<HTMLInputElement>;
  
  scannedProduct: Product | null = null;
  loading = false;
  barcodeInputValue = '';

  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';

  constructor(
    private productApiService: ProductApiService,
    public router: Router
  ) {}

  ngOnInit(): void {
    // Focus on barcode input when component loads
    setTimeout(() => {
      if (this.barcodeInput) {
        this.barcodeInput.nativeElement.focus();
      }
    }, 100);
  }

  onBarcodeEnter(): void {
    if (!this.barcodeInputValue.trim()) {
      return;
    }

    this.searchByBarcode(this.barcodeInputValue.trim());
  }

  searchByBarcode(barcode: string): void {
    this.loading = true;
    this.scannedProduct = null;

    // Search products by barcode
    const query = {
      filterRequest: {
        pagination: { pageNo: 1, pageSize: 1 },
        search: {
          term: barcode,
          searchFields: ['barcode', 'sku'],
          mode: SearchMode.Contains,
          isCaseSensitive: false
        }
      }
    };

    this.productApiService.getProducts(query).subscribe({
      next: (response) => {
        if (response.success && response.data && response.data.items.length > 0) {
          this.scannedProduct = response.data.items[0];
          this.showToastMessage('success', 'Product found!');
          this.barcodeInputValue = '';
        } else {
          this.showToastMessage('warning', 'Product not found with this barcode/SKU');
        }
        this.loading = false;
        // Refocus input for next scan
        setTimeout(() => {
          if (this.barcodeInput) {
            this.barcodeInput.nativeElement.focus();
          }
        }, 100);
      },
      error: () => {
        this.showToastMessage('error', 'Error searching for product');
        this.loading = false;
        setTimeout(() => {
          if (this.barcodeInput) {
            this.barcodeInput.nativeElement.focus();
          }
        }, 100);
      }
    });
  }

  viewProduct(): void {
    if (this.scannedProduct) {
      this.router.navigate(['/products', this.scannedProduct.id]);
    }
  }

  editProduct(): void {
    if (this.scannedProduct) {
      this.router.navigate(['/products', this.scannedProduct.id, 'edit']);
    }
  }

  showToastMessage(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.toastType = type;
    this.toastMessage = message;
    this.showToast = true;
    setTimeout(() => { this.showToast = false; }, 3000);
  }
}


