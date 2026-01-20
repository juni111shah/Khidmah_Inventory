import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import {
  GlobalComponentSettings,
  ButtonSettings,
  InputSettings,
  CardSettings,
  TableSettings,
  ModalSettings,
  DrawerSettings,
  ListSettings,
  MenuSettings,
  SelectSettings,
  TextareaSettings,
  CheckboxSettings,
  RadioSettings,
  SwitchSettings,
  DatePickerSettings,
  FileUploadSettings,
  BadgeSettings,
  AlertSettings,
  ProgressSettings,
  TabsSettings,
  AccordionSettings,
  TooltipSettings,
  PopoverSettings,
  DropdownSettings,
  PaginationSettings,
  BreadcrumbSettings,
  StepperSettings,
  ChipSettings,
  AvatarSettings,
  SkeletonSettings,
  DividerSettings,
  SpacerSettings,
  GridSettings,
  ContainerSettings
} from '../models/component-settings.model';

@Injectable({
  providedIn: 'root'
})
export class ComponentSettingsService {
  private readonly STORAGE_KEY = 'component-settings';
  
  private settingsSubject = new BehaviorSubject<GlobalComponentSettings>(this.getDefaultSettings());
  public settings$: Observable<GlobalComponentSettings> = this.settingsSubject.asObservable();

  constructor() {
    this.loadSettings();
  }

  /**
   * Get default settings for all components
   */
  private getDefaultSettings(): GlobalComponentSettings {
    return {
      buttons: {},
      inputs: {},
      cards: {},
      tables: {},
      modals: {},
      drawers: {},
      lists: {},
      menus: {},
      selects: {},
      textareas: {},
      checkboxes: {},
      radios: {},
      switches: {},
      datePickers: {},
      fileUploads: {},
      badges: {},
      alerts: {},
      progress: {},
      tabs: {},
      accordions: {},
      tooltips: {},
      popovers: {},
      dropdowns: {},
      pagination: {},
      breadcrumbs: {},
      steppers: {},
      chips: {},
      avatars: {},
      skeletons: {},
      dividers: {},
      spacers: {},
      grids: {},
      containers: {}
    };
  }

  /**
   * Get all settings
   */
  getSettings(): GlobalComponentSettings {
    return this.settingsSubject.value;
  }

  /**
   * Get settings for a specific component type and ID
   */
  getComponentSettings<T>(type: keyof GlobalComponentSettings, id: string): T | null {
    const settings = this.settingsSubject.value[type];
    return (settings as any)[id] || null;
  }

  /**
   * Set settings for a specific component
   */
  setComponentSettings<T>(
    type: keyof GlobalComponentSettings,
    id: string,
    settings: Partial<T>
  ): void {
    const currentSettings = { ...this.settingsSubject.value };
    const componentSettings = currentSettings[type] as any;
    
    componentSettings[id] = {
      ...componentSettings[id],
      ...settings,
      id
    };

    this.settingsSubject.next(currentSettings);
    this.saveSettings();
  }

  /**
   * Update multiple component settings at once
   */
  updateSettings(updates: Partial<GlobalComponentSettings>): void {
    const currentSettings = { ...this.settingsSubject.value };
    const newSettings = { ...currentSettings, ...updates };
    
    this.settingsSubject.next(newSettings);
    this.saveSettings();
  }

  /**
   * Reset settings for a specific component
   */
  resetComponentSettings(type: keyof GlobalComponentSettings, id: string): void {
    const currentSettings = { ...this.settingsSubject.value };
    const componentSettings = currentSettings[type] as any;
    delete componentSettings[id];

    this.settingsSubject.next(currentSettings);
    this.saveSettings();
  }

  /**
   * Reset all settings to default
   */
  resetAllSettings(): void {
    this.settingsSubject.next(this.getDefaultSettings());
    this.saveSettings();
  }

  /**
   * Export settings as JSON
   */
  exportSettings(): string {
    return JSON.stringify(this.settingsSubject.value, null, 2);
  }

  /**
   * Import settings from JSON
   */
  importSettings(json: string): boolean {
    try {
      const settings = JSON.parse(json);
      this.settingsSubject.next(settings);
      this.saveSettings();
      return true;
    } catch (error) {
      console.error('Failed to import settings:', error);
      return false;
    }
  }

  /**
   * Save settings to localStorage
   */
  private saveSettings(): void {
    try {
      const settings = this.settingsSubject.value;
      localStorage.setItem(this.STORAGE_KEY, JSON.stringify(settings));
    } catch (error) {
      console.error('Failed to save settings:', error);
    }
  }

  /**
   * Load settings from localStorage
   */
  private loadSettings(): void {
    try {
      const stored = localStorage.getItem(this.STORAGE_KEY);
      if (stored) {
        const settings = JSON.parse(stored);
        this.settingsSubject.next(settings);
      }
    } catch (error) {
      console.error('Failed to load settings:', error);
      this.settingsSubject.next(this.getDefaultSettings());
    }
  }

