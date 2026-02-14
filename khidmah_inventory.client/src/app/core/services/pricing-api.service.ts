import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { PriceOptimization } from '../models/pricing.model';
import { ApiConfigService } from './api-config.service';

@Injectable({ providedIn: 'root' })
export class PricingApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('pricing');
  }

  getSuggestions(productIds?: string[], minMargin?: number, maxMargin?: number): Observable<ApiResponse<PriceOptimization[]>> {
    let params = new HttpParams();
    if (productIds?.length) productIds.forEach((id, i) => params = params.set(`productIds[${i}]`, id));
    if (minMargin != null) params = params.set('minMargin', String(minMargin));
    if (maxMargin != null) params = params.set('maxMargin', String(maxMargin));
    params = params.set('includeHistory', 'true');
    return this.http.get<ApiResponse<PriceOptimization[]>>(`${this.apiUrl}/suggestions`, { params });
  }
}
