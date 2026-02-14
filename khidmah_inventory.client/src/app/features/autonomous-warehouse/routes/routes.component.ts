import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-routes',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './routes.component.html'
})
export class RoutesComponent {
  message = 'Route optimization (nearest neighbor). Select warehouse and tasks to get optimal sequence.';
}
