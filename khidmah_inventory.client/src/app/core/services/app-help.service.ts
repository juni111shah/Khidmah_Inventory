import { Injectable } from '@angular/core';
import { NavigationItem, NavigationService } from './navigation.service';

export interface AppHelpResponse {
  handled: boolean;
  reply: string;
}

interface ModuleHelp {
  name: string;
  route: string;
  keywords: string[];
  purpose: string;
  actions: string[];
}

interface NavHelpEntry {
  label: string;
  route?: string;
  keywords: string[];
}

@Injectable({
  providedIn: 'root'
})
export class AppHelpService {
  private readonly moduleHelp: ModuleHelp[] = [
    {
      name: 'Dashboard',
      route: '/dashboard',
      keywords: ['dashboard', 'overview', 'kpi'],
      purpose: 'Monitor business status, trends, and key metrics.',
      actions: ['Review KPI cards', 'Open related modules from cards', 'Track alerts and performance']
    },
    {
      name: 'Products',
      route: '/products',
      keywords: ['product', 'products', 'catalog', 'item'],
      purpose: 'Manage product catalog, prices, stock settings, and attributes.',
      actions: ['Add or edit products', 'Use barcode scanner', 'Filter/export product lists']
    },
    {
      name: 'Inventory',
      route: '/inventory/stock-levels',
      keywords: ['inventory', 'stock', 'warehouse', 'batch', 'serial', 'transfer'],
      purpose: 'Track stock levels and execute warehouse movements.',
      actions: ['Check stock by warehouse', 'Transfer stock', 'Review batches and serial numbers']
    },
    {
      name: 'Sales Orders',
      route: '/sales-orders',
      keywords: ['sales order', 'sales', 'so', 'customer order'],
      purpose: 'Create and manage customer sales transactions.',
      actions: ['Create sales orders', 'Review order status', 'Open and update order details']
    },
    {
      name: 'Purchase Orders',
      route: '/purchase-orders',
      keywords: ['purchase order', 'purchase', 'po', 'procurement'],
      purpose: 'Create and manage supplier purchase transactions.',
      actions: ['Create purchase orders', 'Track procurement status', 'Edit order details']
    },
    {
      name: 'Customers',
      route: '/customers',
      keywords: ['customer', 'customers', 'client'],
      purpose: 'Manage customer records and relationships.',
      actions: ['Add customers', 'Edit customer details', 'Use customer records in sales orders']
    },
    {
      name: 'Suppliers',
      route: '/suppliers',
      keywords: ['supplier', 'suppliers', 'vendor'],
      purpose: 'Manage supplier records and procurement partners.',
      actions: ['Add suppliers', 'Update supplier records', 'Use supplier in purchase orders']
    },
    {
      name: 'Reports',
      route: '/reports',
      keywords: ['report', 'reports', 'analytics', 'analysis'],
      purpose: 'Generate operational and financial reporting outputs.',
      actions: ['Choose report type', 'Set date range and filters', 'Export report data']
    },
    {
      name: 'Settings',
      route: '/settings',
      keywords: ['settings', 'configuration', 'theme', 'preferences'],
      purpose: 'Configure UI, system preferences, and numbering sequences.',
      actions: ['Switch tabs by configuration area', 'Adjust values', 'Save all changes']
    }
  ];

  constructor(private readonly navigationService: NavigationService) {}

  resolveHelpQuestion(query: string, currentRoute: string): AppHelpResponse {
    const normalized = (query || '').trim().toLowerCase();
    if (!normalized) {
      return { handled: false, reply: '' };
    }

    if (this.isGreeting(normalized)) {
      return {
        handled: true,
        reply: 'Hello! I can guide pages and controls, or execute actions like "create sales order".'
      };
    }

    // Do not hijack actionable commands; let Copilot intent engine execute them.
    if (this.isActionCommand(normalized)) {
      return { handled: false, reply: '' };
    }

    if (this.isGeneralCapabilityQuestion(normalized)) {
      return {
        handled: true,
        reply: `I can guide you across the whole app: navigation, pages, buttons, and workflows. You can ask "how to use sales orders", "what does save button do", or "where is inventory". You can also run "start app tour" or "start page tour".`
      };
    }

    const matchedModule = this.moduleHelp.find(module =>
      module.keywords.some(keyword => normalized.includes(keyword))
    );

    if (matchedModule) {
      const actionText = matchedModule.actions.map(action => `- ${action}`).join('\n');
      return {
        handled: true,
        reply:
          `${matchedModule.name} (${matchedModule.route})\n` +
          `${matchedModule.purpose}\n` +
          `Common actions:\n${actionText}\n` +
          `You can ask me to "start page tour" after opening this page.`
      };
    }

    const actionHelp = this.resolveActionHelp(normalized);
    if (actionHelp) {
      return { handled: true, reply: actionHelp };
    }

    const matchedNavigation = this.findNavigationEntry(normalized);
    if (matchedNavigation) {
      const routeText = matchedNavigation.route || 'group section';
      const hints = [
        `Open ${matchedNavigation.label}`,
        'Review page header description',
        'Use filters/actions on the toolbar',
        'Run page tour for control-by-control guidance'
      ];
      return {
        handled: true,
        reply:
          `${matchedNavigation.label} (${routeText})\n` +
          `This area controls operations related to ${matchedNavigation.label.toLowerCase()}.\n` +
          `Suggested actions:\n${hints.map(item => `- ${item}`).join('\n')}`
      };
    }

    if (this.isCurrentPageQuestion(normalized)) {
      const routeHelp = this.buildCurrentRouteHelp(currentRoute);
      return { handled: true, reply: routeHelp };
    }

    if (normalized.includes('where') || normalized.includes('which page')) {
      const menuText = this.getNavigationEntries()
        .slice(0, 30)
        .map(item => item.route ? `- ${item.label}: ${item.route}` : `- ${item.label}`)
        .join('\n');
      return {
        handled: true,
        reply: `You can find modules from sidebar navigation. Common pages:\n${menuText}`
      };
    }

    return { handled: false, reply: '' };
  }

