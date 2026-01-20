import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ThemeService } from '../../../core/services/theme.service';
import { ThemeConfig } from '../../../core/models/theme.model';
import { IconComponent } from '../icon/icon.component';
import { UnifiedButtonComponent } from '../unified-button/unified-button.component';

@Component({
  selector: 'app-theme-panel',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, IconComponent, UnifiedButtonComponent],
  templateUrl: './theme-panel.component.html'
})
export class ThemePanelComponent implements OnInit {
  isOpen = false;
  theme: ThemeConfig;

  constructor(private themeService: ThemeService) {
    this.theme = this.themeService.getTheme();
  }

  ngOnInit(): void {
    this.themeService.theme$.subscribe(theme => {
      this.theme = { ...theme };
    });
  }

  togglePanel(): void {
    this.isOpen = !this.isOpen;
  }

  closePanel(): void {
    this.isOpen = false;
  }

  onColorChange(property: keyof ThemeConfig, value: string): void {
    this.themeService.updateProperty(property, value);
  }

  onToggleChange(property: keyof ThemeConfig, value: boolean): void {
    this.themeService.updateProperty(property, value);
  }

  onSelectChange(property: keyof ThemeConfig, value: string): void {
    this.themeService.updateProperty(property, value);
  }
}

