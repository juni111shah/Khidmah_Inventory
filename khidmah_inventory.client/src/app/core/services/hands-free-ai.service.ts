import { Injectable } from '@angular/core';

/**
 * Optional AI enhancements for hands-free (future-ready placeholders).
 * - Voice intent expansion: extend command vocabulary
 * - Route optimization: suggest pick order by location
 * - Fatigue detection: placeholder for future ML
 */
@Injectable({
  providedIn: 'root'
})
export class HandsFreeAiService {
  /** Expand raw transcript to intent (future: NLU). */
  expandIntent(_transcript: string): string | null {
    return null;
  }

  /** Suggest optimal task order by location (future: routing). */
  suggestTaskOrder(_taskIds: string[], _warehouseId: string): string[] {
    return [];
  }

  /** Fatigue detection placeholder (future: camera/behavior analysis). */
  getFatigueLevel(): 'low' | 'medium' | 'high' | null {
    return null;
  }
}
