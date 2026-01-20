import { Component, Input, Output, EventEmitter, OnInit, OnDestroy, HostListener, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { Subscription } from 'rxjs';
import { BadgeComponent } from '../badge/badge.component';
import { IconComponent } from '../icon/icon.component';
import { NavigationService, NavigationItem } from '../../../core/services/navigation.service';
import { HasPermissionDirective } from '../../directives/has-permission.directive';

export interface MenuItem {
  label: string;
  icon?: string;
  route?: string;
  children?: MenuItem[];
  badge?: string | number;
  badgeColor?: string;
  permission?: string | string[];
  permissionMode?: 'any' | 'all';
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule, BadgeComponent, IconComponent, HasPermissionDirective],
  templateUrl: './sidebar.component.html'
})
export class SidebarComponent implements OnInit, OnDestroy {
  @Input() menuItems: MenuItem[] = [];
  @Input() collapsed: boolean = false;
  @Input() mobileMenuOpen: boolean = false;
  @Input() usePermissionFilter: boolean = true;
  @Output() collapsedChange = new EventEmitter<boolean>();
  @Output() menuItemClick = new EventEmitter<MenuItem>();
  @ViewChild('sidebarElement', { static: false }) sidebarElement?: ElementRef;
  
  activeRoute: string = '';
  expandedItems: Set<string> = new Set();
  filteredMenuItems: MenuItem[] = [];
  hoveredItem: MenuItem | null = null;
  private navigationSubscription?: Subscription;

  constructor(
    private router: Router,
    private navigationService: NavigationService
  ) {}

