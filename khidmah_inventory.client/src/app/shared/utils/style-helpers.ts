/**
 * Style Helper Utilities
 * Reusable functions for common style operations
 */

/**
 * Generate spacing utility classes
 */
export function getSpacingClass(size: 'xs' | 'sm' | 'md' | 'lg' | 'xl' | 'xxl'): string {
  const spacingMap = {
    xs: 'var(--spacing-xs)',
    sm: 'var(--spacing-sm)',
    md: 'var(--spacing-md)',
    lg: 'var(--spacing-lg)',
    xl: 'var(--spacing-xl)',
    xxl: 'var(--spacing-xxl)'
  };
  return spacingMap[size] || spacingMap.md;
}

/**
 * Generate color utility class
 */
export function getColorClass(color: 'primary' | 'secondary' | 'accent' | 'error' | 'success' | 'warning'): string {
  return `text-${color}`;
}

/**
 * Generate size utility class
 */
export function getSizeClass(size: 'small' | 'medium' | 'large'): string {
  const sizeMap = {
    small: 'sm',
    medium: 'md',
    large: 'lg'
  };
  return sizeMap[size] || sizeMap.medium;
}

/**
 * Combine multiple class names
 */
export function classNames(...classes: (string | boolean | undefined | null)[]): string {
  return classes
    .filter(Boolean)
    .join(' ');
}

/**
 * Generate responsive class
 */
export function getResponsiveClass(baseClass: string, breakpoint?: 'mobile' | 'tablet' | 'desktop'): string {
  if (!breakpoint) {
    return baseClass;
  }
  return `${baseClass}-${breakpoint}`;
}

/**
 * Get CSS variable value
 */
export function getCSSVar(name: string): string {
  if (typeof document !== 'undefined') {
    return getComputedStyle(document.documentElement)
      .getPropertyValue(name)
      .trim();
  }
  return '';
}

/**
 * Set CSS variable value
 */
export function setCSSVar(name: string, value: string): void {
  if (typeof document !== 'undefined') {
    document.documentElement.style.setProperty(name, value);
  }
}

