export interface ThemeConfig {
  // Branding
  logoUrl: string;
  logoHeight: string;

  // Colors
  primaryColor: string;
  secondaryColor: string;
  accentColor: string;
  backgroundColor: string;
  surfaceColor: string;
  textColor: string;
  textSecondaryColor: string;

  // Additional Colors
  successColor: string;
  dangerColor: string;
  warningColor: string;
  infoColor: string;

  // Animations
  animationsEnabled: boolean;
  animationSpeed: 'slow' | 'normal' | 'fast';
  transitionDuration: number;
  animationEasing: string;
  hoverTransform: string;

  // Buttons
  buttonStyle: 'flat' | 'raised' | 'outlined' | 'gradient';
  buttonBorderRadius: string;
  buttonPadding: string;

  // Cards
  cardStyle: 'flat' | 'elevated' | 'outlined';
  cardBorderRadius: string;
  cardElevation: number;
  cardShadow: string;
  cardHoverShadow: string;

  // Charts
  chartBorderRadius: string;
  chartAnimationSpeed: number;
  chartAnimationEasing: string;

  // Layout
  borderRadius: string;
  spacing: string;
}

export const DEFAULT_THEME: ThemeConfig = {
  logoUrl: '',
  logoHeight: '40px',
  primaryColor: '#4f46e5',
  secondaryColor: '#7c3aed',
  accentColor: '#db2777',
  backgroundColor: '#F5F5F5',
  surfaceColor: '#FFFFFF',
  textColor: '#212121',
  textSecondaryColor: '#757575',
  successColor: '#22c55e',
  dangerColor: '#ef4444',
  warningColor: '#f59e0b',
  infoColor: '#0ea5e9',
  animationsEnabled: true,
  animationSpeed: 'normal',
  transitionDuration: 300,
  animationEasing: 'cubic-bezier(0.4, 0, 0.2, 1)',
  hoverTransform: 'translateY(-3px)',
  buttonStyle: 'raised',
  buttonBorderRadius: '8px',
  buttonPadding: '12px 24px',
  cardStyle: 'elevated',
  cardBorderRadius: '16px',
  cardElevation: 2,
  cardShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
  cardHoverShadow: '0 8px 20px rgba(0, 0, 0, 0.06)',
  chartBorderRadius: '15px',
  chartAnimationSpeed: 800,
  chartAnimationEasing: 'easeinout',
  borderRadius: '8px',
  spacing: '16px'
};

