import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FilterRequest, PaginationDto } from '../models/user.model';
import { TableState, SortColumn } from '../models/table-state.model';

const STORAGE_PREFIX = 'table_state_';
const MAX_AGE_MS = 30 * 24 * 60 * 60 * 1000; // 30 days

@Injectable({ providedIn: 'root' })
export class TableStateService {

  constructor(private router: Router) {}

  getState(tableId: string): Partial<TableState> | null {
    const fromUrl = this.restoreFromUrl(tableId);
    const fromStorage = this.restoreFromLocalStorage(tableId);
    return this.mergeState(fromUrl, fromStorage);
  }

  setState(tableId: string, state: Partial<TableState>): void {
    this.persistToLocalStorage(tableId, { tableId, ...state });
    this.persistToUrl(tableId, state);
  }

  patchState(tableId: string, patch: Partial<TableState>): void {
    const current = this.getState(tableId) || {};
    const merged: Partial<TableState> = {
      ...current,
      ...patch,
      filterRequest: patch.filterRequest ?? current.filterRequest,
      sortColumns: patch.sortColumns ?? current.sortColumns,
      visibleColumnKeys: patch.visibleColumnKeys ?? current.visibleColumnKeys
    };
    if (patch.filterRequest) merged.filterRequest = patch.filterRequest;
    if (patch.sortColumns) merged.sortColumns = patch.sortColumns;
    if (patch.visibleColumnKeys) merged.visibleColumnKeys = patch.visibleColumnKeys;
    this.setState(tableId, merged);
  }

  clear(tableId: string): void {
    try {
      localStorage.removeItem(STORAGE_PREFIX + tableId);
    } catch (_) {}
    this.clearUrlParams(tableId);
  }

  persistToUrl(tableId: string, state: Partial<TableState>): void {
    const q: Record<string, string> = {};
    const key = (k: string) => `${tableId}_${k}`;
    if (state.filterRequest?.pagination) {
      const p = state.filterRequest.pagination;
      if (p.pageNo != null) q[key('page')] = String(p.pageNo);
      if (p.pageSize != null) q[key('pageSize')] = String(p.pageSize);
      if (p.sortBy) q[key('sortBy')] = p.sortBy;
      if (p.sortOrder) q[key('sortOrder')] = p.sortOrder;
    }
    if (state.sortColumns?.length) {
      q[key('sort')] = state.sortColumns.map(s => `${s.column}:${s.direction}`).join(',');
    }
    if (state.filterRequest?.search?.term) {
      q[key('search')] = state.filterRequest.search.term;
    }
    if (state.visibleColumnKeys?.length) {
      q[key('cols')] = state.visibleColumnKeys.join(',');
    }
    this.router.navigate([], {
      queryParams: q,
      queryParamsHandling: 'merge',
      replaceUrl: true
    });
  }

  restoreFromUrl(tableId: string): Partial<TableState> | null {
    const params = this.router.parseUrl(this.router.url).queryParams;
    const key = (k: string) => `${tableId}_${k}`;
    const page = params[key('page')];
    const pageSize = params[key('pageSize')];
    const sortBy = params[key('sortBy')];
    const sortOrder = params[key('sortOrder')];
    const sort = params[key('sort')];
    const search = params[key('search')];
    const cols = params[key('cols')];
    if (!page && !pageSize && !sortBy && !sortOrder && !sort && !search && !cols) return null;
    const state: Partial<TableState> = {
      filterRequest: {},
      sortColumns: [],
      visibleColumnKeys: []
    };
    if (page != null || pageSize != null || sortBy || sortOrder) {
      state.filterRequest!.pagination = {
        pageNo: page != null ? +page : 1,
        pageSize: pageSize != null ? +pageSize : 10,
        sortBy: sortBy || undefined,
        sortOrder: sortOrder || undefined
      };
    }
    if (sort) {
      state.sortColumns = (sort as string).split(',').map(s => {
        const [column, direction] = (s as string).split(':');
        return { column: column?.trim() || '', direction: (direction as 'asc' | 'desc') || 'asc' };
      }).filter(s => s.column);
    }
    if (search != null) {
      state.filterRequest!.search = {
        term: search,
        searchFields: [],
        mode: 'Contains' as any,
        isCaseSensitive: false
      };
    }
    if (cols) {
      state.visibleColumnKeys = (cols as string).split(',').map(c => c.trim()).filter(Boolean);
    }
    return state;
  }

  private clearUrlParams(tableId: string): void {
    const params = { ...this.router.parseUrl(this.router.url).queryParams };
    const prefix = `${tableId}_`;
    Object.keys(params).forEach(k => {
      if (k.startsWith(prefix)) delete params[k];
    });
    this.router.navigate([], { queryParams: params, queryParamsHandling: '', replaceUrl: true });
  }

  persistToLocalStorage(tableId: string, state: Partial<TableState>): void {
    try {
      const payload = {
        ...state,
        updatedAt: Date.now()
      };
      localStorage.setItem(STORAGE_PREFIX + tableId, JSON.stringify(payload));
    } catch (_) {}
  }

  restoreFromLocalStorage(tableId: string): Partial<TableState> | null {
    try {
      const raw = localStorage.getItem(STORAGE_PREFIX + tableId);
      if (!raw) return null;
      const data = JSON.parse(raw);
      if (data.updatedAt && Date.now() - data.updatedAt > MAX_AGE_MS) {
        localStorage.removeItem(STORAGE_PREFIX + tableId);
        return null;
      }
      return data;
    } catch (_) {
      return null;
    }
  }

  private mergeState(fromUrl: Partial<TableState> | null, fromStorage: Partial<TableState> | null): Partial<TableState> | null {
    if (!fromUrl && !fromStorage) return null;
    const merged: Partial<TableState> = {
      filterRequest: {},
      sortColumns: [],
      visibleColumnKeys: []
    };
    if (fromStorage?.filterRequest) {
      merged.filterRequest = { ...merged.filterRequest, ...fromStorage.filterRequest };
      if (fromStorage.filterRequest.pagination) {
        merged.filterRequest!.pagination = { ...fromStorage.filterRequest.pagination };
      }
      if (fromStorage.filterRequest.search) {
        merged.filterRequest!.search = { ...fromStorage.filterRequest.search };
      }
      if (fromStorage.filterRequest.filters) {
        merged.filterRequest!.filters = [...fromStorage.filterRequest.filters];
      }
    }
    if (fromStorage?.sortColumns?.length) merged.sortColumns = [...fromStorage.sortColumns];
    if (fromStorage?.visibleColumnKeys?.length) merged.visibleColumnKeys = [...fromStorage.visibleColumnKeys];
    if (fromUrl?.filterRequest?.pagination) {
      merged.filterRequest!.pagination = { ...merged.filterRequest!.pagination, ...fromUrl.filterRequest.pagination };
    }
    if (fromUrl?.filterRequest?.search?.term != null) {
      if (!merged.filterRequest) merged.filterRequest = {};
      merged.filterRequest.search = { ...merged.filterRequest.search, ...fromUrl.filterRequest.search };
    }
    if (fromUrl?.sortColumns?.length) merged.sortColumns = fromUrl.sortColumns;
    if (fromUrl?.visibleColumnKeys?.length) merged.visibleColumnKeys = fromUrl.visibleColumnKeys;
    return merged;
  }
}
