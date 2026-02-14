import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { AutomationApiService } from '../../../core/services/automation-api.service';
import { AutomationRule, ConditionType, ActionType } from '../../../core/models/automation.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';

@Component({
  selector: 'app-automation-builder',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ToastComponent, UnifiedCardComponent, UnifiedButtonComponent],
  templateUrl: './automation-builder.component.html'
})
export class AutomationBuilderComponent {
  form: FormGroup;
  saving = false;
  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'info' = 'success';

  conditionTypes: { value: ConditionType; label: string }[] = [
    { value: 'StockBelowThreshold', label: 'Stock below threshold' },
    { value: 'OrderAboveLimit', label: 'Order above limit' },
    { value: 'ItemNearExpiry', label: 'Item near expiry' },
    { value: 'SlowMoving', label: 'Slow moving' },
    { value: 'StockAboveThreshold', label: 'Stock above threshold' },
    { value: 'NewOrder', label: 'New order' }
  ];
  actionTypes: { value: ActionType; label: string }[] = [
    { value: 'CreatePO', label: 'Create purchase order' },
    { value: 'RequireApproval', label: 'Require approval' },
    { value: 'Notify', label: 'Notify' },
    { value: 'DiscountSuggestion', label: 'Discount suggestion' },
    { value: 'CreateAlert', label: 'Create alert' },
    { value: 'SendEmail', label: 'Send email' }
  ];

  constructor(
    private fb: FormBuilder,
    private automationApi: AutomationApiService,
    private router: Router
  ) {
    this.form = this.fb.group({
      name: [''],
      description: [''],
      isActive: [true],
      conditionType: ['StockBelowThreshold'],
      conditionParam: ['10'],
      actionType: ['CreatePO'],
      actionParam: [''],
      priority: [1]
    });
  }

  save(): void {
    const v = this.form.value;
    const rule: Partial<AutomationRule> = {
      name: v.name,
      description: v.description,
      isActive: v.isActive,
      condition: { type: v.conditionType, params: { threshold: +v.conditionParam || 10 } },
      action: { type: v.actionType, params: v.actionParam ? { value: v.actionParam } : {} },
      priority: +v.priority || 1
    };
    this.saving = true;
    this.automationApi.createRule(rule).subscribe({
      next: (res: ApiResponse<AutomationRule>) => {
        this.saving = false;
        if (res.success) {
          this.toastMessage = 'Rule created';
          this.toastType = 'success';
          this.showToast = true;
          this.router.navigate(['/automation']);
        } else this.showToastMsg(res.message || 'Failed', 'error');
      },
      error: () => {
        this.saving = false;
        this.automationApi.createRuleMock(rule).subscribe(mockRes => {
          this.toastMessage = 'Rule created (mock)';
          this.showToast = true;
          this.router.navigate(['/automation']);
        });
      }
    });
  }

  showToastMsg(msg: string, type: 'success' | 'error' | 'info'): void {
    this.toastMessage = msg;
    this.toastType = type;
    this.showToast = true;
  }
}
