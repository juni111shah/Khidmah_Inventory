import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { WorkflowApiService } from '../../../core/services/workflow-api.service';
import { WorkflowInstance } from '../../../core/models/workflow.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonDetailHeaderComponent } from '../../../shared/components/skeleton-detail-header/skeleton-detail-header.component';
import { SkeletonListComponent } from '../../../shared/components/skeleton-list/skeleton-list.component';

@Component({
  selector: 'app-approval-inbox',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ToastComponent,
    LoadingSpinnerComponent,
    UnifiedCardComponent,
    UnifiedButtonComponent,
    ContentLoaderComponent,
    SkeletonDetailHeaderComponent,
    SkeletonListComponent
  ],
  templateUrl: './approval-inbox.component.html'
})
export class ApprovalInboxComponent implements OnInit {
  loading = true;
  pending: WorkflowInstance[] = [];
  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'info' = 'success';
  approvingId: string | null = null;
  comment = '';

  constructor(private workflowApi: WorkflowApiService) {}

  ngOnInit(): void {
    this.workflowApi.getPendingApprovals().subscribe({
      next: (res: ApiResponse<WorkflowInstance[]>) => {
        this.loading = false;
        if (res.success && res.data) this.pending = res.data;
        else this.pending = [];
      },
      error: () => { this.loading = false; this.pending = []; }
    });
  }

  approve(id: string): void {
    this.approvingId = id;
    const commentToSend = this.comment;
    this.workflowApi.approveStep(id, { comments: commentToSend }).subscribe({
      next: () => {
        this.approvingId = null;
        this.comment = '';
        this.pending = this.pending.filter(p => p.id !== id);
        this.showToastMsg('Approved', 'success');
      },
      error: (err) => {
        this.approvingId = null;
        this.showToastMsg(err.error?.message || 'Failed', 'error');
      }
    });
  }

  showToastMsg(msg: string, type: 'success' | 'error' | 'info'): void {
    this.toastMessage = msg;
    this.toastType = type;
    this.showToast = true;
  }
}
