// Simplified Appearance Settings Model
// Single perfect template with clean, modern design

import { LayoutConfig, LayoutType } from './layout.model';

export interface AppearanceSettings {
  // Colors
  primaryColor: string;
  secondaryColor: string;
  accentColor: string;
  successColor: string;
  warningColor: string;
  dangerColor: string;
  infoColor: string;
  
  // Layout
  selectedLayout: LayoutType;
  layoutConfig: LayoutConfig;
  
  // Branding
  logoUrl: string;
  logoHeight: string;
  
  // Component Styles
  borderRadius: string;
  buttonStyle: 'flat' | 'raised' | 'outlined' | 'gradient' | 'glass';
  cardStyle: 'flat' | 'elevated' | 'outlined' | 'glass';
  cardElevation: number;
  
  // Animations
  animationsEnabled: boolean;
  animationSpeed: 'slow' | 'normal' | 'fast';
}

export const DEFAULT_APPEARANCE_SETTINGS: AppearanceSettings = {
  primaryColor: '#667eea',
  secondaryColor: '#764ba2',
  accentColor: '#9d7ae8',
  successColor: '#4CAF50',
  warningColor: '#FF9800',
  dangerColor: '#F44336',
  infoColor: '#2196F3',
  selectedLayout: 'modern',
  layoutConfig: {
    type: 'modern',
    name: 'Modern',
    description: 'Clean modern layout with integrated header and sidebar',
    icon: 'auto_awesome',
    sidebarPosition: 'left',
    sidebarStyle: 'fixed',
    headerStyle: 'sticky',
    footerStyle: 'static',
    contentStyle: 'full-width',
    sidebarWidth: '280px',
    sidebarCollapsedWidth: '80px',
    headerHeight: '72px',
    footerHeight: '56px',
    showBreadcrumbs: true,
    showPageTitle: true
  },
  logoUrl: '',
  logoHeight: '40px',
  borderRadius: '8px',
  buttonStyle: 'raised',
  cardStyle: 'elevated',
  cardElevation: 2,
  animationsEnabled: true,
  animationSpeed: 'normal'
};
