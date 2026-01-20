import { Component, Input, OnInit, OnChanges, SimpleChanges, ViewChild, ChangeDetectionStrategy, ChangeDetectorRef, AfterViewInit, OnDestroy, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ChartComponent as ApexChartComponent,
  ChartType,
  ApexOptions,
  NgApexchartsModule
} from 'ng-apexcharts';

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
    // Initialize default objects (cached to prevent change detection loops)
    this.initializeDefaults();
    // Initialize with default configs
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
    // Set up ResizeObserver to detect container size changes
    if (typeof ResizeObserver !== 'undefined' && this.chartContainer) {
      this.resizeObserver = new ResizeObserver(() => {
        // Trigger chart resize when container size changes
        this.resizeChart();
      });
      
      this.resizeObserver.observe(this.chartContainer.nativeElement);
    }

    // Also listen to window resize events as a fallback
    window.addEventListener('resize', this.handleWindowResize);
  }

  ngOnDestroy(): void {
    // Clean up ResizeObserver
    if (this.resizeObserver) {
      this.resizeObserver.disconnect();
    }
    // Remove window resize listener
    window.removeEventListener('resize', this.handleWindowResize);
  }

  private resizeChart(): void {
    if (this.chart && this.chartContainer) {
      // Use ApexCharts resize method which is more reliable
      setTimeout(() => {
        try {
          if (this.chart.chart) {
            // Get the actual container width
            const containerWidth = this.chartContainer.nativeElement.offsetWidth;
            if (containerWidth > 0) {
              // Use ApexCharts resize method - this is the proper way to resize
              const apexChart = (this.chart as any).chart;
              if (apexChart && typeof apexChart.resize === 'function') {
                apexChart.resize();
              }
            }
          }
        } catch (error) {
          console.warn('Error resizing chart:', error);
        }
      }, 50);
    }
  }

  private handleWindowResize = (): void => {
    this.resizeChart();
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
    console.log('Chart updateChart called:', {
      hasData: !!this.data,
      datasetsCount: this.data?.datasets?.length || 0,
      labelsCount: this.data?.labels?.length || 0,
      type: this.type
    });
    
    if (!this.data || !this.data.datasets || this.data.datasets.length === 0) {
      // Initialize with empty data to prevent errors
      console.warn('Chart has no data or empty datasets');
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
      console.warn('Chart datasets have no valid data arrays');
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
      if (this.type === 'pie' || this.type === 'donut') {
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
        
        // Debug logging
        console.log('Chart series conversion:', {
          inputDatasets: this.data.datasets.map(ds => ({
            label: ds.label,
            dataLength: ds.data?.length,
            data: ds.data
          })),
          outputSeries: this.chartSeries,
          labels: labels,
          hasValidData: this.hasValidData
        });
        
        if (!this.hasValidData) {
          console.warn('Chart has no valid data:', {
            datasets: this.data.datasets,
            chartSeries: this.chartSeries,
            labels: labels
          });
        }
      }

      // Build ApexCharts options
      this.chartOptions = this.buildApexOptions(labels);
      
      // Update cached config properties to prevent change detection loops
      this.updateCachedConfigs();
      // Manually trigger change detection since we're using OnPush
      if (this.cdr) {
        this.cdr.markForCheck();
      }
      // Trigger resize after chart update to ensure it fits container
      setTimeout(() => {
        this.resizeChart();
      }, 100);
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
    const baseOptions: Partial<ApexOptions> = {
      chart: {
        type: this.mapChartType(this.type),
        height: this.height,
        width: '100%', // Ensure chart expands to fill container
        toolbar: {
          show: true
        },
        zoom: {
          enabled: true
        },
        // Enable automatic resizing
        redrawOnParentResize: true,
        redrawOnWindowResize: true,
        // Ensure chart is responsive
        animations: {
          enabled: true
        },
        // Ensure chart uses full width of container
        parentHeightOffset: 0,
        // Make chart fluid and responsive
        sparkline: {
          enabled: false
        }
      },
      xaxis: {
        categories: labels.length > 0 ? labels : ['No Data'],
        labels: {
          style: {
            fontSize: '12px'
          }
        }
      },
      yaxis: {
        labels: {
          style: {
            fontSize: '12px'
          },
          formatter: (value: number) => {
            try {
              // Apply custom formatter from options if available
              if (this.options?.scales?.y?.ticks?.callback && typeof this.options.scales.y.ticks.callback === 'function') {
                return this.options.scales.y.ticks.callback(value);
              }
              return value.toLocaleString();
            } catch (error) {
              return value.toLocaleString();
            }
          }
        }
      },
      legend: {
        show: true,
        position: 'top',
        fontSize: '14px'
      },
      tooltip: {
        enabled: true,
        shared: true,
        intersect: false
      },
      dataLabels: {
        enabled: false
      },
      stroke: {
        curve: 'smooth',
        width: 2
      },
      fill: {
        type: 'gradient',
        gradient: {
          shadeIntensity: 1,
          opacityFrom: 0.7,
          opacityTo: 0.3,
          stops: [0, 90, 100]
        }
      },
      responsive: [{
        breakpoint: 768,
        options: {
          chart: {
            height: 250
          }
        }
      }]
    };

    // Handle chart type specific options
    if (this.type === 'pie' || this.type === 'donut') {
      baseOptions.labels = labels;
      baseOptions.plotOptions = {
        pie: {
          donut: {
            size: this.type === 'donut' ? '70%' : '0%'
          }
        }
      };
    }

    // Merge with custom options
    if (this.options) {
      // Handle colors - ensure we have colors for all series
      if (this.data?.datasets) {
        const colors = this.data.datasets.flatMap(ds => {
          if (Array.isArray(ds.borderColor)) {
            // Use borderColor if available (usually more visible)
            return ds.borderColor.map((color: string) => this.rgbaToHex(color));
          } else if (ds.borderColor) {
            return [this.rgbaToHex(ds.borderColor)];
          } else if (Array.isArray(ds.backgroundColor)) {
            return ds.backgroundColor.map((color: string) => this.rgbaToHex(color));
          } else if (ds.backgroundColor) {
            return [this.rgbaToHex(ds.backgroundColor)];
          }
          return [];
        });
        if (colors.length > 0) {
          baseOptions.colors = colors;
          console.log('Chart colors set:', colors);
        } else {
          // Default colors if none provided
          baseOptions.colors = ['#36A2EB', '#FF6384', '#4BC0C0', '#FFCE56', '#9966FF', '#FF9F40'];
          console.log('Using default chart colors');
        }
      }

      // Handle fill option for area charts
      if (this.type === 'area') {
        // For area charts, ensure gradient fill is enabled
        baseOptions.fill = {
          type: 'gradient',
          gradient: {
            shadeIntensity: 1,
            opacityFrom: 0.7,
            opacityTo: 0.3,
            stops: [0, 90, 100]
          }
        };
      } else if (this.data?.datasets?.[0]?.fill === false) {
        baseOptions.fill = {
          type: 'solid',
          opacity: 0
        };
      }

      // Merge other options
      // Ensure responsive is always an array (ApexCharts requirement)
      if (this.options.responsive !== undefined) {
        if (Array.isArray(this.options.responsive)) {
          baseOptions.responsive = this.options.responsive;
        } else if (this.options.responsive === true || this.options.responsive === false) {
          // Ignore boolean values - ApexCharts is responsive by default
          // Only set responsive if it's an array of breakpoint configs
        } else {
          // If it's an object, wrap it in an array
          baseOptions.responsive = [this.options.responsive];
        }
      }
      if (this.options.title) {
        baseOptions.title = {
          text: this.options.title,
          style: {
            fontSize: '16px',
            fontWeight: 'bold'
          }
        };
      }
    }

    return baseOptions;
  }

  private mapChartType(type: string): ChartType {
    const typeMap: { [key: string]: ChartType } = {
      'line': 'line',
      'bar': 'bar',
      'pie': 'pie',
      'donut': 'donut',
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
    
    return '#36A2EB'; // Default color
  }

}

