import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { ActivatedRouteSnapshot, NavigationEnd, Route, Router } from '@angular/router';
import { BehaviorSubject, filter } from 'rxjs';
import { NavigationItem, NavigationService } from './navigation.service';

export interface OnboardingStep {
  id: string;
  title: string;
  description: string;
  selector?: string;
  route?: string;
}

export interface ActiveHelpInfo {
  title: string;
  description: string;
  x: number;
  y: number;
}

interface RouteMeta {
  template: string;
  regex: RegExp;
  title?: string;
  description?: string;
  permission?: string;
  permissionMode?: string;
}

const ONBOARDING_TOUR_DONE_KEY = 'khidmah_onboarding_tour_done_v1';
const ONBOARDING_HELP_MODE_KEY = 'khidmah_onboarding_help_mode_v1';

@Injectable({
  providedIn: 'root'
})
export class OnboardingService {
  private readonly isBrowser: boolean;

  private readonly tourVisibleSubject = new BehaviorSubject<boolean>(false);
  private readonly currentStepSubject = new BehaviorSubject<OnboardingStep | null>(null);
  private readonly currentStepIndexSubject = new BehaviorSubject<number>(0);
  private readonly helpModeSubject = new BehaviorSubject<boolean>(false);
  private readonly activeHelpSubject = new BehaviorSubject<ActiveHelpInfo | null>(null);

  readonly tourVisible$ = this.tourVisibleSubject.asObservable();
  readonly currentStep$ = this.currentStepSubject.asObservable();
  readonly currentStepIndex$ = this.currentStepIndexSubject.asObservable();
  readonly helpMode$ = this.helpModeSubject.asObservable();
  readonly activeHelp$ = this.activeHelpSubject.asObservable();

  private steps: OnboardingStep[] = [];
  private initialized = false;
  private shouldAutoStartTour = false;
  private routeMetaIndex: RouteMeta[] = [];

  private readonly appFlowSteps: OnboardingStep[] = [
    {
      id: 'shell.sidebar',
      title: 'Navigation Sidebar',
      description: 'Use the sidebar to move between modules. This is the main app flow entry point.',
      selector: '[data-onboarding="sidebar"]'
    },
    {
      id: 'shell.header-title',
      title: 'Page Context',
      description: 'This header shows where you are and what this page is for.',
      selector: '.header-content'
    },
    {
      id: 'shell.search',
      title: 'Global Search',
      description: 'Search across products, customers, orders, and records instantly.',
      selector: '[data-onboarding="global-search"]'
    },
    {
      id: 'shell.notifications',
      title: 'Notifications',
      description: 'Track approvals, alerts, and important updates in real time.',
      selector: '[data-onboarding="notifications"]'
    },
    {
      id: 'shell.ai',
      title: 'AI Assistant',
      description: 'Use the assistant for quick insights, automation support, and guided actions.',
      selector: '[data-onboarding="ai-assistant"]'
    },
    {
      id: 'shell.profile',
      title: 'User Menu',
      description: 'Access your profile and account actions from here.',
      selector: '[data-onboarding="user-menu"]'
    },
    {
      id: 'module.dashboard',
      title: 'Dashboard',
      description: 'Start here to monitor business KPIs and daily activity.',
      selector: '[data-onboarding="menu-dashboard"]',
      route: '/dashboard'
    },
    {
      id: 'module.inventory',
      title: 'Inventory & Operations',
      description: 'Manage stock levels, warehouses, transfers, and item traceability from the inventory area.',
      selector: '[data-onboarding="menu-inventory"]',
      route: '/inventory/stock-levels'
    },
    {
      id: 'module.orders',
      title: 'Sales & Purchasing',
      description: 'Create and manage sales orders, purchase orders, customers, and suppliers.',
      selector: '[data-onboarding="menu-sales-orders"]',
      route: '/sales-orders'
    },
    {
      id: 'module.settings',
      title: 'Configuration',
      description: 'Use settings to configure roles, users, notifications, and UI preferences.',
      selector: '[data-onboarding="menu-settings"]',
      route: '/settings'
    }
  ];

