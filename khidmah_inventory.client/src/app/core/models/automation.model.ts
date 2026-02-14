export type ConditionType =
  | 'StockBelowThreshold'
  | 'OrderAboveLimit'
  | 'ItemNearExpiry'
  | 'SlowMoving'
  | 'StockAboveThreshold'
  | 'NewOrder';

export type ActionType =
  | 'CreatePO'
  | 'RequireApproval'
  | 'Notify'
  | 'DiscountSuggestion'
  | 'CreateAlert'
  | 'SendEmail';

export interface AutomationCondition {
  type: ConditionType;
  params: Record<string, unknown>;
}

export interface AutomationAction {
  type: ActionType;
  params: Record<string, unknown>;
}

export interface AutomationRule {
  id: string;
  name: string;
  description?: string;
  isActive: boolean;
  condition: AutomationCondition;
  action: AutomationAction;
  priority: number;
  createdAt: string;
  updatedAt?: string;
  lastRunAt?: string;
  runCount: number;
}

export interface AutomationExecution {
  id: string;
  ruleId: string;
  ruleName: string;
  triggeredAt: string;
  status: 'Success' | 'Failed' | 'Skipped';
  message?: string;
  entityType?: string;
  entityId?: string;
}
