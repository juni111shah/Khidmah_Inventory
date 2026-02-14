import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { ApiConfigService } from './api-config.service';

export interface CopilotExecuteRequest {
  input: string;
  confirmed: boolean;
  sessionState?: CopilotConversationState;
}

export interface CopilotExecuteResponse {
  success: boolean;
  action?: string;
  reply?: string;
  nextQuestion?: string;
  confirmationMessage?: string;
  completed?: boolean;
  cancelled?: boolean;
  sessionState?: CopilotConversationState;
  result?: unknown;
  errors?: string[];
}

export interface CopilotConversationState {
  sessionId: string;
  currentTask?: string;
  fields: Record<string, string | null>;
  stepIndex: number;
  awaitingConfirmation: boolean;
  lastQuestion?: string;
  lastAssistantMessage?: string;
}

@Injectable({ providedIn: 'root' })
export class CopilotApiService {
  private baseUrl: string;

  constructor(
    private http: HttpClient,
    apiConfig: ApiConfigService
  ) {
    this.baseUrl = apiConfig.getApiUrl('copilot');
  }

  execute(request: CopilotExecuteRequest): Observable<ApiResponse<CopilotExecuteResponse>> {
    return this.http.post<ApiResponse<CopilotExecuteResponse>>(`${this.baseUrl}/execute`, request);
  }
}
