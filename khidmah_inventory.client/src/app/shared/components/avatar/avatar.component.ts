import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IconComponent } from '../icon/icon.component';

@Component({
  selector: 'app-avatar',
  standalone: true,
  imports: [CommonModule, IconComponent],
  templateUrl: './avatar.component.html'
})
export class AvatarComponent {
  @Input() src: string = '';
  @Input() alt: string = '';
  @Input() size: 'small' | 'medium' | 'large' = 'medium';
  @Input() shape: 'circle' | 'square' = 'circle';
  @Input() initials: string = '';
  @Input() status: 'online' | 'offline' | 'away' | '' = '';
}

