import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface ThemeConfig {
  primaryColor: string;
  secondaryColor: string;
  accentColor: string;
  backgroundColor: string;
  surfaceColor: string;
  textColor: string;
  textSecondaryColor: string;
  errorColor: string;
  successColor: string;
  warningColor: string;
  infoColor: string;
  borderRadius: string;
  spacing: string;
  fontFamily: string;
  buttonStyle: 'raised' | 'flat' | 'outlined';
  cardStyle: 'elevated' | 'outlined' | 'flat';
  cardElevation: number;
}

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private defaultTheme: ThemeConfig = {
    primaryColor: '#2196F3',
    secondaryColor: '#FF9800',
    accentColor: '#4CAF50',
    backgroundColor: '#F5F5F5',
    surfaceColor: '#FFFFFF',
    textColor: '#212121',
    textSecondaryColor: '#757575',
    errorColor: '#F44336',
    successColor: '#4CAF50',
    warningColor: '#FF9800',
    infoColor: '#2196F3',
    borderRadius: '8px',
    spacing: '16px',
    fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", "Roboto", "Oxygen", "Ubuntu", "Cantarell", "Fira Sans", "Droid Sans", "Helvetica Neue", sans-serif',
    buttonStyle: 'raised',
    cardStyle: 'elevated',
    cardElevation: 2
  };

  private themeSubject = new BehaviorSubject<ThemeConfig>(this.defaultTheme);
  public theme$: Observable<ThemeConfig> = this.themeSubject.asObservable();

  constructor() {
    this.loadTheme();
  }

  /**
   * Get current theme configuration
   */
  getTheme(): ThemeConfig {
    return this.themeSubject.value;
  }

  /**
   * Update theme configuration
   */
  setTheme(theme: Partial<ThemeConfig>): void {
    const newTheme = { ...this.themeSubject.value, ...theme };
    this.themeSubject.next(newTheme);
    this.applyTheme(newTheme);
    this.saveTheme(newTheme);
  }

  /**
   * Reset theme to default
   */
  resetTheme(): void {
    this.setTheme(this.defaultTheme);
  }

  /**
   * Apply theme to CSS custom properties
   */
  private applyTheme(theme: ThemeConfig): void {
    const root = document.documentElement;
    
    root.style.setProperty('--primary-color', theme.primaryColor);
    root.style.setProperty('--secondary-color', theme.secondaryColor);
    root.style.setProperty('--accent-color', theme.accentColor);
    root.style.setProperty('--background-color', theme.backgroundColor);
    root.style.setProperty('--surface-color', theme.surfaceColor);
    root.style.setProperty('--text-color', theme.textColor);
    root.style.setProperty('--text-secondary-color', theme.textSecondaryColor);
    root.style.setProperty('--error-color', theme.errorColor);
    root.style.setProperty('--success-color', theme.successColor);
    root.style.setProperty('--warning-color', theme.warningColor);
    root.style.setProperty('--info-color', theme.infoColor);
    root.style.setProperty('--border-radius', theme.borderRadius);
    root.style.setProperty('--spacing', theme.spacing);
    root.style.setProperty('--font-family', theme.fontFamily);
    root.style.setProperty('--button-style', theme.buttonStyle);
    root.style.setProperty('--card-style', theme.cardStyle);
    root.style.setProperty('--card-elevation', theme.cardElevation.toString());

    // Calculate spacing variants
    root.style.setProperty('--spacing-xs', `calc(${theme.spacing} * 0.25)`);
    root.style.setProperty('--spacing-sm', `calc(${theme.spacing} * 0.5)`);
    root.style.setProperty('--spacing-md', theme.spacing);
    root.style.setProperty('--spacing-lg', `calc(${theme.spacing} * 1.5)`);
    root.style.setProperty('--spacing-xl', `calc(${theme.spacing} * 2)`);
    root.style.setProperty('--spacing-xxl', `calc(${theme.spacing} * 3)`);

    // Update Bootstrap variables
    this.updateBootstrapVariables(theme);
  }

  /**
   * Update Bootstrap CSS variables
   */
  private updateBootstrapVariables(theme: ThemeConfig): void {
    const root = document.documentElement;
    const primaryRgb = this.hexToRgb(theme.primaryColor);
    const secondaryRgb = this.hexToRgb(theme.secondaryColor);
    const successRgb = this.hexToRgb(theme.successColor);
    const dangerRgb = this.hexToRgb(theme.errorColor);
    const warningRgb = this.hexToRgb(theme.warningColor);
    const infoRgb = this.hexToRgb(theme.infoColor);

    if (primaryRgb) root.style.setProperty('--bs-primary-rgb', primaryRgb);
    if (secondaryRgb) root.style.setProperty('--bs-secondary-rgb', secondaryRgb);
    if (successRgb) root.style.setProperty('--bs-success-rgb', successRgb);
    if (dangerRgb) root.style.setProperty('--bs-danger-rgb', dangerRgb);
    if (warningRgb) root.style.setProperty('--bs-warning-rgb', warningRgb);
    if (infoRgb) root.style.setProperty('--bs-info-rgb', infoRgb);
  }

  /**
   * Convert hex color to RGB
   */
  private hexToRgb(hex: string): string | null {
    const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    return result
      ? `${parseInt(result[1], 16)}, ${parseInt(result[2], 16)}, ${parseInt(result[3], 16)}`
      : null;
  }

  /**
   * Save theme to localStorage
   */
  private saveTheme(theme: ThemeConfig): void {
    try {
      localStorage.setItem('app-theme', JSON.stringify(theme));
    } catch (error) {
      console.error('Failed to save theme:', error);
    }
  }

  /**
   * Load theme from localStorage
   */
  private loadTheme(): void {
    try {
      const savedTheme = localStorage.getItem('app-theme');
      if (savedTheme) {
        const theme = JSON.parse(savedTheme);
        this.setTheme(theme);
      } else {
        this.applyTheme(this.defaultTheme);
      }
    } catch (error) {
      console.error('Failed to load theme:', error);
      this.applyTheme(this.defaultTheme);
    }
  }
}

