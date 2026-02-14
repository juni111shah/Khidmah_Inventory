export type AccountType = 'Asset' | 'Liability' | 'Equity' | 'Revenue' | 'Expense';

export interface AccountDto {
  id: string;
  companyId: string;
  code: string;
  name: string;
  type: AccountType;
  typeName: string;
  parentAccountId?: string;
  isActive: boolean;
  children?: AccountDto[];
}

export interface JournalEntryDto {
  id: string;
  companyId: string;
  date: string;
  reference: string;
  sourceModule: string;
  sourceId?: string;
  description?: string;
  totalDebit: number;
  totalCredit: number;
  lines: JournalLineDto[];
}

export interface JournalLineDto {
  id: string;
  accountId: string;
  accountCode: string;
  accountName: string;
  debit: number;
  credit: number;
  memo?: string;
}

export interface ProfitLossDto {
  fromDate: string;
  toDate: string;
  revenue: number;
  cogs: number;
  grossProfit: number;
  expenses: number;
  netProfit: number;
  grossMarginPercent?: number;
  netMarginPercent?: number;
  revenueLines: AccountLineDto[];
  expenseLines: AccountLineDto[];
}

export interface BalanceSheetDto {
  asOfDate: string;
  totalAssets: number;
  totalLiabilities: number;
  totalEquity: number;
  totalLiabilitiesAndEquity: number;
  isBalanced: boolean;
  assetLines: AccountLineDto[];
  liabilityLines: AccountLineDto[];
  equityLines: AccountLineDto[];
}

export interface CashFlowDto {
  fromDate: string;
  toDate: string;
  operatingInflow: number;
  operatingOutflow: number;
  netOperating: number;
  investingInflow: number;
  investingOutflow: number;
  netInvesting: number;
  financingInflow: number;
  financingOutflow: number;
  netFinancing: number;
  netCashChange: number;
}

export interface AccountLineDto {
  accountId: string;
  code: string;
  name: string;
  amount: number;
}

export interface GetJournalEntriesResult {
  items: JournalEntryDto[];
  totalCount: number;
  pageNo: number;
  pageSize: number;
}

export interface ImportStandardChartResult {
  created: number;
  skipped: number;
  accounts: AccountDto[];
}
