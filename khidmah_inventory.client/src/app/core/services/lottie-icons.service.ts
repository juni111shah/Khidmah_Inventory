import { Injectable } from '@angular/core';

/**
 * Maps sidebar/nav icon names to free Lottie animation URLs.
 * Use LottieFiles.com free animations: download as Lottie JSON and place under assets/lottie/
 * or use their public CDN URLs (free tier).
 * Default falls back to a bundled minimal animation.
 */
@Injectable({ providedIn: 'root' })
export class LottieIconsService {
  /** Base path for local Lottie files (in assets). */
  private readonly basePath = '/assets/lottie';

  /**
   * Icon key -> Lottie path (relative to basePath) or full URL.
   * Add more from https://lottiefiles.com/free-animations (download Lottie JSON, put in assets/lottie).
   */
  private readonly map: Record<string, string> = {
    speedometer2: 'default.json',
    sunrise: 'default.json',
    bullseye: 'default.json',
    'file-earmark-bar-graph': 'default.json',
    'graph-up-arrow': 'default.json',
    'currency-dollar': 'default.json',
    building: 'default.json',
    people: 'default.json',
    'exclamation-triangle': 'default.json',
    lightbulb: 'default.json',
    'gear-wide-connected': 'default.json',
    'shield-lock': 'default.json',
    'diagram-3': 'default.json',
    plug: 'default.json',
    folder: 'default.json',
    'box-seam': 'default.json',
    'house-door': 'default.json',
    boxes: 'default.json',
    layers: 'default.json',
    'arrow-left-right': 'default.json',
    archive: 'default.json',
    'upc-scan': 'default.json',
    'arrow-repeat': 'default.json',
    'cart-check': 'default.json',
    truck: 'default.json',
    'file-earmark-text': 'default.json',
    'cash-coin': 'default.json',
    'person-lines-fill': 'default.json',
    'bag-check': 'default.json',
    shop: 'default.json',
    gear: 'default.json',
    circle: 'default.json'
  };

  getLottiePath(iconName: string): string | null {
    const key = (iconName || 'circle').toLowerCase();
    const value = this.map[key] ?? this.map['circle'] ?? null;
    if (!value) return null;
    if (value.startsWith('http://') || value.startsWith('https://')) {
      return value;
    }
    return `${this.basePath}/${value}`;
  }

  hasLottie(iconName: string): boolean {
    return !!this.getLottiePath(iconName || 'circle');
  }
}