  private readonly pageTourDefinitions: Array<{
    id: string;
    matchers: RegExp[];
    title: string;
    description: string;
    steps: OnboardingStep[];
  }> = [
    {
      id: 'dashboard',
      matchers: [/^\/dashboard$/, /^\/briefing$/, /^\/command-center$/],
      title: 'Dashboard flow',
      description: 'Track business health and key metrics before taking daily actions.',
      steps: [
        { id: 'dashboard.header', title: 'Dashboard header', description: 'Use this section for quick context and actions.', selector: '.page-header, .header-content' },
        { id: 'dashboard.stats', title: 'KPI cards', description: 'These cards summarize sales, inventory, and operations performance.', selector: '.stat-card, app-stat-card' },
        { id: 'dashboard.cards', title: 'Insight panels', description: 'Review charts and insight cards for trend analysis.', selector: '.card, app-unified-card' }
      ]
    },
    {
      id: 'products',
      matchers: [/^\/products(\/.*)?$/, /^\/categories(\/.*)?$/, /^\/warehouses(\/.*)?$/, /^\/suppliers(\/.*)?$/, /^\/customers(\/.*)?$/],
      title: 'Catalog and master data flow',
      description: 'Create and maintain your core records before processing transactions.',
      steps: [
        { id: 'catalog.filters', title: 'Search and filters', description: 'Use filters to quickly find records in large datasets.', selector: '.filter-panel-wrapper, .filters-section, .input-group' },
        { id: 'catalog.table', title: 'Data listing', description: 'View, sort, and open records from the listing table.', selector: 'app-data-table, .table-responsive, table' },
        { id: 'catalog.primary-action', title: 'Primary action', description: 'Use the main action button to create new records.', selector: '.btn-primary, button[type="submit"]' }
      ]
    },
    {
      id: 'inventory',
      matchers: [/^\/inventory(\/.*)?$/, /^\/autonomous(\/.*)?$/],
      title: 'Inventory operations flow',
      description: 'Monitor stock, trace items, and execute warehouse operations.',
      steps: [
        { id: 'inventory.overview', title: 'Inventory context', description: 'This section gives quick visibility into stock and warehouse operations.', selector: '.page-header, .header-content' },
        { id: 'inventory.stock-grid', title: 'Stock data', description: 'Use this area to inspect quantities, batches, and serial tracking.', selector: 'app-data-table, .table-responsive, table' },
        { id: 'inventory.actions', title: 'Operational actions', description: 'Use create/transfer actions to execute stock movement and updates.', selector: '.btn-primary, .page-header-actions, .card .btn' }
      ]
    },
    {
      id: 'orders',
      matchers: [/^\/sales-orders(\/.*)?$/, /^\/purchase-orders(\/.*)?$/, /^\/pos(\/.*)?$/],
      title: 'Transaction flow',
      description: 'Handle sales and procurement transactions from creation to completion.',
      steps: [
        { id: 'orders.listing', title: 'Orders list', description: 'Track order status and open items for review or editing.', selector: 'app-data-table, .table-responsive, table' },
        { id: 'orders.form', title: 'Order form', description: 'Complete header and line details to create accurate transactions.', selector: 'form, .card form' },
        { id: 'orders.post-actions', title: 'Submit and lifecycle actions', description: 'Use submit/approve/print actions based on workflow stage.', selector: '.btn-primary, .btn-success, .btn-warning, .btn-info' }
      ]
    },
    {
      id: 'reports',
      matchers: [/^\/reports$/, /^\/analytics(\/.*)?$/, /^\/kpi(\/.*)?$/, /^\/intelligence(\/.*)?$/],
      title: 'Reporting and analytics flow',
      description: 'Analyze trends and performance to support operational decisions.',
      steps: [
        { id: 'reports.filters', title: 'Report parameters', description: 'Set date range and filters before generating report output.', selector: '.filter-panel-wrapper, .form-select, .form-control' },
        { id: 'reports.visuals', title: 'Charts and metrics', description: 'Use visual sections to compare trends and detect anomalies.', selector: 'app-chart, canvas, .card' },
        { id: 'reports.export', title: 'Export and sharing', description: 'Export reports when you need external sharing or offline analysis.', selector: '.page-header-actions .btn, .btn-outline-secondary, .btn-primary' }
      ]
    },
    {
      id: 'settings',
      matchers: [/^\/settings$/, /^\/users(\/.*)?$/, /^\/roles(\/.*)?$/, /^\/companies(\/.*)?$/, /^\/platform(\/.*)?$/, /^\/notifications$/],
      title: 'Administration and settings flow',
      description: 'Configure users, permissions, preferences, and system behavior.',
      steps: [
        { id: 'settings.navigation', title: 'Settings navigation', description: 'Select the configuration area you want to manage.', selector: '.nav-tabs, .sidebar, .header-content' },
        { id: 'settings.form', title: 'Configuration form', description: 'Change values and review impact before saving.', selector: 'form, .settings-content, .card' },
        { id: 'settings.save', title: 'Save changes', description: 'Use save actions to apply configuration updates.', selector: '.btn-primary, button[type="submit"]' }
      ]
    }
  ];

