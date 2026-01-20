import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { PermissionService } from '../../../core/services/permission.service';
import { IconComponent } from '../icon/icon.component';
import { HasPermissionDirective } from '../../directives/has-permission.directive';
import { ClickOutsideDirective } from '../../directives/click-outside.directive';

@Component({
  selector: 'app-user-menu',
  standalone: true,
  imports: [CommonModule, IconComponent, HasPermissionDirective, ClickOutsideDirective],
  templateUrl: './user-menu.component.html'
})
export class UserMenuComponent implements OnInit {
  user: any = null;
  showMenu = false;

  constructor(
    public authService: AuthService,
    public permissionService: PermissionService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.user = user;
    });
  }

  toggleMenu(): void {
    this.showMenu = !this.showMenu;
  }

  closeMenu(): void {
    this.showMenu = false;
  }

  viewProfile(): void {
    if (this.user) {
      this.router.navigate(['/users', this.user.id]);
      this.closeMenu();
    }
  }

  goToSettings(): void {
    this.router.navigate(['/settings']);
    this.closeMenu();
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => {
        this.closeMenu();
      },
      error: () => {
        this.closeMenu();
      }
    });
  }
}


