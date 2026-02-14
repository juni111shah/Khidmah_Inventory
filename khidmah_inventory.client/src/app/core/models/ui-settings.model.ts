// UI/UX Settings Models

export interface UIComponentSettings {
  // Global Theme
  themeMode: 'light' | 'dark' | 'auto';
  primaryColor: string;
  secondaryColor: string;
  accentColor: string;
  successColor: string;
  warningColor: string;
  dangerColor: string;
  infoColor: string;

  // Typography
  fontFamily: string;
  fontSize: string;
  fontWeight: string;
  lineHeight: string;
  headingFontSize: string;

  // Spacing
  baseSpacing: string;
  borderRadius: string;
  borderWidth: string;

  // Input Fields
  inputHeight: string;
  inputPadding: string;
  inputBorderRadius: string;
  inputBorderColor: string;
  inputFocusBorderColor: string;
  inputErrorBorderColor: string;
  inputBackgroundColor: string;
  inputTextColor: string;
  inputPlaceholderColor: string;
  inputFontSize: string;

  // Forms
  formSpacing: string;
  formLabelColor: string;
  formLabelFontSize: string;
  formLabelFontWeight: string;
  formErrorColor: string;
  formHintColor: string;

  // Select Fields
  selectHeight: string;
  selectPadding: string;
  selectBorderRadius: string;
  selectBackgroundColor: string;
  selectTextColor: string;
  selectDropdownMaxHeight: string;
  selectOptionHoverColor: string;
  selectOptionSelectedColor: string;

  // Buttons
  buttonHeight: string;
  buttonPadding: string;
  buttonBorderRadius: string;
  buttonFontSize: string;
  buttonFontWeight: string;
  buttonPrimaryBg: string;
  buttonPrimaryText: string;
  buttonPrimaryHoverBg: string;
  buttonSecondaryBg: string;
  buttonSecondaryText: string;
  buttonSecondaryHoverBg: string;
  buttonOutlineBorderWidth: string;

  // Checkboxes & Radios
  checkboxSize: string;
  checkboxBorderRadius: string;
  checkboxCheckedColor: string;
  checkboxBorderColor: string;
  radioSize: string;
  radioCheckedColor: string;

  // Tables
  tableHeaderBg: string;
  tableHeaderText: string;
  tableHeaderFontWeight: string;
  tableRowBg: string;
  tableRowHoverBg: string;
  tableRowBorderColor: string;
  tableStripedBg: string;
  tableBorderRadius: string;
  tableCellPadding: string;

  // Loaders/Spinners
  loaderSize: string;
  loaderColor: string;
  loaderType: 'spinner' | 'dots' | 'bars' | 'pulse';
  loaderSpeed: string;

  // Cards
  cardPadding: string;
  cardBorderRadius: string;
  cardElevation: number;
  cardShadow: string;
  cardBackgroundColor: string;
  cardBorderColor: string;
  cardHeaderBg: string;
  cardHeaderText: string;

  // Menus
  menuBackgroundColor: string;
  menuTextColor: string;
  menuHoverColor: string;
  menuBorderRadius: string;
  menuShadow: string;
  menuPadding: string;
  menuItemHeight: string;

  // Dropdowns
  dropdownBackgroundColor: string;
  dropdownTextColor: string;
  dropdownBorderColor: string;
  dropdownBorderRadius: string;
  dropdownShadow: string;
  dropdownMaxHeight: string;
  dropdownItemHoverColor: string;
  dropdownItemPadding: string;

  // Icons
  iconSize: string;
  iconColor: string;
  iconLibrary: 'material' | 'fontawesome' | 'custom';

  // Tabs
  tabHeight: string;
  tabPadding: string;
  tabActiveColor: string;
  tabInactiveColor: string;
  tabActiveBg: string;
  tabInactiveBg: string;
  tabBorderRadius: string;
  tabIndicatorColor: string;

  // Layout
  containerMaxWidth: string;
  containerPadding: string;
  gridGap: string;
  sidebarWidth: string;
  sidebarCollapsedWidth: string;
  headerHeight: string;
  footerHeight: string;

  // Shadows
  shadowSm: string;
  shadowMd: string;
  shadowLg: string;
  shadowXl: string;

  // Transitions
  transitionDuration: string;
  transitionTiming: string;
}

