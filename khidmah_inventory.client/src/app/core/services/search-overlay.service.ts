import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class SearchOverlayService {
  private open$ = new Subject<boolean>();

  get openStream() {
    return this.open$.asObservable();
  }

  open(): void {
    this.open$.next(true);
  }

  close(): void {
    this.open$.next(false);
  }
}