  // Convenience methods for each component type
  getButtonSettings(id: string): ButtonSettings | null {
    return this.getComponentSettings<ButtonSettings>('buttons', id);
  }

  setButtonSettings(id: string, settings: Partial<ButtonSettings>): void {
    this.setComponentSettings<ButtonSettings>('buttons', id, settings);
  }

  getInputSettings(id: string): InputSettings | null {
    return this.getComponentSettings<InputSettings>('inputs', id);
  }

  setInputSettings(id: string, settings: Partial<InputSettings>): void {
    this.setComponentSettings<InputSettings>('inputs', id, settings);
  }

  getCardSettings(id: string): CardSettings | null {
    return this.getComponentSettings<CardSettings>('cards', id);
  }

  setCardSettings(id: string, settings: Partial<CardSettings>): void {
    this.setComponentSettings<CardSettings>('cards', id, settings);
  }

  getTableSettings(id: string): TableSettings | null {
    return this.getComponentSettings<TableSettings>('tables', id);
  }

  setTableSettings(id: string, settings: Partial<TableSettings>): void {
    this.setComponentSettings<TableSettings>('tables', id, settings);
  }

  getModalSettings(id: string): ModalSettings | null {
    return this.getComponentSettings<ModalSettings>('modals', id);
  }

  setModalSettings(id: string, settings: Partial<ModalSettings>): void {
    this.setComponentSettings<ModalSettings>('modals', id, settings);
  }

  getDrawerSettings(id: string): DrawerSettings | null {
    return this.getComponentSettings<DrawerSettings>('drawers', id);
  }

  setDrawerSettings(id: string, settings: Partial<DrawerSettings>): void {
    this.setComponentSettings<DrawerSettings>('drawers', id, settings);
  }

  getListSettings(id: string): ListSettings | null {
    return this.getComponentSettings<ListSettings>('lists', id);
  }

  setListSettings(id: string, settings: Partial<ListSettings>): void {
    this.setComponentSettings<ListSettings>('lists', id, settings);
  }

  getMenuSettings(id: string): MenuSettings | null {
    return this.getComponentSettings<MenuSettings>('menus', id);
  }

  setMenuSettings(id: string, settings: Partial<MenuSettings>): void {
    this.setComponentSettings<MenuSettings>('menus', id, settings);
  }

  getSelectSettings(id: string): SelectSettings | null {
    return this.getComponentSettings<SelectSettings>('selects', id);
  }

  setSelectSettings(id: string, settings: Partial<SelectSettings>): void {
    this.setComponentSettings<SelectSettings>('selects', id, settings);
  }

  getTextareaSettings(id: string): TextareaSettings | null {
    return this.getComponentSettings<TextareaSettings>('textareas', id);
  }

  setTextareaSettings(id: string, settings: Partial<TextareaSettings>): void {
    this.setComponentSettings<TextareaSettings>('textareas', id, settings);
  }

  getCheckboxSettings(id: string): CheckboxSettings | null {
    return this.getComponentSettings<CheckboxSettings>('checkboxes', id);
  }

  setCheckboxSettings(id: string, settings: Partial<CheckboxSettings>): void {
    this.setComponentSettings<CheckboxSettings>('checkboxes', id, settings);
  }

  getRadioSettings(id: string): RadioSettings | null {
    return this.getComponentSettings<RadioSettings>('radios', id);
  }

  setRadioSettings(id: string, settings: Partial<RadioSettings>): void {
    this.setComponentSettings<RadioSettings>('radios', id, settings);
  }

  getSwitchSettings(id: string): SwitchSettings | null {
    return this.getComponentSettings<SwitchSettings>('switches', id);
  }

  setSwitchSettings(id: string, settings: Partial<SwitchSettings>): void {
    this.setComponentSettings<SwitchSettings>('switches', id, settings);
  }

  getDatePickerSettings(id: string): DatePickerSettings | null {
    return this.getComponentSettings<DatePickerSettings>('datePickers', id);
  }

  setDatePickerSettings(id: string, settings: Partial<DatePickerSettings>): void {
    this.setComponentSettings<DatePickerSettings>('datePickers', id, settings);
  }

  getFileUploadSettings(id: string): FileUploadSettings | null {
    return this.getComponentSettings<FileUploadSettings>('fileUploads', id);
  }

  setFileUploadSettings(id: string, settings: Partial<FileUploadSettings>): void {
    this.setComponentSettings<FileUploadSettings>('fileUploads', id, settings);
  }

  getBadgeSettings(id: string): BadgeSettings | null {
    return this.getComponentSettings<BadgeSettings>('badges', id);
  }

  setBadgeSettings(id: string, settings: Partial<BadgeSettings>): void {
    this.setComponentSettings<BadgeSettings>('badges', id, settings);
  }

  getAlertSettings(id: string): AlertSettings | null {
    return this.getComponentSettings<AlertSettings>('alerts', id);
  }

