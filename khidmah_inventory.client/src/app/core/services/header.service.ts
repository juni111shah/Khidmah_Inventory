import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface HeaderInfo {
  title: string;
  description?: string;
}

@Injectable({
  providedIn: 'root'
})
export class HeaderService {
  private headerInfoSubject = new BehaviorSubject<HeaderInfo>({ title: 'Khidmah Inventory' });
  public headerInfo$: Observable<HeaderInfo> = this.headerInfoSubject.asObservable();

  setHeaderInfo(info: HeaderInfo): void {
    this.headerInfoSubject.next(info);
  }

  getHeaderInfo(): HeaderInfo {
    return this.headerInfoSubject.value;
  }

  reset(): void {
    this.headerInfoSubject.next({ title: 'Khidmah Inventory' });
  }
}

