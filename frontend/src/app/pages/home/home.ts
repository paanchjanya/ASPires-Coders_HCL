import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HotelService } from '../../services/hotel.service';
import { PromotionService } from '../../services/promotion.service';
import { Hotel, Location } from '../../models/hotel.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <!-- Hero Banner -->
    <div class="hero-section position-relative d-flex align-items-center mb-5 fade-in" style="background: linear-gradient(rgba(0,0,0,0.3), rgba(0,0,0,0.3)), url('https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?auto=format&fit=crop&w=1600&q=80') no-repeat center center; background-size: cover; height: 60vh; min-height: 440px;">
      <div class="container text-white text-center z-1 animate-up">
        <span class="text-uppercase fw-semibold mb-2 d-block tracking-wider" style="font-size: 0.85rem; letter-spacing: 0.25em; color: var(--primary-color);">A Aura Stay Collection</span>
        <h1 class="display-3 serif-font mb-3 fw-normal">Discover Bangalore Stays</h1>
        <p class="lead mb-4 mx-auto" style="max-width: 600px; font-weight: 300; font-size: 1.1rem; line-height: 1.8;">Experience refined spaces, bespoke amenities, and boutique luxury in the Silicon Valley of India.</p>
        <a class="btn btn-premium-gold px-4 py-3" routerLink="/hotels">Explore Bangalore Hotels</a>
      </div>
    </div>

    <div class="container mb-5">
      <!-- Search Panel -->
      <div class="glass-panel p-4 position-relative shadow-sm fade-in" style="margin-top: -80px; z-index: 10; background: rgba(255,255,255,0.95); border-radius: 0;">
        <h4 class="serif-font mb-3 fs-5 border-bottom pb-2" style="letter-spacing: 0.02em;">Find Stays in Bangalore</h4>
        <form (ngSubmit)="onSearch()" class="row g-3">
          <div class="col-md-3">
            <label class="form-label small text-muted text-uppercase fw-semibold" style="font-size: 0.7rem; letter-spacing: 0.05em;">Location Hub</label>
            <div class="input-group">
              <span class="input-group-text bg-transparent border-0 border-bottom ps-0 rounded-0"><i class="bi bi-geo-alt" style="color: var(--primary-color);"></i></span>
              <select class="form-select form-control-premium rounded-0 border-0 border-bottom" [(ngModel)]="searchParams.location" name="location">
                <option value="">Any Location Hub</option>
                <option *ngFor="let loc of locations" [value]="loc.name">{{ loc.name }}</option>
              </select>
            </div>
          </div>
          <div class="col-md-3">
            <label class="form-label small text-muted text-uppercase fw-semibold" style="font-size: 0.7rem; letter-spacing: 0.05em;">Check In</label>
            <div class="input-group">
              <span class="input-group-text bg-transparent border-0 border-bottom ps-0 rounded-0"><i class="bi bi-calendar-event" style="color: var(--primary-color);"></i></span>
              <input type="date" class="form-control form-control-premium rounded-0 border-0 border-bottom" [(ngModel)]="searchParams.checkIn" name="checkIn" [min]="minDate">
            </div>
          </div>
          <div class="col-md-3">
            <label class="form-label small text-muted text-uppercase fw-semibold" style="font-size: 0.7rem; letter-spacing: 0.05em;">Check Out</label>
            <div class="input-group">
              <span class="input-group-text bg-transparent border-0 border-bottom ps-0 rounded-0"><i class="bi bi-calendar-check" style="color: var(--primary-color);"></i></span>
              <input type="date" class="form-control form-control-premium rounded-0 border-0 border-bottom" [(ngModel)]="searchParams.checkOut" name="checkOut" [min]="minDate">
            </div>
          </div>
          <div class="col-md-3 d-flex align-items-end">
            <button type="submit" class="btn btn-premium w-100 py-3" style="letter-spacing: 0.1em;">Search Stays</button>
          </div>
        </form>
      </div>

      <!-- Active Seasonal Offers Showcase (Dynamic) -->
      <div class="my-5" *ngIf="activeOffers.length > 0">
        <div class="row g-4">
          <div class="col-md-6" *ngFor="let offer of activeOffers">
            <div class="p-4 bg-white border border-light d-flex align-items-center justify-content-between shadow-sm fade-in">
              <div>
                <span class="badge bg-gold-subtle text-dark border border-warning mb-2 text-uppercase">Seasonal Deal</span>
                <h4 class="serif-font mb-1 fs-5">{{ offer.title }}</h4>
                <p class="text-muted small mb-0">{{ offer.description }}</p>
              </div>
              <div>
                <button class="btn btn-premium-outline py-2.5 px-4 btn-sm" (click)="searchLocation(offer.applicableLocation || '')">
                  Book Now
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Coupon Seeding Banner -->
      <div class="my-5 py-4 px-4 bg-white border border-light d-md-flex align-items-center justify-content-between shadow-sm">
        <div class="mb-3 mb-md-0">
          <span class="badge bg-warning-subtle text-warning border border-warning mb-2">FIRST BOOKING COUPON</span>
          <h4 class="serif-font mb-1 fs-5">New to AuraStay? Get ₹500 off on your first stay!</h4>
          <p class="text-muted small mb-0">Apply promo code <strong class="text-dark">FIRST500</strong> during checkout to redeem.</p>
        </div>
        <div>
          <button class="btn btn-premium-outline py-2.5 px-4 btn-sm" (click)="copyCode('FIRST500')">
            {{ copiedCode === 'FIRST500' ? 'Copied!' : 'Copy Code' }}
          </button>
        </div>
      </div>

      <!-- Bangalore Locations Hub Grid -->
      <div class="my-5">
        <div class="text-center mb-5">
          <span class="text-uppercase text-muted fw-semibold small tracking-wider" style="letter-spacing: 0.2em;">Explore Bangalore</span>
          <h2 class="serif-font display-6 mt-2 mb-3">Stays by Location Hubs</h2>
          <div class="mx-auto bg-dark" style="width: 50px; height: 1.5px;"></div>
        </div>

        <div class="row row-cols-2 row-cols-md-3 row-cols-lg-6 g-3">
          <div class="col" *ngFor="let loc of locations">
            <div class="location-card text-center p-4 border bg-white shadow-sm cursor-pointer" (click)="searchLocation(loc.name)">
              <div class="fs-3 mb-2" style="color: var(--primary-color);"><i class="bi bi-geo-alt-fill"></i></div>
              <h6 class="serif-font mb-0">{{ loc.name }}</h6>
              <span class="text-muted small">Browse stays</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Featured Hotels Showcase -->
      <div class="my-5">
        <div class="text-center mb-5">
          <span class="text-uppercase text-muted fw-semibold small tracking-wider" style="letter-spacing: 0.2em;">Exclusive Collection</span>
          <h2 class="serif-font display-6 mt-2 mb-3">Our Featured Properties</h2>
          <div class="mx-auto bg-dark" style="width: 50px; height: 1.5px;"></div>
        </div>

        <div class="row g-4">
          <div class="col-md-4" *ngFor="let hotel of hotels | slice:0:6">
            <div class="card h-100 hotel-card">
              <div class="position-relative overflow-hidden" style="height: 240px;">
                <img [src]="hotel.imageUrl" [alt]="hotel.name" class="w-100 h-100 object-fit-cover">
                <div class="position-absolute top-0 end-0 m-3 bg-white px-2 py-1 small fw-bold text-dark serif-font shadow-sm">
                  <i class="bi bi-star-fill text-warning me-1"></i> {{ hotel.rating }}
                </div>
              </div>
              <div class="card-body p-4 d-flex flex-column">
                <span class="text-muted text-uppercase fw-semibold small" style="font-size: 0.75rem; letter-spacing: 0.05em;"><i class="bi bi-geo-alt me-1"></i>{{ hotel.location?.name }}</span>
                <h4 class="serif-font mt-2 mb-3 fs-5" style="color: var(--text-dark);">{{ hotel.name }}</h4>
                <p class="text-muted small card-text mb-4" style="line-height: 1.7; display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden;">
                  {{ hotel.description }}
                </p>
                <div class="mt-auto">
                  <a [routerLink]="['/hotels', hotel.id]" class="btn btn-premium-outline btn-sm w-100 py-2.5">Book Now</a>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .tracking-wider {
      letter-spacing: 0.2em;
    }
    .location-card {
      transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
    }
    .location-card:hover {
      transform: translateY(-5px);
      border-color: var(--primary-color) !important;
      box-shadow: 0 8px 24px rgba(197,168,128,0.15) !important;
    }
    .cursor-pointer {
      cursor: pointer;
    }
    .bg-gold-subtle {
      background-color: rgba(197, 168, 128, 0.1);
      color: #9d7d4f !important;
    }
  `]
})
export class HomeComponent implements OnInit {
  private hotelService = inject(HotelService);
  private promotionService = inject(PromotionService);
  private router = inject(Router);

  hotels: Hotel[] = [];
  locations: Location[] = [];
  activeOffers: any[] = [];
  minDate = '';
  copiedCode = '';

  searchParams = {
    location: '',
    checkIn: '',
    checkOut: ''
  };

  ngOnInit(): void {
    const today = new Date();
    this.minDate = today.toISOString().split('T')[0];

    const tomorrow = new Date(today);
    tomorrow.setDate(today.getDate() + 1);
    const dayAfter = new Date(tomorrow);
    dayAfter.setDate(tomorrow.getDate() + 1);

    this.searchParams.checkIn = tomorrow.toISOString().split('T')[0];
    this.searchParams.checkOut = dayAfter.toISOString().split('T')[0];

    this.loadHotels();
    this.loadLocations();
    this.loadActiveOffers();
  }

  loadHotels(): void {
    this.hotelService.getAll().subscribe(data => {
      this.hotels = data;
    });
  }

  loadLocations(): void {
    this.hotelService.getLocations().subscribe(data => {
      this.locations = data;
    });
  }

  loadActiveOffers(): void {
    this.promotionService.getActiveOffers().subscribe({
      next: (offers) => {
        this.activeOffers = offers;
      }
    });
  }

  onSearch(): void {
    this.router.navigate(['/hotels'], {
      queryParams: {
        location: this.searchParams.location,
        checkIn: this.searchParams.checkIn,
        checkOut: this.searchParams.checkOut
      }
    });
  }

  searchLocation(locName: string): void {
    this.router.navigate(['/hotels'], {
      queryParams: {
        location: locName,
        checkIn: this.searchParams.checkIn,
        checkOut: this.searchParams.checkOut
      }
    });
  }

  copyCode(code: string): void {
    navigator.clipboard.writeText(code);
    this.copiedCode = code;
    setTimeout(() => this.copiedCode = '', 2000);
  }
}
