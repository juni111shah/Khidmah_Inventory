import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InventoryApiService } from '../../../core/services/inventory-api.service';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { BadgeComponent } from '../../../shared/components/badge/badge.component';
import { CreateBatchComponent } from '../create-batch/create-batch.component';

@Component({
  selector: 'app-batches-list',
  standalone: true,
  imports: [CommonModule, UnifiedButtonComponent, UnifiedCardComponent, BadgeComponent, CreateBatchComponent],
  templateUrl: './batches-list.component.html'
})
export class BatchesListComponent implements OnInit {
  batches: any[] = [];
  loading: boolean = false;
  showCreateModal: boolean = false;

  constructor(private inventoryApi: InventoryApiService) { }

  ngOnInit(): void {
    this.loadBatches();
  }

  loadBatches() {
    this.loading = true;
    this.inventoryApi.getBatches({}).subscribe((res: any) => {
      this.loading = false;
      if (res.success) {
        this.batches = res.data.items;
      }
    });
  }

  isExpired(date: string): boolean {
    if (!date) return false;
    return new Date(date) < new Date();
  }

  isExpiringSoon(date: string): boolean {
    if (!date) return false;
    const expiry = new Date(date);
    const today = new Date();
    const diff = expiry.getTime() - today.getTime();
    const days = diff / (1000 * 3600 * 24);
    return days > 0 && days < 30;
  }

  getStatusColor(status: string): string {
    switch (status?.toLowerCase()) {
      case 'instock': return 'success';
      case 'recalled': return 'danger';
      case 'expired': return 'warning';
      default: return 'primary';
    }
  }

  recallBatch(batch: any) {
    if (confirm(`Are you sure you want to recall batch ${batch.batchNumber}? This will mark all items as recalled.`)) {
      this.inventoryApi.recallBatch(batch.id, { reason: 'Manual recall' }).subscribe((res: any) => {
        if (res.success) {
          this.loadBatches();
        }
      });
    }
  }

  openCreateModal() {
    this.showCreateModal = true;
  }
}
