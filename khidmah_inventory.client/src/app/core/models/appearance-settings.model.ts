// Comprehensive Appearance Settings Model
// Complete control over all app visual aspects

import { LayoutConfig, LayoutType } from './layout.model';

export interface AppearanceSettings {
  // ============ COLORS ============
  // Primary Colors
  primaryColor: string;
  secondaryColor: string;
  accentColor: string;

  // Status Colors
  successColor: string;
  warningColor: string;
  dangerColor: string;
  infoColor: string;

  // Background Colors
  backgroundColor: string;
  surfaceColor: string;

  // Text Colors
  textColor: string;
  textSecondaryColor: string;

  // Border Colors
  borderColor: string;

  // ============ THEME ============
  themeMode: 'light' | 'dark' | 'auto';

  // ============ LAYOUT ============
  selectedLayout: LayoutType;
  layoutConfig: LayoutConfig;

  // Spacing
  spacing: 'compact' | 'normal' | 'comfortable';
  spacingValue: string; // e.g., '16px'

  // Container
  containerMaxWidth: string;
  contentPadding: string;

  // ============ BRANDING ============
  logoUrl: string;
  logoHeight: string;
  companyName: string;

  // ============ RADIUS ============
  // Global Radius
  borderRadius: string;

  // Component-specific Radius
  buttonBorderRadius: string;
  cardBorderRadius: string;
  inputBorderRadius: string;
  modalBorderRadius: string;
  dropdownBorderRadius: string;
  badgeBorderRadius: string;

  // ============ BUTTONS ============
  buttonStyle: 'flat' | 'raised' | 'outlined' | 'gradient' | 'glass';
  buttonSize: 'small' | 'medium' | 'large';
  buttonPadding: string;
  buttonFontSize: string;
  buttonFontWeight: string;
  buttonTextTransform: 'none' | 'uppercase' | 'lowercase' | 'capitalize';
  buttonShadow: string;
  buttonHoverEffect: 'lift' | 'glow' | 'darken' | 'none';

  // ============ CARDS ============
  cardStyle: 'flat' | 'elevated' | 'outlined' | 'glass';
  cardElevation: number;
  cardPadding: string;
  cardShadow: string;
  cardHoverShadow: string;
  cardHoverEffect: 'lift' | 'glow' | 'scale' | 'none';

  // ============ FORM FIELDS ============
  inputStyle: 'outlined' | 'filled' | 'underlined';
  inputSize: 'small' | 'medium' | 'large';
  inputPadding: string;
  inputFontSize: string;
  inputBorderWidth: string;
  inputFocusEffect: 'border' | 'glow' | 'both';
  labelPosition: 'top' | 'left' | 'floating';

  // ============ TYPOGRAPHY ============
  fontFamily: string;
  fontSize: string;
  fontWeight: string;
  lineHeight: string;
  headingFontFamily: string;
  headingFontWeight: string;

  // ============ ANIMATIONS ============
  animationsEnabled: boolean;
  animationSpeed: 'slow' | 'normal' | 'fast';
  transitionDuration: number;
  animationEasing: string;
  hoverTransform: string;
  pageTransition: 'fade' | 'slide' | 'zoom' | 'none';

  // ============ SIDEBAR ============
  sidebarStyle: 'light' | 'dark' | 'colored';
  sidebarWidth: string;
  sidebarCollapsedWidth: string;
  sidebarItemStyle: 'rounded' | 'square' | 'pill';
  sidebarItemSpacing: string;

  // ============ HEADER ============
  headerStyle: 'light' | 'dark' | 'colored' | 'transparent';
  headerHeight: string;
  headerShadow: string;
  headerBorderBottom: string;

  // ============ TABLES ============
  tableStyle: 'default' | 'striped' | 'bordered' | 'borderless';
  tableHeaderStyle: 'light' | 'dark' | 'colored';
  tableRowHoverEffect: 'background' | 'border' | 'shadow' | 'none';
  tableBorderRadius: string;

  // ============ MODALS ============
  modalBackdrop: 'dark' | 'light' | 'blur';
  modalAnimation: 'fade' | 'slide' | 'zoom' | 'flip';
  modalShadow: string;

  // ============ DROPDOWNS ============
  dropdownStyle: 'default' | 'elevated' | 'bordered';
  dropdownShadow: string;
  dropdownAnimation: 'fade' | 'slide' | 'scale';

  // ============ BADGES ============
  badgeStyle: 'solid' | 'outlined' | 'soft';
  badgeSize: 'small' | 'medium' | 'large';

