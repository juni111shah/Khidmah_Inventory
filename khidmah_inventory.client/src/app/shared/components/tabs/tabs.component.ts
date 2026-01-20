import { Component, Input, Output, EventEmitter, ContentChildren, QueryList, AfterContentInit } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-tab',
  template: '<div *ngIf="isActive"><ng-content></ng-content></div>',
  standalone: true,
  imports: [CommonModule]
})
export class TabComponent {
  @Input() label: string = '';
  @Input() icon: string = '';
  @Input() disabled: boolean = false;
  isActive: boolean = false;
}

@Component({
  selector: 'app-tabs',
  standalone: true,
  imports: [CommonModule, TabComponent],
  templateUrl: './tabs.component.html'
})
export class TabsComponent implements AfterContentInit {
  @Input() activeTab: number = 0;
  @Input() tabStyle: 'flat' | 'pills' | 'underline' | null = null;
  @Output() activeTabChange = new EventEmitter<number>();
  @Output() tabChange = new EventEmitter<number>();

  @ContentChildren(TabComponent) tabs!: QueryList<TabComponent>;

  ngAfterContentInit(): void {
    this.updateActiveTab();
    // Get tab style from CSS variable if not provided
    if (this.tabStyle === null && typeof window !== 'undefined') {
      const style = getComputedStyle(document.documentElement).getPropertyValue('--tab-style').trim();
      if (style && (style === 'flat' || style === 'pills' || style === 'underline')) {
        this.tabStyle = style as 'flat' | 'pills' | 'underline';
      } else {
        this.tabStyle = 'underline';
      }
    }
  }

  selectTab(index: number): void {
    if (this.tabs && index >= 0 && index < this.tabs.length) {
      const tab = this.tabs.toArray()[index];
      if (!tab.disabled) {
        this.activeTab = index;
        this.updateActiveTab();
        this.activeTabChange.emit(index);
        this.tabChange.emit(index);
      }
    }
  }

  private updateActiveTab(): void {
    if (this.tabs) {
      this.tabs.forEach((tab, index) => {
        tab.isActive = index === this.activeTab;
      });
    }
  }
}

