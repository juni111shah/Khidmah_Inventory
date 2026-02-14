import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IconComponent } from '../icon/icon.component';
import { ChartComponent, ChartData } from '../chart/chart.component';
import { CHART_PRIMARY } from '../../constants/chart-colors';

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
  @Input() chartHeight: number = 180;
  /** When true, renders a horizontal bar chart (better for many categories or long labels). */
  @Input() horizontalBars: boolean = false;

  @Output() timeFrameChange = new EventEmitter<TimeFrame>();
  @Output() fullStatsClick = new EventEmitter<void>();
  @Output() menuClick = new EventEmitter<void>();

  maxValue: number = 0;
  averageValue: number = 0;
  chartData: ChartData | null = null;

  // Chart options for ApexCharts (bar width and horizontal flag updated in calculateStats)
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
        borderRadius: 12,
        borderRadiusApplication: 'around',
        columnWidth: '65%',
        barHeight: '70%',
        distributed: false
      }
    },
    colors: ['#0d9488'], // Teal – clear and easy on the eyes
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
      padding: { top: 0, right: 8, bottom: 0, left: 0 },
      xaxis: { lines: { show: false } },
      yaxis: { lines: { show: true } }
    },
    fill: {
      type: 'gradient',
      gradient: {
        shade: 'light',
        type: 'vertical',
        shadeIntensity: 0.5,
        gradientToColors: ['#14b8a6'],
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

      const n = this.barData.length;
      // Wider bars for fewer categories so they're clearly visible
      const columnWidth = n <= 4 ? '70%' : n <= 8 ? '60%' : '50%';
      const barHeight = this.horizontalBars ? '70%' : undefined;

      // Create chart data for ApexCharts
      this.chartData = {
        labels: this.barData.map(d => d.label),
        datasets: [{
          label: 'Performance',
          data: this.barData.map(d => d.value),
          backgroundColor: CHART_PRIMARY,
          borderColor: CHART_PRIMARY,
          borderWidth: 0
        }]
      };

      const gradientType = this.horizontalBars ? 'horizontal' : 'vertical';
      const fillOpts = {
        type: 'gradient' as const,
        gradient: {
          shade: 'light',
          type: gradientType,
          shadeIntensity: 0.5,
          gradientToColors: ['#14b8a6'],
          inverseColors: true,
          opacityFrom: 1,
          opacityTo: 0.7,
          stops: [0, 100]
        }
      };

      // Update chart options with labels, bar width, and horizontal when needed
      this.chartOptions = {
        ...this.chartOptions,
        chart: {
          ...this.chartOptions.chart,
          horizontal: this.horizontalBars
        },
        plotOptions: {
          ...this.chartOptions.plotOptions,
          bar: {
            ...this.chartOptions.plotOptions.bar,
            columnWidth,
            ...(barHeight ? { barHeight } : {})
          }
        },
        fill: fillOpts,
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


