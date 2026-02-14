/**
 * App-wide chart color palette. Use for all charts (bar, line, pie, doughnut)
 * so colors are consistent. Prefer theme-based colors via getChartColorsFromTheme().
 */
export const CHART_COLORS = [
  '#6366f1', // indigo (primary)
  '#22c55e', // green (success)
  '#f59e0b', // amber (warning)
  '#0ea5e9', // sky (info)
  '#ef4444', // red (error)
  '#8b5cf6', // violet
  '#ec4899', // pink
  '#14b8a6', // teal
] as const;

/** Single-series line/bar default (e.g. primary) */
export const CHART_PRIMARY = CHART_COLORS[0];
/** Secondary series (e.g. success) */
export const CHART_SECONDARY = CHART_COLORS[1];

/** Theme colors for charts (order matches semantic use: primary, success, warning, info, danger, secondary, accent). */
export interface ChartThemeColors {
  primaryColor: string;
  successColor: string;
  warningColor: string;
  infoColor: string;
  dangerColor: string;
  secondaryColor?: string;
  accentColor?: string;
}

/** Build chart palette from theme so chart colors follow app theme. */
export function getChartColorsFromTheme(theme: ChartThemeColors): string[] {
  const base = [
    theme.primaryColor,
    theme.successColor,
    theme.warningColor,
    theme.infoColor,
    theme.dangerColor,
    theme.secondaryColor ?? theme.primaryColor,
    theme.accentColor ?? theme.primaryColor,
  ].filter(Boolean);
  return base.length ? base : [...CHART_COLORS];
}

/** Get palette slice for N segments. Uses theme colors if theme provided, else static CHART_COLORS. */
export function getChartColors(count: number, theme?: ChartThemeColors): string[] {
  const palette = theme ? getChartColorsFromTheme(theme) : [...CHART_COLORS];
  const out: string[] = [];
  for (let i = 0; i < count; i++) {
    out.push(palette[i % palette.length]);
  }
  return out;
}
