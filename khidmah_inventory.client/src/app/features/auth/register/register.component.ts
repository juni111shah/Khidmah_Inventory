import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { UnifiedInputComponent } from '../../../shared/components/unified-input/unified-input.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    ToastComponent,
    LoadingSpinnerComponent,
    IconComponent,
    UnifiedInputComponent,
    UnifiedButtonComponent,
    UnifiedCardComponent
  ],
  templateUrl: './register.component.html'
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  loading = false;

  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'error';

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      userName: ['', [Validators.required, Validators.minLength(3)]],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      phoneNumber: [''],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required]],
      companyId: ['', [Validators.required]]
    }, {
      validators: this.passwordMatchValidator
    });
  }

  ngOnInit(): void {
    // If already logged in, redirect
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/']);
    }
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');

    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    return null;
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.markFormGroupTouched(this.registerForm);
      return;
    }

    this.loading = true;
    const formValue = this.registerForm.value;
    const registerData = {
      email: formValue.email,
      userName: formValue.userName,
      password: formValue.password,
      firstName: formValue.firstName,
      lastName: formValue.lastName,
      phoneNumber: formValue.phoneNumber || undefined,
      companyId: formValue.companyId
    };

    this.authService.register(registerData).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.showToastMessage('success', 'User registered successfully!');
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 2000);
        } else {
          this.showToastMessage('error', response.message || 'Registration failed');
          this.loading = false;
        }
      },
      error: (error: any) => {
        const message = error?.error?.message || 'An error occurred during registration';
        this.showToastMessage('error', message);
        this.loading = false;
      }
    });
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
  }

  private showToastMessage(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.toastType = type;
    this.toastMessage = message;
    this.showToast = true;
    setTimeout(() => {
      this.showToast = false;
    }, 5000);
  }

  get email() { return this.registerForm.get('email'); }
  get userName() { return this.registerForm.get('userName'); }
  get firstName() { return this.registerForm.get('firstName'); }
  get lastName() { return this.registerForm.get('lastName'); }
  get password() { return this.registerForm.get('password'); }
  get confirmPassword() { return this.registerForm.get('confirmPassword'); }
  get companyId() { return this.registerForm.get('companyId'); }

  getErrorMessage(controlName: string): string {
    const control = this.registerForm.get(controlName);
    if (!control || !control.errors || !control.touched) return '';

    if (control.errors['required']) return `${controlName.charAt(0).toUpperCase() + controlName.slice(1)} is required`;
    if (control.errors['email']) return 'Please enter a valid email';
    if (control.errors['minlength']) return `${controlName.charAt(0).toUpperCase() + controlName.slice(1)} must be at least ${control.errors['minlength'].requiredLength} characters`;
    if (control.errors['passwordMismatch']) return 'Passwords do not match';
    
    return 'Invalid field';
  }
}
