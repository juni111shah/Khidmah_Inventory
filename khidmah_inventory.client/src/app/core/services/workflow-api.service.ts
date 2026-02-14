import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import {
  Workflow,
  WorkflowInstance,
  CreateWorkflowRequest,
  StartWorkflowRequest,
  ApproveWorkflowStepRequest
} from '../models/workflow.model';
import { ApiConfigService } from './api-config.service';

@Injectable({ providedIn: 'root' })
export class WorkflowApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('workflows');
  }

  create(request: CreateWorkflowRequest): Observable<ApiResponse<Workflow>> {
    return this.http.post<ApiResponse<Workflow>>(this.apiUrl, request);
  }

  start(request: StartWorkflowRequest): Observable<ApiResponse<WorkflowInstance>> {
    return this.http.post<ApiResponse<WorkflowInstance>>(`${this.apiUrl}/start`, request);
  }

  approveStep(workflowInstanceId: string, request: ApproveWorkflowStepRequest): Observable<ApiResponse<WorkflowInstance>> {
    return this.http.post<ApiResponse<WorkflowInstance>>(`${this.apiUrl}/${workflowInstanceId}/approve`, request);
  }

  /** Optional: when backend adds list endpoints */
  getWorkflows(): Observable<ApiResponse<Workflow[]>> {
    return this.http.get<ApiResponse<Workflow[]>>(this.apiUrl);
  }

  getPendingApprovals(): Observable<ApiResponse<WorkflowInstance[]>> {
    return this.http.get<ApiResponse<WorkflowInstance[]>>(`${this.apiUrl}/pending`);
  }
}
