import { Component, Input, Output, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ThemeService } from '../../../core/services/theme.service';
import { HeaderService } from '../../../core/services/header.service';
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
  private headerSubscription?: Subscription;

  constructor(
    public themeService: ThemeService,
    private headerService: HeaderService
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
  }

  ngOnDestroy(): void {
    if (this.headerSubscription) {
      this.headerSubscription.unsubscribe();
    }
  }

  onMenuToggle(): void {
    this.menuToggle.emit();
  }
}

