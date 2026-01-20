import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UnifiedButtonComponent } from '../unified-button/unified-button.component';
import { UnifiedCardComponent } from '../unified-card/unified-card.component';
import { UnifiedInputComponent } from '../unified-input/unified-input.component';
import { ThemeService } from '../../services/theme.service';

@Component({
  selector: 'app-ui-showcase',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    UnifiedButtonComponent,
    UnifiedCardComponent,
    UnifiedInputComponent
  ],
  templateUrl: './ui-showcase.component.html'
})
export class UiShowcaseComponent {
  email: string = '';
  password: string = '';
  isLoading: boolean = false;

  constructor(public themeService: ThemeService) {}

  onSave(): void {
    this.isLoading = true;
    setTimeout(() => {
      this.isLoading = false;
      alert('Saved successfully!');
    }, 2000);
  }

  onChangeTheme(): void {
    this.themeService.setTheme({
      primaryColor: '#673AB7',
      secondaryColor: '#FF5722',
      borderRadius: '12px'
    });
  }

  onResetTheme(): void {
    this.themeService.resetTheme();
  }
}

