import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CollaborationApiService } from '../../../core/services/collaboration-api.service';
import { ActivityLog, Comment } from '../../../core/models/collaboration.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { DrawerComponent } from '../drawer/drawer.component';
import { UnifiedButtonComponent } from '../unified-button/unified-button.component';
import { UnifiedTextareaComponent } from '../unified-textarea/unified-textarea.component';
import { LoadingSpinnerComponent } from '../loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-activity-feed-panel',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DrawerComponent,
    UnifiedButtonComponent,
    UnifiedTextareaComponent,
    LoadingSpinnerComponent
  ],
  templateUrl: './activity-feed-panel.component.html'
})
export class ActivityFeedPanelComponent implements OnInit, OnChanges {
  @Input() entityType: string = '';
  @Input() entityId: string = '';
  @Input() open = false;
  @Input() title = 'Activity & comments';
  @Input() canComment = true;
  @Output() openChange = new EventEmitter<boolean>();

  activityLogs: ActivityLog[] = [];
  comments: Comment[] = [];
  loading = false;
  savingComment = false;
  newComment = '';

  constructor(private collaborationApi: CollaborationApiService) {}

  ngOnInit(): void {
    if (this.entityType && this.entityId) this.load();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if ((changes['entityType'] || changes['entityId']) && this.entityType && this.entityId) this.load();
  }

  load(): void {
    if (!this.entityType || !this.entityId) return;
    this.loading = true;
    this.collaborationApi.getActivityFeed(this.entityType, this.entityId, 50).subscribe({
      next: (res: ApiResponse<ActivityLog[]>) => {
        if (res.success && res.data) this.activityLogs = res.data;
        else this.activityLogs = [];
      },
      error: () => this.activityLogs = []
    });
    this.collaborationApi.getComments(this.entityType, this.entityId).subscribe({
      next: (res: ApiResponse<Comment[]>) => {
        this.loading = false;
        if (res.success && res.data) this.comments = res.data;
        else this.comments = [];
      },
      error: () => { this.loading = false; this.comments = []; }
    });
  }

  close(): void {
    this.open = false;
    this.openChange.emit(false);
  }

  addComment(): void {
    if (!this.newComment.trim() || !this.canComment) return;
    this.savingComment = true;
    this.collaborationApi.createComment({
      entityType: this.entityType,
      entityId: this.entityId,
      content: this.newComment.trim()
    }).subscribe({
      next: (res: ApiResponse<Comment>) => {
        this.savingComment = false;
        if (res.success && res.data) {
          this.comments = [res.data, ...this.comments];
          this.newComment = '';
        }
      },
      error: () => this.savingComment = false
    });
  }
}
