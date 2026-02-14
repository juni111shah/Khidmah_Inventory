import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import {
  CopilotApiService,
  CopilotConversationState,
  CopilotExecuteResponse
} from './copilot-api.service';

export interface AssistantTurn {
  reply: string;
  raw: CopilotExecuteResponse;
}

@Injectable({ providedIn: 'root' })
export class AiOrchestratorService {
  private state: CopilotConversationState | undefined;

  constructor(private copilotApi: CopilotApiService) {}

  reset(): void {
    this.state = undefined;
  }

  send(input: string, confirmed = false): Observable<AssistantTurn> {
    return this.copilotApi.execute({
      input,
      confirmed,
      sessionState: this.state
    }).pipe(
      map((res) => {
        const payload = res.data ?? { success: false, errors: ['No response from copilot.'] };
        this.state = payload.sessionState;
        const reply =
          payload.reply ||
          payload.nextQuestion ||
          payload.confirmationMessage ||
          (payload.errors && payload.errors.length ? payload.errors.join(' ') : 'Done.');
        return { reply, raw: payload };
      })
    );
  }
}
