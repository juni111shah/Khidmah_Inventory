import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TimeRangeType } from '../../../core/models/analytics.model';

@Component({
  selector: 'app-time-range-filter',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './time-range-filter.component.html'
})
export class TimeRangeFilterComponent implements OnInit {
  @Input() selectedRange: TimeRangeType = TimeRangeType.Last30Days;
  @Input() customFromDate?: string;
  @Input() customToDate?: string;
  @Output() rangeChange = new EventEmitter<{
    range: TimeRangeType;
    fromDate?: string;
    toDate?: string;
  }>();

  TimeRangeType = TimeRangeType;
  timeRanges = [
    { value: TimeRangeType.Today, label: 'Today' },
    { value: TimeRangeType.Yesterday, label: 'Yesterday' },
    { value: TimeRangeType.Last7Days, label: 'Last 7 Days' },
    { value: TimeRangeType.Last30Days, label: 'Last 30 Days' },
    { value: TimeRangeType.ThisMonth, label: 'This Month' },
    { value: TimeRangeType.LastMonth, label: 'Last Month' },
    { value: TimeRangeType.ThisQuarter, label: 'This Quarter' },
    { value: TimeRangeType.LastQuarter, label: 'Last Quarter' },
    { value: TimeRangeType.ThisYear, label: 'This Year' },
    { value: TimeRangeType.LastYear, label: 'Last Year' },
    { value: TimeRangeType.Custom, label: 'Custom Range' }
  ];

  showCustomDates = false;

  ngOnInit(): void {
    this.showCustomDates = this.selectedRange === TimeRangeType.Custom;
  }

  onRangeChange(): void {
    this.showCustomDates = this.selectedRange === TimeRangeType.Custom;
    this.emitChange();
  }

  onCustomDateChange(): void {
    if (this.selectedRange === TimeRangeType.Custom) {
      this.emitChange();
    }
  }

  private emitChange(): void {
    this.rangeChange.emit({
      range: this.selectedRange,
      fromDate: this.selectedRange === TimeRangeType.Custom ? this.customFromDate : undefined,
      toDate: this.selectedRange === TimeRangeType.Custom ? this.customToDate : undefined
    });
  }
}


