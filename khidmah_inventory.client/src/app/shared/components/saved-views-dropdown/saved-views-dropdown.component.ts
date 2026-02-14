import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SavedView, TableState, SortColumn } from '../../../core/models/table-state.model';
import { SavedViewsService } from '../../../core/services/saved-views.service';
import { TableStateService } from '../../../core/services/table-state.service';
import { UnifiedButtonComponent } from '../unified-button/unified-button.component';
import { IconComponent } from '../icon/icon.component';
import { ClickOutsideDirective } from '../../directives/click-outside.directive';

@Component({
  selector: 'app-saved-views-dropdown',
  standalone: true,
  imports: [CommonModule, FormsModule, UnifiedButtonComponent, IconComponent, ClickOutsideDirective],
  templateUrl: './saved-views-dropdown.component.html',
  styleUrls: ['./saved-views-dropdown.component.scss']
})
export class SavedViewsDropdownComponent {
  @Input() tableId: string = '';
  @Input() currentState: { filterRequest?: any; sortColumns?: { column: string; direction: string }[]; visibleColumnKeys?: string[] } = {};

  @Output() loadView = new EventEmitter<Partial<{ filterRequest: any; sortColumns: any[]; visibleColumnKeys: string[] }>>();

  isOpen = false;
  views: SavedView[] = [];
  saveName = '';
  showSaveInput = false;
  editingViewId: string | null = null;
  editingName = '';

  constructor(
    private savedViews: SavedViewsService,
    private tableState: TableStateService
  ) {}

  get canSave(): boolean {
    return !!this.tableId && !!this.saveName.trim();
  }

  open(): void {
    this.isOpen = true;
    this.views = this.tableId ? this.savedViews.list(this.tableId) : [];
    this.showSaveInput = false;
    this.saveName = '';
    this.editingViewId = null;
  }

  close(): void {
    this.isOpen = false;
    this.showSaveInput = false;
    this.editingViewId = null;
  }

  onToggle(): void {
    if (this.isOpen) this.close();
    else this.open();
  }

  onLoad(view: SavedView): void {
    if (view.state) {
      this.loadView.emit(view.state);
      this.close();
    }
  }

  saveCurrent(): void {
    if (!this.canSave || !this.tableId) return;
    const sortColumns: SortColumn[] | undefined = this.currentState.sortColumns?.map(
      (s): SortColumn => ({ column: s.column, direction: s.direction === 'desc' ? 'desc' : 'asc' })
    );
    const state: Partial<TableState> = {
      filterRequest: this.currentState.filterRequest,
      sortColumns,
      visibleColumnKeys: this.currentState.visibleColumnKeys
    };
    this.savedViews.save(this.tableId, this.saveName.trim(), state, false);
    this.views = this.savedViews.list(this.tableId);
    this.saveName = '';
    this.showSaveInput = false;
  }

  setDefault(view: SavedView): void {
    this.savedViews.setDefault(view.id);
    this.views = this.savedViews.list(this.tableId);
  }

  startRename(view: SavedView): void {
    this.editingViewId = view.id;
    this.editingName = view.name;
  }

  confirmRename(): void {
    if (this.editingViewId && this.editingName.trim()) {
      this.savedViews.rename(this.editingViewId, this.editingName.trim());
      this.views = this.savedViews.list(this.tableId);
      this.editingViewId = null;
    }
  }

  cancelRename(): void {
    this.editingViewId = null;
  }

  deleteView(view: SavedView, event: Event): void {
    event.stopPropagation();
    if (confirm(`Delete view "${view.name}"?`)) {
      this.savedViews.delete(view.id);
      this.views = this.savedViews.list(this.tableId);
    }
  }
}
