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
  
  // Animations
  animationsEnabled: boolean;
  animationSpeed: 'slow' | 'normal' | 'fast';
  transitionDuration: number;
  
  // Buttons
  buttonStyle: 'flat' | 'raised' | 'outlined' | 'gradient';
  buttonBorderRadius: string;
  buttonPadding: string;
  
  // Cards
  cardStyle: 'flat' | 'elevated' | 'outlined';
  cardBorderRadius: string;
  cardElevation: number;
  cardShadow: string;
  
  // Layout
  borderRadius: string;
  spacing: string;
}

export const DEFAULT_THEME: ThemeConfig = {
  logoUrl: '',
  logoHeight: '40px',
  primaryColor: '#2196F3',
  secondaryColor: '#FF9800',
  accentColor: '#4CAF50',
  backgroundColor: '#F5F5F5',
  surfaceColor: '#FFFFFF',
  textColor: '#212121',
  textSecondaryColor: '#757575',
  animationsEnabled: true,
  animationSpeed: 'normal',
  transitionDuration: 300,
  buttonStyle: 'raised',
  buttonBorderRadius: '8px',
  buttonPadding: '12px 24px',
  cardStyle: 'elevated',
  cardBorderRadius: '12px',
  cardElevation: 2,
  cardShadow: '0 2px 8px rgba(0,0,0,0.1)',
  borderRadius: '8px',
  spacing: '16px'
};