  constructor(
    private readonly router: Router,
    private readonly navigationService: NavigationService,
    @Inject(PLATFORM_ID) platformId: object
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  initialize(): void {
    if (!this.isBrowser || this.initialized) {
      return;
    }

    this.initialized = true;
    this.rebuildRouteMetaIndex();
    this.helpModeSubject.next(this.readBoolean(ONBOARDING_HELP_MODE_KEY, false));
    document.body.classList.toggle('help-mode-enabled', this.helpModeSubject.value);
    this.shouldAutoStartTour = !this.readBoolean(ONBOARDING_TOUR_DONE_KEY, false);

    this.router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe(() => {
      window.setTimeout(() => this.annotateInteractiveElements(), 120);

      if (this.shouldAutoStartTour && !this.isAuthUrl(this.router.url) && !this.tourVisibleSubject.value) {
        this.shouldAutoStartTour = false;
        window.setTimeout(() => this.startTour(), 700);
      }
    });

    document.addEventListener('click', this.onDocumentClickCapture, true);

    this.annotateInteractiveElements();

    if (this.shouldAutoStartTour && !this.isAuthUrl(this.router.url)) {
      this.shouldAutoStartTour = false;
      window.setTimeout(() => this.startTour(), 700);
    }
  }

  dispose(): void {
    if (!this.isBrowser) {
      return;
    }

    document.removeEventListener('click', this.onDocumentClickCapture, true);
  }

  startTour(): void {
    this.steps = this.withExistingTargets([
      ...this.appFlowSteps,
      ...this.buildSidebarCoverageSteps(),
      ...this.getCurrentPageTourSteps()
    ]);
    if (this.steps.length === 0) {
      return;
    }
    this.currentStepIndexSubject.next(0);
    this.tourVisibleSubject.next(true);
    this.showStep(0);
  }

  startPageTour(): void {
    this.steps = this.withExistingTargets(this.getCurrentPageTourSteps());
    if (this.steps.length === 0) {
      this.steps = this.withExistingTargets([
        {
          id: 'page.default',
          title: 'Page overview',
          description: 'Use this page header, actions, and data area to complete tasks on this screen.',
          selector: '.header-content'
        }
      ]);
    }
    if (this.steps.length === 0) {
      return;
    }
    this.currentStepIndexSubject.next(0);
    this.tourVisibleSubject.next(true);
    this.showStep(0);
  }

  endTour(markDone: boolean = true): void {
    this.tourVisibleSubject.next(false);
    this.currentStepSubject.next(null);
    this.clearHighlights();
    if (markDone) {
      this.writeBoolean(ONBOARDING_TOUR_DONE_KEY, true);
    }
  }

  nextStep(): void {
    const next = this.currentStepIndexSubject.value + 1;
    if (next >= this.steps.length) {
      this.endTour(true);
      return;
    }

    this.currentStepIndexSubject.next(next);
    this.showStep(next);
  }

  previousStep(): void {
    const prev = this.currentStepIndexSubject.value - 1;
    if (prev < 0) {
      return;
    }

    this.currentStepIndexSubject.next(prev);
    this.showStep(prev);
  }

  toggleHelpMode(): void {
    const next = !this.helpModeSubject.value;
    this.helpModeSubject.next(next);
    this.writeBoolean(ONBOARDING_HELP_MODE_KEY, next);
    document.body.classList.toggle('help-mode-enabled', next);

    if (!next) {
      this.activeHelpSubject.next(null);
    }
  }

  closeActiveHelp(): void {
    this.activeHelpSubject.next(null);
  }

  openCurrentStepRoute(startPageTour: boolean = false): void {
    const step = this.currentStepSubject.value;
    if (!step?.route) {
      return;
    }

    this.router.navigateByUrl(step.route).then(() => {
      if (startPageTour) {
        this.startPageTourWithRetry();
        return;
      }
      window.setTimeout(() => {
        this.annotateInteractiveElements();
        this.highlightCurrentStepTarget(step);
      }, 350);
    }).catch(() => {});
  }

  getTotalSteps(): number {
    return this.steps.length || this.appFlowSteps.length;
  }

  private getCurrentPageTourSteps(): OnboardingStep[] {
    const path = this.getNormalizedPath(this.router.url);
    const deepSteps = this.getDeepRouteSteps(path);
    const universalSteps = this.getUniversalPageSteps(path);
    const dynamicSteps = this.getDynamicMarkedPageSteps();

    if (deepSteps.length > 0) {
      return this.mergeTourSteps([...deepSteps, ...universalSteps, ...dynamicSteps]);
    }

    const routeMetaIntro = this.getRouteMetaIntroStep(path);
    if (routeMetaIntro) {
      return this.mergeTourSteps([routeMetaIntro, ...universalSteps, ...dynamicSteps]);
    }

    if (dynamicSteps.length > 0) {
      return this.mergeTourSteps([...dynamicSteps, ...universalSteps]);
    }

    const def = this.pageTourDefinitions.find(entry => entry.matchers.some(m => m.test(path)));

    const baseIntro: OnboardingStep = def
      ? {
          id: `${def.id}.intro`,
          title: def.title,
          description: def.description,
          selector: '.header-content'
        }
      : {
          id: 'page.intro',
          title: 'Current page flow',
          description: 'This page follows the standard flow: review context, apply filters, then complete actions.',
          selector: '.header-content'
        };

    const standardSteps: OnboardingStep[] = [
      {
        id: 'page.context',
        title: 'Page context',
        description: 'Read the title and description to understand what this page controls.',
        selector: '.header-content'
      },
      {
        id: 'page.actions',
        title: 'Main actions',
        description: 'Use primary and secondary action buttons to perform operations.',
        selector: '.btn-primary, .page-header-actions, .card .btn'
      },
      {
        id: 'page.data',
        title: 'Data and details',
        description: 'This section contains forms, tables, or cards for records and results.',
        selector: 'form, app-data-table, .table-responsive, .card'
      }
    ];

    const custom = def?.steps ?? [];
    return this.mergeTourSteps([baseIntro, ...custom, ...universalSteps, ...standardSteps]);
  }

  private getUniversalPageSteps(path: string): OnboardingStep[] {
    const pathLabel = this.humanizeOnboardingKey(path.replace(/^\//, '').replace(/\//g, '-')) || 'current page';

    return [
      {
        id: 'universal.header',
        title: 'Page overview',
        description: `This section explains the purpose of ${pathLabel} and what this page controls.`,
        selector: '.header-content, .page-header'
      },
      {
        id: 'universal.actions',
        title: 'Main actions',
        description: 'Use these buttons for primary operations like create, save, export, or refresh.',
        selector: '.actions-container, .page-header-actions, .listing-actions, .d-flex.justify-content-end.gap-2'
      },
      {
        id: 'universal.filters',
        title: 'Search and filters',
        description: 'Use search and filters to narrow records and find data faster.',
        selector: '.filter-panel-wrapper, [filter-panel], .global-search, .input-group'
      },
      {
        id: 'universal.tabs',
        title: 'Tabs and sections',
        description: 'Use tabs to switch between configuration or data sections.',
        selector: '.tabs-header, .nav-tabs, app-tabs'
      },
      {
        id: 'universal.table',
        title: 'Data table',
        description: 'Review, sort, and navigate records from this data area.',
        selector: 'app-data-table, .table-responsive, table'
      },
      {
        id: 'universal.form',
        title: 'Form area',
        description: 'Enter or update required fields, then save your changes.',
        selector: 'form, .form-content, .product-form-container, .so-form-container, .po-form-container'
      },
      {
        id: 'universal.cards',
        title: 'Insight cards',
        description: 'Cards summarize important information and quick actions.',
        selector: 'app-unified-card, .card'
      },
      {
        id: 'universal.charts',
        title: 'Visual analytics',
        description: 'Charts help you understand trends, performance, and exceptions.',
        selector: 'app-chart, canvas, .apexcharts-canvas'
      },
      {
        id: 'universal.save',
        title: 'Finalize actions',
        description: 'Use Save/Submit actions to apply updates on this page.',
        selector: '.btn-primary, button[type="submit"]'
      }
    ];
  }

  private getRouteMetaIntroStep(path: string): OnboardingStep | null {
    const snapshot = this.getDeepestRouteSnapshot(this.router.routerState.snapshot.root);
    const header = snapshot?.data?.['header'] as { title?: string; description?: string } | undefined;
    const routeMeta = this.getRouteMetaForPath(path);
    const title = header?.title || routeMeta?.title;
    const description = header?.description || routeMeta?.description;
    if (!title && !description) return null;

    return {
      id: `route.meta.${path.replace(/\W+/g, '-')}`,
      title: title || 'Page overview',
      description: description || 'This page provides specific business operations.',
      selector: '.header-content'
    };
  }

  private getDeepestRouteSnapshot(snapshot: ActivatedRouteSnapshot | null): ActivatedRouteSnapshot | null {
    let current = snapshot;
    while (current?.firstChild) {
      current = current.firstChild;
    }
    return current ?? null;
  }

  private mergeTourSteps(steps: OnboardingStep[]): OnboardingStep[] {
    const seen = new Set<string>();
    const merged: OnboardingStep[] = [];

    for (const step of steps) {
      const key = `${step.id}|${step.selector || ''}|${step.title}`;
      if (seen.has(key)) {
        continue;
      }
      seen.add(key);
      merged.push(step);
    }

    return merged;
  }

  private buildSidebarCoverageSteps(): OnboardingStep[] {
    const menuItems = this.navigationService.getMenuItemsSync();
    const flattened = this.flattenMenuItems(menuItems);

    return flattened.map((entry, index) => {
      const key = this.toMenuOnboardingKey(entry.item.label);
      const selector = `[data-onboarding="${key}"]`;
      const hasElement = this.isBrowser && !!document.querySelector(selector);

      const routeMeta = this.getRouteMetaForPath(entry.item.route);
      const routeText = entry.item.route ? `Route: ${entry.item.route}` : 'Route: group section';
      const subItems = entry.item.children?.map(child => child.label).filter(Boolean) ?? [];
      const guide = this.getRouteGuide(entry.item.route, entry.item.label);
      const purpose = routeMeta?.description || guide.purpose;
      const pageTitle = routeMeta?.title || entry.item.label;
      const permission = routeMeta?.permission
        ? `Access: ${routeMeta.permission}${routeMeta.permissionMode ? ` (${routeMeta.permissionMode})` : ''}`
        : null;
      const detailParts: string[] = [
        `Page: ${pageTitle}`,
        `Purpose: ${purpose}`,
        `What you can do: ${guide.actions.join(' | ')}`,
        routeText
      ];
      if (permission) {
        detailParts.push(permission);
      }
      if (subItems.length > 0) {
        detailParts.push(`Sub-items: ${subItems.join(', ')}`);
      }
      const description = detailParts.join('\n');

      return {
        id: `sidebar.auto.${index}`,
        title: `Sidebar: ${entry.path}`,
        description,
        selector: hasElement ? selector : undefined,
        route: entry.item.route
      };
    });
  }

  private flattenMenuItems(
    items: NavigationItem[],
    parentPath = ''
  ): Array<{ item: NavigationItem; path: string }> {
    const out: Array<{ item: NavigationItem; path: string }> = [];

    for (const item of items) {
      const path = parentPath ? `${parentPath} > ${item.label}` : item.label;
      out.push({ item, path });
      if (item.children && item.children.length > 0) {
        out.push(...this.flattenMenuItems(item.children, path));
      }
    }

    return out;
  }

  private getDynamicMarkedPageSteps(): OnboardingStep[] {
    if (!this.isBrowser) {
      return [];
    }

    const excluded = new Set([
      'sidebar',
      'global-search',
      'notifications',
      'ai-assistant',
      'user-menu'
    ]);

    const nodes = Array.from(document.querySelectorAll<HTMLElement>('[data-onboarding]'));
    const seen = new Set<string>();
    const steps: OnboardingStep[] = [];

    for (const node of nodes) {
      const key = node.getAttribute('data-onboarding')?.trim();
      if (!key || seen.has(key)) {
        continue;
      }
      if (excluded.has(key) || key.startsWith('menu-')) {
        continue;
      }

      const title = node.getAttribute('data-help-title')?.trim() || this.humanizeOnboardingKey(key);
      const description =
        node.getAttribute('data-help')?.trim() ||
        `Use this section for ${title.toLowerCase()} in the current page flow.`;

      steps.push({
        id: `dynamic.${key}`,
        title,
        description,
        selector: `[data-onboarding="${key}"]`
      });
      seen.add(key);

      if (steps.length >= 12) {
        break;
      }
    }

    if (steps.length === 0) {
      return [];
    }

    return [
      {
        id: 'dynamic.intro',
        title: 'Page workflow',
        description: 'This is a guided walkthrough of key controls detected on the current page.',
        selector: '.header-content'
      },
      ...steps
    ];
  }

  private getDeepRouteSteps(path: string): OnboardingStep[] {
    if (/^\/products$/.test(path)) {
      return [
        {
          id: 'products.list.intro',
          title: 'Products list flow',
          description: 'Manage product records from search to creation and export.',
          selector: '[data-onboarding="products-list-shell"]'
        },
        {
          id: 'products.list.actions',
          title: 'Quick actions',
          description: 'Use scanner and create actions for fast product operations.',
          selector: '[data-onboarding="products-list-actions"]'
        },
        {
          id: 'products.list.create',
          title: 'Add product',
          description: 'Create a new product record.',
          selector: '[data-onboarding="products-list-create"]'
        },
        {
          id: 'products.list.filters',
          title: 'Filter products',
          description: 'Use quick filters or the filter builder to narrow the list.',
          selector: '[data-onboarding="products-list-filters"]'
        },
        {
          id: 'products.list.table',
          title: 'Products table',
          description: 'Review, sort, and open product records from this table.',
          selector: '[data-onboarding="products-list-table"]'
        }
      ];
    }

    if (/^\/products\/barcode-scanner$/.test(path)) {
      return [
        {
          id: 'products.scanner.intro',
          title: 'Barcode scanner flow',
          description: 'Scan item barcodes to quickly find product records.',
          selector: '.barcode-scanner-container'
        },
        {
          id: 'products.scanner.input',
          title: 'Scan input',
          description: 'Enter or scan a barcode value to search.',
          selector: '.barcode-input, .input-wrapper'
        },
        {
          id: 'products.scanner.action',
          title: 'Scan action',
          description: 'Run the scan and view matching product details.',
          selector: '.scan-btn'
        }
      ];
    }

    if (/^\/products\/(new|[^/]+|[^/]+\/edit)$/.test(path)) {
      return [
        {
          id: 'products.form.intro',
          title: 'Product form flow',
          description: 'Complete product details, pricing, and stock setup before saving.',
          selector: '[data-onboarding="product-form-shell"]'
        },
        {
          id: 'products.form.tabs',
          title: 'Form tabs',
          description: 'Use tabs to move between details and AI insights.',
          selector: '[data-onboarding="product-form-tabs"]'
        },
        {
          id: 'products.form.basic',
          title: 'Basic information',
          description: 'Define core identity fields such as name, SKU, and category.',
          selector: '[data-onboarding="product-form-basic-info"]'
        },
        {
          id: 'products.form.stock',
          title: 'Stock management',
          description: 'Configure reorder and tracking behavior for this product.',
          selector: '[data-onboarding="product-form-stock"]'
        },
        {
          id: 'products.form.save',
          title: 'Save actions',
          description: 'Use update/create to finalize product changes.',
          selector: '[data-onboarding="product-form-save-actions"]'
        }
      ];
    }

    if (/^\/sales-orders$/.test(path)) {
      return [
        {
          id: 'sales.list.intro',
          title: 'Sales orders list flow',
          description: 'Track sales order status and create new orders from this page.',
          selector: '[data-onboarding="sales-orders-list-shell"]'
        },
        {
          id: 'sales.list.actions',
          title: 'Order actions',
          description: 'Use these actions to create and manage saved views.',
          selector: '[data-onboarding="sales-orders-list-actions"]'
        },
        {
          id: 'sales.list.create',
          title: 'New sales order',
          description: 'Create a new sales order transaction.',
          selector: '[data-onboarding="sales-orders-list-create"]'
        },
        {
          id: 'sales.list.table',
          title: 'Sales orders table',
          description: 'Open orders, inspect statuses, and navigate records.',
          selector: '[data-onboarding="sales-orders-list-table"]'
        }
      ];
    }

    if (/^\/sales-orders\/(new|[^/]+|[^/]+\/edit)$/.test(path)) {
      return [
        {
          id: 'sales.form.intro',
          title: 'Sales order form flow',
          description: 'Fill customer, dates, and notes, then save the order.',
          selector: '[data-onboarding="sales-order-form-shell"]'
        },
        {
          id: 'sales.form.header-actions',
          title: 'Header actions',
          description: 'Use back and activity controls while reviewing the order.',
          selector: '[data-onboarding="sales-order-form-header-actions"]'
        },
        {
          id: 'sales.form.details',
          title: 'Order details',
          description: 'Complete customer and scheduling fields.',
          selector: '[data-onboarding="sales-order-form-details"]'
        },
        {
          id: 'sales.form.save',
          title: 'Save actions',
          description: 'Cancel or save to create/update the sales order.',
          selector: '[data-onboarding="sales-order-form-save-actions"]'
        }
      ];
    }

    if (/^\/purchase-orders$/.test(path)) {
      return [
        {
          id: 'purchase.list.intro',
          title: 'Purchase orders list flow',
          description: 'Track purchase orders and create new procurement records.',
          selector: '[data-onboarding="purchase-orders-list-shell"]'
        },
        {
          id: 'purchase.list.actions',
          title: 'Order actions',
          description: 'Use these actions to create orders and manage views.',
          selector: '[data-onboarding="purchase-orders-list-actions"]'
        },
        {
          id: 'purchase.list.create',
          title: 'New purchase order',
          description: 'Create a new purchase order transaction.',
          selector: '[data-onboarding="purchase-orders-list-create"]'
        },
        {
          id: 'purchase.list.table',
          title: 'Purchase orders table',
          description: 'Review procurement records and open details.',
          selector: '[data-onboarding="purchase-orders-list-table"]'
        }
      ];
    }

    if (/^\/purchase-orders\/(new|[^/]+|[^/]+\/edit)$/.test(path)) {
      return [
        {
          id: 'purchase.form.intro',
          title: 'Purchase order form flow',
          description: 'Fill supplier details, expected dates, then save.',
          selector: '[data-onboarding="purchase-order-form-shell"]'
        },
        {
          id: 'purchase.form.header-actions',
          title: 'Header actions',
          description: 'Use back and activity options when reviewing order context.',
          selector: '[data-onboarding="purchase-order-form-header-actions"]'
        },
        {
          id: 'purchase.form.details',
          title: 'Order details',
          description: 'Complete supplier and date fields required for processing.',
          selector: '[data-onboarding="purchase-order-form-details"]'
        },
        {
          id: 'purchase.form.save',
          title: 'Save actions',
          description: 'Cancel or save to create/update the purchase order.',
          selector: '[data-onboarding="purchase-order-form-save-actions"]'
        }
      ];
    }

    if (/^\/settings$/.test(path)) {
      return [
        {
          id: 'settings.intro',
          title: 'Settings flow',
          description: 'Configure system look-and-feel and behavior from this page.',
          selector: '[data-onboarding="settings-content"]'
        },
        {
          id: 'settings.actions',
          title: 'Global settings actions',
          description: 'Reset defaults or save all changes from here.',
          selector: '[data-onboarding="settings-actions"]'
        },
        {
          id: 'settings.tabs',
          title: 'Settings sections',
          description: 'Use tabs to move between colors, layout, UI, and numbering.',
          selector: '[data-onboarding="settings-tabs"]'
        },
        {
          id: 'settings.palette',
          title: 'Color palette',
          description: 'Adjust branding and status colors with live preview.',
          selector: '[data-onboarding="settings-color-palette"]'
        },
        {
          id: 'settings.numbering',
          title: 'Numbering sequences',
          description: 'Define prefixes for purchase, sales, and invoice documents.',
          selector: '[data-onboarding="settings-numbering-sequences"]'
        }
      ];
    }

    return [];
  }

  private showStep(index: number): void {
    const step = this.steps[index];
    if (!step) {
      this.endTour(false);
      return;
    }

    this.currentStepSubject.next(step);
    window.setTimeout(() => this.highlightCurrentStepTarget(step), 50);
  }

  private highlightCurrentStepTarget(step: OnboardingStep): void {
    this.clearHighlights();
    if (!step.selector) {
      return;
    }

    const element = document.querySelector(step.selector) as HTMLElement | null;
    if (!element) {
      return;
    }

    element.classList.add('tour-target-highlight');
    element.scrollIntoView({ behavior: 'smooth', block: 'center', inline: 'nearest' });
  }

  private withExistingTargets(steps: OnboardingStep[]): OnboardingStep[] {
    if (!this.isBrowser) {
      return steps;
    }

    return steps.filter(step => {
      if (!step.selector) {
        return true;
      }
      return !!document.querySelector(step.selector);
    });
  }

  private clearHighlights(): void {
    const highlighted = document.querySelectorAll('.tour-target-highlight');
    highlighted.forEach(element => element.classList.remove('tour-target-highlight'));
  }

  private annotateInteractiveElements(): void {
    if (!this.isBrowser) {
      return;
    }

    this.tagCorePageTargets();

    this.applyElementDescription(document.querySelector('.header-title') as HTMLElement | null, () => ({
      title: 'Page title',
      description: 'Shows your current page in the app flow.'
    }));

    const clickableSelectors = [
      'button',
      'a',
      '.btn',
      '[role="button"]',
      '.nav-link',
      '.submenu-link',
      'input[type="submit"]'
    ];
    const clickable = document.querySelectorAll<HTMLElement>(clickableSelectors.join(','));
    clickable.forEach(element => {
      if (this.shouldSkipHelpAnnotation(element)) {
        return;
      }
      this.applyElementDescription(element, () => this.buildActionDescription(element));
    });

    const fieldSelectors = 'input:not([type="button"]):not([type="submit"]), textarea, select';
    const fields = document.querySelectorAll<HTMLElement>(fieldSelectors);
    fields.forEach(element => {
      if (this.shouldSkipHelpAnnotation(element)) {
        return;
      }
      this.applyElementDescription(element, () => this.buildFieldDescription(element));
    });
  }

  private applyElementDescription(
    element: HTMLElement | null,
    descriptionBuilder: () => { title: string; description: string }
  ): void {
    if (!element) {
      return;
    }

    const existingHelp = element.getAttribute('data-help');
    if (existingHelp && existingHelp.trim().length > 0) {
      return;
    }

    const data = descriptionBuilder();
    if (!data.description) {
      return;
    }

    element.setAttribute('data-help-title', data.title);
    element.setAttribute('data-help', data.description);

    if (!element.getAttribute('title')) {
      element.setAttribute('title', data.description);
    }
    if (!element.getAttribute('aria-label')) {
      element.setAttribute('aria-label', data.title);
    }
  }

  private buildActionDescription(element: HTMLElement): { title: string; description: string } {
    const rawText = this.extractElementLabel(element);
    const label = rawText || 'Action';
    const lower = label.toLowerCase();

    if (element.classList.contains('nav-link') || element.classList.contains('submenu-link')) {
      return {
        title: label,
        description: `Navigate to ${label}.`
      };
    }

    const map: Array<{ keys: string[]; text: string }> = [
      { keys: ['save'], text: 'Save your current changes.' },
      { keys: ['create', 'new', 'add'], text: 'Create a new record.' },
      { keys: ['edit', 'update'], text: 'Edit the selected record.' },
      { keys: ['delete', 'remove'], text: 'Delete the selected record.' },
      { keys: ['cancel', 'close'], text: 'Cancel the current action.' },
      { keys: ['search', 'find'], text: 'Search records using filters or keywords.' },
      { keys: ['filter'], text: 'Refine the current list using filters.' },
      { keys: ['export'], text: 'Export current data to a file.' },
      { keys: ['import'], text: 'Import data from a file.' },
      { keys: ['approve'], text: 'Approve the pending item or workflow step.' },
      { keys: ['reject'], text: 'Reject the pending item or workflow step.' },
      { keys: ['submit'], text: 'Submit this form for processing.' },
      { keys: ['print'], text: 'Print the current view or document.' },
      { keys: ['refresh', 'reload'], text: 'Refresh data on this page.' }
    ];

    for (const item of map) {
      if (item.keys.some(key => lower.includes(key))) {
        return { title: label, description: item.text };
      }
    }

    return {
      title: label,
      description: `Execute "${label}" action.`
    };
  }

  private buildFieldDescription(element: HTMLElement): { title: string; description: string } {
    const placeholder = element.getAttribute('placeholder')?.trim();
    const ariaLabel = element.getAttribute('aria-label')?.trim();
    const label = ariaLabel || placeholder || 'Input field';

    return {
      title: label,
      description: placeholder ? `Enter ${placeholder.toLowerCase()}.` : 'Provide a value for this field.'
    };
  }

  private shouldSkipHelpAnnotation(element: HTMLElement): boolean {
    if (element.hasAttribute('data-help-skip')) {
      return true;
    }

    const className = element.className || '';
    if (className.includes('tour-') || className.includes('onboarding-')) {
      return true;
    }

    return false;
  }

  private extractElementLabel(element: HTMLElement): string {
    const attrs = [
      element.getAttribute('data-help-title'),
      element.getAttribute('aria-label'),
      element.getAttribute('title')
    ];
    for (const value of attrs) {
      if (value && value.trim()) {
        return value.trim();
      }
    }

    const text = (element.textContent || '').replace(/\s+/g, ' ').trim();
    if (text) {
      return text;
    }

    if (element.id) {
      return element.id;
    }

    return '';
  }

  private onDocumentClickCapture = (event: Event): void => {
    if (!this.helpModeSubject.value) {
      return;
    }

    const target = event.target as HTMLElement | null;
    if (!target) {
      return;
    }

    const actionable = target.closest<HTMLElement>(
      '[data-help], button, a, .btn, [role="button"], input, select, textarea'
    );

    if (!actionable) {
      this.activeHelpSubject.next(null);
      return;
    }

    if (actionable.closest('.onboarding-tour-card') || actionable.closest('.onboarding-help-fab')) {
      return;
    }

    const title = actionable.getAttribute('data-help-title') || this.extractElementLabel(actionable) || 'Element';
    const description = actionable.getAttribute('data-help') || this.buildActionDescription(actionable).description;
    const rect = actionable.getBoundingClientRect();

    this.activeHelpSubject.next({
      title,
      description,
      x: Math.max(12, rect.left + rect.width / 2),
      y: Math.max(12, rect.top + rect.height + 10)
    });

    event.preventDefault();
    event.stopPropagation();
  };

  private readBoolean(key: string, fallback: boolean): boolean {
    if (!this.isBrowser) {
      return fallback;
    }
    const value = localStorage.getItem(key);
    if (value === null) {
      return fallback;
    }
    return value === '1';
  }

  private writeBoolean(key: string, value: boolean): void {
    if (!this.isBrowser) {
      return;
    }
    localStorage.setItem(key, value ? '1' : '0');
  }

  private isAuthUrl(url: string): boolean {
    return url.includes('/login') || url.includes('/register') || url.includes('/auth');
  }

  private getNormalizedPath(url: string): string {
    const hashless = url.split('#')[0];
    return hashless.split('?')[0] || '/';
  }

  private humanizeOnboardingKey(key: string): string {
    return key
      .replace(/[-_]+/g, ' ')
      .replace(/\s+/g, ' ')
      .trim()
      .replace(/\b\w/g, ch => ch.toUpperCase());
  }

  private toMenuOnboardingKey(label: string): string {
    return `menu-${label.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/^-+|-+$/g, '')}`;
  }

  private rebuildRouteMetaIndex(): void {
    this.routeMetaIndex = [];
    this.walkRoutes(this.router.config, '');
  }

  private walkRoutes(routes: Route[], prefix: string): void {
    for (const route of routes) {
      const path = (route.path || '').trim();
      if (path === '**') continue;

      const joined = `${prefix}/${path}`.replace(/\/+/g, '/');
      const normalized = joined.startsWith('/') ? joined : `/${joined}`;
      const data = (route.data || {}) as {
        header?: { title?: string; description?: string };
        permission?: string | string[];
        permissionMode?: string;
      };
      const permissionValue = Array.isArray(data.permission) ? data.permission.join(', ') : data.permission;

      if (path) {
        this.routeMetaIndex.push({
          template: normalized,
          regex: this.routeTemplateToRegex(normalized),
          title: data.header?.title,
          description: data.header?.description,
          permission: permissionValue,
          permissionMode: data.permissionMode
        });
      }

      if (route.children?.length) {
        this.walkRoutes(route.children, normalized);
      }
    }
  }

  private routeTemplateToRegex(template: string): RegExp {
    const escaped = template
      .replace(/[.*+?^${}()|[\]\\]/g, '\\$&')
      .replace(/\\:([A-Za-z0-9_]+)/g, '[^/]+');
    return new RegExp(`^${escaped}$`, 'i');
  }

  private getRouteMetaForPath(path: string | undefined): RouteMeta | null {
    if (!path) return null;
    const normalized = this.getNormalizedPath(path);
    return this.routeMetaIndex.find(meta => meta.regex.test(normalized)) || null;
  }

  private getRouteGuide(route: string | undefined, label: string): { purpose: string; actions: string[] } {
    if (!route) {
      return {
        purpose: `${label} groups related pages in the app.`,
        actions: ['Open this group', 'Choose a sub-item page', 'Follow module-specific actions']
      };
    }

    const normalized = route.toLowerCase();
    const guides: Array<{ startsWith: string; purpose: string; actions: string[] }> = [
      { startsWith: '/dashboard', purpose: 'Monitor top metrics and business activity.', actions: ['Review KPI cards', 'Check trends', 'Navigate to operational modules'] },
      { startsWith: '/briefing', purpose: 'View a concise daily business briefing.', actions: ['Read highlights', 'Open suggested follow-ups', 'Track key alerts'] },
      { startsWith: '/command-center', purpose: 'Manage executive-level operational overview.', actions: ['Review key signals', 'Prioritize actions', 'Jump to affected modules'] },
      { startsWith: '/reports', purpose: 'Generate and review business reports.', actions: ['Select report type', 'Set date range', 'Export output'] },
      { startsWith: '/kpi', purpose: 'Analyze KPIs by business domain.', actions: ['Compare metrics', 'Monitor performance', 'Spot anomalies'] },
      { startsWith: '/finance', purpose: 'Manage accounting and financial statements.', actions: ['Review accounts', 'Track journal entries', 'Generate statements'] },
      { startsWith: '/currency', purpose: 'Manage currencies for multi-currency operations.', actions: ['Create currencies', 'Set base currency', 'Review status'] },
      { startsWith: '/exchange-rates', purpose: 'Maintain FX rates for conversions.', actions: ['Add rates', 'Update rates', 'Validate effective dates'] },
      { startsWith: '/intelligence', purpose: 'Use analytics and predictive insights.', actions: ['Open intelligence modules', 'Review recommendations', 'Take action from insights'] },
      { startsWith: '/automation', purpose: 'Configure and monitor automation workflows.', actions: ['Create rules', 'Manage builder', 'Review execution history'] },
      { startsWith: '/users', purpose: 'Manage user accounts and access.', actions: ['Create users', 'Edit profile/access', 'Activate/deactivate users'] },
      { startsWith: '/roles', purpose: 'Control role-based permissions.', actions: ['Create roles', 'Assign permissions', 'Update role definitions'] },
      { startsWith: '/companies', purpose: 'Manage tenant/company records.', actions: ['Create companies', 'Edit details', 'Assign company users'] },
      { startsWith: '/workflows', purpose: 'Run approval and workflow routing.', actions: ['Design workflows', 'Start workflows', 'Approve pending items'] },
      { startsWith: '/platform', purpose: 'Configure external integrations.', actions: ['Manage API keys', 'Set webhooks', 'Configure integrations/reports'] },
      { startsWith: '/categories', purpose: 'Organize products into categories.', actions: ['Create categories', 'Edit hierarchy', 'Maintain category data'] },
      { startsWith: '/products', purpose: 'Manage product catalog and settings.', actions: ['Add products', 'Update prices/details', 'Scan barcodes and export lists'] },
      { startsWith: '/warehouses', purpose: 'Manage warehouses and storage locations.', actions: ['Create warehouses', 'Edit warehouse metadata', 'Review location setup'] },
      { startsWith: '/autonomous', purpose: 'Operate autonomous warehouse features.', actions: ['View autonomous dashboard', 'Open routes/live ops', 'Track task execution'] },
      { startsWith: '/copilot', purpose: 'Use AI Copilot for guided operations.', actions: ['Ask operational questions', 'Trigger tours/help', 'Run command-style tasks'] },
      { startsWith: '/inventory', purpose: 'Control stock operations and traceability.', actions: ['Check stock levels', 'Transfer/adjust stock', 'Manage batches and serials'] },
      { startsWith: '/reorder', purpose: 'Manage reorder suggestions and replenishment.', actions: ['Review suggestions', 'Generate reorder docs', 'Track reorder status'] },
      { startsWith: '/suppliers', purpose: 'Maintain supplier records.', actions: ['Create suppliers', 'Update supplier details', 'Use suppliers in purchase orders'] },
      { startsWith: '/purchase-orders', purpose: 'Handle procurement transactions.', actions: ['Create purchase orders', 'Review statuses', 'Edit/track order details'] },
      { startsWith: '/customers', purpose: 'Maintain customer records.', actions: ['Create customers', 'Edit customer details', 'Use customers in sales orders'] },
      { startsWith: '/sales-orders', purpose: 'Manage customer sales transactions.', actions: ['Create sales orders', 'Review order lifecycle', 'Update order details'] },
      { startsWith: '/pos', purpose: 'Run point-of-sale transactions.', actions: ['Open POS screen', 'Process sales', 'Finalize sessions'] },
      { startsWith: '/notifications', purpose: 'Review user and system notifications.', actions: ['Open notifications', 'Review messages', 'Mark/read updates'] },
      { startsWith: '/settings', purpose: 'Configure app behavior and appearance.', actions: ['Change tabs by section', 'Adjust settings values', 'Save all changes'] }
    ];

    const match = guides.find(g => normalized.startsWith(g.startsWith));
    if (match) {
      return { purpose: match.purpose, actions: match.actions };
    }

    return {
      purpose: `Use ${label} to manage related records and workflows.`,
      actions: ['Open the page', 'Use filters/actions', 'Create or update records']
    };
  }

  private tagCorePageTargets(): void {
    const routeKey = this.getNormalizedPath(this.router.url)
      .replace(/^\//, '')
      .replace(/\//g, '-')
      .replace(/[^a-zA-Z0-9-]/g, '')
      .toLowerCase() || 'dashboard';

    this.ensureOnboardingTag(document.querySelector('.header-content') as HTMLElement | null, `${routeKey}-header`);
    this.ensureOnboardingTag(document.querySelector('.page-header') as HTMLElement | null, `${routeKey}-page-header`);
    this.ensureOnboardingTag(document.querySelector('.page-header-actions') as HTMLElement | null, `${routeKey}-page-actions`);
    this.ensureOnboardingTag(document.querySelector('.listing-actions') as HTMLElement | null, `${routeKey}-listing-actions`);
    this.ensureOnboardingTag(document.querySelector('app-data-table') as HTMLElement | null, `${routeKey}-data-table`);
    this.ensureOnboardingTag(document.querySelector('.table-responsive') as HTMLElement | null, `${routeKey}-table`);
    this.ensureOnboardingTag(document.querySelector('form') as HTMLElement | null, `${routeKey}-form`);
    this.ensureOnboardingTag(document.querySelector('.actions-container') as HTMLElement | null, `${routeKey}-actions`);
    this.ensureOnboardingTag(document.querySelector('.tabs-header') as HTMLElement | null, `${routeKey}-tabs`);
    this.ensureOnboardingTag(document.querySelector('.settings-content-area') as HTMLElement | null, `${routeKey}-content`);
  }

  private ensureOnboardingTag(element: HTMLElement | null, key: string): void {
    if (!element || !key) {
      return;
    }
    if (!element.getAttribute('data-onboarding')) {
      element.setAttribute('data-onboarding', key);
    }
  }

  private startPageTourWithRetry(attempt: number = 0): void {
    const started = this.tryStartPageTourNow();
    if (started) {
      return;
    }

    if (attempt >= 10) {
      return;
    }

    window.setTimeout(() => this.startPageTourWithRetry(attempt + 1), 220);
  }

  private tryStartPageTourNow(): boolean {
    const candidates = this.withExistingTargets(this.getCurrentPageTourSteps());
    if (candidates.length === 0) {
      return false;
    }

    this.steps = candidates;
    this.currentStepIndexSubject.next(0);
    this.tourVisibleSubject.next(true);
    this.showStep(0);
    return true;
  }
}

