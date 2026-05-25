import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <div class="container my-5 fade-in">
      <div class="row justify-content-center">
        <div class="col-md-5">
          <div class="bg-white border p-4 p-md-5 shadow-sm">
            <h2 class="serif-font text-center mb-4 fs-3">Create Account</h2>

            <div *ngIf="errorMessage" class="alert alert-danger border-0 rounded-0 py-2 small" role="alert">
              {{ errorMessage }}
            </div>

            <form (ngSubmit)="onSubmit()" #registerForm="ngForm">
              <div class="mb-3">
                <label for="username" class="form-label small text-muted text-uppercase fw-semibold" style="font-size: 0.75rem; letter-spacing: 0.05em;">Username</label>
                <input type="text" class="form-control form-control-premium" id="username" name="username" [(ngModel)]="user.username" required minlength="3" #usernameInput="ngModel">
                <div *ngIf="usernameInput.invalid && (usernameInput.dirty || usernameInput.touched)" class="text-danger small mt-1" style="font-size: 0.8rem;">
                  Username must be at least 3 characters.
                </div>
              </div>

              <div class="mb-3">
                <label for="email" class="form-label small text-muted text-uppercase fw-semibold" style="font-size: 0.75rem; letter-spacing: 0.05em;">Email Address</label>
                <input type="email" class="form-control form-control-premium" id="email" name="email" [(ngModel)]="user.email" required email #emailInput="ngModel">
                <div *ngIf="emailInput.invalid && (emailInput.dirty || emailInput.touched)" class="text-danger small mt-1" style="font-size: 0.8rem;">
                  Please enter a valid email address.
                </div>
              </div>

              <div class="mb-4">
                <label for="password" class="form-label small text-muted text-uppercase fw-semibold" style="font-size: 0.75rem; letter-spacing: 0.05em;">Password</label>
                <input type="password" class="form-control form-control-premium" id="password" name="password" [(ngModel)]="user.password" required minlength="6" #pwdInput="ngModel">
                <div *ngIf="pwdInput.invalid && (pwdInput.dirty || pwdInput.touched)" class="text-danger small mt-1" style="font-size: 0.8rem;">
                  Password must be at least 6 characters.
                </div>
              </div>

              <button type="submit" class="btn btn-premium w-100 py-3 mt-2" [disabled]="registerForm.invalid || loading">
                <span *ngIf="loading" class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                Register
              </button>
            </form>

            <div class="text-center mt-4">
              <p class="small text-muted mb-0">Already have an account? <a routerLink="/login" class="text-dark fw-bold text-decoration-none border-bottom border-dark pb-1">Sign in here</a></p>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class RegisterComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  user = { username: '', email: '', password: '' };
  loading = false;
  errorMessage = '';

  onSubmit(): void {
    this.loading = true;
    this.errorMessage = '';

    this.authService.register(this.user).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.error?.message || 'Registration failed. The email may already be in use.';
      }
    });
  }
}
