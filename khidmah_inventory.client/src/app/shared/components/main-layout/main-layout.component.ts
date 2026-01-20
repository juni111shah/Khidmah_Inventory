import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutService } from '../../../core/services/layout.service';
import { LayoutConfig } from '../../../core/models/layout.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './main-layout.component.html'
})
export class MainLayoutComponent implements OnInit, OnDestroy {
  @Input() sidebarCollapsed: boolean = false;
  @Input() showHeader: boolean = true;
  @Input() showFooter: boolean = true;
  @Input() showSidebar: boolean = true;

  layout: LayoutConfig | null = null;
  private layoutSubscription?: Subscription;

  constructor(private layoutService: LayoutService) {}

  ngOnInit(): void {
    this.layout = this.layoutService.getCurrentLayout();
    this.layoutSubscription = this.layoutService.currentLayout$.subscribe(layout => {
      this.layout = layout;
    });
  }

  ngOnDestroy(): void {
    this.layoutSubscription?.unsubscribe();
  }

  get layoutClasses(): string {
    if (!this.layout) {
      // Default to classic layout if no layout is set
      return 'layout-classic sidebar-left sidebar-fixed header-fixed footer-static content-full-width';
    }
    
    const classes: string[] = [
      `layout-${this.layout.type}`,
      `sidebar-${this.layout.sidebarPosition}`,
      `sidebar-${this.layout.sidebarStyle}`,
      `header-${this.layout.headerStyle}`,
      `footer-${this.layout.footerStyle}`,
      `content-${this.layout.contentStyle}`
    ];

    if (this.sidebarCollapsed) {
      classes.push('sidebar-collapsed');
    }

    if (!this.showSidebar || this.layout.sidebarPosition === 'none') {
      classes.push('no-sidebar');
    }

    return classes.join(' ');
  }
}

