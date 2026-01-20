import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ComponentSettingsService } from '../../services/component-settings.service';
import { GlobalComponentSettings } from '../../models/component-settings.model';
import { UnifiedButtonComponent } from '../unified-button/unified-button.component';
import { UnifiedCardComponent } from '../unified-card/unified-card.component';
import { UnifiedInputComponent } from '../unified-input/unified-input.component';
import { UnifiedSelectComponent } from '../unified-select/unified-select.component';
import { UnifiedTextareaComponent } from '../unified-textarea/unified-textarea.component';

@Component({
  selector: 'app-settings-panel',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    UnifiedButtonComponent,
    UnifiedCardComponent,
    UnifiedInputComponent,
    UnifiedSelectComponent,
    UnifiedTextareaComponent
  ],
  templateUrl: './settings-panel.component.html'
})
export class SettingsPanelComponent implements OnInit {
  settings: GlobalComponentSettings = {} as GlobalComponentSettings;
  selectedComponentType: string = 'buttons';
  selectedComponentId: string = '';
  exportJson: string = '';
  importJson: string = '';
  activeTab: 'component' | 'import-export' | 'reset' = 'component';

  componentTypes = [
    { value: 'buttons', label: 'Buttons' },
    { value: 'inputs', label: 'Inputs' },
    { value: 'cards', label: 'Cards' },
    { value: 'tables', label: 'Tables' },
    { value: 'modals', label: 'Modals' },
    { value: 'drawers', label: 'Drawers' },
    { value: 'lists', label: 'Lists' },
    { value: 'menus', label: 'Menus' },
    { value: 'selects', label: 'Selects' },
    { value: 'textareas', label: 'Textareas' },
    { value: 'checkboxes', label: 'Checkboxes' },
    { value: 'radios', label: 'Radios' },
    { value: 'switches', label: 'Switches' },
    { value: 'datePickers', label: 'Date Pickers' },
    { value: 'fileUploads', label: 'File Uploads' },
    { value: 'badges', label: 'Badges' },
    { value: 'alerts', label: 'Alerts' },
    { value: 'progress', label: 'Progress' },
    { value: 'tabs', label: 'Tabs' },
    { value: 'accordions', label: 'Accordions' },
    { value: 'tooltips', label: 'Tooltips' },
    { value: 'popovers', label: 'Popovers' },
    { value: 'dropdowns', label: 'Dropdowns' },
    { value: 'pagination', label: 'Pagination' },
    { value: 'breadcrumbs', label: 'Breadcrumbs' },
    { value: 'steppers', label: 'Steppers' },
    { value: 'chips', label: 'Chips' },
    { value: 'avatars', label: 'Avatars' },
    { value: 'skeletons', label: 'Skeletons' },
    { value: 'dividers', label: 'Dividers' },
    { value: 'spacers', label: 'Spacers' },
    { value: 'grids', label: 'Grids' },
    { value: 'containers', label: 'Containers' }
  ];

  constructor(private settingsService: ComponentSettingsService) {}

  ngOnInit(): void {
    this.settingsService.settings$.subscribe(settings => {
      this.settings = settings;
    });
  }

  getComponentIds(type: string): { value: string; label: string }[] {
    const components = (this.settings as any)[type];
    return components ? Object.keys(components).map(id => ({ value: id, label: id })) : [];
  }

  getComponentSettings(type: string, id: string): any {
    const components = (this.settings as any)[type];
    return components ? components[id] : null;
  }

  saveComponentSettings(): void {
    if (this.selectedComponentType && this.selectedComponentId) {
      // Settings are saved automatically when using setComponentSettings
    }
  }

  exportSettings(): void {
    this.exportJson = this.settingsService.exportSettings();
  }

  importSettings(): void {
    if (this.importJson) {
      const success = this.settingsService.importSettings(this.importJson);
      if (success) {
        alert('Settings imported successfully!');
        this.importJson = '';
      } else {
        alert('Failed to import settings. Please check the JSON format.');
      }
    }
  }

  resetAllSettings(): void {
    if (confirm('Are you sure you want to reset all settings to default?')) {
      this.settingsService.resetAllSettings();
    }
  }

  downloadSettings(): void {
    this.exportSettings();
    const blob = new Blob([this.exportJson], { type: 'application/json' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'component-settings.json';
    link.click();
    window.URL.revokeObjectURL(url);
  }

  setActiveTab(tab: 'component' | 'import-export' | 'reset'): void {
    this.activeTab = tab;
  }
}

