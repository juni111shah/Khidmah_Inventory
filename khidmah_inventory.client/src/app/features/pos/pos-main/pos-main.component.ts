import { Component, OnInit, OnDestroy } from '@angular/core';
import { PosApiService, CreatePosSaleCommand } from '../../../core/services/pos-api.service';
import { ProductApiService } from '../../../core/services/product-api.service';
import { CustomerApiService } from '../../../core/services/customer-api.service';
import { CategoryApiService } from '../../../core/services/category-api.service';
import { WarehouseApiService } from '../../../core/services/warehouse-api.service';
import { Router } from '@angular/router';
import { SearchMode } from '../../../core/models/user.model';

interface CartItem {
  productId: string;
  productName: string;
  quantity: number;
  unitPrice: number;
}

@Component({
  selector: 'app-pos-main',
  templateUrl: './pos-main.component.html',
  styleUrls: ['./pos-main.component.scss']
})
export class PosMainComponent implements OnInit {
  activeSessionId: string | null = null;
  openingBalance: number = 0;

  products: any[] = [];
  filteredProducts: any[] = [];
  categories: any[] = [];
  customers: any[] = [];

  searchQuery: string = '';
  selectedCategory: string | null = null;
  selectedCustomerId: string = '';
  selectedWarehouseId: string = '';
  warehouses: any[] = [];

  cart: CartItem[] = [];

  showCheckoutModal: boolean = false;
  paymentMethod: string = 'Cash';
  amountPaid: number = 0;

  loading: boolean = false;
  barcodeValue: string = '';

  showReceiptModal: boolean = false;
  lastSale: any = null;

  constructor(
    private posApi: PosApiService,
    private productApi: ProductApiService,
    private customerApi: CustomerApiService,
    private categoryApi: CategoryApiService,
    private warehouseApi: WarehouseApiService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.checkActiveSession();
    this.loadInitialData();
  }

  checkActiveSession() {
    this.posApi.getActiveSession().subscribe({
      next: (res: any) => {
        const isSuccess = res.success === true || res.Success === true || res.succeeded === true || res.Succeeded === true;
        if (isSuccess && (res.data || res.Data)) {
          this.activeSessionId = res.data || res.Data;
        }
      },
      error: (err) => {
        // 404 is expected if no session is found, so we just ignore it
        if (err.status !== 404) {
          console.error('Error checking active session:', err);
        }
      }
    });
  }

  loadInitialData() {
    this.loading = true;

    // Load Categories
    this.categoryApi.getCategories({}).subscribe({
      next: (res: any) => {
        if (res.success && res.data) this.categories = res.data.items;
      }
    });

    // Load Customers
    this.customerApi.getCustomers({}).subscribe({
      next: (res: any) => {
        if (res.success && res.data) {
          this.customers = res.data.items;
          if (this.customers.length > 0) {
            this.selectedCustomerId = this.customers[0].id;
          }
        }
      }
    });

    // Load Products
    this.productApi.getProducts({}).subscribe({
      next: (res: any) => {
        this.loading = false;
        if (res.success && res.data) {
          this.products = res.data.items;
          this.filteredProducts = [...this.products];
        }
      },
      error: () => this.loading = false
    });

    // Load Warehouses
    this.warehouseApi.getWarehouses({}).subscribe({
      next: (res: any) => {
        if (res.success && res.data) {
          this.warehouses = res.data.items;
          if (this.warehouses.length > 0) {
            this.selectedWarehouseId = this.warehouses[0].id;
          }
        }
      }
    });
  }

  openSession() {
    if (this.openingBalance < 0) {
        alert('Opening balance cannot be negative');
        return;
    }

    console.log('Opening POS session with balance:', this.openingBalance);
    this.posApi.openSession(this.openingBalance).subscribe({
      next: (res: any) => {
        console.log('Open session response:', res);
        // Robust check for success (handle different casings and structures)
        const isSuccess = res.success === true || res.Success === true || res.succeeded === true || res.Succeeded === true;
        const message = res.message || res.Message || 'Session opened';

        if (isSuccess) {
          this.activeSessionId = res.data || res.Data;
          alert('Session opened successfully');
        } else {
          const errors = res.errors || res.Errors || [];
          const errorMsg = errors.length > 0 ? errors.join(', ') : message;
          alert('Failed to open session: ' + errorMsg);
        }
      },
      error: (err) => {
        console.error('Open session error:', err);
        const errorBody = err.error || {};
        const message = errorBody.message || errorBody.Message || errorBody.errors?.join(', ') || err.message || 'Failed to open session';
        alert('Error: ' + message);
      }
    });
  }

