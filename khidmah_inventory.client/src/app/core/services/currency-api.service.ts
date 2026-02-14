import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { ApiConfigService } from './api-config.service';
import {
  CurrencyDto,
  ExchangeRateDto,
  GetCurrenciesListResult,
  GetExchangeRatesResult
} from '../models/currency.model';

@Injectable({
  providedIn: 'root'
})
export class CurrencyApiService {
  private currencyUrl: string;
  private ratesUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.currencyUrl = this.apiConfig.getApiUrl('currency');
    this.ratesUrl = this.apiConfig.getApiUrl('exchange-rates');
  }

  getCurrencies(includeInactive = false): Observable<ApiResponse<GetCurrenciesListResult>> {
    const params = new HttpParams().set('includeInactive', includeInactive.toString());
    return this.http.get<ApiResponse<GetCurrenciesListResult>>(this.currencyUrl, { params });
  }

  getCurrency(id: string): Observable<ApiResponse<CurrencyDto>> {
    return this.http.get<ApiResponse<CurrencyDto>>(`${this.currencyUrl}/${id}`);
  }

  createCurrency(body: { code: string; name: string; symbol: string; isBase: boolean }): Observable<ApiResponse<CurrencyDto>> {
    return this.http.post<ApiResponse<CurrencyDto>>(this.currencyUrl, body);
  }

  updateCurrency(id: string, body: { code: string; name: string; symbol: string; isBase: boolean }): Observable<ApiResponse<CurrencyDto>> {
    return this.http.put<ApiResponse<CurrencyDto>>(`${this.currencyUrl}/${id}`, body);
  }

  deleteCurrency(id: string): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.currencyUrl}/${id}`);
  }

  getExchangeRates(params: {
    fromDate?: string;
    toDate?: string;
    fromCurrencyId?: string;
    toCurrencyId?: string;
  }): Observable<ApiResponse<GetExchangeRatesResult>> {
    let httpParams = new HttpParams();
    if (params.fromDate) httpParams = httpParams.set('fromDate', params.fromDate);
    if (params.toDate) httpParams = httpParams.set('toDate', params.toDate);
    if (params.fromCurrencyId) httpParams = httpParams.set('fromCurrencyId', params.fromCurrencyId);
    if (params.toCurrencyId) httpParams = httpParams.set('toCurrencyId', params.toCurrencyId);
    return this.http.get<ApiResponse<GetExchangeRatesResult>>(this.ratesUrl, { params: httpParams });
  }

  createExchangeRate(body: {
    fromCurrencyId: string;
    toCurrencyId: string;
    rate: number;
    date: string;
  }): Observable<ApiResponse<ExchangeRateDto>> {
    return this.http.post<ApiResponse<ExchangeRateDto>>(this.ratesUrl, body);
  }
}
