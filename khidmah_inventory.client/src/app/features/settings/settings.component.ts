import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AppearanceSettingsService } from '../../core/services/appearance-settings.service';
import { AppearanceSettings, DEFAULT_APPEARANCE_SETTINGS } from '../../core/models/appearance-settings.model';
import { HeaderService } from '../../core/services/header.service';
import { ToastComponent } from '../../shared/components/toast/toast.component';
import { Subscription } from 'rxjs';
import { UnifiedButtonComponent } from '../../shared/components/unified-button/unified-button.component';
import { UnifiedCardComponent } from '../../shared/components/unified-card/unified-card.component';
import { UnifiedCheckboxComponent } from '../../shared/components/unified-checkbox/unified-checkbox.component';
import { FormFieldComponent } from '../../shared/components/form-field/form-field.component';

type SettingsTab = 'colors' | 'layout' | 'radius' | 'buttons' | 'cards' | 'fields' | 'typography' | 'animations' | 'components' | 'effects';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ToastComponent,
    UnifiedButtonComponent,
    UnifiedCardComponent,
    UnifiedCheckboxComponent,
    FormFieldComponent
  ],
  templateUrl: './settings.component.html'
})
export class SettingsComponent implements OnInit, OnDestroy {
  // Settings
  settings: AppearanceSettings = { ...DEFAULT_APPEARANCE_SETTINGS };

  // Active tab
  activeTab: SettingsTab = 'colors';

  // Toast
  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';

  // Saving state
  saving = false;

  private subscriptions = new Subscription();

  // ============ DROPDOWN OPTIONS ============

  // Theme & Colors
  themeModeOptions = [
    { value: 'light', label: 'Light' },
    { value: 'dark', label: 'Dark' },
    { value: 'auto', label: 'Auto' }
  ];

  // Layout & Spacing
  spacingOptions = [
    { value: 'compact', label: 'Compact' },
    { value: 'normal', label: 'Normal' },
    { value: 'comfortable', label: 'Comfortable' }
  ];

  sidebarStyleOptions = [
    { value: 'light', label: 'Light' },
    { value: 'dark', label: 'Dark' },
    { value: 'colored', label: 'Colored' }
  ];

  sidebarItemStyleOptions = [
    { value: 'rounded', label: 'Rounded' },
    { value: 'square', label: 'Square' },
    { value: 'pill', label: 'Pill' }
  ];

  headerStyleOptions = [
    { value: 'light', label: 'Light' },
    { value: 'dark', label: 'Dark' },
    { value: 'colored', label: 'Colored' },
    { value: 'transparent', label: 'Transparent' }
  ];

  // Buttons
  buttonStyleOptions = [
    { value: 'flat', label: 'Flat' },
    { value: 'raised', label: 'Raised' },
    { value: 'outlined', label: 'Outlined' },
    { value: 'gradient', label: 'Gradient' },
    { value: 'glass', label: 'Glass' }
  ];

  buttonSizeOptions = [
    { value: 'small', label: 'Small' },
    { value: 'medium', label: 'Medium' },
    { value: 'large', label: 'Large' }
  ];

  buttonHoverEffectOptions = [
    { value: 'lift', label: 'Lift' },
    { value: 'glow', label: 'Glow' },
    { value: 'darken', label: 'Darken' },
    { value: 'none', label: 'None' }
  ];

  textTransformOptions = [
    { value: 'none', label: 'None' },
    { value: 'uppercase', label: 'Uppercase' },
    { value: 'lowercase', label: 'Lowercase' },
    { value: 'capitalize', label: 'Capitalize' }
  ];

  // Cards
  cardStyleOptions = [
    { value: 'flat', label: 'Flat' },
    { value: 'elevated', label: 'Elevated' },
    { value: 'outlined', label: 'Outlined' },
    { value: 'glass', label: 'Glass' }
  ];

  cardHoverEffectOptions = [
    { value: 'lift', label: 'Lift' },
    { value: 'glow', label: 'Glow' },
    { value: 'scale', label: 'Scale' },
    { value: 'none', label: 'None' }
  ];

  // Form Fields
  inputStyleOptions = [
    { value: 'outlined', label: 'Outlined' },
    { value: 'filled', label: 'Filled' },
    { value: 'underlined', label: 'Underlined' }
  ];

  inputSizeOptions = [
    { value: 'small', label: 'Small' },
    { value: 'medium', label: 'Medium' },
    { value: 'large', label: 'Large' }
  ];

