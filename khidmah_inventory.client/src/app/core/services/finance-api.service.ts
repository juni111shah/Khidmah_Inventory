import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { ApiConfigService } from './api-config.service';
import {
  AccountDto,
  JournalEntryDto,
  GetJournalEntriesResult,
  ProfitLossDto,
  BalanceSheetDto,
  CashFlowDto,
  ImportStandardChartResult
} from '../models/finance.model';

@Injectable({
  providedIn: 'root'
})
export class FinanceApiService {
  private baseUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.baseUrl = this.apiConfig.getApiUrl('finance');
  }

  getAccountsTree(includeInactive = false): Observable<ApiResponse<AccountDto[]>> {
    const params = new HttpParams().set('includeInactive', includeInactive.toString());
    return this.http.get<ApiResponse<AccountDto[]>>(`${this.baseUrl}/accounts/tree`, { params });
  }

  getAccount(id: string): Observable<ApiResponse<AccountDto>> {
    return this.http.get<ApiResponse<AccountDto>>(`${this.baseUrl}/accounts/${id}`);
  }

  createAccount(body: { code: string; name: string; type: string; parentAccountId?: string }): Observable<ApiResponse<AccountDto>> {
    return this.http.post<ApiResponse<AccountDto>>(`${this.baseUrl}/accounts`, body);
  }

  updateAccount(id: string, body: { code: string; name: string; type: string; isActive: boolean }): Observable<ApiResponse<AccountDto>> {
    return this.http.put<ApiResponse<AccountDto>>(`${this.baseUrl}/accounts/${id}`, body);
  }

  deleteAccount(id: string): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.baseUrl}/accounts/${id}`);
  }

  importStandardChart(): Observable<ApiResponse<ImportStandardChartResult>> {
    return this.http.post<ApiResponse<ImportStandardChartResult>>(`${this.baseUrl}/accounts/import-standard`, {});
  }

  getJournalEntries(params: {
    dateFrom?: string;
    dateTo?: string;
    sourceModule?: string;
    pageNo?: number;
    pageSize?: number;
  }): Observable<ApiResponse<GetJournalEntriesResult>> {
    let httpParams = new HttpParams();
    if (params.dateFrom) httpParams = httpParams.set('dateFrom', params.dateFrom);
    if (params.dateTo) httpParams = httpParams.set('dateTo', params.dateTo);
    if (params.sourceModule) httpParams = httpParams.set('sourceModule', params.sourceModule);
    if (params.pageNo != null) httpParams = httpParams.set('pageNo', params.pageNo.toString());
    if (params.pageSize != null) httpParams = httpParams.set('pageSize', params.pageSize.toString());
    return this.http.get<ApiResponse<GetJournalEntriesResult>>(`${this.baseUrl}/journals`, { params: httpParams });
  }

  getProfitLoss(fromDate: string, toDate: string): Observable<ApiResponse<ProfitLossDto>> {
    const params = new HttpParams().set('fromDate', fromDate).set('toDate', toDate);
    return this.http.get<ApiResponse<ProfitLossDto>>(`${this.baseUrl}/statements/pl`, { params });
  }

  getBalanceSheet(asOfDate: string): Observable<ApiResponse<BalanceSheetDto>> {
    const params = new HttpParams().set('asOfDate', asOfDate);
    return this.http.get<ApiResponse<BalanceSheetDto>>(`${this.baseUrl}/statements/balance-sheet`, { params });
  }

  getCashFlow(fromDate: string, toDate: string): Observable<ApiResponse<CashFlowDto>> {
    const params = new HttpParams().set('fromDate', fromDate).set('toDate', toDate);
    return this.http.get<ApiResponse<CashFlowDto>>(`${this.baseUrl}/statements/cash-flow`, { params });
  }
}
