import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ThemeConfig, DEFAULT_THEME } from '../models/theme.model';
import { ThemeApiService } from './theme-api.service';

const THEME_STORAGE_KEY = 'app_theme';
const USER_THEME_STORAGE_KEY = 'user_theme';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private themeSubject = new BehaviorSubject<ThemeConfig>(DEFAULT_THEME);
  public theme$: Observable<ThemeConfig> = this.themeSubject.asObservable();

  private currentTheme: ThemeConfig = DEFAULT_THEME;
  private isBrowser: boolean;

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private themeApi: ThemeApiService
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
    this.initializeTheme();
  }

  private initializeTheme(): void {
    if (!this.isBrowser) {
      this.applyTheme(DEFAULT_THEME);
      return;
    }

    // Try to load from localStorage first
    const storedTheme = this.loadFromLocalStorage();
    if (storedTheme) {
      // Merge with DEFAULT_THEME to ensure all properties exist
      this.currentTheme = { ...DEFAULT_THEME, ...storedTheme };
      this.applyTheme(this.currentTheme);
      this.themeSubject.next(this.currentTheme);
    } else {
      // Try to load from backend
      this.loadFromBackend();
    }
  }

  private loadFromLocalStorage(): ThemeConfig | null {
    try {
      const stored = localStorage.getItem(THEME_STORAGE_KEY);
      if (stored) {
        return JSON.parse(stored);
      }
    } catch (error) {
      console.error('Error loading theme from localStorage:', error);
    }
    return null;
  }

  private loadFromBackend(): void {
    // Try user theme first, fallback to global
    this.themeApi.getUserTheme().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.updateTheme(response.data, false);
        } else {
          // If user theme fails, try global theme
          this.loadGlobalTheme();
        }
      },
      error: () => {
        // If user theme fails, try global theme
        this.loadGlobalTheme();
      }
    });
  }

  private loadGlobalTheme(): void {
    this.themeApi.getGlobalTheme().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.updateTheme(response.data, false);
        } else {
          // Use default theme if backend fails
          this.updateTheme(DEFAULT_THEME, false);
        }
      },
      error: () => {
        // Use default theme if backend fails
        this.updateTheme(DEFAULT_THEME, false);
      }
    });
  }

  private saveToLocalStorage(theme: ThemeConfig): void {
    if (this.isBrowser) {
      try {
        localStorage.setItem(THEME_STORAGE_KEY, JSON.stringify(theme));
      } catch (error) {
        console.error('Error saving theme to localStorage:', error);
      }
    }
  }

  getTheme(): ThemeConfig {
    return { ...this.currentTheme };
  }

  updateTheme(theme: Partial<ThemeConfig>, saveToBackend: boolean = true): void {
    // Merge with DEFAULT_THEME first, then current theme, then new values
    this.currentTheme = { ...DEFAULT_THEME, ...this.currentTheme, ...theme };
    this.applyTheme(this.currentTheme);
    this.themeSubject.next(this.currentTheme);
    this.saveToLocalStorage(this.currentTheme);

    if (saveToBackend && this.isBrowser) {
      // Save to backend asynchronously
      this.themeApi.saveUserTheme(this.currentTheme).subscribe({
        error: (error) => {
          console.error('Error saving theme to backend:', error);
        }
      });
    }
  }

  updateProperty<K extends keyof ThemeConfig>(key: K, value: ThemeConfig[K]): void {
    this.updateTheme({ [key]: value } as Partial<ThemeConfig>);
  }

  resetTheme(): void {
    this.updateTheme(DEFAULT_THEME);
  }

  private applyTheme(theme: ThemeConfig): void {
    if (!this.isBrowser) return;

    const root = document.documentElement;

    // Branding
    root.style.setProperty('--logo-url', theme.logoUrl ? `url(${theme.logoUrl})` : 'none');
    root.style.setProperty('--logo-height', theme.logoHeight);

    // Colors
    root.style.setProperty('--primary-color', theme.primaryColor);
    root.style.setProperty('--secondary-color', theme.secondaryColor);
    root.style.setProperty('--accent-color', theme.accentColor);
    root.style.setProperty('--background-color', theme.backgroundColor);
    root.style.setProperty('--surface-color', theme.surfaceColor);
    root.style.setProperty('--text-color', theme.textColor);
    root.style.setProperty('--text-secondary-color', theme.textSecondaryColor);

    // Additional Colors
    root.style.setProperty('--success-color', theme.successColor);
    root.style.setProperty('--danger-color', theme.dangerColor);
    root.style.setProperty('--warning-color', theme.warningColor);
    root.style.setProperty('--info-color', theme.infoColor);

    // Animations
    root.style.setProperty('--animations-enabled', theme.animationsEnabled ? '1' : '0');
    const speedMultiplier = theme.animationSpeed === 'slow' ? 1.5 : theme.animationSpeed === 'fast' ? 0.5 : 1;
    root.style.setProperty('--transition-duration', `${theme.transitionDuration * speedMultiplier}ms`);
    root.style.setProperty('--animation-easing', theme.animationEasing);
    root.style.setProperty('--hover-transform', theme.hoverTransform);

    // Buttons
    root.style.setProperty('--button-style', theme.buttonStyle);
    root.style.setProperty('--button-border-radius', theme.buttonBorderRadius);
    root.style.setProperty('--button-padding', theme.buttonPadding);

    // Cards
    root.style.setProperty('--card-style', theme.cardStyle);
    root.style.setProperty('--card-border-radius', theme.cardBorderRadius);
    root.style.setProperty('--card-elevation', theme.cardElevation.toString());
    root.style.setProperty('--card-shadow', theme.cardShadow);
    root.style.setProperty('--card-hover-shadow', theme.cardHoverShadow);

    // Charts
    root.style.setProperty('--chart-border-radius', theme.chartBorderRadius);
    root.style.setProperty('--chart-animation-speed', theme.chartAnimationSpeed.toString());
    root.style.setProperty('--chart-animation-easing', theme.chartAnimationEasing);

    // Layout
    root.style.setProperty('--border-radius', theme.borderRadius);
    root.style.setProperty('--spacing', theme.spacing);

    // Apply animation class
    if (theme.animationsEnabled) {
      document.body.classList.add('animations-enabled');
      document.body.classList.remove('animations-disabled');
    } else {
      document.body.classList.add('animations-disabled');
      document.body.classList.remove('animations-enabled');
    }
  }

  uploadLogo(file: File): Observable<{ logoUrl: string }> {
    return this.themeApi.uploadLogo(file).pipe(
      map(response => {
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Failed to upload logo');
      })
    );
  }

  setLogo(logoUrl: string): void {
    this.updateProperty('logoUrl', logoUrl);
  }
}