  inputFocusEffectOptions = [
    { value: 'border', label: 'Border Only' },
    { value: 'glow', label: 'Glow Only' },
    { value: 'both', label: 'Border & Glow' }
  ];

  labelPositionOptions = [
    { value: 'top', label: 'Top' },
    { value: 'left', label: 'Left' },
    { value: 'floating', label: 'Floating' }
  ];

  // Animations
  animationSpeedOptions = [
    { value: 'slow', label: 'Slow' },
    { value: 'normal', label: 'Normal' },
    { value: 'fast', label: 'Fast' }
  ];

  pageTransitionOptions = [
    { value: 'fade', label: 'Fade' },
    { value: 'slide', label: 'Slide' },
    { value: 'zoom', label: 'Zoom' },
    { value: 'none', label: 'None' }
  ];

  // Tables
  tableStyleOptions = [
    { value: 'default', label: 'Default' },
    { value: 'striped', label: 'Striped' },
    { value: 'bordered', label: 'Bordered' },
    { value: 'borderless', label: 'Borderless' }
  ];

  tableHeaderStyleOptions = [
    { value: 'light', label: 'Light' },
    { value: 'dark', label: 'Dark' },
    { value: 'colored', label: 'Colored' }
  ];

  tableRowHoverEffectOptions = [
    { value: 'background', label: 'Background' },
    { value: 'border', label: 'Border' },
    { value: 'shadow', label: 'Shadow' },
    { value: 'none', label: 'None' }
  ];

  // Modals
  modalBackdropOptions = [
    { value: 'dark', label: 'Dark' },
    { value: 'light', label: 'Light' },
    { value: 'blur', label: 'Blur' }
  ];

  modalAnimationOptions = [
    { value: 'fade', label: 'Fade' },
    { value: 'slide', label: 'Slide' },
    { value: 'zoom', label: 'Zoom' },
    { value: 'flip', label: 'Flip' }
  ];

  // Dropdowns
  dropdownStyleOptions = [
    { value: 'default', label: 'Default' },
    { value: 'elevated', label: 'Elevated' },
    { value: 'bordered', label: 'Bordered' }
  ];

  dropdownAnimationOptions = [
    { value: 'fade', label: 'Fade' },
    { value: 'slide', label: 'Slide' },
    { value: 'scale', label: 'Scale' }
  ];

  // Badges
  badgeStyleOptions = [
    { value: 'solid', label: 'Solid' },
    { value: 'outlined', label: 'Outlined' },
    { value: 'soft', label: 'Soft' }
  ];

  badgeSizeOptions = [
    { value: 'small', label: 'Small' },
    { value: 'medium', label: 'Medium' },
    { value: 'large', label: 'Large' }
  ];

  // Charts
  chartColorSchemeOptions = [
    { value: 'default', label: 'Default' },
    { value: 'vibrant', label: 'Vibrant' },
    { value: 'pastel', label: 'Pastel' },
    { value: 'monochrome', label: 'Monochrome' }
  ];

  // Effects
  shadowIntensityOptions = [
    { value: 'none', label: 'None' },
    { value: 'light', label: 'Light' },
    { value: 'medium', label: 'Medium' },
    { value: 'strong', label: 'Strong' }
  ];

  constructor(
    private appearanceService: AppearanceSettingsService,
    private headerService: HeaderService
  ) {}

  ngOnInit(): void {
    this.headerService.setHeaderInfo({
      title: 'App Settings',
      description: 'Customize every aspect of your application appearance'
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

  // Tab Navigation
  setActiveTab(tab: SettingsTab): void {
    this.activeTab = tab;
  }

  // Update Setting (Real-time)
  updateSetting(key: keyof AppearanceSettings, value: any): void {
    (this.settings as any)[key] = value;
    this.applySettings();
  }

  // Apply Settings (Real-time)
  applySettings(): void {
    this.appearanceService.updateSettings(this.settings, false); // Don't save to API yet
  }

  // Save Settings (Explicit save)
  saveSettings(): void {
    this.saving = true;
    this.appearanceService.updateSettings(this.settings, true); // Save to API

    // Simulate API save
    setTimeout(() => {
      this.saving = false;
      this.showToastNotification('Settings saved successfully!', 'success');
    }, 500);
  }

  // Reset Settings
  resetSettings(): void {
    if (confirm('Are you sure you want to reset all settings to default? This action cannot be undone.')) {
      this.appearanceService.resetSettings();
      this.settings = this.appearanceService.getSettings();
      this.showToastNotification('Settings reset to default', 'info');
    }
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
}
