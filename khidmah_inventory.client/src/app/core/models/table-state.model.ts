import { FilterRequest, FilterDto } from './user.model';

/** Operator for filter builder. Backend FilterDto.operator can use =, !=, >, <, >=, <=, in, contains, etc. */
export type FilterOperator = 'equals' | 'contains' | 'startsWith' | 'endsWith' | 'greater' | 'less' | 'greaterOrEqual' | 'lessOrEqual' | 'between' | 'inList' | 'notEquals';

export interface FilterRule {
  id: string;
  column: string;
  operator: FilterOperator;
  value: any;
  value2?: any; // for 'between'
  valueList?: any[]; // for 'inList'
}

export interface FilterGroup {
  id: string;
  logic: 'and' | 'or';
  rules: FilterRule[];
  groups?: FilterGroup[];
}

/** Converts to backend FilterDto[]. Between -> two rules (>= and <=). InList -> operator 'in', value array. */
export function filterGroupToFilterDtos(group: FilterGroup): FilterDto[] {
  const result: FilterDto[] = [];
  const opMap: Record<FilterOperator, string> = {
    equals: '=',
    notEquals: '!=',
    contains: 'Contains',
    startsWith: 'StartsWith',
    endsWith: 'EndsWith',
    greater: '>',
    less: '<',
    greaterOrEqual: '>=',
    lessOrEqual: '<=',
    between: 'between',
    inList: 'in'
  };
  function collect(g: FilterGroup): void {
    g.rules.forEach(r => {
      if (r.operator === 'between' && r.value != null && r.value2 != null) {
        result.push({ column: r.column, operator: '>=', value: r.value });
        result.push({ column: r.column, operator: '<=', value: r.value2 });
      } else if (r.operator === 'inList' && r.valueList?.length) {
        result.push({ column: r.column, operator: 'in', value: r.valueList });
      } else if (r.value !== undefined && r.value !== null && r.value !== '') {
        result.push({ column: r.column, operator: opMap[r.operator] || '=', value: r.value });
      }
    });
    g.groups?.forEach(collect);
  }
  collect(group);
  return result;
}

export interface SortColumn {
  column: string;
  direction: 'asc' | 'desc';
}

export interface TableState {
  tableId: string;
  filterRequest: FilterRequest;
  sortColumns: SortColumn[];
  visibleColumnKeys: string[];
  updatedAt?: number;
}

export interface SavedView {
  id: string;
  tableId: string;
  name: string;
  state: Partial<TableState>;
  isDefault: boolean;
  createdAt: number;
}

/** Column definition for filter builder (key, label, type for value input). */
export interface FilterBuilderColumn {
  key: string;
  label: string;
  type?: 'text' | 'number' | 'date' | 'boolean';
  options?: { label: string; value: any }[];
}

/** Build a single rule id. */
export function newRuleId(): string {
  return 'r_' + Math.random().toString(36).slice(2, 11);
}

/** Build a single group id. */
export function newGroupId(): string {
  return 'g_' + Math.random().toString(36).slice(2, 11);
}

/** Create empty FilterGroup with one empty rule. */
export function createEmptyFilterGroup(): FilterGroup {
  return {
    id: newGroupId(),
    logic: 'and',
    rules: [{ id: newRuleId(), column: '', operator: 'equals', value: '' }]
  };
}

/** Convert FilterDto[] to a single FilterGroup (flat AND). Lossy for OR logic. */
export function filterDtosToGroup(dtos: FilterDto[]): FilterGroup {
  const opToEnum: Record<string, FilterOperator> = {
    '=': 'equals', '!=': 'notEquals', '>': 'greater', '<': 'less',
    '>=': 'greaterOrEqual', '<=': 'lessOrEqual',
    'Contains': 'contains', 'StartsWith': 'startsWith', 'EndsWith': 'endsWith',
    'in': 'inList'
  };
  const rules: FilterRule[] = dtos.map((d, i) => ({
    id: newRuleId(),
    column: d.column,
    operator: opToEnum[d.operator] || 'equals',
    value: Array.isArray(d.value) ? undefined : d.value,
    valueList: Array.isArray(d.value) ? d.value : undefined
  }));
  if (rules.length === 0) rules.push({ id: newRuleId(), column: '', operator: 'equals', value: '' });
  return { id: newGroupId(), logic: 'and', rules };
}