  setAlertSettings(id: string, settings: Partial<AlertSettings>): void {
    this.setComponentSettings<AlertSettings>('alerts', id, settings);
  }

  getProgressSettings(id: string): ProgressSettings | null {
    return this.getComponentSettings<ProgressSettings>('progress', id);
  }

  setProgressSettings(id: string, settings: Partial<ProgressSettings>): void {
    this.setComponentSettings<ProgressSettings>('progress', id, settings);
  }

  getTabsSettings(id: string): TabsSettings | null {
    return this.getComponentSettings<TabsSettings>('tabs', id);
  }

  setTabsSettings(id: string, settings: Partial<TabsSettings>): void {
    this.setComponentSettings<TabsSettings>('tabs', id, settings);
  }

  getAccordionSettings(id: string): AccordionSettings | null {
    return this.getComponentSettings<AccordionSettings>('accordions', id);
  }

  setAccordionSettings(id: string, settings: Partial<AccordionSettings>): void {
    this.setComponentSettings<AccordionSettings>('accordions', id, settings);
  }

  getTooltipSettings(id: string): TooltipSettings | null {
    return this.getComponentSettings<TooltipSettings>('tooltips', id);
  }

  setTooltipSettings(id: string, settings: Partial<TooltipSettings>): void {
    this.setComponentSettings<TooltipSettings>('tooltips', id, settings);
  }

  getPopoverSettings(id: string): PopoverSettings | null {
    return this.getComponentSettings<PopoverSettings>('popovers', id);
  }

  setPopoverSettings(id: string, settings: Partial<PopoverSettings>): void {
    this.setComponentSettings<PopoverSettings>('popovers', id, settings);
  }

  getDropdownSettings(id: string): DropdownSettings | null {
    return this.getComponentSettings<DropdownSettings>('dropdowns', id);
  }

  setDropdownSettings(id: string, settings: Partial<DropdownSettings>): void {
    this.setComponentSettings<DropdownSettings>('dropdowns', id, settings);
  }

  getPaginationSettings(id: string): PaginationSettings | null {
    return this.getComponentSettings<PaginationSettings>('pagination', id);
  }

  setPaginationSettings(id: string, settings: Partial<PaginationSettings>): void {
    this.setComponentSettings<PaginationSettings>('pagination', id, settings);
  }

  getBreadcrumbSettings(id: string): BreadcrumbSettings | null {
    return this.getComponentSettings<BreadcrumbSettings>('breadcrumbs', id);
  }

  setBreadcrumbSettings(id: string, settings: Partial<BreadcrumbSettings>): void {
    this.setComponentSettings<BreadcrumbSettings>('breadcrumbs', id, settings);
  }

  getStepperSettings(id: string): StepperSettings | null {
    return this.getComponentSettings<StepperSettings>('steppers', id);
  }

  setStepperSettings(id: string, settings: Partial<StepperSettings>): void {
    this.setComponentSettings<StepperSettings>('steppers', id, settings);
  }

  getChipSettings(id: string): ChipSettings | null {
    return this.getComponentSettings<ChipSettings>('chips', id);
  }

  setChipSettings(id: string, settings: Partial<ChipSettings>): void {
    this.setComponentSettings<ChipSettings>('chips', id, settings);
  }

  getAvatarSettings(id: string): AvatarSettings | null {
    return this.getComponentSettings<AvatarSettings>('avatars', id);
  }

  setAvatarSettings(id: string, settings: Partial<AvatarSettings>): void {
    this.setComponentSettings<AvatarSettings>('avatars', id, settings);
  }

  getSkeletonSettings(id: string): SkeletonSettings | null {
    return this.getComponentSettings<SkeletonSettings>('skeletons', id);
  }

  setSkeletonSettings(id: string, settings: Partial<SkeletonSettings>): void {
    this.setComponentSettings<SkeletonSettings>('skeletons', id, settings);
  }

  getDividerSettings(id: string): DividerSettings | null {
    return this.getComponentSettings<DividerSettings>('dividers', id);
  }

  setDividerSettings(id: string, settings: Partial<DividerSettings>): void {
    this.setComponentSettings<DividerSettings>('dividers', id, settings);
  }

  getSpacerSettings(id: string): SpacerSettings | null {
    return this.getComponentSettings<SpacerSettings>('spacers', id);
  }

  setSpacerSettings(id: string, settings: Partial<SpacerSettings>): void {
    this.setComponentSettings<SpacerSettings>('spacers', id, settings);
  }

  getGridSettings(id: string): GridSettings | null {
    return this.getComponentSettings<GridSettings>('grids', id);
  }

  setGridSettings(id: string, settings: Partial<GridSettings>): void {
    this.setComponentSettings<GridSettings>('grids', id, settings);
  }

  getContainerSettings(id: string): ContainerSettings | null {
    return this.getComponentSettings<ContainerSettings>('containers', id);
  }

  setContainerSettings(id: string, settings: Partial<ContainerSettings>): void {
    this.setComponentSettings<ContainerSettings>('containers', id, settings);
  }
}

