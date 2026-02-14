export interface ActivityLog {
  id: string;
  entityType: string;
  entityId: string;
  action: string;
  description: string;
  userName?: string;
  createdAt: string;
  timeAgo: string;
}

export interface Comment {
  id: string;
  entityType: string;
  entityId: string;
  content: string;
  parentCommentId?: string;
  isEdited: boolean;
  editedAt?: string;
  userId?: string;
  userName?: string;
  createdAt: string;
  timeAgo: string;
  replies: Comment[];
}

export interface CreateCommentRequest {
  entityType: string;
  entityId: string;
  content: string;
  parentCommentId?: string;
}
