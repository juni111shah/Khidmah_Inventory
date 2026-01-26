import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { ApiConfigService } from './api-config.service';

export interface PosSession {
  id: string;
  userId: string;
  startTime: Date;
  endTime?: Date;
  openingBalance: number;
  closingBalance?: number;
  expectedBalance?: number;
  status: string;
}

export interface PosSaleItem {
  productId: string;
  quantity: number;
  unitPrice: number;
  discountAmount: number;
}

export interface CreatePosSaleCommand {
  posSessionId: string;
  customerId: string;
  paymentMethod: string;
  amountPaid: number;
  items: PosSaleItem[];
  warehouseId: string;
}

@Injectable({
  providedIn: 'root'
})
export class PosApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('pos');
  }

  openSession(openingBalance: number): Observable<ApiResponse<PosSession>> {
    return this.http.post<ApiResponse<PosSession>>(`${this.apiUrl}/sessions/open`, { openingBalance });
  }

  closeSession(sessionId: string, closingBalance: number): Observable<ApiResponse<PosSession>> {
    return this.http.post<ApiResponse<PosSession>>(`${this.apiUrl}/sessions/close`, { sessionId, closingBalance });
  }

  getActiveSession(): Observable<ApiResponse<PosSession | null>> {
    return this.http.get<ApiResponse<PosSession | null>>(`${this.apiUrl}/sessions/active`);
  }

  createSale(command: CreatePosSaleCommand): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/sales`, command);
  }
}
