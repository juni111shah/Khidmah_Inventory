import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { ThemeService } from './core/services/theme.service';
import { LayoutService } from './core/services/layout.service';
import { AppearanceSettingsService } from './core/services/appearance-settings.service';
import { NavigationService, NavigationItem } from './core/services/navigation.service';
import { RouteHeaderService } from './core/services/route-header.service';
import { MenuItem } from './shared/components/sidebar/sidebar.component';
import { Subscription, filter, take } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'Khidmah Inventory';
  sidebarCollapsed: boolean = false;
  mobileMenuOpen: boolean = false;
  isAuthPage: boolean = false;
  menuItems: MenuItem[] = [];
  private routerSubscription?: Subscription;
  private menuItemsSubscription?: Subscription;
  private appearanceSettingsSubscription?: Subscription;
  
  get isAuthRoute(): boolean {
    return this.isAuthPage;
  }
  
  get currentYear(): number {
    return new Date().getFullYear();
  }
  
  get copyrightText(): string {
    return `© ${this.currentYear} Khidmah Inventory. All rights reserved.`;
  }

  getLogoInitials(): string {
    if (!this.title) return '';
    
    // Split title into words and get first letter of each word
    const words = this.title.trim().split(/\s+/);
    
    if (words.length === 0) return '';
    
    // If single word, take first 2 characters
    if (words.length === 1) {
      return words[0].substring(0, 2).toUpperCase();
    }
    
    // If multiple words, take first letter of first two words
    return (words[0].charAt(0) + words[1].charAt(0)).toUpperCase();
  }

  constructor(
    public themeService: ThemeService,
    public layoutService: LayoutService,
    public router: Router,
    private navigationService: NavigationService,
    private routeHeaderService: RouteHeaderService,
    private appearanceSettingsService: AppearanceSettingsService
  ) {}

  ngOnInit() {
    // Initialize theme and layout (these are BehaviorSubjects, so we just need to trigger initial load)
    // Use take(1) to automatically unsubscribe after first emission
    this.themeService.theme$.pipe(take(1)).subscribe();
    this.layoutService.currentLayout$.pipe(take(1)).subscribe();
    
    // Subscribe to appearance settings changes to apply them globally
    this.appearanceSettingsSubscription = this.appearanceSettingsService.settings$.subscribe(() => {
      // Settings are already applied by the service, but we can trigger change detection if needed
    });
    
    // Initialize route header service to automatically set headers from route data
    this.routeHeaderService.initialize();
    
    // Load dynamic menu items based on permissions
    this.loadMenuItems();
    
    // Check initial route
    this.checkAuthRoute();
    
    // Subscribe to route changes
    this.routerSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.checkAuthRoute();
      });
  }

  ngOnDestroy(): void {
    this.routerSubscription?.unsubscribe();
    this.menuItemsSubscription?.unsubscribe();
    this.appearanceSettingsSubscription?.unsubscribe();
  }

  private loadMenuItems(): void {
    // Subscribe to navigation service to get permission-filtered menu items
    this.menuItemsSubscription = this.navigationService.getMenuItems().subscribe(
      (navigationItems: NavigationItem[]) => {
        // Convert NavigationItem[] to MenuItem[]
        this.menuItems = this.convertNavigationItemsToMenuItems(navigationItems);
      }
    );
  }

  private convertNavigationItemsToMenuItems(navigationItems: NavigationItem[]): MenuItem[] {
    return navigationItems.map(item => ({
      label: item.label,
      icon: item.icon,
      route: item.route,
      permission: item.permission,
      permissionMode: item.permissionMode,
      badge: item.badge,
      badgeColor: item.badgeColor,
      children: item.children ? this.convertNavigationItemsToMenuItems(item.children) : undefined
    }));
  }

  private checkAuthRoute(): void {
    const url = this.router.url;
    this.isAuthPage = url.includes('/login') || url.includes('/register') || url.includes('/auth');
  }

  onSidebarToggle(collapsed: boolean): void {
    this.sidebarCollapsed = collapsed;
  }

  onMenuToggle(): void {
    // Toggle sidebar collapsed state
    this.sidebarCollapsed = !this.sidebarCollapsed;
    this.mobileMenuOpen = !this.sidebarCollapsed;
  }

  onMenuItemClick(item: MenuItem): void {
    // Handle menu item clicks if needed
  }
}

