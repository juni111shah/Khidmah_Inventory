import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AiAssistantApiService } from '../../../core/services/ai-assistant-api.service';
import { ChatMessage } from '../../../core/models/ai-assistant.model';
import { DrawerComponent } from '../drawer/drawer.component';
import { UnifiedButtonComponent } from '../unified-button/unified-button.component';
import { LoadingSpinnerComponent } from '../loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-ai-assistant-panel',
  standalone: true,
  imports: [CommonModule, FormsModule, DrawerComponent, UnifiedButtonComponent, LoadingSpinnerComponent],
  templateUrl: './ai-assistant-panel.component.html',
  styles: [`
    .ai-assistant-panel { min-height: 0; }
    .ai-panel-messages { min-height: 200px; }
    .ai-panel-message { display: flex; }
    .ai-panel-message-user { justify-content: flex-end; }
    .ai-panel-message-assistant { justify-content: flex-start; }
    .ai-panel-bubble {
      max-width: 88%;
      padding: 0.75rem 1rem;
      border-radius: 1rem;
      box-shadow: 0 1px 3px rgba(0,0,0,0.06);
    }
    .ai-panel-message-user .ai-panel-bubble {
      background: var(--primary-color);
      color: #fff;
      border-bottom-right-radius: 0.25rem;
    }
    .ai-panel-message-assistant .ai-panel-bubble {
      background: var(--background-color);
      color: var(--text-color);
      border: 1px solid var(--border-color);
      border-bottom-left-radius: 0.25rem;
    }
    .ai-panel-bubble-label {
      font-size: 0.7rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.02em;
      margin-bottom: 0.35rem;
      opacity: 0.9;
    }
    .ai-panel-bubble-content {
      font-size: 0.9375rem;
      line-height: 1.45;
      white-space: pre-wrap;
      word-break: break-word;
    }
    .ai-panel-bubble-metrics .badge { font-size: 0.7rem; }
    .ai-panel-bubble-links .btn { font-size: 0.8rem; }
    .ai-panel-footer { flex-shrink: 0; }
    .ai-panel-input-group {
      border-radius: 50px;
      overflow: hidden;
      box-shadow: 0 1px 3px rgba(0,0,0,0.08);
    }
    .ai-panel-input {
      border: 1px solid var(--border-color);
      border-right: none;
      border-radius: 50px 0 0 50px;
      padding-left: 1rem;
    }
    .ai-panel-input:focus {
      border-color: var(--primary-color);
      box-shadow: none;
    }
    .ai-panel-send {
      border-radius: 0 50px 50px 0;
      border: none;
      font-weight: 600;
      transition: opacity 0.2s ease, transform 0.05s ease;
    }
    .ai-panel-send:hover:not(:disabled) { opacity: 0.9; }
    .ai-panel-send:active:not(:disabled) { transform: scale(0.98); }
  `]
})
export class AiAssistantPanelComponent {
  @Input() isOpen = false;
  @Output() isOpenChange = new EventEmitter<boolean>();

  question = '';
  messages: ChatMessage[] = [];
  loading = false;

  constructor(
    private aiAssistantApi: AiAssistantApiService,
    private router: Router
  ) {}

  close(): void {
    this.isOpen = false;
    this.isOpenChange.emit(false);
  }

  send(): void {
    const q = this.question.trim();
    if (!q) return;
    this.messages.push({ id: `u-${Date.now()}`, role: 'user', content: q, timestamp: new Date().toISOString() });
    this.question = '';
    this.loading = true;
    this.aiAssistantApi.ask({ question: q }).subscribe({
      next: res => {
        this.loading = false;
        if (res.success && res.data) {
          this.messages.push({
            id: `a-${Date.now()}`,
            role: 'assistant',
            content: res.data.answer,
            timestamp: new Date().toISOString(),
            metrics: res.data.metrics,
            links: res.data.links
          });
        }
      },
      error: () => {
        this.aiAssistantApi.askMock(q).subscribe(mockRes => {
          this.loading = false;
          if (mockRes.success && mockRes.data) {
            this.messages.push({
              id: `a-${Date.now()}`,
              role: 'assistant',
              content: mockRes.data.answer,
              timestamp: new Date().toISOString(),
              metrics: mockRes.data.metrics,
              links: mockRes.data.links
            });
          }
        });
      }
    });
  }

  navigate(url: string): void {
    this.router.navigateByUrl(url);
    this.close();
  }
}
