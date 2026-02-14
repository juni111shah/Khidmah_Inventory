import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-live-ops',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './live-ops.component.html'
})
export class LiveOpsComponent {
  message = 'Command center: workers, positions, tasks, progress, heatmap. Auto-updates via SignalR.';
}
