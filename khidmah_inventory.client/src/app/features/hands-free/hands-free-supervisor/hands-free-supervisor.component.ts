import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderService } from '../../../core/services/header.service';
import { HandsFreeApiService } from '../../../core/services/hands-free-api.service';
import { HandsFreeSupervisorWorkerDto } from '../../../core/models/hands-free.model';

/**
 * Supervisor view: who is picking, progress, speed, errors.
 * Uses GET api/warehouse/handsfree/sessions for live session data.
 */
@Component({
  selector: 'app-hands-free-supervisor',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './hands-free-supervisor.component.html',
  styleUrls: ['./hands-free-supervisor.component.scss']
})
export class HandsFreeSupervisorComponent implements OnInit {
  workers: HandsFreeSupervisorWorkerDto[] = [];
  loading = false;

  constructor(
    private header: HeaderService,
    private handsFreeApi: HandsFreeApiService
  ) {}

  ngOnInit(): void {
    this.header.setHeaderInfo({
      title: 'Hands-free supervisor',
      description: 'Monitor picking sessions, progress and errors'
    });
    this.loadSessions();
  }

  loadSessions(): void {
    this.loading = true;
    this.handsFreeApi.getSessions(120).subscribe({
      next: (res) => {
        this.loading = false;
        this.workers = res.success && res.data ? res.data : [];
      },
      error: () => {
        this.loading = false;
        this.workers = [];
      }
    });
  }

  progress(worker: HandsFreeSupervisorWorkerDto): number {
    if (!worker.totalTasks) return 0;
    return Math.min(100, Math.round((worker.completedTasks / worker.totalTasks) * 100));
  }
}
