import { Component, OnInit, OnDestroy, HostListener, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { SearchApiService } from '../../../core/services/search-api.service';
import { SearchOverlayService } from '../../../core/services/search-overlay.service';
import {
  GlobalSearchResultDto,
  GlobalSearchItemDto,
  GlobalSearchQuery
} from '../../../core/models/search.model';
import { ApiResponse } from '../../../core/models/api-response.model';

const RECENT_KEY = 'global_search_recent';
const MAX_RECENT = 8;

@Component({
  selector: 'app-search-overlay',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './search-overlay.component.html',
  styleUrls: ['./search-overlay.component.scss']
})
export class SearchOverlayComponent implements OnInit, OnDestroy {
  @ViewChild('searchInput') searchInput!: ElementRef<HTMLInputElement>;

  readonly sectionKeys: (keyof GlobalSearchResultDto)[] = ['products', 'customers', 'suppliers', 'purchaseOrders', 'salesOrders'];

  visible = false;
  searchTerm = '';
  loading = false;
  result: GlobalSearchResultDto | null = null;
  recentSearches: string[] = [];
  private search$ = new Subject<string>();
  private subs = new Subscription();

  constructor(
    private searchApi: SearchApiService,
    private searchOverlay: SearchOverlayService,
    private router: Router
  ) {}

  ngOnInit(): void {
    try {
      const saved = localStorage.getItem(RECENT_KEY);
      if (saved) this.recentSearches = JSON.parse(saved);
    } catch {}
    this.subs.add(
      this.searchOverlay.openStream.subscribe(show => {
        this.visible = show;
        if (show) {
          this.searchTerm = '';
          this.result = null;
          setTimeout(() => this.searchInput?.nativeElement?.focus(), 100);
        }
      })
    );
    this.subs.add(
      this.search$.pipe(
        debounceTime(280),
        distinctUntilChanged(),
        switchMap(term => {
          this.loading = true;
          return this.searchApi.globalSearch({ searchTerm: term, limitPerGroup: 10 });
        })
      ).subscribe({
        next: (res: ApiResponse<GlobalSearchResultDto>) => {
          this.loading = false;
          this.result = res.success && res.data ? res.data : null;
        },
        error: () => { this.loading = false; this.result = null; }
      })
    );
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  @HostListener('document:keydown', ['$event'])
  onKeyDown(event: KeyboardEvent): void {
    if (event.key === 'Escape') {
      this.close();
      return;
    }
    if (!this.visible) return;
    if (event.key === 'Enter') {
      const first = this.getFirstItem();
      if (first) {
        this.navigate(first);
        this.close();
      }
      event.preventDefault();
    }
  }

  close(): void {
    this.searchOverlay.close();
  }

  onInput(): void {
    const term = this.searchTerm.trim();
    if (term.length >= 2) this.search$.next(term);
    else this.result = null;
  }

  getFirstItem(): GlobalSearchItemDto | null {
    if (!this.result) return null;
    if (this.result.products?.length) return this.result.products[0];
    if (this.result.customers?.length) return this.result.customers[0];
    if (this.result.suppliers?.length) return this.result.suppliers[0];
    if (this.result.purchaseOrders?.length) return this.result.purchaseOrders[0];
    if (this.result.salesOrders?.length) return this.result.salesOrders[0];
    return null;
  }

  navigate(item: GlobalSearchItemDto): void {
    if (this.searchTerm.trim()) this.addRecent(this.searchTerm.trim());
    this.router.navigateByUrl(item.route);
    this.close();
  }

  selectRecent(term: string): void {
    this.searchTerm = term;
    this.onInput();
  }

  private addRecent(term: string): void {
    if (!term) return;
    this.recentSearches = [term, ...this.recentSearches.filter(t => t !== term)].slice(0, MAX_RECENT);
    try {
      localStorage.setItem(RECENT_KEY, JSON.stringify(this.recentSearches));
    } catch {}
  }

  getSectionLabel(key: keyof GlobalSearchResultDto): string {
    const labels: Record<string, string> = {
      products: 'Products',
      customers: 'Customers',
      suppliers: 'Suppliers',
      purchaseOrders: 'Purchase Orders',
      salesOrders: 'Sales Orders'
    };
    return labels[key] || key;
  }

  getSectionIcon(key: keyof GlobalSearchResultDto): string {
    const icons: Record<string, string> = {
      products: 'bi-box-seam',
      customers: 'bi-person',
      suppliers: 'bi-truck',
      purchaseOrders: 'bi-cart-check',
      salesOrders: 'bi-bag-check'
    };
    return icons[key] || 'bi-file-text';
  }

  hasAnyResults(): boolean {
    if (!this.result) return false;
    return (
      (this.result.products?.length ?? 0) > 0 ||
      (this.result.customers?.length ?? 0) > 0 ||
      (this.result.suppliers?.length ?? 0) > 0 ||
      (this.result.purchaseOrders?.length ?? 0) > 0 ||
      (this.result.salesOrders?.length ?? 0) > 0
    );
  }
}