  // ============ CHARTS ============
  chartBorderRadius: string;
  chartAnimationSpeed: number;
  chartAnimationEasing: string;
  chartColorScheme: 'default' | 'vibrant' | 'pastel' | 'monochrome';

  // ============ SCROLLBAR ============
  customScrollbar: boolean;
  scrollbarWidth: string;
  scrollbarColor: string;

  // ============ EFFECTS ============
  glassEffect: boolean;
  glassBlur: string;
  glassOpacity: number;
  shadowIntensity: 'none' | 'light' | 'medium' | 'strong';
}

export const DEFAULT_APPEARANCE_SETTINGS: AppearanceSettings = {
  // Colors
  primaryColor: '#667eea',
  secondaryColor: '#764ba2',
  accentColor: '#9d7ae8',
  successColor: '#4CAF50',
  warningColor: '#FF9800',
  dangerColor: '#F44336',
  infoColor: '#2196F3',
  backgroundColor: '#F5F5F5',
  surfaceColor: '#FFFFFF',
  textColor: '#212121',
  textSecondaryColor: '#757575',
  borderColor: '#E0E0E0',

  // Theme
  themeMode: 'light',

  // Layout
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
  spacing: 'normal',
  spacingValue: '16px',
  containerMaxWidth: '1400px',
  contentPadding: '24px',

  // Branding
  logoUrl: '',
  logoHeight: '40px',
  companyName: 'Khidmah Inventory',

  // Radius
  borderRadius: '8px',
  buttonBorderRadius: '8px',
  cardBorderRadius: '16px',
  inputBorderRadius: '8px',
  modalBorderRadius: '16px',
  dropdownBorderRadius: '12px',
  badgeBorderRadius: '50px',

  // Buttons
  buttonStyle: 'raised',
  buttonSize: 'medium',
  buttonPadding: '12px 24px',
  buttonFontSize: '14px',
  buttonFontWeight: '600',
  buttonTextTransform: 'none',
  buttonShadow: '0 2px 4px rgba(0,0,0,0.1)',
  buttonHoverEffect: 'lift',

  // Cards
  cardStyle: 'elevated',
  cardElevation: 2,
  cardPadding: '24px',
  cardShadow: '0 2px 8px rgba(0,0,0,0.08)',
  cardHoverShadow: '0 8px 20px rgba(0,0,0,0.12)',
  cardHoverEffect: 'lift',

  // Form Fields
  inputStyle: 'outlined',
  inputSize: 'medium',
  inputPadding: '12px 16px',
  inputFontSize: '14px',
  inputBorderWidth: '1px',
  inputFocusEffect: 'both',
  labelPosition: 'top',

  // Typography
  fontFamily: "'Bricolage Grotesque', sans-serif",
  fontSize: '14px',
  fontWeight: '400',
  lineHeight: '1.5',
  headingFontFamily: "'Bricolage Grotesque', sans-serif",
  headingFontWeight: '700',

  // Animations
  animationsEnabled: true,
  animationSpeed: 'normal',
  transitionDuration: 300,
  animationEasing: 'cubic-bezier(0.4, 0, 0.2, 1)',
  hoverTransform: 'translateY(-3px)',
  pageTransition: 'fade',

  // Sidebar
  sidebarStyle: 'light',
  sidebarWidth: '280px',
  sidebarCollapsedWidth: '80px',
  sidebarItemStyle: 'pill',
  sidebarItemSpacing: '4px',

  // Header
  headerStyle: 'light',
  headerHeight: '72px',
  headerShadow: '0 2px 4px rgba(0,0,0,0.1)',
  headerBorderBottom: '1px solid #E0E0E0',

  // Tables
  tableStyle: 'default',
  tableHeaderStyle: 'light',
  tableRowHoverEffect: 'background',
  tableBorderRadius: '0px',

  // Modals
  modalBackdrop: 'dark',
  modalAnimation: 'fade',
  modalShadow: '0 20px 60px rgba(0,0,0,0.3)',

  // Dropdowns
  dropdownStyle: 'elevated',
  dropdownShadow: '0 4px 12px rgba(0,0,0,0.15)',
  dropdownAnimation: 'fade',

  // Badges
  badgeStyle: 'solid',
  badgeSize: 'medium',

  // Charts
  chartBorderRadius: '15px',
  chartAnimationSpeed: 800,
  chartAnimationEasing: 'easeinout',
  chartColorScheme: 'default',

  // Scrollbar
  customScrollbar: true,
  scrollbarWidth: '8px',
  scrollbarColor: '#CCCCCC',

  // Effects
  glassEffect: false,
  glassBlur: '10px',
  glassOpacity: 0.8,
  shadowIntensity: 'medium'
};
