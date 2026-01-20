import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { DOCUMENT } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class StylesService {
  private isBrowser: boolean;

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    @Inject(DOCUMENT) private document: Document
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
  }

  /**
   * Add a CSS class to an element
   */
  addClass(element: HTMLElement, className: string): void {
    if (this.isBrowser && element) {
      element.classList.add(className);
    }
  }

  /**
   * Remove a CSS class from an element
   */
  removeClass(element: HTMLElement, className: string): void {
    if (this.isBrowser && element) {
      element.classList.remove(className);
    }
  }

  /**
   * Toggle a CSS class on an element
   */
  toggleClass(element: HTMLElement, className: string): void {
    if (this.isBrowser && element) {
      element.classList.toggle(className);
    }
  }

  /**
   * Set a CSS custom property value
   */
  setCSSVariable(name: string, value: string): void {
    if (this.isBrowser) {
      this.document.documentElement.style.setProperty(name, value);
    }
  }

  /**
   * Get a CSS custom property value
   */
  getCSSVariable(name: string): string {
    if (this.isBrowser) {
      return getComputedStyle(this.document.documentElement)
        .getPropertyValue(name)
        .trim();
    }
    return '';
  }

  /**
   * Set multiple CSS custom properties
   */
  setCSSVariables(variables: Record<string, string>): void {
    if (this.isBrowser) {
      Object.entries(variables).forEach(([name, value]) => {
        this.setCSSVariable(name, value);
      });
    }
  }

  /**
   * Get computed style for an element
   */
  getComputedStyle(element: HTMLElement, property?: string): string | CSSStyleDeclaration {
    if (this.isBrowser && element) {
      const styles = window.getComputedStyle(element);
      return property ? styles.getPropertyValue(property).trim() : styles;
    }
    return '';
  }

  /**
   * Check if element has a class
   */
  hasClass(element: HTMLElement, className: string): boolean {
    if (this.isBrowser && element) {
      return element.classList.contains(className);
    }
    return false;
  }

  /**
   * Add styles to head dynamically
   */
  addStyleSheet(css: string, id?: string): void {
    if (this.isBrowser) {
      const style = this.document.createElement('style');
      if (id) {
        style.id = id;
      }
      style.textContent = css;
      this.document.head.appendChild(style);
    }
  }

  /**
   * Remove dynamically added stylesheet
   */
  removeStyleSheet(id: string): void {
    if (this.isBrowser) {
      const style = this.document.getElementById(id);
      if (style) {
        style.remove();
      }
    }
  }
}