  ngOnInit(): void {
    this.updateActiveRoute();
    this.checkAndExpandSettings();
    
    // If menuItems are provided from parent (already filtered), use them
    // Otherwise, subscribe to NavigationService for dynamic filtering
    if (this.menuItems && this.menuItems.length > 0) {
      // Menu items are already provided and filtered by parent component
      this.filteredMenuItems = this.menuItems;
    } else if (this.usePermissionFilter) {
      // No menu items provided, get them from NavigationService
      this.navigationSubscription = this.navigationService.getMenuItems().subscribe(items => {
        this.filteredMenuItems = items as MenuItem[];
      });
    } else {
      this.filteredMenuItems = this.menuItems;
    }
    
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        this.updateActiveRoute();
        this.checkAndExpandSettings();
      });
  }

  ngOnDestroy(): void {
    this.navigationSubscription?.unsubscribe();
  }

  get displayMenuItems(): MenuItem[] {
    return this.usePermissionFilter ? this.filteredMenuItems : this.menuItems;
  }

  private updateActiveRoute(): void {
    // Get full URL including query params
    this.activeRoute = this.router.url;
  }
  
  private checkAndExpandSettings(): void {
    // Auto-expand Settings if on settings page
    if (this.activeRoute.includes('/settings') && this.menuItems) {
      const settingsItem = this.menuItems.find(item => item.label === 'Settings');
      if (settingsItem && settingsItem.children) {
        this.expandedItems.add('Settings');
      }
    }
  }

  toggleSidebar(): void {
    this.collapsed = !this.collapsed;
    this.collapsedChange.emit(this.collapsed);
  }

  toggleSubmenu(item: MenuItem): void {
    if (item.children && item.children.length > 0) {
      const key = item.label;
      if (this.expandedItems.has(key)) {
        // Close this item
        this.expandedItems.delete(key);
      } else {
        // If collapsed, close all other items first (only one open at a time)
        if (this.collapsed && !this.isTopNavigationLayout()) {
          this.expandedItems.clear();
        }
        // Open this item
        this.expandedItems.add(key);
      }
    }
  }

  isExpanded(item: MenuItem): boolean {
    return this.expandedItems.has(item.label);
  }

  isActive(item: MenuItem): boolean {
    if (item.route) {
      // Check if route matches exactly (including query params)
      const itemRoute = item.route.split('?')[0];
      const itemQuery = item.route.includes('?') ? item.route.split('?')[1] : '';
      const currentRoute = this.activeRoute.split('?')[0];
      const currentQuery = this.activeRoute.includes('?') ? this.activeRoute.split('?')[1] : '';
      
      // First check if the base route matches
      if (currentRoute !== itemRoute && !currentRoute.startsWith(itemRoute + '/')) {
        return false;
      }
      
      // If item has query params, they must match exactly with current query params
      if (itemQuery) {
        if (!currentQuery) {
          return false; // Item has query params but current doesn't
        }
        
        // Parse query params and compare
        const itemParams = this.parseQueryParams(itemQuery);
        const currentParams = this.parseQueryParams(currentQuery);
        
        // Check if all item query params match current query params exactly
        for (const key in itemParams) {
          if (itemParams[key] !== currentParams[key]) {
            return false;
          }
        }
        // Also check that we're not missing any required params from item
        return Object.keys(itemParams).length > 0;
      }
      
      // If item has no query params but current does, check if base route matches exactly
      if (!itemQuery && currentQuery) {
        return currentRoute === itemRoute; // Exact match only, no sub-routes
      }
      
      // Both have no query params, check route
      return currentRoute === itemRoute || currentRoute.startsWith(itemRoute + '/');
    }
    // Check if any child is active
    if (item.children) {
      return item.children.some(child => this.isActive(child));
    }
    return false;
  }

  private parseQueryParams(queryString: string): { [key: string]: string } {
    const params: { [key: string]: string } = {};
    if (!queryString) return params;
    
    queryString.split('&').forEach(param => {
      const [key, value] = param.split('=');
      if (key) {
        params[key] = value ? decodeURIComponent(value) : '';
      }
    });
    return params;
  }

  getQueryParams(route: string | undefined): any {
    if (!route || !route.includes('?')) {
      return {};
    }
    const queryString = route.split('?')[1];
    const params: any = {};
    queryString.split('&').forEach(param => {
      const [key, value] = param.split('=');
      if (key && value) {
        params[key] = decodeURIComponent(value);
      }
    });
    return params;
  }

  onItemClick(item: MenuItem, event?: Event): void {
    if (event) {
      event.preventDefault();
      event.stopPropagation();
    }
    
    if (item.children && item.children.length > 0) {
      // Toggle submenu (works for both collapsed and expanded states)
      this.toggleSubmenu(item);
    } else if (item.route) {
      // If clicking on a menu item without children (including submenu items), close any open submenus
      if (this.collapsed && !this.isTopNavigationLayout() && this.expandedItems.size > 0) {
        this.expandedItems.clear();
        this.hoveredItem = null;
      }
      
      // Handle routes with query parameters
      const [path, queryString] = item.route.split('?');
      if (queryString) {
        const params: any = {};
        queryString.split('&').forEach(param => {
          const [key, value] = param.split('=');
          if (key && value) {
            params[key] = decodeURIComponent(value);
          }
        });
        this.router.navigate([path], { queryParams: params });
      } else {
        this.router.navigate([item.route]);
      }
      this.menuItemClick.emit(item);
    } else {
      // If clicking on a menu item without route, close any open submenus
      if (this.collapsed && !this.isTopNavigationLayout() && this.expandedItems.size > 0) {
        this.expandedItems.clear();
        this.hoveredItem = null;
      }
      this.menuItemClick.emit(item);
    }
  }

  isTopNavigationLayout(): boolean {
    return document.body.classList.contains('layout-top-navigation');
  }

  onItemHover(item: MenuItem): void {
    if (!!this.collapsed && !this.isTopNavigationLayout()) {
      this.hoveredItem = item;
    }
  }

  onItemLeave(): void {
    // Only clear hover if we're not showing a popout submenu
    // This allows the submenu to stay open when hovering over it
    setTimeout(() => {
      if (!this.hoveredItem || !this.shouldShowPopoutSubmenu(this.hoveredItem)) {
        this.hoveredItem = null;
      }
    }, 100);
  }

  onSubmenuLeave(): void {
    this.hoveredItem = null;
  }

  shouldShowPopoutSubmenu(item: MenuItem): boolean {
    return !!this.collapsed && 
           !this.isTopNavigationLayout() && 
           !!item.children && 
           item.children.length > 0 && 
           (this.isExpanded(item) || this.hoveredItem === item);
  }

  shouldShowTooltip(item: MenuItem): boolean {
    return !!this.collapsed && 
           !this.isTopNavigationLayout() && 
           this.hoveredItem === item && 
           (!item.children || item.children.length === 0);
  }

  getSubmenuTop(item: MenuItem): number {
    if (!this.collapsed || this.isTopNavigationLayout()) {
      return 0;
    }
    
    // Find the nav-item element for this item
    const navItems = document.querySelectorAll('.sidebar.collapsed .nav-item');
    let itemIndex = -1;
    this.displayMenuItems.forEach((menuItem, index) => {
      if (menuItem.label === item.label) {
        itemIndex = index;
      }
    });
    
    if (itemIndex >= 0 && navItems[itemIndex]) {
      const navItemElement = navItems[itemIndex] as HTMLElement;
      const rect = navItemElement.getBoundingClientRect();
      return rect.top;
    }
    
    return 0;
  }

  getSubmenuLeft(): number {
    if (!this.collapsed || this.isTopNavigationLayout()) {
      return 0;
    }
    // 80px collapsed sidebar width + 8px gap
    return 88;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    // Close submenus when clicking outside the sidebar (only in collapsed mode)
    if (this.collapsed && !this.isTopNavigationLayout() && this.expandedItems.size > 0) {
      const target = event.target as HTMLElement;
      const sidebar = this.sidebarElement?.nativeElement || document.querySelector('.sidebar');
      const popoutSubmenu = document.querySelector('.submenu-popout');
      
      // Check if click is inside the popout submenu (including submenu links)
      const clickedInsideSubmenu = popoutSubmenu && popoutSubmenu.contains(target);
      const clickedOnSubmenuLink = target.closest('.submenu-link');
      
      // If clicking on a submenu link, don't close (let navigation happen)
      if (clickedOnSubmenuLink && clickedInsideSubmenu) {
        return;
      }
      
      // Check if click is on a sidebar nav-link (clicking on another menu item)
      const clickedOnNavLink = target.closest('.nav-link');
      
      // Check if click is outside both sidebar and pop-out submenu
      const clickedInsideSidebar = sidebar && sidebar.contains(target);
      
      // Close submenu if:
      // 1. Clicked outside both sidebar and submenu, OR
      // 2. Clicked on a different nav-link (not the one that opened the submenu)
      if (!clickedInsideSubmenu) {
        if (!clickedInsideSidebar) {
          // Clicked completely outside - close submenu
          this.expandedItems.clear();
          this.hoveredItem = null;
        } else if (clickedOnNavLink) {
          // Clicked on a nav-link (menu item) - check if it's a different item
          const navItem = clickedOnNavLink.closest('.nav-item');
          if (navItem) {
            // Get the label from the nav-link
            const navLabel = navItem.querySelector('.nav-label')?.textContent?.trim();
            if (navLabel) {
              // If clicked on a different menu item, close the submenu
              const isExpandedItem = Array.from(this.expandedItems).some(expandedLabel => {
                return expandedLabel === navLabel;
              });
              // If clicked on a menu item that doesn't have the submenu open, close all submenus
              if (!isExpandedItem) {
                this.expandedItems.clear();
                this.hoveredItem = null;
              }
            }
          }
        } else {
          // Clicked inside sidebar but not on a nav-link (e.g., empty space) - close submenu
          this.expandedItems.clear();
          this.hoveredItem = null;
        }
      }
    }
  }
}

