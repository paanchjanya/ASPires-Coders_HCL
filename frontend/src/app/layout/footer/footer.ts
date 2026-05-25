import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <footer class="bg-dark text-white pt-5 pb-4 mt-auto">
      <div class="container">
        <div class="row">
          <div class="col-md-4 mb-4 mb-md-0">
            <h5 class="serif-font text-uppercase mb-3" style="letter-spacing: 0.1em; color: var(--primary-color);">AuraStay</h5>
            <p class="text-secondary small" style="line-height: 1.8;">
              A refined selection of handpicked luxury hotels around the globe. Offering exceptional comfort, signature designs, and responsive guest support.
            </p>
          </div>
          <div class="col-md-4 mb-4 mb-md-0 d-flex flex-column align-items-md-center">
            <div>
              <h5 class="serif-font text-uppercase mb-3" style="letter-spacing: 0.05rem;">Quick Links</h5>
              <ul class="list-unstyled">
                <li class="mb-2"><a routerLink="/" class="text-secondary text-decoration-none small hover-link">Home</a></li>
                <li class="mb-2"><a routerLink="/my-bookings" class="text-secondary text-decoration-none small hover-link">My Bookings</a></li>
                <li class="mb-2"><a routerLink="/login" class="text-secondary text-decoration-none small hover-link">Sign In</a></li>
              </ul>
            </div>
          </div>
          <div class="col-md-4">
            <h5 class="serif-font text-uppercase mb-3" style="letter-spacing: 0.05rem;">Contact</h5>
            <p class="text-secondary small mb-2"><i class="bi bi-geo-alt me-2 text-gold"></i> 100 Manhattan Ave, New York, NY</p>
            <p class="text-secondary small mb-2"><i class="bi bi-envelope me-2 text-gold"></i> concierge&#64;aurastay.com</p>
            <p class="text-secondary small"><i class="bi bi-telephone me-2 text-gold"></i> +1 (555) 019-2834</p>
          </div>
        </div>
        <hr class="border-secondary my-4">
        <div class="row align-items-center">
          <div class="col-md-6 text-center text-md-start">
            <p class="text-secondary small mb-0">&copy; 2026 AuraStay. All rights reserved.</p>
          </div>
          <div class="col-md-6 text-center text-md-end mt-3 mt-md-0">
            <a href="#" class="text-secondary me-3"><i class="bi bi-instagram"></i></a>
            <a href="#" class="text-secondary me-3"><i class="bi bi-facebook"></i></a>
            <a href="#" class="text-secondary"><i class="bi bi-twitter-x"></i></a>
          </div>
        </div>
      </div>
    </footer>
  `,
  styles: [`
    .text-gold {
      color: var(--primary-color);
    }
    .hover-link {
      transition: var(--transition-smooth);
    }
    .hover-link:hover {
      color: var(--primary-color) !important;
    }
  `]
})
export class FooterComponent {}
