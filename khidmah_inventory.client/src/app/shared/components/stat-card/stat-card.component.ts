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
      animations: { enabled: true, easing: 'easeinout', speed: 800 },
      sparkline: { enabled: false },
      background: 'transparent'
    },
    plotOptions: {
      bar: {
        borderRadius: 20,
        borderRadiusApplication: 'around', // Rounded on both ends
        columnWidth: '35%',
        distributed: false
      }
    },
    colors: ['#f43f5e'], // Pink/Rose as in image
    dataLabels: { enabled: false },
    xaxis: {
      categories: [],
      labels: {
        style: { fontSize: '10px', colors: '#64748b', fontWeight: 500 }
      },
      axisBorder: { show: false },
      axisTicks: { show: false }
    },
    yaxis: {
      labels: {
        style: { fontSize: '10px', colors: '#64748b', fontWeight: 500 }
      },
      tickAmount: 4
    },
    grid: {
      show: true,
      borderColor: '#f1f5f9',
      strokeDashArray: 4,
      xaxis: { lines: { show: false } },
      yaxis: { lines: { show: true } }
    },
    fill: {
      type: 'gradient',
      gradient: {
        shade: 'light',
        type: 'vertical',
        shadeIntensity: 0.5,
        gradientToColors: ['#fb7185'], // Lighter pink
        inverseColors: true,
        opacityFrom: 1,
        opacityTo: 0.7,
        stops: [0, 100]
      }
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


