export interface StaffMetric {
  userId: string;
  userName: string;
  email?: string;
  salesTotal: number;
  transactionCount: number;
  averageTransactionValue: number;
  discountRate: number;
  averageDiscountPercent: number;
  averageTransactionSpeedSeconds?: number;
  approvalDelayHours?: number;
  rank: number;
}

export interface StaffPerformanceData {
  staff: StaffMetric[];
  periodFrom: string;
  periodTo: string;
}
