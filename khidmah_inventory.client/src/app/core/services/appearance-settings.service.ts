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
  private prefersColorSchemeQuery: MediaQueryList | null = null;
  private prefersColorSchemeListener: (() => void) | null = null;

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
      this.currentSettings = { ...DEFAULT_APPEARANCE_SETTINGS, ...stored };
      // Sync layout with layout service
      if (this.currentSettings.layoutConfig) {
        this.layoutService.setCustomLayout(this.currentSettings.layoutConfig);
      }
      this.applySettings(this.currentSettings);
      this.settingsSubject.next(this.currentSettings);
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

    // Merge with defaults to ensure all properties exist
    const s = { ...DEFAULT_APPEARANCE_SETTINGS, ...settings };

    // ============ THEME MODE (resolve first so we know whether to apply light colors) ============
    const resolvedTheme = this.resolveThemeMode(s.themeMode);
    document.documentElement.setAttribute('data-theme', resolvedTheme);
    body.setAttribute('data-theme', s.themeMode);
    this.setupPrefersColorSchemeListener(s.themeMode);

    // ============ COLORS (only set when light theme so dark theme CSS variables take effect) ============
    if (resolvedTheme === 'light') {
      root.style.setProperty('--primary-color', s.primaryColor);
      root.style.setProperty('--secondary-color', s.secondaryColor);
      root.style.setProperty('--accent-color', s.accentColor);
      root.style.setProperty('--success-color', s.successColor);
      root.style.setProperty('--warning-color', s.warningColor);
      root.style.setProperty('--danger-color', s.dangerColor);
      root.style.setProperty('--info-color', s.infoColor);
      root.style.setProperty('--error-color', s.dangerColor);
      root.style.setProperty('--background-color', s.backgroundColor);
      root.style.setProperty('--surface-color', s.surfaceColor);
      root.style.setProperty('--text-color', s.textColor);
      root.style.setProperty('--text-secondary-color', s.textSecondaryColor);
      root.style.setProperty('--border-color', s.borderColor);
      body.style.backgroundColor = s.backgroundColor;
      body.style.color = s.textColor;
    } else {
      // Dark theme: remove inline overrides so stylesheet dark variables apply
      root.style.removeProperty('--primary-color');
      root.style.removeProperty('--secondary-color');
      root.style.removeProperty('--accent-color');
      root.style.removeProperty('--success-color');
      root.style.removeProperty('--warning-color');
      root.style.removeProperty('--danger-color');
      root.style.removeProperty('--info-color');
      root.style.removeProperty('--error-color');
      root.style.removeProperty('--background-color');
      root.style.removeProperty('--surface-color');
      root.style.removeProperty('--text-color');
      root.style.removeProperty('--text-secondary-color');
      root.style.removeProperty('--border-color');
      body.style.removeProperty('background-color');
      body.style.removeProperty('color');
    }

    // ============ BRANDING ============
    root.style.setProperty('--logo-url', s.logoUrl ? `url(${s.logoUrl})` : 'none');
    root.style.setProperty('--logo-height', s.logoHeight);
    root.style.setProperty('--company-name', `"${s.companyName}"`);

    // ============ SPACING ============
    root.style.setProperty('--spacing', s.spacingValue);
    root.style.setProperty('--container-max-width', s.containerMaxWidth);
    root.style.setProperty('--content-padding', s.contentPadding);
    body.setAttribute('data-spacing', s.spacing);

    // ============ RADIUS ============
    root.style.setProperty('--border-radius', s.borderRadius);
    root.style.setProperty('--button-border-radius', s.buttonBorderRadius);
    root.style.setProperty('--card-border-radius', s.cardBorderRadius);
    root.style.setProperty('--input-border-radius', s.inputBorderRadius);
    root.style.setProperty('--modal-border-radius', s.modalBorderRadius);
    root.style.setProperty('--dropdown-border-radius', s.dropdownBorderRadius);
    root.style.setProperty('--badge-border-radius', s.badgeBorderRadius);
    root.style.setProperty('--table-border-radius', s.tableBorderRadius);
    root.style.setProperty('--chart-border-radius', s.chartBorderRadius);

    // ============ BUTTONS ============
    root.style.setProperty('--button-style', s.buttonStyle);
    root.style.setProperty('--button-size', s.buttonSize);
    root.style.setProperty('--button-padding', s.buttonPadding);
    root.style.setProperty('--button-font-size', s.buttonFontSize);
    root.style.setProperty('--button-font-weight', s.buttonFontWeight);
    root.style.setProperty('--button-text-transform', s.buttonTextTransform);
    root.style.setProperty('--button-shadow', s.buttonShadow);
    root.style.setProperty('--button-hover-effect', s.buttonHoverEffect);
    body.setAttribute('data-button-style', s.buttonStyle);
    body.setAttribute('data-button-hover-effect', s.buttonHoverEffect);

    // ============ CARDS ============
    root.style.setProperty('--card-style', s.cardStyle);
    root.style.setProperty('--card-elevation', (s.cardElevation ?? 2).toString());
    root.style.setProperty('--card-padding', s.cardPadding);
    root.style.setProperty('--card-shadow', s.cardShadow);
    root.style.setProperty('--card-hover-shadow', s.cardHoverShadow);
    root.style.setProperty('--card-hover-effect', s.cardHoverEffect);
    body.setAttribute('data-card-style', s.cardStyle);
    body.setAttribute('data-card-hover-effect', s.cardHoverEffect);

    // ============ FORM FIELDS ============
    root.style.setProperty('--input-style', s.inputStyle);
    root.style.setProperty('--input-size', s.inputSize);
    root.style.setProperty('--input-padding', s.inputPadding);
    root.style.setProperty('--input-font-size', s.inputFontSize);
    root.style.setProperty('--input-border-width', s.inputBorderWidth);
    root.style.setProperty('--input-focus-effect', s.inputFocusEffect);
    root.style.setProperty('--label-position', s.labelPosition);
    body.setAttribute('data-input-style', s.inputStyle);
    body.setAttribute('data-label-position', s.labelPosition);

    // ============ TYPOGRAPHY ============
    root.style.setProperty('--font-family', s.fontFamily);
    root.style.setProperty('--font-size', s.fontSize);
    root.style.setProperty('--font-weight', s.fontWeight);
    root.style.setProperty('--line-height', s.lineHeight);
    root.style.setProperty('--heading-font-family', s.headingFontFamily);
    root.style.setProperty('--heading-font-weight', s.headingFontWeight);
    body.style.fontFamily = s.fontFamily;
    body.style.fontSize = s.fontSize;
    body.style.lineHeight = s.lineHeight;

    // ============ ANIMATIONS ============
    root.style.setProperty('--animations-enabled', s.animationsEnabled ? '1' : '0');
    const speedMultiplier = s.animationSpeed === 'slow' ? 1.5 : s.animationSpeed === 'fast' ? 0.5 : 1;
    root.style.setProperty('--transition-duration', `${s.transitionDuration * speedMultiplier}ms`);
    root.style.setProperty('--animation-easing', s.animationEasing);
    root.style.setProperty('--hover-transform', s.hoverTransform);
    root.style.setProperty('--page-transition', s.pageTransition);

    if (s.animationsEnabled) {
      body.classList.add('animations-enabled');
      body.classList.remove('animations-disabled');
    } else {
      body.classList.add('animations-disabled');
      body.classList.remove('animations-enabled');
    }
    body.setAttribute('data-animation-speed', s.animationSpeed);
    body.setAttribute('data-page-transition', s.pageTransition);

    // ============ SIDEBAR ============
    root.style.setProperty('--sidebar-style', s.sidebarStyle);
    root.style.setProperty('--sidebar-width', s.sidebarWidth);
    root.style.setProperty('--sidebar-collapsed-width', s.sidebarCollapsedWidth);
    root.style.setProperty('--sidebar-item-style', s.sidebarItemStyle);
    root.style.setProperty('--sidebar-item-spacing', s.sidebarItemSpacing);
    root.style.setProperty('--layout-sidebar-width', s.sidebarWidth);
    body.setAttribute('data-sidebar-style', s.sidebarStyle);
    body.setAttribute('data-sidebar-item-style', s.sidebarItemStyle);

    // ============ HEADER ============
    root.style.setProperty('--header-style', s.headerStyle);
    root.style.setProperty('--header-height', s.headerHeight);
    root.style.setProperty('--header-shadow', s.headerShadow);
    root.style.setProperty('--header-border-bottom', s.headerBorderBottom);
    root.style.setProperty('--layout-header-height', s.headerHeight);
    body.setAttribute('data-header-style', s.headerStyle);

    // ============ TABLES ============
    root.style.setProperty('--table-style', s.tableStyle);
    root.style.setProperty('--table-header-style', s.tableHeaderStyle);
    root.style.setProperty('--table-row-hover-effect', s.tableRowHoverEffect);
    body.setAttribute('data-table-style', s.tableStyle);
    body.setAttribute('data-table-header-style', s.tableHeaderStyle);

    // ============ MODALS ============
    root.style.setProperty('--modal-backdrop', s.modalBackdrop);
    root.style.setProperty('--modal-animation', s.modalAnimation);
    root.style.setProperty('--modal-shadow', s.modalShadow);
    body.setAttribute('data-modal-backdrop', s.modalBackdrop);
    body.setAttribute('data-modal-animation', s.modalAnimation);

    // ============ DROPDOWNS ============
    root.style.setProperty('--dropdown-style', s.dropdownStyle);
    root.style.setProperty('--dropdown-shadow', s.dropdownShadow);
    root.style.setProperty('--dropdown-animation', s.dropdownAnimation);
    body.setAttribute('data-dropdown-style', s.dropdownStyle);

    // ============ BADGES ============
    root.style.setProperty('--badge-style', s.badgeStyle);
    root.style.setProperty('--badge-size', s.badgeSize);
    body.setAttribute('data-badge-style', s.badgeStyle);
    body.setAttribute('data-badge-size', s.badgeSize);

    // ============ CHARTS ============
    root.style.setProperty('--chart-animation-speed', (s.chartAnimationSpeed ?? 800).toString());
    root.style.setProperty('--chart-animation-easing', s.chartAnimationEasing);
    root.style.setProperty('--chart-color-scheme', s.chartColorScheme);
    body.setAttribute('data-chart-color-scheme', s.chartColorScheme);

    // ============ SCROLLBAR ============
    root.style.setProperty('--scrollbar-width', s.scrollbarWidth);
    root.style.setProperty('--scrollbar-color', s.scrollbarColor);
    if (s.customScrollbar) {
      body.classList.add('custom-scrollbar');
    } else {
      body.classList.remove('custom-scrollbar');
    }

    // ============ EFFECTS ============
    root.style.setProperty('--glass-blur', s.glassBlur);
    root.style.setProperty('--glass-opacity', (s.glassOpacity ?? 0.1).toString());
    root.style.setProperty('--shadow-intensity', s.shadowIntensity);
    if (s.glassEffect) {
      body.classList.add('glass-effect-enabled');
    } else {
      body.classList.remove('glass-effect-enabled');
    }
    body.setAttribute('data-shadow-intensity', s.shadowIntensity);

    // Apply layout
    this.applyLayout(s.layoutConfig);

    // Apply sidebar and header styles
    this.applySidebarStyles(s);
    this.applyHeaderStyles(s);
    this.applyFooterStyles(s);
    this.applyContentStyles(s);

    // Force update colors and elements
    setTimeout(() => {
      const colors = {
        primaryColor: s.primaryColor,
        secondaryColor: s.secondaryColor,
        accentColor: s.accentColor,
        successColor: s.successColor,
        warningColor: s.warningColor,
        dangerColor: s.dangerColor,
        infoColor: s.infoColor
      };
      this.forceUpdateButtonColors(colors);
      this.forceUpdateBadgeColors(colors);
      this.forceUpdateAllElements({
        borderRadius: s.borderRadius,
        buttonStyle: s.buttonStyle,
        cardStyle: s.cardStyle,
        cardElevation: s.cardElevation
      });

      // Apply layout class
      if (s.layoutConfig?.type) {
        Object.keys(LAYOUT_PRESETS).forEach(type => {
          body.classList.remove(`layout-${type}`);
        });
        body.classList.add(`layout-${s.layoutConfig.type}`);
      }
    }, 100);
  }

  /**
   * Resolve theme mode: 'auto' -> 'light' | 'dark' based on prefers-color-scheme
   */
  private resolveThemeMode(themeMode: 'light' | 'dark' | 'auto'): 'light' | 'dark' {
    if (themeMode === 'dark') return 'dark';
    if (themeMode === 'light') return 'light';
    if (this.isBrowser && typeof window.matchMedia !== 'undefined') {
      return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }
    return 'light';
  }

  private setupPrefersColorSchemeListener(themeMode: 'light' | 'dark' | 'auto'): void {
    if (!this.isBrowser || themeMode !== 'auto') {
      if (this.prefersColorSchemeListener && this.prefersColorSchemeQuery) {
        this.prefersColorSchemeQuery.removeEventListener('change', this.prefersColorSchemeListener);
        this.prefersColorSchemeQuery = null;
        this.prefersColorSchemeListener = null;
      }
      return;
    }
    const query = window.matchMedia('(prefers-color-scheme: dark)');
    const listener = () => {
      const resolved = this.resolveThemeMode('auto');
      document.documentElement.setAttribute('data-theme', resolved);
    };
    if (this.prefersColorSchemeListener && this.prefersColorSchemeQuery) {
      this.prefersColorSchemeQuery.removeEventListener('change', this.prefersColorSchemeListener);
    }
    this.prefersColorSchemeQuery = query;
    this.prefersColorSchemeListener = listener;
    query.addEventListener('change', listener);
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

