import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { LayoutConfig, LayoutType, DEFAULT_LAYOUT, LAYOUT_PRESETS } from '../models/layout.model';

@Injectable({
  providedIn: 'root'
})
export class LayoutService {
  private currentLayoutSubject = new BehaviorSubject<LayoutConfig>(DEFAULT_LAYOUT);
  public currentLayout$: Observable<LayoutConfig> = this.currentLayoutSubject.asObservable();

  constructor() {
    // Load layout from localStorage on initialization
    this.loadLayout();
  }

  /**
   * Get current layout configuration
   */
  getCurrentLayout(): LayoutConfig {
    return this.currentLayoutSubject.value;
  }

  /**
   * Set layout by type
   */
  setLayout(layoutType: LayoutType): void {
    const layout = LAYOUT_PRESETS[layoutType] || DEFAULT_LAYOUT;
    this.currentLayoutSubject.next(layout);
    this.saveLayout(layout);
    this.applyLayoutStyles(layout);
  }

  /**
   * Set custom layout configuration
   */
  setCustomLayout(layout: Partial<LayoutConfig>): void {
    const currentLayout = this.currentLayoutSubject.value;
    const newLayout: LayoutConfig = {
      ...currentLayout,
      ...layout
    };
    this.currentLayoutSubject.next(newLayout);
    this.saveLayout(newLayout);
    this.applyLayoutStyles(newLayout);
  }

  /**
   * Get all available layout presets
   */
  getAvailableLayouts(): LayoutConfig[] {
    return Object.values(LAYOUT_PRESETS);
  }

  /**
   * Save layout to localStorage
   */
  private saveLayout(layout: LayoutConfig): void {
    try {
      localStorage.setItem('app_layout', JSON.stringify(layout));
    } catch (error) {
      console.error('Failed to save layout to localStorage:', error);
    }
  }

  /**
   * Load layout from localStorage
   */
  private loadLayout(): void {
    try {
      const savedLayout = localStorage.getItem('app_layout');
      if (savedLayout) {
        const layout = JSON.parse(savedLayout) as LayoutConfig;
        // Validate layout exists
        if (LAYOUT_PRESETS[layout.type]) {
          this.currentLayoutSubject.next(layout);
          this.applyLayoutStyles(layout);
        } else {
          // If saved layout type doesn't exist, use default
          this.setLayout('modern');
        }
      } else {
        // No saved layout, use default
        this.applyLayoutStyles(DEFAULT_LAYOUT);
      }
    } catch (error) {
      console.error('Failed to load layout from localStorage:', error);
      this.applyLayoutStyles(DEFAULT_LAYOUT);
    }
  }

  /**
   * Apply layout styles to document root
   */
  private applyLayoutStyles(layout: LayoutConfig): void {
    const root = document.documentElement;
    
    // Set CSS variables for layout dimensions
    root.style.setProperty('--layout-sidebar-width', layout.sidebarWidth);
    root.style.setProperty('--layout-sidebar-collapsed-width', layout.sidebarCollapsedWidth);
    root.style.setProperty('--layout-header-height', layout.headerHeight);
    root.style.setProperty('--layout-footer-height', layout.footerHeight);
    
    if (layout.containerMaxWidth) {
      root.style.setProperty('--layout-container-max-width', layout.containerMaxWidth);
    }

    // Add layout type class to body
    document.body.classList.remove(...Object.keys(LAYOUT_PRESETS).map(type => `layout-${type}`));
    document.body.classList.add(`layout-${layout.type}`);
    
    // Also apply to main-layout element if it exists
    const mainLayout = document.querySelector('.main-layout') as HTMLElement;
    if (mainLayout) {
      // Remove all layout classes from main-layout
      Object.keys(LAYOUT_PRESETS).forEach(type => {
        mainLayout.classList.remove(`layout-${type}`);
      });
      // Add the new layout class
      mainLayout.classList.add(`layout-${layout.type}`);
    }
    
    // Add layout-specific classes
    document.body.classList.toggle('sidebar-left', layout.sidebarPosition === 'left');
    document.body.classList.toggle('sidebar-right', layout.sidebarPosition === 'right');
    document.body.classList.toggle('sidebar-top', layout.sidebarPosition === 'top');
    document.body.classList.toggle('sidebar-none', layout.sidebarPosition === 'none');
    
    document.body.classList.toggle('header-fixed', layout.headerStyle === 'fixed');
    document.body.classList.toggle('header-sticky', layout.headerStyle === 'sticky');
    document.body.classList.toggle('header-static', layout.headerStyle === 'static');
    
    document.body.classList.toggle('footer-fixed', layout.footerStyle === 'fixed');
    document.body.classList.toggle('footer-sticky', layout.footerStyle === 'sticky');
    document.body.classList.toggle('footer-static', layout.footerStyle === 'static');
    
    document.body.classList.toggle('content-full-width', layout.contentStyle === 'full-width');
    document.body.classList.toggle('content-boxed', layout.contentStyle === 'boxed');
    document.body.classList.toggle('content-centered', layout.contentStyle === 'centered');
  }

  /**
   * Reset to default layout
   */
  resetLayout(): void {
    this.setLayout('modern');
  }
}

