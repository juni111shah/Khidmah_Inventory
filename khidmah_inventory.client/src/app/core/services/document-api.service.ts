import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiConfigService } from './api-config.service';

@Injectable({
  providedIn: 'root'
})
export class DocumentApiService {
  private baseUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.baseUrl = this.apiConfig.getApiUrl('documents');
  }

  generateInvoicePdf(salesOrderId: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/invoice/${salesOrderId}`, {
      responseType: 'blob'
    });
  }

  generatePurchaseOrderPdf(purchaseOrderId: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/purchase-order/${purchaseOrderId}`, {
      responseType: 'blob'
    });
  }
}
