import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './pagination.component.html'
})
export class PaginationComponent implements OnInit {
  @Input() currentPage: number = 1;
  @Input() totalPages: number = 1;
  @Input() pageSize: number = 10;
  @Input() totalItems: number = 0;
  @Input() showFirstLast: boolean = true;
  @Input() maxVisiblePages: number = 5;
  @Input() hasPreviousPage?: boolean;
  @Input() hasNextPage?: boolean;
  @Input() pageSizeOptions: number[] = [5, 10, 25, 50, 100];
  @Input() showPageSizeOptions: boolean = true;

  @Output() pageChange = new EventEmitter<number>();
  @Output() pageSizeChange = new EventEmitter<number>();

  ngOnInit(): void {
    if (!this.pageSizeOptions.includes(this.pageSize)) {
      this.pageSizeOptions = [...this.pageSizeOptions, this.pageSize].sort((a, b) => a - b);
    }
  }

  onPageSizeChange(size: any): void {
    const newSize = Number(size);
    this.pageSize = newSize;
    this.pageSizeChange.emit(newSize);
  }

  get pages(): number[] {
    const pages: number[] = [];
    const start = Math.max(1, this.currentPage - Math.floor(this.maxVisiblePages / 2));
    const end = Math.min(this.totalPages, start + this.maxVisiblePages - 1);
    
    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    
    return pages;
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages && page !== this.currentPage) {
      this.currentPage = page;
      this.pageChange.emit(page);
    }
  }

  goToFirst(): void {
    this.goToPage(1);
  }

  goToLast(): void {
    this.goToPage(this.totalPages);
  }

  goToPrevious(): void {
    if (this.canGoPrevious()) {
      this.goToPage(this.currentPage - 1);
    }
  }

  goToNext(): void {
    if (this.canGoNext()) {
      this.goToPage(this.currentPage + 1);
    }
  }

  canGoPrevious(): boolean {
    if (this.hasPreviousPage !== undefined) {
      return this.hasPreviousPage;
    }
    return this.currentPage > 1;
  }

  canGoNext(): boolean {
    if (this.hasNextPage !== undefined) {
      return this.hasNextPage;
    }
    return this.currentPage < this.totalPages;
  }

  get startItem(): number {
    return (this.currentPage - 1) * this.pageSize + 1;
  }

  get endItem(): number {
    return Math.min(this.currentPage * this.pageSize, this.totalItems);
  }
}

