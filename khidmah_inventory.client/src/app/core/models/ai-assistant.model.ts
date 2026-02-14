export interface ChatMessage {
  id: string;
  role: 'user' | 'assistant';
  content: string;
  timestamp: string;
  metrics?: MetricCard[];
  links?: QuickLink[];
}

export interface MetricCard {
  label: string;
  value: string | number;
  trend?: 'up' | 'down' | 'flat';
  change?: string;
}

export interface QuickLink {
  label: string;
  url: string;
  icon?: string;
}

export interface AskRequest {
  question: string;
  context?: Record<string, unknown>;
}

export interface AskResponse {
  answer: string;
  metrics?: MetricCard[];
  links?: QuickLink[];
}