  closeSession() {
    if (confirm('Are you sure you want to close this session?')) {
        this.posApi.closeSession(this.activeSessionId!, 0).subscribe({
          next: (res: any) => {
            const isSuccess = res.success === true || res.Success === true || res.succeeded === true || res.Succeeded === true;
            if (isSuccess) {
              this.activeSessionId = null;
              alert('Session closed successfully');
            }
          },
          error: (err) => {
            console.error('Close session error:', err);
            const errorBody = err.error || {};
            const message = errorBody.message || errorBody.Message || errorBody.errors?.join(', ') || 'Unknown error';
            alert('Error closing session: ' + message);
          }
        });
    }
  }

  filterProducts() {
    this.filteredProducts = this.products.filter(p => {
      const matchSearch = p.name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
                          p.sku.toLowerCase().includes(this.searchQuery.toLowerCase());
      const matchCategory = this.selectedCategory ? p.categoryId === this.selectedCategory : true;
      return matchSearch && matchCategory;
    });
  }

  onBarcodeScanned() {
    if (!this.barcodeValue) return;

    const query = {
      filterRequest: {
        pagination: { pageNo: 1, pageSize: 1 },
        search: {
          term: this.barcodeValue,
          searchFields: ['barcode', 'sku'],
          mode: SearchMode.ExactMatch,
          isCaseSensitive: false
        }
      }
    };

    this.productApi.getProducts(query as any).subscribe((res: any) => {
      if (res.success && res.data && res.data.items.length > 0) {
          this.addToCart(res.data.items[0]);
          this.barcodeValue = ''; // Clear for next scan
      } else {
        // Fallback to fuzzy search if exact match fails
          const fuzzyQuery = { ...query };
          fuzzyQuery.filterRequest.search.mode = SearchMode.Contains;
          this.productApi.getProducts(fuzzyQuery as any).subscribe((res2: any) => {
            if (res2.success && res2.data && res2.data.items.length > 0) {
              this.addToCart(res2.data.items[0]);
              this.barcodeValue = '';
            }
          });
      }
    });
  }

  selectCategory(catId: string | null) {
    this.selectedCategory = catId;
    this.filterProducts();
  }

  addToCart(product: any) {
    const existing = this.cart.find(item => item.productId === product.id);
    if (existing) {
      existing.quantity++;
    } else {
      this.cart.push({
        productId: product.id,
        productName: product.name,
        quantity: 1,
        unitPrice: product.salePrice
      });
    }
  }

  updateQty(item: CartItem, delta: number) {
    item.quantity += delta;
    if (item.quantity <= 0) {
      this.cart = this.cart.filter(i => i !== item);
    }
  }

  clearCart() {
    this.cart = [];
  }

  get subtotal() {
    return this.cart.reduce((sum, item) => sum + (item.unitPrice * item.quantity), 0);
  }

  get tax() {
    return this.subtotal * 0.15; // 15% VAT
  }

  get total() {
    return this.subtotal + this.tax;
  }

  checkout() {
    this.amountPaid = this.total;
    this.showCheckoutModal = true;
  }

  processSale() {
    const command: CreatePosSaleCommand = {
      posSessionId: this.activeSessionId!,
      customerId: this.selectedCustomerId,
      paymentMethod: this.paymentMethod,
      amountPaid: this.amountPaid,
      warehouseId: this.selectedWarehouseId,
      items: this.cart.map(i => ({
        productId: i.productId,
        quantity: i.quantity,
        unitPrice: i.unitPrice,
        discountAmount: 0
      }))
    };

    this.posApi.createSale(command).subscribe({
      next: (res: any) => {
        const isSuccess = res.success === true || res.Success === true || res.succeeded === true || res.Succeeded === true;
        if (isSuccess) {
          this.lastSale = {
              id: res.data || res.Data,
              items: [...this.cart],
              total: this.total,
              subtotal: this.subtotal,
              tax: this.tax,
              amountPaid: this.amountPaid,
              change: this.amountPaid - this.total,
              date: new Date(),
              customer: this.customers.find(c => c.id === this.selectedCustomerId)?.name || 'Walk-in Customer',
              paymentMethod: this.paymentMethod
          };
          this.clearCart();
          this.showCheckoutModal = false;
          this.showReceiptModal = true;
          this.loadInitialData(); // Refresh stock
        } else {
          alert('Error: ' + (res.message || res.Message || 'Failed to process sale'));
        }
      },
      error: (err) => {
        console.error('Process sale error:', err);
        const errorBody = err.error || {};
        const message = errorBody.message || errorBody.Message || errorBody.errors?.join(', ') || 'Failed to process sale';
        alert('Error: ' + message);
      }
    });
  }

  printReceipt() {
    window.print();
  }
}
