import { Injectable } from '@angular/core';

export type IconLibrary = 'fa' | 'material' | 'material-outlined';

/**
 * Icon Service
 * Provides common icon names and helper methods for icon usage throughout the app
 */
@Injectable({
  providedIn: 'root'
})
export class IconService {
  // Common Material Icons
  readonly material = {
    // Navigation
    menu: 'menu',
    close: 'close',
    arrowBack: 'arrow_back',
    arrowForward: 'arrow_forward',
    home: 'home',
    dashboard: 'dashboard',
    
    // Actions
    add: 'add',
    edit: 'edit',
    delete: 'delete',
    save: 'save',
    cancel: 'cancel',
    search: 'search',
    filter: 'filter_list',
    refresh: 'refresh',
    download: 'download',
    upload: 'upload',
    print: 'print',
    share: 'share',
    settings: 'settings',
    moreVert: 'more_vert',
    moreHoriz: 'more_horiz',
    
    // Status
    check: 'check',
    checkCircle: 'check_circle',
    error: 'error',
    warning: 'warning',
    info: 'info',
    success: 'check_circle',
    
    // Content
    folder: 'folder',
    file: 'description',
    image: 'image',
    attachment: 'attach_file',
    
    // User
    person: 'person',
    accountCircle: 'account_circle',
    logout: 'logout',
    login: 'login',
    
    // Inventory
    inventory: 'inventory_2',
    shoppingCart: 'shopping_cart',
    store: 'store',
    warehouse: 'warehouse',
    package: 'inventory',
    
    // Business
    business: 'business',
    company: 'corporate_fare',
    group: 'group',
    
    // Time
    calendar: 'calendar_today',
    schedule: 'schedule',
    time: 'access_time',
    
    // Communication
    email: 'email',
    notification: 'notifications',
    message: 'message',
    
    // UI
    visibility: 'visibility',
    visibilityOff: 'visibility_off',
    expandMore: 'expand_more',
    expandLess: 'expand_less',
    chevronRight: 'chevron_right',
    chevronLeft: 'chevron_left',
    arrowDropDown: 'arrow_drop_down',
    arrowDropUp: 'arrow_drop_up'
  };

  // Common Font Awesome Icons
  readonly fa = {
    // Navigation
    menu: 'bars',
    close: 'times',
    arrowBack: 'arrow-left',
    arrowForward: 'arrow-right',
    home: 'home',
    dashboard: 'tachometer-alt',
    
    // Actions
    add: 'plus',
    edit: 'edit',
    delete: 'trash',
    save: 'save',
    cancel: 'times-circle',
    search: 'search',
    filter: 'filter',
    refresh: 'sync',
    download: 'download',
    upload: 'upload',
    print: 'print',
    share: 'share',
    settings: 'cog',
    moreVert: 'ellipsis-v',
    moreHoriz: 'ellipsis-h',
    
    // Status
    check: 'check',
    checkCircle: 'check-circle',
    error: 'exclamation-circle',
    warning: 'exclamation-triangle',
    info: 'info-circle',
    success: 'check-circle',
    
    // Content
    folder: 'folder',
    file: 'file',
    image: 'image',
    attachment: 'paperclip',
    
    // User
    person: 'user',
    accountCircle: 'user-circle',
    logout: 'sign-out-alt',
    login: 'sign-in-alt',
    
    // Inventory
    inventory: 'boxes',
    shoppingCart: 'shopping-cart',
    store: 'store',
    warehouse: 'warehouse',
    package: 'box',
    
    // Business
    business: 'briefcase',
    company: 'building',
    group: 'users',
    
    // Time
    calendar: 'calendar',
    schedule: 'clock',
    time: 'clock',
    
    // Communication
    email: 'envelope',
    notification: 'bell',
    message: 'comment',
    
    // UI
    visibility: 'eye',
    visibilityOff: 'eye-slash',
    expandMore: 'chevron-down',
    expandLess: 'chevron-up',
    chevronRight: 'chevron-right',
    chevronLeft: 'chevron-left',
    arrowDropDown: 'caret-down',
    arrowDropUp: 'caret-up'
  };

  /**
   * Get icon name for a specific library
   * @param iconKey - Key from the icon collections
   * @param library - Icon library to use
   * @returns Icon name for the specified library
   */
  getIcon(iconKey: string, library: IconLibrary = 'material'): string {
    if (library === 'fa' || library === 'material-outlined') {
      return (this.fa as any)[iconKey] || iconKey;
    }
    return (this.material as any)[iconKey] || iconKey;
  }

  /**
   * Get all available icon keys
   */
  getIconKeys(): string[] {
    return Object.keys(this.material);
  }
}

