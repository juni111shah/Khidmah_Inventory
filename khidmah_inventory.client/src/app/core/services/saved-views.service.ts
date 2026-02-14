import { Injectable } from '@angular/core';
import { SavedView, TableState } from '../models/table-state.model';

const STORAGE_KEY = 'saved_table_views';

@Injectable({ providedIn: 'root' })
export class SavedViewsService {

  list(tableId: string): SavedView[] {
    const all = this.loadAll();
    return all.filter(v => v.tableId === tableId).sort((a, b) => a.createdAt - b.createdAt);
  }

  getDefault(tableId: string): SavedView | null {
    return this.list(tableId).find(v => v.isDefault) || null;
  }

  save(tableId: string, name: string, state: Partial<TableState>, isDefault = false): SavedView {
    const all = this.loadAll();
    const view: SavedView = {
      id: `view_${Date.now()}_${Math.random().toString(36).slice(2, 9)}`,
      tableId,
      name,
      state: { tableId, ...state },
      isDefault,
      createdAt: Date.now()
    };
    if (isDefault) {
      all.filter(v => v.tableId === tableId).forEach(v => v.isDefault = false);
    }
    all.push(view);
    this.saveAll(all);
    return view;
  }

  load(viewId: string): SavedView | null {
    return this.loadAll().find(v => v.id === viewId) || null;
  }

  setDefault(viewId: string): void {
    const all = this.loadAll();
    const view = all.find(v => v.id === viewId);
    if (!view) return;
    all.filter(v => v.tableId === view.tableId).forEach(v => v.isDefault = v.id === viewId);
    view.isDefault = true;
    this.saveAll(all);
  }

  rename(viewId: string, name: string): void {
    const all = this.loadAll();
    const view = all.find(v => v.id === viewId);
    if (view) {
      view.name = name;
      this.saveAll(all);
    }
  }

  delete(viewId: string): void {
    const all = this.loadAll().filter(v => v.id !== viewId);
    this.saveAll(all);
  }

  private loadAll(): SavedView[] {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (!raw) return [];
      return JSON.parse(raw);
    } catch {
      return [];
    }
  }

  private saveAll(views: SavedView[]): void {
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(views));
    } catch (_) {}
  }
}
