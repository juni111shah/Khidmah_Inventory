import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AuthInterceptor } from './core/interceptors/auth.interceptor';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SettingsComponent } from './features/settings/settings.component';
import { ThemedButtonComponent } from './shared/components/themed-button/themed-button.component';
import { ThemedCardComponent } from './shared/components/themed-card/themed-card.component';
import { SidebarComponent } from './shared/components/sidebar/sidebar.component';
import { MainLayoutComponent } from './shared/components/main-layout/main-layout.component';
import { AppHeaderComponent } from './shared/components/app-header/app-header.component';
import { AppFooterComponent } from './shared/components/app-footer/app-footer.component';
import { LoadingSpinnerComponent } from './shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from './shared/components/empty-state/empty-state.component';
import { BadgeComponent } from './shared/components/badge/badge.component';
import { InputFieldComponent } from './shared/components/input-field/input-field.component';
import { ModalComponent } from './shared/components/modal/modal.component';
import { ToastComponent } from './shared/components/toast/toast.component';
import { AvatarComponent } from './shared/components/avatar/avatar.component';
import { DropdownComponent } from './shared/components/dropdown/dropdown.component';
import { TabsComponent, TabComponent } from './shared/components/tabs/tabs.component';
import { PaginationComponent } from './shared/components/pagination/pagination.component';
import { UserMenuComponent } from './shared/components/user-menu/user-menu.component';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    AppRoutingModule,
    // Import standalone components
    SettingsComponent,
    ThemedButtonComponent,
    ThemedCardComponent,
    SidebarComponent,
    MainLayoutComponent,
    AppHeaderComponent,
    AppFooterComponent,
    LoadingSpinnerComponent,
    EmptyStateComponent,
    BadgeComponent,
    InputFieldComponent,
    ModalComponent,
    ToastComponent,
    AvatarComponent,
    DropdownComponent,
    TabsComponent,
    TabComponent,
    PaginationComponent,
    UserMenuComponent
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
