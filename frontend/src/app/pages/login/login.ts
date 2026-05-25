import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <div class="container my-5 fade-in">
      <div class="row justify-content-center">
        <div class="col-md-5">
          <div class="bg-white border p-4 p-md-5 shadow-sm">
            <h2 class="serif-font text-center mb-4 fs-3">Sign In</h2>

            <div *ngIf="errorMessage" class="alert alert-danger border-0 rounded-0 py-2 small" role="alert">
              {{ errorMessage }}
            </div>

            <form (ngSubmit)="onSubmit()" #loginForm="ngForm">
              <div class="mb-3">
                <label for="email" class="form-label small text-muted text-uppercase fw-semibold" style="font-size: 0.75rem; letter-spacing: 0.05em;">Email Address</label>
                <input type="email" class="form-control form-control-premium" id="email" name="email" [(ngModel)]="credentials.email" required email #emailInput="ngModel">
                <div *ngIf="emailInput.invalid && (emailInput.dirty || emailInput.touched)" class="text-danger small mt-1" style="font-size: 0.8rem;">
                  Please enter a valid email.
                </div>
              </div>

              <div class="mb-4">
                <label for="password" class="form-label small text-muted text-uppercase fw-semibold" style="font-size: 0.75rem; letter-spacing: 0.05em;">Password</label>
                <input type="password" class="form-control form-control-premium" id="password" name="password" [(ngModel)]="credentials.password" required #pwdInput="ngModel">
                <div *ngIf="pwdInput.invalid && (pwdInput.dirty || pwdInput.touched)" class="text-danger small mt-1" style="font-size: 0.8rem;">
                  Password is required.
                </div>
              </div>

              <button type="submit" class="btn btn-premium w-100 py-3 mt-2" [disabled]="loginForm.invalid || loading">
                <span *ngIf="loading" class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                Login
              </button>
            </form>

            <div class="text-center mt-4">
              <p class="small text-muted mb-0">Don't have an account? <a routerLink="/register" class="text-dark fw-bold text-decoration-none border-bottom border-dark pb-1">Register here</a></p>
            </div>
            
            <div class="mt-4 p-3 bg-light border-start border-4 border-warning" style="font-size: 0.8rem;">
              <span class="fw-bold">Test Credentials:</span><br>
              <strong>Admin:</strong> admin&#64;hotelbooking.com / Admin&#64;123<br>
              <strong>User:</strong> user&#64;hotelbooking.com / User&#64;123
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class LoginComponent implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  credentials = { email: '', password: '' };
  loading = false;
  errorMessage = '';
  returnUrl = '';

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    if (this.authService.isLoggedIn) {
      this.router.navigate([this.returnUrl]);
    }
  }

  onSubmit(): void {
    this.loading = true;
    this.errorMessage = '';

    this.authService.login(this.credentials).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate([this.returnUrl]);
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.error?.message || 'Login failed. Please check your credentials.';
      }
    });
  }
}
