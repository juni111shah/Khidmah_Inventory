import { Injectable, Inject, PLATFORM_ID, NgZone, ApplicationRef } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject, Observable } from 'rxjs';
import { AppearanceSettings, DEFAULT_APPEARANCE_SETTINGS } from '../models/appearance-settings.model';
import { SettingsApiService } from './settings-api.service';
import { LayoutService } from './layout.service';
import { LAYOUT_PRESETS } from '../models/layout.model';
import { catchError, tap } from 'rxjs/operators';
import { of } from 'rxjs';

const APPEARANCE_SETTINGS_STORAGE_KEY = 'app_appearance_settings';

@Injectable({
  providedIn: 'root'
})
export class AppearanceSettingsService {
  private settingsSubject = new BehaviorSubject<AppearanceSettings>(DEFAULT_APPEARANCE_SETTINGS);
  public settings$: Observable<AppearanceSettings> = this.settingsSubject.asObservable();

  private currentSettings: AppearanceSettings = DEFAULT_APPEARANCE_SETTINGS;
  private isBrowser: boolean;
  private autoSaveTimeout: any;

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private settingsApi: SettingsApiService,
    private layoutService: LayoutService,
    private ngZone: NgZone,
    private appRef: ApplicationRef
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
    this.initializeSettings();
  }

  private initializeSettings(): void {
    if (!this.isBrowser) {
      this.applySettings(DEFAULT_APPEARANCE_SETTINGS);
      return;
    }

    // Try to load from localStorage first (for instant display)
    const stored = this.loadFromLocalStorage();
    if (stored) {
      this.currentSettings = stored;
      // Sync layout with layout service
      if (stored.layoutConfig) {
        this.layoutService.setCustomLayout(stored.layoutConfig);
      }
      this.applySettings(stored);
      this.settingsSubject.next(stored);
    } else {
      // Load layout from layout service if no stored settings
      const currentLayout = this.layoutService.getCurrentLayout();
      if (currentLayout) {
        this.currentSettings.layoutConfig = currentLayout;
        this.currentSettings.selectedLayout = currentLayout.type;
      }
      this.applySettings(this.currentSettings);
    }

    // Then load from API (may override localStorage)
    this.loadFromApi();
  }

  private loadFromLocalStorage(): AppearanceSettings | null {
    try {
      const stored = localStorage.getItem(APPEARANCE_SETTINGS_STORAGE_KEY);
      if (stored) {
        return JSON.parse(stored);
      }
    } catch (error) {
      console.error('Error loading appearance settings from localStorage:', error);
    }
    return null;
  }

  private loadFromApi(): void {
    // Load from API
    this.settingsApi.getAppearanceSettings()
      .pipe(
        catchError(() => {
          console.warn('Failed to load appearance settings from API, using localStorage');
          return of(null);
        })
      )
      .subscribe(response => {
        if (response?.success && response.data) {
          // Merge API settings with current settings
          this.currentSettings = { ...this.currentSettings, ...response.data };
          if (this.currentSettings.layoutConfig) {
            this.layoutService.setCustomLayout(this.currentSettings.layoutConfig);
          }
          this.applySettings(this.currentSettings);
          this.settingsSubject.next(this.currentSettings);
          this.saveToLocalStorage(this.currentSettings);
        }
      });
  }

  getSettings(): AppearanceSettings {
    return { ...this.currentSettings };
  }

  updateSettings(settings: Partial<AppearanceSettings>, saveToApi: boolean = true): void {
    this.currentSettings = { ...this.currentSettings, ...settings };
    this.applySettings(this.currentSettings);
    this.settingsSubject.next(this.currentSettings);
    this.saveToLocalStorage(this.currentSettings);

    // Force change detection to ensure all components update
    if (this.isBrowser) {
      this.ngZone.run(() => {
        // Trigger change detection
        this.appRef.tick();
        
        // Also use requestAnimationFrame to ensure DOM updates are applied
        requestAnimationFrame(() => {
          // Force a reflow to ensure CSS variables are applied
          document.body.offsetHeight;
        });
      });
    }

    if (saveToApi && this.isBrowser) {
      // Debounce API saves
      if (this.autoSaveTimeout) {
        clearTimeout(this.autoSaveTimeout);
      }
      this.autoSaveTimeout = setTimeout(() => {
        this.saveToApi(this.currentSettings);
      }, 1000); // Save after 1 second of no changes
    }
  }

  updateProperty<K extends keyof AppearanceSettings>(key: K, value: AppearanceSettings[K]): void {
    this.updateSettings({ [key]: value } as Partial<AppearanceSettings>);
  }

  /**
   * Force immediate re-application of all settings
   * Useful when settings need to be applied immediately without waiting for change detection
   */
  forceApplySettings(): void {
    if (this.isBrowser) {
      this.applySettings(this.currentSettings);
      // Trigger change detection
      this.ngZone.run(() => {
        this.appRef.tick();
      });
    }
  }

  resetSettings(): void {
    this.updateSettings(DEFAULT_APPEARANCE_SETTINGS);
  }

  private saveToLocalStorage(settings: AppearanceSettings): void {
    if (this.isBrowser) {
      try {
        localStorage.setItem(APPEARANCE_SETTINGS_STORAGE_KEY, JSON.stringify(settings));
      } catch (error) {
        console.error('Error saving appearance settings to localStorage:', error);
      }
    }
  }

  private saveToApi(settings: AppearanceSettings): void {
    // Save to API using settings API service
    this.settingsApi.saveAppearanceSettings(settings).subscribe({
      next: (response) => {
        if (response?.success) {
          console.log('Appearance settings saved to API successfully');
        }
      },
      error: (error) => {
        console.error('Error saving appearance settings to API:', error);
        // Settings are already saved to localStorage, so this is not critical
      }
    });
  }

  private applySettings(settings: AppearanceSettings): void {
    if (!this.isBrowser) return;

    const root = document.documentElement;
    const body = document.body;

    // Apply colors from settings
    const colors = {
      primaryColor: settings.primaryColor,
      secondaryColor: settings.secondaryColor,
      accentColor: settings.accentColor,
      successColor: settings.successColor,
      warningColor: settings.warningColor,
      dangerColor: settings.dangerColor,
      infoColor: settings.infoColor
    };

    // Apply color variables IMMEDIATELY - these will cascade to all components using CSS variables
    root.style.setProperty('--primary-color', colors.primaryColor);
    root.style.setProperty('--secondary-color', colors.secondaryColor);
    root.style.setProperty('--accent-color', colors.accentColor);
    root.style.setProperty('--success-color', colors.successColor);
    root.style.setProperty('--warning-color', colors.warningColor);
    root.style.setProperty('--danger-color', colors.dangerColor);
    root.style.setProperty('--info-color', colors.infoColor);
    root.style.setProperty('--error-color', colors.dangerColor);
    
    // Force update all buttons and elements directly (use setTimeout to ensure DOM is ready)
    setTimeout(() => {
      this.forceUpdateButtonColors(colors);
      this.forceUpdateBadgeColors(colors);
    }, 0);

    // Apply background and surface colors
    const bgColor = '#F5F5F5';
    const surfaceColor = '#FFFFFF';
    const textColor = '#212121';
    const textSecondaryColor = '#757575';
    
    // Apply background and text colors IMMEDIATELY
    root.style.setProperty('--background-color', bgColor);
    root.style.setProperty('--surface-color', surfaceColor);
    root.style.setProperty('--text-color', textColor);
    root.style.setProperty('--text-secondary-color', textSecondaryColor);
    
    // Also apply to body immediately
    body.style.backgroundColor = bgColor;
    body.style.color = textColor;

    // Apply branding
    root.style.setProperty('--logo-url', settings.logoUrl ? `url(${settings.logoUrl})` : 'none');
    root.style.setProperty('--logo-height', settings.logoHeight);

    // Apply component styles
    const componentStyles = {
      borderRadius: settings.borderRadius,
      buttonStyle: settings.buttonStyle,
      cardStyle: settings.cardStyle,
      cardElevation: settings.cardElevation
    };

    root.style.setProperty('--border-radius', componentStyles.borderRadius);
    root.style.setProperty('--button-style', componentStyles.buttonStyle);
    root.style.setProperty('--button-border-radius', componentStyles.borderRadius);
    root.style.setProperty('--button-padding', '12px 24px');
    root.style.setProperty('--card-style', componentStyles.cardStyle);
    root.style.setProperty('--card-elevation', componentStyles.cardElevation.toString());
    root.style.setProperty('--card-border-radius', componentStyles.borderRadius);
    
    // CRITICAL: Apply button and card style attributes to body for global application
    body.setAttribute('data-button-style', componentStyles.buttonStyle);
    body.setAttribute('data-card-style', componentStyles.cardStyle);
    
    // Remove table design attribute (not used in single template)
    body.removeAttribute('data-table-design');
    
    // Remove tab style attribute (not used in single template)
    body.removeAttribute('data-tab-style');
    
    // Apply card shadow based on elevation
    const elevation = componentStyles.cardElevation || 2;
    // Card shadows
    const shadowMap: Record<number, string> = {
      0: 'none',
      1: '0 1px 3px rgba(0,0,0,0.08), 0 1px 2px rgba(0,0,0,0.12)',
      2: '0 2px 8px rgba(0,0,0,0.08), 0 2px 4px rgba(0,0,0,0.12)',
      3: '0 4px 12px rgba(0,0,0,0.1), 0 2px 6px rgba(0,0,0,0.15)',
      4: '0 8px 16px rgba(0,0,0,0.12), 0 4px 8px rgba(0,0,0,0.18)',
      5: '0 12px 24px rgba(0,0,0,0.15), 0 6px 12px rgba(0,0,0,0.2)'
    };
    root.style.setProperty('--card-shadow', shadowMap[elevation] || shadowMap[2]);
    
    // Apply input styles
    root.style.setProperty('--input-border-radius', componentStyles.borderRadius);
    
    // Reset table styles to defaults (single template)
    root.style.setProperty('--table-header-background', 'var(--background-color, #F5F5F5)');
    root.style.setProperty('--table-header-text-color', 'var(--text-color, #212121)');
    root.style.setProperty('--table-header-border-color', 'var(--border-color, rgba(0, 0, 0, 0.12))');
    root.style.setProperty('--table-row-hover-color', 'rgba(0, 0, 0, 0.04)');
    root.style.setProperty('--table-row-selected-color', 'rgba(33, 150, 243, 0.1)');
    root.style.setProperty('--table-border-color', 'var(--border-color, rgba(0, 0, 0, 0.12))');
    root.style.setProperty('--table-striped-background', 'rgba(0, 0, 0, 0.02)');
    root.style.setProperty('--table-border-radius', '0px');

    // Apply typography - using default system fonts
    root.style.setProperty('--font-family', '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif');
    root.style.setProperty('--font-size-md', '14px');

    // Apply layout
    this.applyLayout(settings.layoutConfig);

    // Apply animations
    root.style.setProperty('--animations-enabled', settings.animationsEnabled ? '1' : '0');
    const speedMultiplier = settings.animationSpeed === 'slow' ? 1.5 : settings.animationSpeed === 'fast' ? 0.5 : 1;
    root.style.setProperty('--transition-duration', `${300 * speedMultiplier}ms`);
    
    // Apply animation style
    root.style.setProperty('--animation-style', 'smooth');
    body.setAttribute('data-animation-style', 'smooth');

    // Apply animation class
    if (settings.animationsEnabled) {
      body.classList.add('animations-enabled');
      body.classList.remove('animations-disabled');
    } else {
      body.classList.add('animations-disabled');
      body.classList.remove('animations-enabled');
    }

    // Apply light theme backgrounds
    root.style.setProperty('--background-color', '#F5F5F5');
    root.style.setProperty('--surface-color', '#FFFFFF');
    root.style.setProperty('--text-color', '#212121');
    root.style.setProperty('--text-secondary-color', '#757575');
    body.style.backgroundColor = '#F5F5F5';
    body.style.color = '#212121';

    // Apply sidebar and header styles
    this.applySidebarStyles(settings);
    this.applyHeaderStyles(settings);
    this.applyFooterStyles(settings);
    this.applyContentStyles(settings);
    
    // Force immediate re-application after a short delay to ensure DOM is ready
    setTimeout(() => {
      this.forceUpdateAllElements(componentStyles);
      
      // Apply layout class
      if (settings.layoutConfig?.type) {
        const body = document.body;
        Object.keys(LAYOUT_PRESETS).forEach(type => {
          body.classList.remove(`layout-${type}`);
        });
        body.classList.add(`layout-${settings.layoutConfig.type}`);
      }
      
    }, 100);
    
  }
  
  private applyLayout(layout: any): void {
    if (!this.isBrowser || !layout) return;
    
    // Use layout service to apply layout
    if (layout.type) {
      this.layoutService.setLayout(layout.type);
    } else {
      this.layoutService.setCustomLayout(layout);
    }
    
    // Apply layout class to body
    setTimeout(() => {
      const body = document.body;
      if (layout.type) {
        Object.keys(LAYOUT_PRESETS).forEach(type => {
          body.classList.remove(`layout-${type}`);
        });
        body.classList.add(`layout-${layout.type}`);
      }
    }, 0);
  }

  private applySidebarStyles(settings: AppearanceSettings): void {
    const root = document.documentElement;
    
    // Default light sidebar - use primary color from settings
    const primaryColor = settings.primaryColor || '#667eea';
    const hexToRgb = (hex: string): string => {
      const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
      return result ? `${parseInt(result[1], 16)}, ${parseInt(result[2], 16)}, ${parseInt(result[3], 16)}` : '102, 126, 234';
    };
    const activeRgb = hexToRgb(primaryColor);
    root.style.setProperty('--sidebar-background', 'var(--surface-color)');
    root.style.setProperty('--sidebar-text', 'var(--text-color)');
    root.style.setProperty('--sidebar-hover', 'rgba(0, 0, 0, 0.05)');
    root.style.setProperty('--sidebar-active', primaryColor);
    root.style.setProperty('--sidebar-active-rgb', activeRgb);
    root.style.setProperty('--sidebar-active-bg', `rgba(${activeRgb}, 0.1)`);
    root.style.setProperty('--sidebar-border-color', 'rgba(0, 0, 0, 0.08)');
    root.style.setProperty('--sidebar-margin', '0px');
    root.style.setProperty('--sidebar-border-radius', '0px');
    
    // Apply to sidebar element
    setTimeout(() => {
      const sidebarElement = document.querySelector('.sidebar') as HTMLElement;
      if (sidebarElement) {
        sidebarElement.style.margin = '0px';
        sidebarElement.style.borderRadius = '0px';
      }
    }, 0);
  }

  private applyHeaderStyles(settings: AppearanceSettings): void {
    const root = document.documentElement;
    
    // Default header styles
    root.style.setProperty('--header-background', 'var(--surface-color)');
    root.style.setProperty('--header-text', 'var(--text-color)');
    root.style.setProperty('--header-border-color', 'rgba(0, 0, 0, 0.08)');
    root.style.setProperty('--header-shadow', '0 2px 4px rgba(0, 0, 0, 0.1)');
    root.style.setProperty('--header-design', 'flat');
    root.style.setProperty('--header-border-radius', '0px');
    root.style.setProperty('--header-margin', '0px');
    root.style.setProperty('--header-height', '72px');
    root.style.setProperty('--layout-header-height', '72px');
    
    setTimeout(() => {
      const headerElement = document.querySelector('.app-header') as HTMLElement;
      if (headerElement) {
        headerElement.setAttribute('data-header-design', 'flat');
      }
    }, 0);
  }

  private applyFooterStyles(settings: AppearanceSettings): void {
    const root = document.documentElement;
    
    // Default footer styles
    root.style.setProperty('--footer-background', 'var(--surface-color)');
    root.style.setProperty('--footer-text', 'var(--text-color)');
    root.style.setProperty('--footer-border-radius', '0px');
    root.style.setProperty('--footer-margin', '0px');
  }

  private applyContentStyles(settings: AppearanceSettings): void {
    const root = document.documentElement;
    const body = document.body;
    
    root.style.setProperty('--content-background', 'var(--background-color)');
    root.style.setProperty('--content-text', 'var(--text-color)');
    root.style.setProperty('--hover-color', 'rgba(0, 0, 0, 0.05)');
  }

  // Removed applyThemeMode - using single light theme only

  private forceUpdateButtonColors(colors: any): void {
    // Update all button elements with theme colors
    const buttons = document.querySelectorAll('button.btn-primary, .btn-primary, button.btn-secondary, .btn-secondary, button.btn-success, .btn-success, button.btn-danger, .btn-danger, button.btn-warning, .btn-warning, button.btn-info, .btn-info, .custom-btn-primary, .custom-btn-secondary, .custom-btn-success, .custom-btn-danger, .custom-btn-warning, .custom-btn-info');
    buttons.forEach(btn => {
      const btnEl = btn as HTMLElement;
      const classes = btnEl.className;
      
      if (classes.includes('primary') || classes.includes('btn-primary')) {
        btnEl.style.setProperty('background-color', colors.primaryColor, 'important');
        btnEl.style.setProperty('border-color', colors.primaryColor, 'important');
      } else if (classes.includes('secondary') || classes.includes('btn-secondary')) {
        btnEl.style.setProperty('background-color', colors.secondaryColor, 'important');
        btnEl.style.setProperty('border-color', colors.secondaryColor, 'important');
      } else if (classes.includes('success') || classes.includes('btn-success')) {
        btnEl.style.setProperty('background-color', colors.successColor, 'important');
        btnEl.style.setProperty('border-color', colors.successColor, 'important');
      } else if (classes.includes('danger') || classes.includes('btn-danger') || classes.includes('error')) {
        btnEl.style.setProperty('background-color', colors.dangerColor, 'important');
        btnEl.style.setProperty('border-color', colors.dangerColor, 'important');
      } else if (classes.includes('warning') || classes.includes('btn-warning')) {
        btnEl.style.setProperty('background-color', colors.warningColor, 'important');
        btnEl.style.setProperty('border-color', colors.warningColor, 'important');
      } else if (classes.includes('info') || classes.includes('btn-info')) {
        btnEl.style.setProperty('background-color', colors.infoColor, 'important');
        btnEl.style.setProperty('border-color', colors.infoColor, 'important');
      }
    });
  }


  private forceUpdateBadgeColors(colors: any): void {
    // Update badge elements with theme colors
    const badges = document.querySelectorAll('.badge-primary, .badge-secondary, .badge-success, .badge-danger, .badge-warning, .badge-info, .badge.badge-primary, .badge.badge-secondary, .badge.badge-success, .badge.badge-danger, .badge.badge-warning, .badge.badge-info');
    badges.forEach(badge => {
      const badgeEl = badge as HTMLElement;
      const classes = badgeEl.className;
      
      if (classes.includes('primary') || classes.includes('badge-primary')) {
        badgeEl.style.setProperty('background-color', colors.primaryColor, 'important');
      } else if (classes.includes('secondary') || classes.includes('badge-secondary')) {
        badgeEl.style.setProperty('background-color', colors.secondaryColor, 'important');
      } else if (classes.includes('success') || classes.includes('badge-success')) {
        badgeEl.style.setProperty('background-color', colors.successColor, 'important');
      } else if (classes.includes('danger') || classes.includes('badge-danger') || classes.includes('error')) {
        badgeEl.style.setProperty('background-color', colors.dangerColor, 'important');
      } else if (classes.includes('warning') || classes.includes('badge-warning')) {
        badgeEl.style.setProperty('background-color', colors.warningColor, 'important');
      } else if (classes.includes('info') || classes.includes('badge-info')) {
        badgeEl.style.setProperty('background-color', colors.infoColor, 'important');
      }
    });
  }

  /**
   * Force update all elements to use component styles
   */
  private forceUpdateAllElements(componentStyles: any): void {
    if (!this.isBrowser) return;

    // Force update all buttons
    const buttons = document.querySelectorAll('button, .btn, [class*="btn"]');
    buttons.forEach(btn => {
      const btnEl = btn as HTMLElement;
      // Remove old style classes
      btnEl.classList.remove('btn-flat', 'btn-raised', 'btn-outlined', 'btn-gradient', 'btn-glass');
      // Add appropriate class based on button style
      if (componentStyles.buttonStyle === 'flat') {
        btnEl.classList.add('btn-flat');
      } else if (componentStyles.buttonStyle === 'raised') {
        btnEl.classList.add('btn-raised');
      } else if (componentStyles.buttonStyle === 'outlined') {
        btnEl.classList.add('btn-outlined');
      } else if (componentStyles.buttonStyle === 'gradient') {
        btnEl.classList.add('btn-gradient');
      } else if (componentStyles.buttonStyle === 'glass') {
        btnEl.classList.add('btn-glass');
      }
    });

    // Force update all cards
    const cards = document.querySelectorAll('.card, .page-card, [class*="card"]');
    cards.forEach(card => {
      const cardEl = card as HTMLElement;
      // Remove old style classes
      cardEl.classList.remove('card-flat', 'card-elevated', 'card-outlined', 'card-glass');
      // Add appropriate class based on card style
      if (componentStyles.cardStyle === 'flat') {
        cardEl.classList.add('card-flat');
      } else if (componentStyles.cardStyle === 'elevated') {
        cardEl.classList.add('card-elevated');
      } else if (componentStyles.cardStyle === 'outlined') {
        cardEl.classList.add('card-outlined');
      } else if (componentStyles.cardStyle === 'glass') {
        cardEl.classList.add('card-glass');
      }
    });

    // Force update all tables
    if (componentStyles.tableStyle?.design) {
      const tables = document.querySelectorAll('table, .table, .custom-table, .data-table');
      tables.forEach(table => {
        const tableEl = table as HTMLElement;
        tableEl.setAttribute('data-table-design', componentStyles.tableStyle.design);
      });
    }
  }
}

