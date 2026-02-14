import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { ApiConfigService } from './api-config.service';
import { ProductIntelligence, DashboardIntelligence, AiRecommendation } from '../models/intelligence.model';

@Injectable({
  providedIn: 'root'
})
export class IntelligenceApiService {
  private baseUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.baseUrl = this.apiConfig.getApiUrl('intelligence');
  }

  getProductIntelligence(productId: string, daysForVelocity = 30): Observable<ApiResponse<ProductIntelligence>> {
    return this.http.get<ApiResponse<ProductIntelligence>>(
      `${this.baseUrl}/product/${productId}`,
      { params: { daysForVelocity: daysForVelocity.toString() } }
    );
  }

  getDashboardIntelligence(predictionDays = 7): Observable<ApiResponse<DashboardIntelligence>> {
    return this.http.get<ApiResponse<DashboardIntelligence>>(
      `${this.baseUrl}/dashboard`,
      { params: { predictionDays: predictionDays.toString() } }
    );
  }

  getRecommendations(productId?: string, horizonDays = 14): Observable<ApiResponse<AiRecommendation[]>> {
    const params: Record<string, string> = { horizonDays: horizonDays.toString() };
    if (productId) {
      params['productId'] = productId;
    }
    return this.http.get<ApiResponse<AiRecommendation[]>>(
      `${this.baseUrl}/recommendations`,
      { params }
    );
  }
}
