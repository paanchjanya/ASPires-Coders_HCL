import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { PromotionService } from '../../services/promotion.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <nav class="navbar navbar-expand-lg navbar-light bg-white border-bottom py-3 sticky-top">
      <div class="container">
        <a class="navbar-brand d-flex align-items-center" routerLink="/">
          <span class="fs-4 fw-bold serif-font" style="letter-spacing: 0.05em; color: var(--text-dark);">AURA<span style="color: var(--primary-color);">STAY</span></span>
        </a>
        <button class="navbar-toggler border-0" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
          <span class="navbar-toggler-icon"></span>
        </button>
        
        <div class="collapse navbar-collapse" id="navbarNav">
          <ul class="navbar-nav ms-auto align-items-lg-center">
            <li class="nav-item">
              <a class="nav-link text-uppercase fw-semibold px-3" style="font-size: 0.8rem; letter-spacing: 0.05em;" routerLink="/" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}">Home</a>
            </li>
            <li class="nav-item" *ngIf="authService.isLoggedIn">
              <a class="nav-link text-uppercase fw-semibold px-3" style="font-size: 0.8rem; letter-spacing: 0.05em;" routerLink="/my-bookings" routerLinkActive="active">My Bookings</a>
            </li>
            <li class="nav-item" *ngIf="authService.isLoggedIn && (authService.isAdmin || authService.isSuperAdmin)">
              <a class="nav-link text-uppercase fw-semibold px-3" style="font-size: 0.8rem; letter-spacing: 0.05em; color: var(--primary-color);" routerLink="/admin" routerLinkActive="active">Admin Dashboard</a>
            </li>
            
            <!-- Loyalty Status -->
            <li class="nav-item px-2 mt-2 mt-lg-0" *ngIf="authService.isLoggedIn && !authService.isAdmin && !authService.isSuperAdmin">
              <span class="badge text-uppercase border tracking-wider" [ngClass]="getBadgeClass()" style="font-size: 0.75rem; letter-spacing: 0.05em; padding: 0.45em 0.9em; border-radius: 0;">
                <i class="bi bi-patch-check-fill me-1"></i> {{ pointsData?.badge || 'Bronze' }} ({{ pointsData?.availablePoints || 0 }} pts)
              </span>
            </li>
            
            <li class="nav-item ms-lg-3 mt-3 mt-lg-0" *ngIf="!authService.isLoggedIn">
              <a class="btn btn-premium btn-sm" routerLink="/login">Sign In</a>
            </li>
            
            <li class="nav-item ms-lg-3 mt-3 mt-lg-0 d-flex align-items-center" *ngIf="authService.isLoggedIn">
              <span class="me-3 text-muted px-2 border-end" style="font-size: 0.85rem;">
                Welcome, <strong>{{ authService.currentUserValue?.username }}</strong>
                <span class="small text-muted font-monospace" *ngIf="authService.isSuperAdmin"> (SuperAdmin)</span>
                <span class="small text-muted font-monospace" *ngIf="authService.isAdmin"> (Admin)</span>
              </span>
              <button class="btn btn-premium-outline btn-sm" (click)="onLogout()">Log Out</button>
            </li>
          </ul>
        </div>
      </div>
    </nav>
  `,
  styles: [`
    .nav-link {
      color: var(--text-dark);
      transition: var(--transition-smooth);
    }
    .nav-link:hover, .nav-link.active {
      color: var(--primary-color) !important;
    }
    .bg-bronze { background-color: rgba(205, 127, 50, 0.1); color: #cd7f32; border-color: rgba(205, 127, 50, 0.3) !important; }
    .bg-silver { background-color: rgba(192, 192, 192, 0.1); color: #7f8c8d; border-color: rgba(192, 192, 192, 0.3) !important; }
    .bg-gold { background-color: rgba(212, 175, 55, 0.1); color: #d4af37; border-color: rgba(212, 175, 55, 0.3) !important; }
  `]
})
export class NavbarComponent implements OnInit {
  public authService = inject(AuthService);
  private promotionService = inject(PromotionService);
  private router = inject(Router);

  pointsData: any = null;

  ngOnInit(): void {
    if (this.authService.isLoggedIn && !this.authService.isAdmin && !this.authService.isSuperAdmin) {
      this.loadLoyaltyPoints();
    }
  }

  loadLoyaltyPoints(): void {
    this.promotionService.getLoyaltyPoints().subscribe({
      next: (data) => {
        this.pointsData = data;
      },
      error: () => {}
    });
  }

  getBadgeClass(): string {
    const badge = this.pointsData?.badge?.toLowerCase() || 'bronze';
    if (badge === 'gold') return 'bg-gold';
    if (badge === 'silver') return 'bg-silver';
    return 'bg-bronze';
  }

  onLogout(): void {
    this.authService.logout();
    this.pointsData = null;
    this.router.navigate(['/']);
  }
}
