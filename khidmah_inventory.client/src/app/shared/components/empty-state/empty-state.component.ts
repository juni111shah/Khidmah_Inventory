import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './empty-state.component.html'
})
export class EmptyStateComponent {
  @Input() icon: string = '📭';
  @Input() title: string = 'No data available';
  @Input() message: string = 'There is no data to display at this time.';
  @Input() showAction: boolean = false;
  @Input() actionLabel: string = 'Add New';
}

