import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { SalesReport, InventoryReport, PurchaseReport } from '../models/report.model';
import { ApiConfigService } from './api-config.service';

@Injectable({
  providedIn: 'root'
})
export class ReportApiService {
  private apiUrl: string;

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) {
    this.apiUrl = this.apiConfig.getApiUrl('reports');
  }

  getSalesReport(fromDate: Date, toDate: Date, customerId?: string): Observable<ApiResponse<SalesReport>> {
    let params = new HttpParams()
      .set('fromDate', fromDate.toISOString())
      .set('toDate', toDate.toISOString());
    if (customerId) {
      params = params.set('customerId', customerId);
    }
    return this.http.get<ApiResponse<SalesReport>>(`${this.apiUrl}/sales`, { params });
  }

  getInventoryReport(warehouseId?: string, categoryId?: string, lowStockOnly?: boolean): Observable<ApiResponse<InventoryReport>> {
    let params = new HttpParams();
    if (warehouseId) {
      params = params.set('warehouseId', warehouseId);
    }
    if (categoryId) {
      params = params.set('categoryId', categoryId);
    }
    if (lowStockOnly !== undefined) {
      params = params.set('lowStockOnly', lowStockOnly.toString());
    }
    return this.http.get<ApiResponse<InventoryReport>>(`${this.apiUrl}/inventory`, { params });
  }

  getPurchaseReport(fromDate: Date, toDate: Date, supplierId?: string): Observable<ApiResponse<PurchaseReport>> {
    let params = new HttpParams()
      .set('fromDate', fromDate.toISOString())
      .set('toDate', toDate.toISOString());
    if (supplierId) {
      params = params.set('supplierId', supplierId);
    }
    return this.http.get<ApiResponse<PurchaseReport>>(`${this.apiUrl}/purchase`, { params });
  }
}

