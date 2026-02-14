import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { trigger, transition, style, animate } from '@angular/animations';

@Component({
  selector: 'app-content-loader',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (loading) {
      <div class="content-loader-skeleton" [class.content-loader-skeleton--no-min-height]="!preserveMinHeight">
        <ng-content select="[skeleton]"></ng-content>
      </div>
    } @else {
      <div class="content-loader-content" [@fadeIn]>
        <ng-content></ng-content>
      </div>
    }
  `,
  styles: [`
    .content-loader-skeleton:not(.content-loader-skeleton--no-min-height) {
      min-height: 120px;
    }
    .content-loader-content {
      display: block;
    }
  `],
  animations: [
    trigger('fadeIn', [
      transition(':enter', [
        style({ opacity: 0 }),
        animate('250ms ease-out', style({ opacity: 1 }))
      ])
    ])
  ]
})
export class ContentLoaderComponent {
  @Input() loading = false;
  /** When true (default), skeleton wrapper keeps min-height to reduce layout shift */
  @Input() preserveMinHeight = true;
}
