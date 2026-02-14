import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AiAssistantPanelComponent } from '../ai-assistant-panel/ai-assistant-panel.component';

@Component({
  selector: 'app-ai-assistant-trigger',
  standalone: true,
  imports: [CommonModule, AiAssistantPanelComponent],
  templateUrl: './ai-assistant-trigger.component.html',
  styles: [`
    .ai-trigger-btn {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      width: 2.25rem;
      height: 2.25rem;
      padding: 0;
      border: none;
      border-radius: 0.5rem;
      background: transparent;
      color: var(--text-color);
      cursor: pointer;
      transition: background-color 0.2s ease, color 0.2s ease;
    }
    .ai-trigger-btn:hover {
      background-color: var(--background-color);
      color: var(--primary-color);
    }
    .ai-trigger-btn:focus-visible {
      outline: 2px solid var(--primary-color);
      outline-offset: 2px;
    }
    .ai-trigger-btn-active,
    .ai-trigger-btn:active {
      background-color: var(--background-color);
      color: var(--primary-color);
    }
  `]
})
export class AiAssistantTriggerComponent {
  panelOpen = false;

  openPanel(): void {
    this.panelOpen = true;
  }
}