export const DEFAULT_UI_COMPONENT_SETTINGS: UIComponentSettings = {
  themeMode: 'light',
  primaryColor: '#2196F3',
  secondaryColor: '#FF9800',
  accentColor: '#4CAF50',
  successColor: '#4CAF50',
  warningColor: '#FF9800',
  dangerColor: '#F44336',
  infoColor: '#2196F3',

  fontFamily: "'Bricolage Grotesque', sans-serif",
  fontSize: '14px',
  fontWeight: '400',
  lineHeight: '1.5',
  headingFontSize: '24px',

  baseSpacing: '16px',
  borderRadius: '8px',
  borderWidth: '1px',

  inputHeight: '40px',
  inputPadding: '12px',
  inputBorderRadius: '8px',
  inputBorderColor: '#E0E0E0',
  inputFocusBorderColor: '#2196F3',
  inputErrorBorderColor: '#F44336',
  inputBackgroundColor: '#FFFFFF',
  inputTextColor: '#212121',
  inputPlaceholderColor: '#9E9E9E',
  inputFontSize: '14px',

  formSpacing: '16px',
  formLabelColor: '#212121',
  formLabelFontSize: '14px',
  formLabelFontWeight: '500',
  formErrorColor: '#F44336',
  formHintColor: '#757575',

  selectHeight: '40px',
  selectPadding: '12px',
  selectBorderRadius: '8px',
  selectBackgroundColor: '#FFFFFF',
  selectTextColor: '#212121',
  selectDropdownMaxHeight: '300px',
  selectOptionHoverColor: '#F5F5F5',
  selectOptionSelectedColor: '#E3F2FD',

  buttonHeight: '40px',
  buttonPadding: '12px 24px',
  buttonBorderRadius: '8px',
  buttonFontSize: '14px',
  buttonFontWeight: '500',
  buttonPrimaryBg: '#2196F3',
  buttonPrimaryText: '#FFFFFF',
  buttonPrimaryHoverBg: '#1976D2',
  buttonSecondaryBg: '#FF9800',
  buttonSecondaryText: '#FFFFFF',
  buttonSecondaryHoverBg: '#F57C00',
  buttonOutlineBorderWidth: '2px',

  checkboxSize: '20px',
  checkboxBorderRadius: '4px',
  checkboxCheckedColor: '#2196F3',
  checkboxBorderColor: '#757575',
  radioSize: '20px',
  radioCheckedColor: '#2196F3',

  tableHeaderBg: '#F5F5F5',
  tableHeaderText: '#212121',
  tableHeaderFontWeight: '600',
  tableRowBg: '#FFFFFF',
  tableRowHoverBg: '#F5F5F5',
  tableRowBorderColor: '#E0E0E0',
  tableStripedBg: '#FAFAFA',
  tableBorderRadius: '8px',
  tableCellPadding: '12px',

  loaderSize: '40px',
  loaderColor: '#2196F3',
  loaderType: 'spinner',
  loaderSpeed: '1s',

  cardPadding: '24px',
  cardBorderRadius: '12px',
  cardElevation: 2,
  cardShadow: '0 2px 8px rgba(0,0,0,0.1)',
  cardBackgroundColor: '#FFFFFF',
  cardBorderColor: '#E0E0E0',
  cardHeaderBg: '#F5F5F5',
  cardHeaderText: '#212121',

  menuBackgroundColor: '#FFFFFF',
  menuTextColor: '#212121',
  menuHoverColor: '#F5F5F5',
  menuBorderRadius: '8px',
  menuShadow: '0 2px 8px rgba(0,0,0,0.1)',
  menuPadding: '8px',
  menuItemHeight: '40px',

  dropdownBackgroundColor: '#FFFFFF',
  dropdownTextColor: '#212121',
  dropdownBorderColor: '#E0E0E0',
  dropdownBorderRadius: '8px',
  dropdownShadow: '0 2px 8px rgba(0,0,0,0.1)',
  dropdownMaxHeight: '300px',
  dropdownItemHoverColor: '#F5F5F5',
  dropdownItemPadding: '12px',

  iconSize: '24px',
  iconColor: '#212121',
  iconLibrary: 'material',

  tabHeight: '48px',
  tabPadding: '12px 24px',
  tabActiveColor: '#2196F3',
  tabInactiveColor: '#757575',
  tabActiveBg: 'transparent',
  tabInactiveBg: 'transparent',
  tabBorderRadius: '8px',
  tabIndicatorColor: '#2196F3',

  containerMaxWidth: '1200px',
  containerPadding: '24px',
  gridGap: '16px',
  sidebarWidth: '250px',
  sidebarCollapsedWidth: '64px',
  headerHeight: '64px',
  footerHeight: '48px',

  shadowSm: '0 1px 2px rgba(0,0,0,0.05)',
  shadowMd: '0 2px 8px rgba(0,0,0,0.1)',
  shadowLg: '0 4px 16px rgba(0,0,0,0.15)',
  shadowXl: '0 8px 32px rgba(0,0,0,0.2)',

  transitionDuration: '300ms',
  transitionTiming: 'ease-in-out'
};

