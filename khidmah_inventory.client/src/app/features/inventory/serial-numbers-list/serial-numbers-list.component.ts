import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InventoryApiService } from '../../../core/services/inventory-api.service';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { BadgeComponent } from '../../../shared/components/badge/badge.component';
import { AssignSerialNumbersComponent } from '../assign-serial-numbers/assign-serial-numbers.component';

@Component({
  selector: 'app-serial-numbers-list',
  standalone: true,
  imports: [CommonModule, UnifiedButtonComponent, UnifiedCardComponent, BadgeComponent, AssignSerialNumbersComponent],
  templateUrl: './serial-numbers-list.component.html'
})
export class SerialNumbersListComponent implements OnInit {
  serialNumbers: any[] = [];
  loading: boolean = false;
  showAssignModal: boolean = false;

  constructor(private inventoryApi: InventoryApiService) { }

  ngOnInit(): void {
    this.loadSerialNumbers();
  }

  loadSerialNumbers() {
    this.loading = true;
    this.inventoryApi.getSerialNumbers({}).subscribe((res: any) => {
      this.loading = false;
      if (res.success) {
        this.serialNumbers = res.data.items;
      }
    });
  }

  getStatusColor(status: string): string {
    switch (status?.toLowerCase()) {
      case 'instock': return 'success';
      case 'sold': return 'primary';
      case 'returned': return 'warning';
      case 'damaged': return 'danger';
      default: return 'secondary';
    }
  }

  openCreateModal() {
     this.showAssignModal = true;
  }
}
