import { Component, Input, OnChanges, SimpleChanges, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-sparkline-chart',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sparkline-chart.component.html'
})
export class SparklineChartComponent implements OnInit, OnChanges {
  @Input() data: number[] = [];
  @Input() color: string = '#1976d2';
  @Input() height: number = 40;
  @Input() width: number = 100;
  @Input() showArea: boolean = true;
  @Input() showBars: boolean = true; // Show vertical bars

  path: string = '';
  areaPath: string = '';
  viewBox: string = '';
  gradientId: string = '';
  bars: Array<{x: number, y: number, width: number, height: number}> = [];

  ngOnInit(): void {
    this.gradientId = 'gradient-' + this.color.replace(/[^a-zA-Z0-9]/g, '') + '-' + Math.random().toString(36).substring(2, 11);
    this.generateChart();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['color']) {
      this.gradientId = 'gradient-' + this.color.replace(/[^a-zA-Z0-9]/g, '') + '-' + Math.random().toString(36).substring(2, 11);
    }
    if (changes['data'] || changes['width'] || changes['height'] || changes['color']) {
      this.generateChart();
    }
  }

  private generateChart(): void {
    if (!this.data || this.data.length === 0) {
      // Generate sample data if none provided
      this.data = this.generateSampleData();
    }

    const width = this.width;
    const height = this.height;
    const padding = 2;
    const chartWidth = width - padding * 2;
    const chartHeight = height - padding * 2;

    this.viewBox = `0 0 ${width} ${height}`;

    const min = Math.min(...this.data);
    const max = Math.max(...this.data);
    const range = max - min || 1; // Avoid division by zero

    // Calculate points
    const points: Array<{x: number, y: number}> = [];
    this.data.forEach((value, index) => {
      const x = padding + (index / (this.data.length - 1 || 1)) * chartWidth;
      const normalizedValue = (value - min) / range;
      const y = padding + chartHeight - (normalizedValue * chartHeight);
      points.push({ x, y });
    });

    // Generate smooth curve using Catmull-Rom spline
    this.path = this.generateSmoothPath(points);
    
    // Generate area path for gradient
    const areaPoints: string[] = [`M ${padding} ${height - padding}`];
    points.forEach(point => {
      areaPoints.push(`L ${point.x},${point.y}`);
    });
    areaPoints.push(`L ${width - padding} ${height - padding} Z`);
    this.areaPath = areaPoints.join(' ');

    // Generate bar data
    if (this.showBars) {
      this.bars = [];
      const barWidth = chartWidth / this.data.length * 0.6; // 60% of available space per bar
      const barGap = chartWidth / this.data.length * 0.4; // 40% gap
      
      this.data.forEach((value, index) => {
        const x = padding + (index / (this.data.length - 1 || 1)) * chartWidth - barWidth / 2;
        const normalizedValue = (value - min) / range;
        const barHeight = normalizedValue * chartHeight;
        const y = padding + chartHeight - barHeight;
        
        this.bars.push({
          x: x,
          y: y,
          width: barWidth,
          height: barHeight
        });
      });
    }
  }

  private generateSmoothPath(points: Array<{x: number, y: number}>): string {
    if (points.length < 2) return '';
    if (points.length === 2) {
      return `M ${points[0].x},${points[0].y} L ${points[1].x},${points[1].y}`;
    }

    let path = `M ${points[0].x},${points[0].y}`;
    
    for (let i = 0; i < points.length - 1; i++) {
      const p0 = i > 0 ? points[i - 1] : points[i];
      const p1 = points[i];
      const p2 = points[i + 1];
      const p3 = i < points.length - 2 ? points[i + 2] : p2;

      // Calculate control points for smooth curve (Catmull-Rom to Bezier conversion)
      const cp1x = p1.x + (p2.x - p0.x) / 6;
      const cp1y = p1.y + (p2.y - p0.y) / 6;
      const cp2x = p2.x - (p3.x - p1.x) / 6;
      const cp2y = p2.y - (p3.y - p1.y) / 6;

      path += ` C ${cp1x},${cp1y} ${cp2x},${cp2y} ${p2.x},${p2.y}`;
    }

    return path;
  }

  private generateSampleData(): number[] {
    // Generate random sample data
    const data: number[] = [];
    const base = Math.random() * 50 + 20;
    for (let i = 0; i < 7; i++) {
      data.push(base + (Math.random() - 0.5) * 20);
    }
    return data;
  }
}


