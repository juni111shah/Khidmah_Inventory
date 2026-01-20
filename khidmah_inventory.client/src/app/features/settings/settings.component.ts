import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AppearanceSettingsService } from '../../core/services/appearance-settings.service';
import { AppearanceSettings, DEFAULT_APPEARANCE_SETTINGS } from '../../core/models/appearance-settings.model';
import { HeaderService } from '../../core/services/header.service';
import { IconComponent } from '../../shared/components/icon/icon.component';
import { ToastComponent } from '../../shared/components/toast/toast.component';
import { Subscription } from 'rxjs';
import { UnifiedButtonComponent } from '../../shared/components/unified-button/unified-button.component';
import { UnifiedCardComponent } from '../../shared/components/unified-card/unified-card.component';
import { UnifiedCheckboxComponent } from '../../shared/components/unified-checkbox/unified-checkbox.component';
import { UnifiedSelectComponent } from '../../shared/components/unified-select/unified-select.component';
import { FormFieldComponent } from '../../shared/components/form-field/form-field.component';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    IconComponent,
    ToastComponent,
    UnifiedButtonComponent,
    UnifiedCardComponent,
    UnifiedCheckboxComponent,
    UnifiedSelectComponent,
    FormFieldComponent
  ],
  templateUrl: './settings.component.html'
})
export class SettingsComponent implements OnInit, OnDestroy {
  // Settings
  settings: AppearanceSettings = { ...DEFAULT_APPEARANCE_SETTINGS };

  // Active section
  activeSection: 'colors' | 'components' = 'colors';
  
  // Toast
  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';

  // Saving state
  saving = false;
  
  private subscriptions = new Subscription();

  constructor(
    private appearanceService: AppearanceSettingsService,
    private headerService: HeaderService
  ) {
  }

  ngOnInit(): void {
    this.headerService.setHeaderInfo({
      title: 'Appearance Settings',
      description: 'Customize the look and feel of your application'
    });

    // Load current settings
    this.settings = this.appearanceService.getSettings();

    // Subscribe to settings changes
    this.subscriptions.add(
      this.appearanceService.settings$.subscribe(settings => {
        this.settings = settings;
      })
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
    this.headerService.reset();
  }

  // Color Methods
  updateColor(colorKey: keyof AppearanceSettings, value: string): void {
    (this.settings as any)[colorKey] = value;
    this.applySettings();
  }

  // Component Style Methods
  updateComponentStyle(property: keyof AppearanceSettings, value: any): void {
    (this.settings as any)[property] = value;
    this.applySettings();
  }

  // Apply Settings (Real-time)
  applySettings(): void {
    this.appearanceService.updateSettings(this.settings, true);
  }

  // Save Settings (Explicit save)
  saveSettings(): void {
    this.saving = true;
    this.appearanceService.updateSettings(this.settings, true);
    
    // Simulate API save (will be replaced with actual API call)
    setTimeout(() => {
      this.saving = false;
      this.showToastNotification('Settings saved successfully!', 'success');
    }, 500);
  }

  // Reset Settings
  resetSettings(): void {
    if (confirm('Are you sure you want to reset all appearance settings to default?')) {
      this.appearanceService.resetSettings();
      this.settings = this.appearanceService.getSettings();
      this.showToastNotification('Settings reset to default', 'info');
    }
  }

  // Navigation
  setActiveSection(section: 'colors' | 'components'): void {
    this.activeSection = section;
  }

  // Toast
  showToastNotification(message: string, type: 'success' | 'error' | 'warning' | 'info' = 'success'): void {
    this.toastMessage = message;
    this.toastType = type;
    this.showToast = true;
    setTimeout(() => {
      this.showToast = false;
    }, 3000);
  }

  buttonStyleOptions = [
    { value: 'flat', label: 'Flat' },
    { value: 'raised', label: 'Raised' },
    { value: 'outlined', label: 'Outlined' },
    { value: 'gradient', label: 'Gradient' }
  ];

  cardStyleOptions = [
    { value: 'flat', label: 'Flat' },
    { value: 'elevated', label: 'Elevated' },
    { value: 'outlined', label: 'Outlined' }
  ];

  animationSpeedOptions = [
    { value: 'slow', label: 'Slow' },
    { value: 'normal', label: 'Normal' },
    { value: 'fast', label: 'Fast' }
  ];
}
