import { Component, Input, Output, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ThemeService } from '../../../core/services/theme.service';
import { HeaderService } from '../../../core/services/header.service';
import { AppearanceSettingsService } from '../../../core/services/appearance-settings.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './app-header.component.html'
})
export class AppHeaderComponent implements OnInit, OnDestroy {
  @Input() title: string = 'Application';
  @Input() description: string = '';
  @Input() showMenuToggle: boolean = true;
  @Input() showUserMenu: boolean = true;
  @Input() sidebarCollapsed: boolean = false;
  @Output() menuToggle = new EventEmitter<void>();

  displayTitle: string = 'Application';
  displayDescription: string = '';
  isDarkMode: boolean = false;
  private headerSubscription?: Subscription;
  private appearanceSubscription?: Subscription;

  constructor(
    public themeService: ThemeService,
    private headerService: HeaderService,
    private appearanceSettings: AppearanceSettingsService
  ) {}

  ngOnInit(): void {
    // Subscribe to header service for dynamic updates
    this.headerSubscription = this.headerService.headerInfo$.subscribe(info => {
      this.displayTitle = info.title || this.title;
      this.displayDescription = info.description || this.description;
    });
    
    // Initialize with current header info
    const currentInfo = this.headerService.getHeaderInfo();
    if (currentInfo.title) {
      this.displayTitle = currentInfo.title;
      this.displayDescription = currentInfo.description || '';
    }

    // Track theme mode for mode button icon
    this.updateDarkModeState();
    this.appearanceSubscription = this.appearanceSettings.settings$.subscribe(() => {
      this.updateDarkModeState();
    });
  }

  ngOnDestroy(): void {
    if (this.headerSubscription) {
      this.headerSubscription.unsubscribe();
    }
    this.appearanceSubscription?.unsubscribe();
  }

  private updateDarkModeState(): void {
    const theme = document.documentElement.getAttribute('data-theme');
    this.isDarkMode = theme === 'dark';
  }

  onMenuToggle(): void {
    this.menuToggle.emit();
  }

  onThemeModeToggle(): void {
    const settings = this.appearanceSettings.getSettings();
    const next = settings.themeMode === 'dark' ? 'light' : 'dark';
    this.appearanceSettings.updateSettings({ themeMode: next }, true);
    this.updateDarkModeState();
  }
}

