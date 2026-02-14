import { FilterRequest } from '../../core/models/user.model';
import { PagedResult } from '../../core/models/api-response.model';
import { SearchMode } from '../../core/models/user.model';

/**
 * Applies client-side search, filters, sort and pagination to a list.
 * Use when the API returns the full list and you want data-table search/sort/filter/pagination.
 */
export function applyClientSideTable<T>(
  data: T[],
  filterRequest: FilterRequest,
  searchFields: (keyof T)[],
  getCellValue?: (row: T, key: string) => any
): { pagedResult: PagedResult<T>; displayData: T[] } {
  let result = [...data];

  const search = filterRequest.search;
  if (search?.term?.trim() && searchFields.length > 0) {
    const term = (search.isCaseSensitive ? search.term : search.term.toLowerCase()).trim();
    const fields = search.searchFields?.length ? search.searchFields as (keyof T)[] : searchFields;
    result = result.filter(row => {
      const match = fields.some(field => {
        const val = getCellValue ? getCellValue(row, field as string) : (row as any)[field];
        const str = val == null ? '' : String(val);
        const compare = search.isCaseSensitive ? str : str.toLowerCase();
        switch (search.mode) {
          case SearchMode.StartsWith: return compare.startsWith(term);
          case SearchMode.EndsWith: return compare.endsWith(term);
          case SearchMode.ExactMatch: return compare === term;
          default: return compare.includes(term);
        }
      });
      return match;
    });
  }

  const filters = filterRequest.filters;
  if (filters?.length) {
    result = result.filter(row => {
      return filters.every(f => {
        const val = getCellValue ? getCellValue(row, f.column) : (row as any)[f.column];
        const fVal = f.value;
        if (fVal === '' || fVal === null || fVal === undefined) return true;
        const normVal = val === true || val === 'true' ? true : val === false || val === 'false' ? false : val;
        const normF = fVal === true || fVal === 'true' ? true : fVal === false || fVal === 'false' ? false : fVal;
        if (f.operator === '=') return normVal === normF || String(val).toLowerCase() === String(fVal).toLowerCase();
        if (f.operator === '!=') return normVal !== normF;
        if (f.operator === 'contains') return String(val).toLowerCase().includes(String(fVal).toLowerCase());
        return true;
      });
    });
  }

  const pag = filterRequest.pagination;
  const sortBy = pag?.sortBy;
  const sortOrder = pag?.sortOrder === 'descending' ? 'desc' : 'asc';
  if (sortBy) {
    const getVal = (row: T) => getCellValue ? getCellValue(row, sortBy) : (row as any)[sortBy];
    result.sort((a, b) => {
      const aVal = getVal(a);
      const bVal = getVal(b);
      const aNum = typeof aVal === 'number' ? aVal : (aVal instanceof Date ? aVal.getTime() : null);
      const bNum = typeof bVal === 'number' ? bVal : (bVal instanceof Date ? bVal.getTime() : null);
      if (aNum != null && bNum != null) {
        return sortOrder === 'asc' ? aNum - bNum : bNum - aNum;
      }
      const aStr = aVal == null ? '' : String(aVal);
      const bStr = bVal == null ? '' : String(bVal);
      const cmp = aStr.localeCompare(bStr, undefined, { numeric: true });
      return sortOrder === 'asc' ? cmp : -cmp;
    });
  }

  const totalCount = result.length;
  const pageNo = Math.max(1, pag?.pageNo ?? 1);
  const pageSize = Math.max(1, pag?.pageSize ?? 10);
  const totalPages = Math.ceil(totalCount / pageSize) || 1;
  const start = (pageNo - 1) * pageSize;
  const displayData = result.slice(start, start + pageSize);

  const pagedResult: PagedResult<T> = {
    items: displayData,
    totalCount,
    pageNo,
    pageSize,
    totalPages,
    hasPreviousPage: pageNo > 1,
    hasNextPage: pageNo < totalPages
  };

  return { pagedResult, displayData };
}
