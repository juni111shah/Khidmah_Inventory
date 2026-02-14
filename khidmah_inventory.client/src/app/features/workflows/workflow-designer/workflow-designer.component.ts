import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { WorkflowApiService } from '../../../core/services/workflow-api.service';
import { ApiResponse } from '../../../core/models/api-response.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';

@Component({
  selector: 'app-workflow-designer',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ToastComponent, UnifiedCardComponent, UnifiedButtonComponent],
  templateUrl: './workflow-designer.component.html'
})
export class WorkflowDesignerComponent {
  form: FormGroup;
  saving = false;
  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'info' = 'success';

  entityTypes = ['PurchaseOrder', 'SalesOrder', 'Product', 'StockTransaction'];

  constructor(
    private fb: FormBuilder,
    private workflowApi: WorkflowApiService,
    private router: Router
  ) {
    this.form = this.fb.group({
      name: [''],
      description: [''],
      entityType: ['PurchaseOrder'],
      steps: ['[{"stepId":"step1","name":"Submit","order":1},{"stepId":"step2","name":"Approve","order":2}]']
    });
  }

  save(): void {
    const def = this.form.get('steps')?.value;
    try {
      JSON.parse(def);
    } catch {
      this.showToastMsg('Invalid JSON in steps', 'error');
      return;
    }
    this.saving = true;
    this.workflowApi.create({
      name: this.form.get('name')?.value,
      description: this.form.get('description')?.value,
      entityType: this.form.get('entityType')?.value,
      workflowDefinition: this.form.get('steps')?.value
    }).subscribe({
      next: (res: ApiResponse<any>) => {
        this.saving = false;
        if (res.success) {
          this.showToastMsg('Workflow created', 'success');
          this.router.navigate(['/workflows']);
        } else this.showToastMsg(res.message || 'Failed', 'error');
      },
      error: () => { this.saving = false; this.showToastMsg('Failed', 'error'); }
    });
  }

  showToastMsg(msg: string, type: 'success' | 'error' | 'info'): void {
    this.toastMessage = msg;
    this.toastType = type;
    this.showToast = true;
  }
}