  private isGeneralCapabilityQuestion(text: string): boolean {
    return text.includes('help') ||
      text.includes('what can you do') ||
      text.includes('guide me') ||
      text.includes('how can you help') ||
      text === '/help';
  }

  private isGreeting(text: string): boolean {
    return text === 'hello' ||
      text === 'hi' ||
      text === 'hey' ||
      text === 'السلام عليكم' ||
      text === 'مرحبا';
  }

  private isCurrentPageQuestion(text: string): boolean {
    return text.includes('this page') ||
      text.includes('current page') ||
      text.includes('where am i') ||
      text.includes('what is this screen');
  }

  private resolveActionHelp(text: string): string | null {
    if (!this.isControlExplanationQuery(text)) {
      return null;
    }

    const actionMap: Array<{ keys: string[]; help: string }> = [
      { keys: ['save button', 'save'], help: 'Save stores your current changes on the page.' },
      { keys: ['create button', 'add button', 'new button'], help: 'Create/Add opens or submits a new record flow.' },
      { keys: ['update button', 'edit button', 'update'], help: 'Update/Edit modifies an existing record.' },
      { keys: ['delete button', 'remove button', 'delete'], help: 'Delete/Remove permanently removes a record if allowed.' },
      { keys: ['cancel button', 'cancel'], help: 'Cancel closes the current action without saving.' },
      { keys: ['export button', 'download button', 'export'], help: 'Export/Download generates a file from current data or report.' },
      { keys: ['filter button', 'filters'], help: 'Filter narrows data by conditions like date, status, category, or search text.' },
      { keys: ['search'], help: 'Search finds records quickly by keyword or identifier.' }
    ];

    for (const item of actionMap) {
      if (item.keys.some(key => text.includes(key))) {
        return `${item.help} If you want, I can start a page tour and point to that control visually.`;
      }
    }

    return null;
  }

  private isControlExplanationQuery(text: string): boolean {
    return text.includes('button') ||
      text.includes('control') ||
      text.includes('what does') ||
      text.includes('how does') ||
      text.includes('meaning of') ||
      text.includes('function of');
  }

  private isActionCommand(text: string): boolean {
    const commandStarters = ['create', 'add', 'new', 'open', 'go to', 'update', 'edit', 'delete', 'remove', 'generate', 'export'];
    const actionEntities = [
      'sales order', 'sales', 'purchase order', 'purchase', 'product', 'customer', 'supplier',
      'invoice', 'report', 'inventory', 'stock', 'workflow'
    ];

    return commandStarters.some(prefix => text.startsWith(prefix)) &&
      actionEntities.some(entity => text.includes(entity));
  }

  private buildCurrentRouteHelp(currentRoute: string): string {
    const route = this.normalizeRoute(currentRoute);
    const matched = this.moduleHelp.find(module => route.startsWith(module.route));
    if (matched) {
      return `You are on ${matched.name} (${matched.route}). ${matched.purpose} Use "start page tour" for guided walkthrough of controls on this page.`;
    }
    const navMatch = this.getNavigationEntries().find(item => item.route && route.startsWith(item.route));
    if (navMatch) {
      return `You are on ${navMatch.label} (${navMatch.route}). Use the page header for context, then use the visible actions and filters. Run "start page tour" for step-by-step guidance.`;
    }
    return `You are on ${route || '/dashboard'}. Use "start page tour" and I will guide the visible actions and sections.`;
  }

  private findNavigationEntry(text: string): NavHelpEntry | null {
    const entries = this.getNavigationEntries();
    return entries.find(entry => entry.keywords.some(key => text.includes(key))) || null;
  }

  private getNavigationEntries(): NavHelpEntry[] {
    const out: NavHelpEntry[] = [];
    const walk = (items: NavigationItem[]) => {
      for (const item of items) {
        const keywords = this.buildKeywords(item.label, item.route);
        out.push({ label: item.label, route: item.route, keywords });
        if (Array.isArray(item.children) && item.children.length > 0) {
          walk(item.children);
        }
      }
    };
    walk(this.navigationService.getMenuItemsSync());
    return out;
  }

  private buildKeywords(label: string, route?: string): string[] {
    const words = label.toLowerCase().split(/[^a-z0-9]+/).filter(Boolean);
    const routeWords = (route || '').toLowerCase().split(/[^a-z0-9]+/).filter(Boolean);
    return Array.from(new Set([...words, ...routeWords, label.toLowerCase(), route?.toLowerCase() || ''])).filter(Boolean);
  }

  private normalizeRoute(route: string): string {
    return (route || '/').split('?')[0];
  }
}

