import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CopilotApiService, CopilotConversationState, CopilotExecuteResponse } from '../../../core/services/copilot-api.service';

@Component({
  selector: 'app-copilot',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './copilot.component.html'
})
export class CopilotComponent {
  input = '';
  result: CopilotExecuteResponse | null = null;
  private state: CopilotConversationState | undefined;
  private pendingConfirm = false;
  loading = false;

  constructor(private copilotApi: CopilotApiService) {}

  submit(confirmed = false): void {
    const message = (this.input || '').trim();
    const payloadInput = confirmed ? 'done' : message;
    if (!payloadInput) return;

    this.loading = true;
    this.result = null;

    this.copilotApi.execute({ input: payloadInput, confirmed, sessionState: this.state }).subscribe({
      next: (res) => {
        this.loading = false;
        this.result = res.data ?? null;
        this.state = this.result?.sessionState;
        this.pendingConfirm = !!this.result?.confirmationMessage;
      },
      error: () => {
        this.loading = false;
        this.result = { success: false, errors: ['Copilot execution failed.'] };
      }
    });
  }

  confirm(): void {
    if (!this.pendingConfirm) return;
    this.input = 'done';
    this.submit(true);
  }
}
