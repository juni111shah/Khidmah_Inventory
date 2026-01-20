import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IconComponent } from '../icon/icon.component';
import { ChartComponent, ChartData } from '../chart/chart.component';

export interface StatCardData {
  value: number;
  label?: string;
  period?: string;
}

export interface StatBarData {
  label: string;
  value: number;
  average?: number;
}

export type TimeFrame = 'day' | 'week' | 'month';

@Component({
  selector: 'app-stat-card',
  standalone: true,
  imports: [CommonModule, IconComponent, ChartComponent],
  templateUrl: './stat-card.component.html'
})
export class StatCardComponent implements OnInit, OnChanges {
  @Input() title: string = '';
  @Input() icon: string = '';
  @Input() iconColor: string = '#ff69b4';
  @Input() source: string = '';
  @Input() range: string = ''; // e.g., "42-175 bpm"
  @Input() period: string = ''; // e.g., "November 2025"
  @Input() barData: StatBarData[] = [];
  @Input() showFullStats: boolean = true;
  @Input() unit: string = ''; // e.g., "bpm", "units", etc.
  @Input() selectedTimeFrame: TimeFrame = 'month';

  @Output() timeFrameChange = new EventEmitter<TimeFrame>();
  @Output() fullStatsClick = new EventEmitter<void>();
  @Output() menuClick = new EventEmitter<void>();

  maxValue: number = 0;
  averageValue: number = 0;
  chartData: ChartData | null = null;

  // Chart options for ApexCharts
  chartOptions: any = {
    chart: {
      type: 'bar',
      height: 200,
      toolbar: { show: false },
      animations: { enabled: true, easing: 'easeInOutQuart', speed: 1000 }
    },
    plotOptions: {
      bar: {
        borderRadius: 4,
        columnWidth: '60%',
        distributed: false
      }
    },
    colors: ['#0d6efd'],
    dataLabels: { enabled: false },
    xaxis: {
      categories: [],
      labels: { style: { fontSize: '11px' } }
    },
    yaxis: {
      labels: { style: { fontSize: '11px' } },
      title: { text: 'Performance' }
    },
    grid: {
      show: false
    },
    tooltip: {
      theme: 'light',
      y: {
        formatter: (value: number) => value.toString()
      }
    }
  };

  ngOnInit(): void {
    this.calculateStats();
  }

  ngOnChanges(): void {
    this.calculateStats();
  }

  private calculateStats(): void {
    if (this.barData && this.barData.length > 0) {
      this.maxValue = Math.max(...this.barData.map(d => d.value), 1);
      const sum = this.barData.reduce((acc, d) => acc + d.value, 0);
      this.averageValue = Math.round(sum / this.barData.length);

      // Create chart data for ApexCharts
      this.chartData = {
        labels: this.barData.map(d => d.label),
        datasets: [{
          label: 'Performance',
          data: this.barData.map(d => d.value),
          backgroundColor: '#0d6efd',
          borderColor: '#0d6efd',
          borderWidth: 0
        }]
      };

      // Update chart options with labels
      this.chartOptions = {
        ...this.chartOptions,
        xaxis: {
          ...this.chartOptions.xaxis,
          categories: this.barData.map(d => d.label)
        }
      };
    } else {
      this.chartData = null;
    }
  }

  selectTimeFrame(frame: TimeFrame): void {
    this.selectedTimeFrame = frame;
    this.timeFrameChange.emit(frame);
  }

  onFullStatsClick(): void {
    this.fullStatsClick.emit();
  }

  onMenuClick(): void {
    this.menuClick.emit();
  }

}


