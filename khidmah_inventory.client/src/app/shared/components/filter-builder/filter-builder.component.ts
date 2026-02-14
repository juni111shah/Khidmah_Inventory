import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  FilterGroup,
  FilterRule,
  FilterOperator,
  FilterBuilderColumn,
  filterGroupToFilterDtos,
  createEmptyFilterGroup,
  filterDtosToGroup,
  newRuleId,
  newGroupId
} from '../../../core/models/table-state.model';
import { FilterDto } from '../../../core/models/user.model';
import { UnifiedButtonComponent } from '../unified-button/unified-button.component';
import { IconComponent } from '../icon/icon.component';

const OPERATORS: { value: FilterOperator; label: string; needsValue2?: boolean; needsList?: boolean }[] = [
  { value: 'equals', label: 'Equals' },
  { value: 'notEquals', label: 'Not equals' },
  { value: 'contains', label: 'Contains' },
  { value: 'startsWith', label: 'Starts with' },
  { value: 'endsWith', label: 'Ends with' },
  { value: 'greater', label: 'Greater than' },
  { value: 'less', label: 'Less than' },
  { value: 'greaterOrEqual', label: 'Greater or equal' },
  { value: 'lessOrEqual', label: 'Less or equal' },
  { value: 'between', label: 'Between', needsValue2: true },
  { value: 'inList', label: 'In list', needsList: true }
];

@Component({
  selector: 'app-filter-builder',
  standalone: true,
  imports: [CommonModule, FormsModule, UnifiedButtonComponent, IconComponent],
  templateUrl: './filter-builder.component.html',
  styleUrls: ['./filter-builder.component.scss']
})
export class FilterBuilderComponent implements OnChanges {
  @Input() columns: FilterBuilderColumn[] = [];
  @Input() initialFilters: FilterDto[] = [];
  @Input() group: FilterGroup | null = null;

  @Output() apply = new EventEmitter<FilterDto[]>();
  @Output() clear = new EventEmitter<void>();

  logic: 'and' | 'or' = 'and';
  rules: FilterRule[] = [];
  operators = OPERATORS;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['group'] && this.group) {
      this.logic = this.group.logic;
      this.rules = this.group.rules?.length ? this.group.rules.map(r => ({ ...r })) : [this.newRule()];
    } else if (changes['initialFilters'] && this.initialFilters?.length) {
      const g = filterDtosToGroup(this.initialFilters);
      this.logic = g.logic;
      this.rules = g.rules.length ? g.rules : [this.newRule()];
    } else if (!this.rules.length) {
      this.rules = [this.newRule()];
    }
  }

  getColumnLabel(key: string): string {
    return this.columns.find(c => c.key === key)?.label ?? key;
  }

  getColumnOptions(columnKey: string): { label: string; value: any }[] {
    return this.columns.find(c => c.key === columnKey)?.options ?? [];
  }

  columnHasOptions(columnKey: string): boolean {
    const opts = this.getColumnOptions(columnKey);
    return opts.length > 0;
  }

  getColumnType(columnKey: string): 'text' | 'number' | 'date' | 'boolean' | undefined {
    return this.columns.find(c => c.key === columnKey)?.type;
  }

  getOperatorsForColumn(columnKey: string): typeof OPERATORS {
    const col = this.columns.find(c => c.key === columnKey);
    const t = col?.type ?? 'text';
    if (t === 'number' || t === 'date') {
      return OPERATORS.filter(op => ['equals', 'notEquals', 'greater', 'less', 'greaterOrEqual', 'lessOrEqual', 'between'].includes(op.value));
    }
    if (t === 'boolean') {
      return OPERATORS.filter(op => op.value === 'equals' || op.value === 'notEquals');
    }
    return OPERATORS;
  }

  newRule(): FilterRule {
    return { id: newRuleId(), column: '', operator: 'equals', value: '' };
  }

  addRule(): void {
    this.rules.push(this.newRule());
  }

  removeRule(index: number): void {
    this.rules.splice(index, 1);
    if (this.rules.length === 0) this.rules.push(this.newRule());
  }

  onOperatorChange(rule: FilterRule): void {
    if (rule.operator === 'between') {
      rule.value2 = rule.value2 ?? rule.value;
    }
    if (rule.operator === 'inList') {
      rule.valueList = Array.isArray(rule.valueList) ? rule.valueList : (rule.value ? [rule.value] : []);
      rule.value = undefined;
    } else {
      rule.valueList = undefined;
      rule.value2 = undefined;
    }
  }

  getCurrentGroup(): FilterGroup {
    return {
      id: newGroupId(),
      logic: this.logic,
      rules: this.rules.filter(r => r.column)
    };
  }

  applyFilters(): void {
    const g = this.getCurrentGroup();
    const dtos = filterGroupToFilterDtos(g);
    this.apply.emit(dtos);
  }

  clearFilters(): void {
    this.rules = [this.newRule()];
    this.logic = 'and';
    this.clear.emit();
  }

  getInListDisplay(rule: FilterRule): string {
    return (rule.valueList || []).join(', ');
  }

  setInListValue(rule: FilterRule, str: string): void {
    rule.valueList = str.split(',').map(s => s.trim()).filter(Boolean);
  }

  trackByRuleId(_: number, r: FilterRule): string {
    return r.id;
  }
}
