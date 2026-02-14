export interface Workflow {
  id: string;
  name: string;
  description: string;
  entityType: string;
  workflowDefinition: string;
  isActive: boolean;
  version: number;
  createdAt: string;
}

export interface WorkflowInstance {
  id: string;
  workflowId: string;
  workflowName: string;
  entityId: string;
  entityType: string;
  currentStep: string;
  status: string;
  currentAssigneeId?: string;
  currentAssigneeName?: string;
  comments?: string;
  completedAt?: string;
  history: WorkflowHistoryItem[];
  createdAt: string;
}

export interface WorkflowHistoryItem {
  id: string;
  step: string;
  action: string;
  userId?: string;
  userName?: string;
  comments?: string;
  createdAt: string;
}

export interface CreateWorkflowRequest {
  name: string;
  description: string;
  entityType: string;
  workflowDefinition: string;
}

export interface StartWorkflowRequest {
  workflowId: string;
  entityId: string;
  entityType: string;
  initialAssigneeId?: string;
}

export interface ApproveWorkflowStepRequest {
  comments?: string;
}

export interface WorkflowStepDefinition {
  stepId: string;
  name: string;
  assigneeRole?: string;
  order: number;
}
