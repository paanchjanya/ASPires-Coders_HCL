import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HotelService } from '../../services/hotel.service';
import { Hotel, Location } from '../../models/hotel.model';

@Component({
  selector: 'app-hotel-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <div class="container my-5 fade-in">
      <div class="row mb-5">
        <div class="col-12">
          <span class="text-muted text-uppercase fw-semibold tracking-wider font-monospace" style="font-size: 0.8rem; letter-spacing: 0.15em;">Search Results</span>
          <h1 class="serif-font display-6 mt-1 mb-0">Find Your Perfect Bangalore Stay</h1>
          <p class="text-muted mt-2">Explore 24 handpicked properties spread across 6 prime locations in Bangalore city.</p>
        </div>
      </div>

      <div class="row">
        <!-- Sidebar Filters -->
        <div class="col-lg-3 mb-4">
          <div class="bg-white border p-4 shadow-sm">
            <h5 class="serif-font mb-4 border-bottom pb-2" style="font-size: 1.15rem;">Refine Search</h5>

            <div class="mb-4">
              <label class="form-label small text-muted text-uppercase fw-semibold" style="font-size: 0.7rem; letter-spacing: 0.05em;">Hotel Name</label>
              <input type="text" class="form-control form-control-premium" placeholder="e.g. Royal Orchid" [(ngModel)]="filters.name" (ngModelChange)="applyFilters()">
            </div>

            <div class="mb-4">
              <label class="form-label small text-muted text-uppercase fw-semibold" style="font-size: 0.7rem; letter-spacing: 0.05em;">Location Hub</label>
              <select class="form-select form-control-premium rounded-0 border-0 border-bottom" [(ngModel)]="filters.location" (ngModelChange)="applyFilters()">
                <option value="">All Locations</option>
                <option *ngFor="let loc of locations" [value]="loc.name">{{ loc.name }}</option>
              </select>
            </div>

            <div class="mb-4">
              <label class="form-label small text-muted text-uppercase fw-semibold" style="font-size: 0.7rem; letter-spacing: 0.05em;">Max Price (₹)</label>
              <input type="number" class="form-control form-control-premium" placeholder="e.g. 5000" [(ngModel)]="filters.maxPrice" (ngModelChange)="applyFilters()">
            </div>

            <div class="mb-4">
              <label class="form-label small text-muted text-uppercase fw-semibold" style="font-size: 0.7rem; letter-spacing: 0.05em;">Capacity (Guests)</label>
              <input type="number" class="form-control form-control-premium" placeholder="e.g. 2" [(ngModel)]="filters.guests" (ngModelChange)="applyFilters()">
            </div>

            <div class="mb-3">
              <label class="form-label small text-muted text-uppercase fw-semibold" style="font-size: 0.7rem; letter-spacing: 0.05em;">Amenities</label>
              <div *ngFor="let am of availableAmenities" class="form-check mb-2">
                <input type="checkbox" class="form-check-input" [id]="'am-' + am.id" [checked]="filters.amenityIds.includes(am.id)" (change)="toggleAmenity(am.id)">
                <label class="form-check-label small text-muted ms-1" [for]="'am-' + am.id">
                  <i class="bi" [ngClass]="am.icon" style="color: var(--primary-color); margin-right: 4px;"></i> {{ am.name }}
                </label>
              </div>
            </div>

            <button class="btn btn-premium-outline w-100 mt-3 py-2 btn-sm" (click)="resetFilters()">Clear All Filters</button>
          </div>
        </div>

        <!-- Hotels List -->
        <div class="col-lg-9">
          <!-- Loading Spinner -->
          <div *ngIf="loading" class="text-center py-5">
            <div class="spinner-border text-dark" role="status">
              <span class="visually-hidden">Loading...</span>
            </div>
          </div>

          <div *ngIf="!loading && filteredHotels.length === 0" class="text-center py-5 bg-white border border-light shadow-sm">
            <i class="bi bi-search text-muted fs-1 mb-3 d-block"></i>
            <h4 class="serif-font">No Properties Match Your Search</h4>
            <p class="text-muted small">Try broadening your filters or clearing them to start fresh.</p>
            <button class="btn btn-premium btn-sm mt-3" (click)="resetFilters()">Clear Filters</button>
          </div>

          <div class="row g-4" *ngIf="!loading && filteredHotels.length > 0">
            <div class="col-12" *ngFor="let hotel of filteredHotels">
              <div class="card hotel-card h-100 flex-md-row border border-light">
                <div class="col-md-5 overflow-hidden" style="min-height: 250px;">
                  <img [src]="hotel.imageUrl" [alt]="hotel.name" class="w-100 h-100 object-fit-cover">
                </div>
                <div class="col-md-7 p-4 p-md-5 d-flex flex-column">
                  <div class="d-flex justify-content-between align-items-start mb-2">
                    <span class="text-muted text-uppercase fw-semibold small" style="font-size: 0.75rem; letter-spacing: 0.05em;"><i class="bi bi-geo-alt me-1" style="color: var(--primary-color);"></i>{{ hotel.location?.name }}, Bangalore</span>
                    <span class="fw-bold serif-font small text-dark"><i class="bi bi-star-fill text-warning me-1"></i> {{ hotel.rating }}</span>
                  </div>

                  <h3 class="serif-font mb-3 fs-4" style="color: var(--text-dark);">{{ hotel.name }}</h3>

                  <p class="text-muted small card-text mb-4" style="line-height: 1.7; display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden;">
                    {{ hotel.description }}
                  </p>

                  <div class="mt-auto d-flex flex-wrap align-items-center justify-content-between g-3 pt-3 border-top">
                    <div>
                      <span class="text-muted small d-block" style="font-size: 0.75rem;">Starting from</span>
                      <span class="fs-4 fw-bold serif-font" style="color: var(--primary-color);">₹{{ getStartingPrice(hotel) | number }}</span><span class="text-muted small">/night</span>
                    </div>
                    <div>
                      <a [routerLink]="['/hotels', hotel.id]" class="btn btn-premium py-2.5 px-4 btn-sm" [queryParams]="{ checkIn: queryCheckIn, checkOut: queryCheckOut }">Explore Rooms</a>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class HotelListComponent implements OnInit {
  private hotelService = inject(HotelService);
  private route = inject(ActivatedRoute);

  hotels: Hotel[] = [];
  filteredHotels: Hotel[] = [];
  locations: Location[] = [];
  loading = true;

  queryCheckIn = '';
  queryCheckOut = '';

  availableAmenities = [
    { id: 1, name: 'Free High-Speed Wi-Fi', icon: 'bi-wifi' },
    { id: 2, name: 'Swimming Pool', icon: 'bi-water' },
    { id: 3, name: 'Fitness Center / Gym', icon: 'bi-activity' },
    { id: 4, name: 'Luxury Spa', icon: 'bi-heart-pulse' },
    { id: 5, name: 'Complimentary Breakfast', icon: 'bi-egg-fried' }
  ];

  filters = {
    name: '',
    location: '',
    maxPrice: null as number | null,
    guests: null as number | null,
    amenityIds: [] as number[]
  };

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.filters.location = params['location'] || '';
      this.queryCheckIn = params['checkIn'] || '';
      this.queryCheckOut = params['checkOut'] || '';
      this.loadLocations();
      this.loadHotels();
    });
  }

  loadLocations(): void {
    this.hotelService.getLocations().subscribe(data => {
      this.locations = data;
    });
  }

  loadHotels(): void {
    this.loading = true;
    this.hotelService.getAll().subscribe({
      next: (data) => {
        this.hotels = data;
        this.applyFilters();
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  toggleAmenity(id: number): void {
    const idx = this.filters.amenityIds.indexOf(id);
    if (idx === -1) {
      this.filters.amenityIds.push(id);
    } else {
      this.filters.amenityIds.splice(idx, 1);
    }
    this.applyFilters();
  }

  applyFilters(): void {
    this.filteredHotels = this.hotels.filter(hotel => {
      if (this.filters.name && !hotel.name.toLowerCase().includes(this.filters.name.toLowerCase())) {
        return false;
      }

      if (this.filters.location && hotel.location?.name?.toLowerCase() !== this.filters.location.toLowerCase()) {
        return false;
      }

      if (this.filters.amenityIds.length > 0) {
        const hotelAmenityIds = hotel.hotelAmenities?.map(ha => ha.amenityId) || [];
        const hasAllAmenities = this.filters.amenityIds.every(id => hotelAmenityIds.includes(id));
        if (!hasAllAmenities) return false;
      }

      if (!hotel.rooms || hotel.rooms.length === 0) {
        return true;
      }

      if (this.filters.maxPrice) {
        const hasMatchingPrice = hotel.rooms.some(r => r.pricePerNight <= (this.filters.maxPrice as number));
        if (!hasMatchingPrice) return false;
      }

      if (this.filters.guests) {
        const hasMatchingCapacity = hotel.rooms.some(r => r.capacity >= (this.filters.guests as number));
        if (!hasMatchingCapacity) return false;
      }

      return true;
    });
  }

  resetFilters(): void {
    this.filters = {
      name: '',
      location: '',
      maxPrice: null,
      guests: null,
      amenityIds: []
    };
    this.applyFilters();
  }

  getStartingPrice(hotel: Hotel): number {
    if (!hotel.rooms || hotel.rooms.length === 0) return 0;
    return Math.min(...hotel.rooms.map(r => r.pricePerNight));
  }
}
