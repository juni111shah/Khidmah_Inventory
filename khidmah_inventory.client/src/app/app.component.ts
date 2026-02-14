import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { ThemeService } from './core/services/theme.service';
import { LayoutService } from './core/services/layout.service';
import { AppearanceSettingsService } from './core/services/appearance-settings.service';
import { NavigationService, NavigationItem } from './core/services/navigation.service';
import { RouteHeaderService } from './core/services/route-header.service';
import { ToastNotificationService } from './core/services/toast-notification.service';
import { SignalRService } from './core/services/signalr.service';
import { AuthService } from './core/services/auth.service';
import { SearchOverlayService } from './core/services/search-overlay.service';
import { OnboardingService } from './core/services/onboarding.service';
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
  globalToastShow = false;
  globalToastMessage = '';
  globalToastType: 'success' | 'error' | 'warning' | 'info' = 'info';
  private routerSubscription?: Subscription;
  private menuItemsSubscription?: Subscription;
  private appearanceSettingsSubscription?: Subscription;
  private toastSubscription?: Subscription;
  private userSubscription?: Subscription;

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
    private appearanceSettingsService: AppearanceSettingsService,
    private toastNotification: ToastNotificationService,
    private signalRService: SignalRService,
    private authService: AuthService,
    private searchOverlayService: SearchOverlayService,
    private onboardingService: OnboardingService
  ) {}

  @HostListener('document:keydown', ['$event'])
  onGlobalKeyDown(event: KeyboardEvent): void {
    if ((event.ctrlKey || event.metaKey) && event.key === 'k') {
      event.preventDefault();
      if (!this.isAuthRoute) this.searchOverlayService.open();
    }
  }


  ngOnInit() {
    this.checkScreenSize();
    this.onboardingService.initialize();
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

    // Start SignalR when already logged in
    if (this.authService.isAuthenticated()) {
      this.signalRService.startConnection().catch(() => {});
    }
    this.userSubscription = this.authService.currentUser$.subscribe(user => {
      if (!user) this.signalRService.stopConnection().catch(() => {});
    });

    // Global toast for real-time notifications
    this.toastSubscription = this.toastNotification.getToast().subscribe(t => {
      this.globalToastMessage = t.message;
      this.globalToastType = t.type;
      this.globalToastShow = true;
    });

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
    this.toastSubscription?.unsubscribe();
    this.userSubscription?.unsubscribe();
    this.onboardingService.dispose();
  }

  onGlobalToastClose(): void {
    this.globalToastShow = false;
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

  @HostListener('window:resize', ['$event'])
  onResize(event: any) {
    this.checkScreenSize();
  }

  private checkScreenSize(): void {
    const isMobile = window.innerWidth < 992;
    if (isMobile && !this.sidebarCollapsed) {
      this.sidebarCollapsed = true;
      this.mobileMenuOpen = false;
    }
  }

  onSidebarToggle(collapsed: boolean): void {
    this.sidebarCollapsed = collapsed;
    if (window.innerWidth < 992) {
      this.mobileMenuOpen = !collapsed;
    }
  }

  onMenuToggle(): void {
    // Toggle sidebar collapsed state
    this.sidebarCollapsed = !this.sidebarCollapsed;

    // On mobile, this controls the slide-in menu
    if (window.innerWidth < 992) {
      this.mobileMenuOpen = !this.sidebarCollapsed;
    }
  }

  onMenuItemClick(item: MenuItem): void {
    // On mobile, close selection after click
    if (window.innerWidth < 992) {
      this.sidebarCollapsed = true;
      this.mobileMenuOpen = false;
    }
  }
}

