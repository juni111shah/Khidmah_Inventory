export type LayoutType = 'modern';

export interface LayoutConfig {
  type: LayoutType;
  name: string;
  description: string;
  icon: string;
  sidebarPosition: 'left' | 'right' | 'top' | 'none';
  sidebarStyle: 'fixed' | 'overlay' | 'collapsible' | 'none';
  headerStyle: 'fixed' | 'static' | 'sticky' | 'none';
  footerStyle: 'fixed' | 'static' | 'sticky' | 'none';
  contentStyle: 'full-width' | 'boxed' | 'centered';
  sidebarWidth: string;
  sidebarCollapsedWidth: string;
  headerHeight: string;
  footerHeight: string;
  containerMaxWidth?: string;
  showBreadcrumbs: boolean;
  showPageTitle: boolean;
}

export const LAYOUT_PRESETS: Record<LayoutType, LayoutConfig> = {
  modern: {
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
  }
};

export const DEFAULT_LAYOUT: LayoutConfig = LAYOUT_PRESETS.modern;

