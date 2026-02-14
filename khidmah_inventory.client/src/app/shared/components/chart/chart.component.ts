import { Component, Input, OnInit, OnChanges, SimpleChanges, ViewChild, ChangeDetectionStrategy, ChangeDetectorRef, AfterViewInit, OnDestroy, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ChartComponent as ApexChartComponent,
  ChartType,
  ApexOptions,
  NgApexchartsModule
} from 'ng-apexcharts';
import { CHART_COLORS, getChartColors } from '../../constants/chart-colors';

export interface ChartData {
  labels: string[];
  datasets: ChartDataset[];
}

export interface ChartDataset {
  label: string;
  data: number[];
  backgroundColor?: string | string[];
  borderColor?: string | string[];
  borderWidth?: number;
  fill?: boolean;
  tension?: number;
  type?: string;
}

@Component({
  selector: 'app-chart',
  standalone: true,
  imports: [CommonModule, NgApexchartsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="chart-container" #chartContainer [style.height.px]="height">
      <apx-chart
        *ngIf="hasValidData"
        #chart
        [series]="chartSeries"
        [chart]="chartConfig"
        [xaxis]="xaxisConfig"
        [yaxis]="yaxisConfig"
        [colors]="colorsConfig"
        [stroke]="strokeConfig"
        [fill]="fillConfig"
        [legend]="legendConfig"
        [tooltip]="tooltipConfig"
        [dataLabels]="dataLabelsConfig"
        [plotOptions]="plotOptionsConfig"
        [responsive]="responsiveConfig"
        [title]="titleConfig"
        [labels]="labelsConfig"
      ></apx-chart>
      <div *ngIf="!hasValidData" class="chart-placeholder">
        <p>No data available</p>
      </div>
    </div>
  `,
  styles: [`
    .chart-container {
      position: relative;
      width: 100%;
      min-width: 0;
      box-sizing: border-box;
      overflow: visible;
      flex: 1;
      display: flex;
      flex-direction: column;
    }
    .chart-placeholder {
      display: flex;
      align-items: center;
      justify-content: center;
      height: 100%;
      color: #999;
      flex: 1;
    }
    :host {
      display: block;
      width: 100%;
      min-width: 0;
      flex: 1;
      box-sizing: border-box;
    }
    apx-chart {
      width: 100% !important;
      display: block;
    }
  `]
})
export class ChartComponent implements OnInit, OnChanges, AfterViewInit, OnDestroy {
  @ViewChild('chart', { static: false }) chart!: ApexChartComponent;
  @ViewChild('chartContainer', { static: false }) chartContainer!: ElementRef<HTMLDivElement>;
  @Input() type: string = 'line';
  @Input() data: ChartData | null = null;
  @Input() options: any = {};
  @Input() height: number = 300;

  private resizeObserver?: ResizeObserver;
  private resizeTimeoutId: ReturnType<typeof setTimeout> | null = null;
  private readonly RESIZE_DEBOUNCE_MS = 150;

  chartSeries: any = [];
  chartOptions: Partial<ApexOptions> = {};
  hasValidData: boolean = false;

  // Cached config properties to prevent change detection loops
  chartConfig: any;
  xaxisConfig: any;
  yaxisConfig: any;
  colorsConfig: string[] = [];
  strokeConfig: any;
  fillConfig: any;
  legendConfig: any;
  tooltipConfig: any;
  dataLabelsConfig: any;
  plotOptionsConfig: any;
  responsiveConfig: any[] = [];
  titleConfig: any;
  labelsConfig: string[] = [];

  // Cached default objects to ensure same reference
  private defaultChart: any;
  private defaultXAxis: any;
  private defaultYAxis: any;
  private defaultStroke: any;
  private defaultFill: any;
  private defaultLegend: any;
  private defaultTooltip: any;
  private defaultDataLabels: any;

  constructor(private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.initializeDefaults();
    this.updateCachedConfigs();
    this.updateChart();
  }

  ngAfterViewInit(): void {
    // Wait for the next tick to ensure chart is initialized
    setTimeout(() => {
      this.setupResizeObserver();
    }, 100);
  }

  private setupResizeObserver(): void {
    // Set up ResizeObserver to detect container size changes (debounced to avoid resize loops)
    if (typeof ResizeObserver !== 'undefined' && this.chartContainer) {
      this.resizeObserver = new ResizeObserver(() => {
        this.scheduleResize();
      });

      this.resizeObserver.observe(this.chartContainer.nativeElement);
    }

    // Also listen to window resize events as a fallback
    window.addEventListener('resize', this.handleWindowResize);
  }

  private scheduleResize(): void {
    if (this.resizeTimeoutId != null) clearTimeout(this.resizeTimeoutId);
    this.resizeTimeoutId = setTimeout(() => {
      this.resizeTimeoutId = null;
      this.resizeChartImmediate();
    }, this.RESIZE_DEBOUNCE_MS);
  }

  ngOnDestroy(): void {
    if (this.resizeTimeoutId != null) clearTimeout(this.resizeTimeoutId);
    this.resizeTimeoutId = null;
    if (this.resizeObserver) {
      this.resizeObserver.disconnect();
    }
    window.removeEventListener('resize', this.handleWindowResize);
  }

  /** Debounced entry point – use from ResizeObserver and window resize. */
  private resizeChart(): void {
    this.scheduleResize();
  }

  private resizeChartImmediate(): void {
    if (this.chart && this.chartContainer) {
      try {
        if (this.chart.chart) {
          const containerWidth = this.chartContainer.nativeElement.offsetWidth;
          if (containerWidth > 0) {
            const apexChart = (this.chart as any).chart;
            if (apexChart && typeof apexChart.resize === 'function') {
              apexChart.resize();
            }
          }
        }
      } catch {
        // ignore resize errors
      }
    }
  }

  private handleWindowResize = (): void => {
    this.scheduleResize();
  };

  private initializeDefaults(): void {
    this.defaultChart = {
      type: 'line',
      height: this.height,
      toolbar: { show: true },
      zoom: { enabled: true }
    };
    this.defaultXAxis = {
      categories: [],
      labels: { style: { fontSize: '12px' } }
    };
    this.defaultYAxis = {
      labels: { style: { fontSize: '12px' }, formatter: (value: number) => value.toLocaleString() }
    };
    this.defaultStroke = {
      curve: 'smooth',
      width: 2
    };
    this.defaultFill = {
      type: 'gradient',
      gradient: {
        shadeIntensity: 1,
        opacityFrom: 0.7,
        opacityTo: 0.3,
        stops: [0, 90, 100]
      }
    };
    this.defaultLegend = {
      show: true,
      position: 'top',
      fontSize: '14px'
    };
    this.defaultTooltip = {
      enabled: true,
      shared: true,
      intersect: false
    };
    this.defaultDataLabels = {
      enabled: false
    };
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['data'] || changes['type'] || changes['options']) {
      this.updateChart();
    }
  }

  private updateChart(): void {
    if (!this.data || !this.data.datasets || this.data.datasets.length === 0) {
      // Initialize with empty data to prevent errors
      this.chartSeries = [];
      this.chartOptions = this.buildApexOptions([]);
      this.updateCachedConfigs();
      this.hasValidData = false;
      if (this.cdr) {
        this.cdr.markForCheck();
      }
      return;
    }

    // Validate that datasets have data arrays
    const hasValidDatasets = this.data.datasets.some(ds =>
      ds && Array.isArray(ds.data) && ds.data.length > 0
    );

    if (!hasValidDatasets) {
      this.chartSeries = [];
      this.chartOptions = this.buildApexOptions([]);
      this.updateCachedConfigs();
      this.hasValidData = false;
      if (this.cdr) {
        this.cdr.markForCheck();
      }
      return;
    }

    try {
      // Convert Chart.js format to ApexCharts format
      const labels = (this.data.labels || []).map(label =>
        label != null ? String(label) : ''
      ).filter(label => label !== '');

      // Convert datasets to ApexCharts series
      if (this.type === 'pie' || this.type === 'donut' || this.type === 'doughnut') {
        // For pie/donut charts, use first dataset as array of numbers
        const firstDataset = this.data.datasets[0];
        if (firstDataset && Array.isArray(firstDataset.data) && firstDataset.data.length > 0) {
          this.chartSeries = firstDataset.data.map(d => typeof d === 'number' ? d : 0);
          this.hasValidData = this.chartSeries.length > 0;
        } else {
          this.chartSeries = [];
          this.hasValidData = false;
        }
      } else {
        // For line/area/bar charts, return array of series objects
        // Don't filter out datasets - include all of them even if some values are zero
        this.chartSeries = this.data.datasets
          .filter(dataset => dataset && Array.isArray(dataset.data))
          .map(dataset => {
            const seriesData = dataset.data.map(d => {
              const num = typeof d === 'number' ? d : (typeof d === 'string' ? parseFloat(d) : 0);
              return isNaN(num) ? 0 : num;
            });

            return {
              name: dataset.label || 'Series',
              data: seriesData
            };
          });

        // Chart is valid if we have at least one series with data
        this.hasValidData = this.chartSeries.length > 0 &&
                            this.chartSeries.some((s: any) => s.data && s.data.length > 0) &&
                            labels.length > 0;
      }

      // Build ApexCharts options
      this.chartOptions = this.buildApexOptions(labels);

      // Update cached config properties to prevent change detection loops
      this.updateCachedConfigs();
      // Manually trigger change detection since we're using OnPush
      if (this.cdr) {
        this.cdr.markForCheck();
      }
      // Resize is handled by ResizeObserver when container is laid out; avoid calling resize here to prevent loops
    } catch (error) {
      console.error('Error updating chart:', error);
      // Initialize with empty data on error
      this.chartSeries = [];
      this.chartOptions = this.buildApexOptions([]);
      this.updateCachedConfigs();
      this.hasValidData = false;
      if (this.cdr) {
        this.cdr.markForCheck();
      }
    }
  }

  private updateCachedConfigs(): void {
    this.chartConfig = this.chartOptions.chart || this.defaultChart;
    this.xaxisConfig = this.chartOptions.xaxis || this.defaultXAxis;
    this.yaxisConfig = this.chartOptions.yaxis || this.defaultYAxis;
    this.colorsConfig = this.chartOptions.colors || [];
    this.strokeConfig = this.chartOptions.stroke || this.defaultStroke;
    this.fillConfig = this.chartOptions.fill || this.defaultFill;
    this.legendConfig = this.chartOptions.legend || this.defaultLegend;
    this.tooltipConfig = this.chartOptions.tooltip || this.defaultTooltip;
    this.dataLabelsConfig = this.chartOptions.dataLabels || this.defaultDataLabels;
    this.plotOptionsConfig = this.chartOptions.plotOptions;
    // Ensure responsive is always an array
    this.responsiveConfig = Array.isArray(this.chartOptions.responsive)
      ? this.chartOptions.responsive
      : (this.chartOptions.responsive ? [this.chartOptions.responsive] : []);
    this.titleConfig = this.chartOptions.title;
    this.labelsConfig = this.chartOptions.labels || [];
  }

  private buildApexOptions(labels: string[]): Partial<ApexOptions> {
    const isLineOrArea = this.type === 'line' || this.type === 'area';
    const baseOptions: Partial<ApexOptions> = {
      chart: {
        type: this.mapChartType(this.type),
        height: this.height,
        width: '100%',
        fontFamily: "'Bricolage Grotesque', sans-serif",
        toolbar: { show: false },
        zoom: { enabled: false },
        redrawOnParentResize: true,
        redrawOnWindowResize: true,
        animations: { enabled: true, easing: 'easeinout', speed: 700 },
        parentHeightOffset: 0,
        sparkline: { enabled: false },
        background: 'transparent',
        ...this.options?.chart
      },
      xaxis: {
        categories: labels.length > 0 ? labels : ['No Data'],
        axisBorder: { show: false },
        axisTicks: { show: false },
        labels: {
          style: { fontSize: '12px', colors: '#64748b', fontWeight: 500 },
          rotate: -45,
          rotateAlways: false,
          hideOverlappingLabels: true,
          trim: true
        },
        ...this.options?.xaxis
      },
      yaxis: {
        labels: {
          style: { fontSize: '12px', colors: '#64748b', fontWeight: 500 },
          formatter: (value: number) => {
            if (this.options?.yaxis?.labels?.formatter) {
              return this.options.yaxis.labels.formatter(value);
            }
            return value.toLocaleString();
          }
        },
        ...this.options?.yaxis
      },
      grid: {
        borderColor: '#e2e8f0',
        strokeDashArray: 3,
        padding: { left: 12, right: 12, top: 8, bottom: 4 },
        xaxis: { lines: { show: false } },
        yaxis: { lines: { show: true } },
        ...this.options?.grid
      },
      legend: {
        show: true,
        position: 'top',
        horizontalAlign: 'right',
        fontSize: '12px',
        fontWeight: 600,
        labels: { colors: '#334155' },
        markers: { radius: 6, width: 8, height: 8, strokeWidth: 0 },
        itemMargin: { horizontal: 10, vertical: 4 },
        ...this.options?.legend
      },
      tooltip: {
        enabled: true,
        theme: 'light',
        shared: true,
        intersect: false,
        style: { fontSize: '12px' },
        ...this.options?.tooltip
      },
      dataLabels: {
        enabled: false,
        ...this.options?.dataLabels
      },
      stroke: {
        curve: 'smooth',
        width: isLineOrArea ? 3 : 2,
        lineCap: 'round',
        ...this.options?.stroke
      },
      fill: {
        type: 'gradient',
        gradient: {
          shadeIntensity: 1,
          opacityFrom: isLineOrArea ? 0.55 : 0.75,
          opacityTo: isLineOrArea ? 0.08 : 0.35,
          stops: [0, 50, 100]
        },
        ...this.options?.fill
      },
      plotOptions: this.mergeBarPlotOptions(labels),
      responsive: this.options?.responsive || [{
        breakpoint: 768,
        options: {
          chart: { height: 250 }
        }
      }]
    };

    if (isLineOrArea) {
      baseOptions.markers = {
        size: 4,
        strokeWidth: 2,
        strokeColors: '#fff',
        hover: { size: 6 }
      };
    }

    // Handle chart type specific options
    const isDonut = this.type === 'donut' || this.type === 'doughnut';
    if (this.type === 'pie' || this.type === 'donut' || this.type === 'doughnut') {
      baseOptions.labels = labels;
      if (!baseOptions.plotOptions) baseOptions.plotOptions = {};
      baseOptions.plotOptions.pie = {
        donut: { size: isDonut ? '70%' : '0%', labels: { show: false } },
        expandOnClick: true,
        ...this.options?.plotOptions?.pie
      };
      baseOptions.stroke = { show: true, width: 2, colors: ['#fff'] };
    }

    // Set colors from datasets or app palette
    const isPieDonut = this.type === 'pie' || this.type === 'donut' || this.type === 'doughnut';
    const isBarDistributed = this.type === 'bar' && labels.length > 1 && (this.data?.datasets?.length === 1);
    if (this.data?.datasets) {
      const first = this.data.datasets[0];
      if (isPieDonut && first && Array.isArray(first.backgroundColor) && first.backgroundColor.length > 0) {
        baseOptions.colors = (first.backgroundColor as string[]).map(c => this.rgbaToHex(c));
      } else if (isBarDistributed && first && Array.isArray(first.backgroundColor) && first.backgroundColor.length > 0) {
        baseOptions.colors = (first.backgroundColor as string[]).map(c => this.rgbaToHex(c));
      } else if (!isBarDistributed) {
        const colors = this.data.datasets.map((ds, i) => {
          if (ds.borderColor && typeof ds.borderColor === 'string') return this.rgbaToHex(ds.borderColor);
          if (ds.backgroundColor && typeof ds.backgroundColor === 'string') return this.rgbaToHex(ds.backgroundColor);
          return CHART_COLORS[i % CHART_COLORS.length];
        });
        baseOptions.colors = colors;
      }
    }
    if (!baseOptions.colors || (Array.isArray(baseOptions.colors) && baseOptions.colors.length === 0)) {
      baseOptions.colors = getChartColors(Math.max(labels.length, 1));
    }

    return baseOptions;
  }

  private mergeBarPlotOptions(labels: string[]): any {
    const defaultBar = {
      borderRadius: 8,
      borderRadiusApplication: 'around' as const,
      columnWidth: '58%',
      distributed: this.type === 'bar' && labels.length > 1
    };
    const fromOptions = this.options?.plotOptions?.bar;
    if (!fromOptions) return { bar: defaultBar, ...this.options?.plotOptions };
    const merged = { ...defaultBar, ...fromOptions };
    const cw = merged.columnWidth;
    const num = typeof cw === 'string' ? parseFloat(cw) : 0;
    if (typeof cw === 'string' && !isNaN(num) && num < 45) {
      merged.columnWidth = '50%';
    }
    return { bar: merged, ...this.options?.plotOptions };
  }

  private mapChartType(type: string): ChartType {
    const typeMap: { [key: string]: ChartType } = {
      'line': 'line',
      'bar': 'bar',
      'pie': 'pie',
      'donut': 'donut',
      'doughnut': 'donut',
      'area': 'area',
      'radar': 'radar'
    };
    return typeMap[type] || 'line';
  }

  private rgbaToHex(rgba: string): string {
    // Convert rgba to hex
    if (rgba.startsWith('#')) {
      return rgba;
    }

    const match = rgba.match(/rgba?\((\d+),\s*(\d+),\s*(\d+)(?:,\s*[\d.]+)?\)/);
    if (match) {
      const r = parseInt(match[1]);
      const g = parseInt(match[2]);
      const b = parseInt(match[3]);
      return '#' + [r, g, b].map(x => {
        const hex = x.toString(16);
        return hex.length === 1 ? '0' + hex : hex;
      }).join('');
    }

    return CHART_COLORS[0];
  }

}

