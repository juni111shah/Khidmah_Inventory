import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SearchOverlayService } from '../../../core/services/search-overlay.service';

@Component({
  selector: 'app-global-search',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './global-search.component.html',
  styles: [`:host { display: inline-flex; flex-shrink: 0; }`]
})
export class GlobalSearchComponent {
  constructor(private searchOverlay: SearchOverlayService) {}

  openOverlay(): void {
    this.searchOverlay.open();
  }
}
