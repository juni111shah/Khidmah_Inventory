import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ManagementStory as ManagementStoryModel } from '../../../core/models/decision-support.model';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';

@Component({
  selector: 'app-management-story',
  standalone: true,
  imports: [CommonModule, UnifiedCardComponent],
  template: `
    <app-unified-card header="Management story" customClass="border-0 shadow-sm">
      <ng-container *ngIf="story">
        <p class="mb-3">{{ story.summary }}</p>
        <div *ngIf="story.highlights?.length" class="mb-3">
          <h6 class="small text-muted">Highlights</h6>
          <ul class="list-unstyled mb-0">
            <li *ngFor="let h of story.highlights" class="small"><i class="bi bi-check2 text-success me-1"></i>{{ h }}</li>
          </ul>
        </div>
        <div *ngIf="story.risks?.length" class="mb-3">
          <h6 class="small text-muted text-danger">Risks</h6>
          <ul class="list-unstyled mb-0">
            <li *ngFor="let r of story.risks" class="small"><i class="bi bi-exclamation-triangle text-warning me-1"></i>{{ r }}</li>
          </ul>
        </div>
        <div *ngIf="story.recommendations?.length">
          <h6 class="small text-muted">Recommendations</h6>
          <ul class="list-unstyled mb-0">
            <li *ngFor="let rec of story.recommendations" class="small"><i class="bi bi-arrow-right-circle me-1"></i>{{ rec }}</li>
          </ul>
        </div>
      </ng-container>
      <div *ngIf="!story" class="text-muted small">No story generated.</div>
    </app-unified-card>
  `
})
export class ManagementStoryComponent {
  @Input() story: ManagementStoryModel | null = null;
}
