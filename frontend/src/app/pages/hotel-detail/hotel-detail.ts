import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HotelService } from '../../services/hotel.service';
import { BookingService } from '../../services/booking.service';
import { AuthService } from '../../services/auth.service';

interface RoomAvailabilityInfo {
  roomId: number;
  isAvailable: boolean;
  checked: boolean;
  checking: boolean;
}

interface Review {
  name: string;
  avatar: string;
  date: string;
  rating: number;
  text: string;
  location: string;
}

@Component({
  selector: 'app-hotel-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <div *ngIf="loading" class="container my-5 text-center py-5">
      <div class="spinner-border text-dark" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>

    <div *ngIf="!loading && hotel" class="fade-in">
      <!-- Hotel Banner -->
      <div class="hotel-banner position-relative d-flex align-items-end"
           [style.background]="'linear-gradient(rgba(0,0,0,0.05), rgba(0,0,0,0.72)), url(' + hotel.imageUrl + ') no-repeat center center'"
           style="background-size: cover; height: 48vh; min-height: 360px;">
        <div class="container text-white pb-5">
          <div class="row">
            <div class="col-md-8">
              <span class="text-uppercase tracking-wider" style="color: var(--primary-color); letter-spacing: 0.1em; font-size: 0.8rem;">
                <i class="bi bi-geo-alt me-1" style="color: var(--primary-color);"></i>{{ hotel.location?.name }}, Bangalore
              </span>
              <h1 class="display-4 serif-font mt-2 mb-0 fw-semibold">{{ hotel.name }}</h1>
              <div class="d-flex gap-3 mt-2 flex-wrap">
                <span class="small" style="opacity:0.85;"><i class="bi bi-check-circle me-1" style="color:var(--primary-color);"></i>Free Wi-Fi</span>
                <span class="small" style="opacity:0.85;"><i class="bi bi-shield-check me-1" style="color:var(--primary-color);"></i>Verified Property</span>
                <span class="small" style="opacity:0.85;"><i class="bi bi-award me-1" style="color:var(--primary-color);"></i>Best in {{ hotel.location?.name }}</span>
              </div>
            </div>
            <div class="col-md-4 text-md-end d-flex align-items-end justify-content-md-end mt-3 mt-md-0">
              <div class="bg-white text-dark px-4 py-3 fw-bold serif-font d-flex align-items-center shadow-sm gap-2">
                <i class="bi bi-star-fill text-warning"></i>
                <span class="fs-5">{{ hotel.rating }}</span>
                <span class="text-muted small fw-normal">/ 5.0</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="container my-5">
        <div class="row g-5">
          <!-- Left Details Column -->
          <div class="col-lg-8">
            <!-- About Section -->
            <h3 class="serif-font border-bottom pb-3 mb-4">About the Property</h3>
            <p class="text-muted" style="line-height: 1.9; font-size: 1.05rem;">{{ hotel.description }}</p>

            <!-- Amenities -->
            <div class="mt-4 mb-5" *ngIf="hotel.hotelAmenities?.length">
              <h5 class="serif-font mb-3">Property Amenities</h5>
              <div class="d-flex flex-wrap gap-3">
                <span class="badge badge-gold px-3 py-2" style="font-size: 0.82rem;"
                      *ngFor="let ha of hotel.hotelAmenities">
                  <i class="bi {{ ha.amenity?.icon }} me-2"></i>{{ ha.amenity?.name }}
                </span>
              </div>
            </div>

            <!-- Available Accommodations -->
            <div class="mt-5">
              <h3 class="serif-font border-bottom pb-3 mb-4">Available Accommodations</h3>

              <!-- Room Search Filter Bar -->
              <div class="mb-4">
                <div class="input-group">
                  <span class="input-group-text bg-transparent border-0 border-bottom ps-0 rounded-0">
                    <i class="bi bi-search" style="color: var(--primary-color);"></i>
                  </span>
                  <input type="text" class="form-control form-control-premium border-0 border-bottom rounded-0 ps-2"
                         placeholder="Search room types (e.g. Super Deluxe, Normal, AC)..."
                         [(ngModel)]="roomQuery"
                         (ngModelChange)="onRoomQueryChange()">
                </div>
              </div>

              <!-- No Results -->
              <div *ngIf="getFilteredRooms().length === 0" class="text-center py-5 text-muted">
                <i class="bi bi-search display-4 d-block mb-3" style="opacity:0.3;"></i>
                <p>No rooms match your search. Try "normal", "deluxe" or "AC".</p>
              </div>

              <!-- Room Cards Grid -->
              <div class="row g-4">
                <div class="col-12" *ngFor="let room of getFilteredRooms()">
                  <div class="card h-100 flex-md-row border border-light shadow-sm overflow-hidden position-relative"
                       *ngIf="room.status !== 'Unavailable'">

                    <!-- Maintenance Badge -->
                    <div *ngIf="room.status === 'Maintenance'"
                         class="position-absolute top-0 start-0 m-2 badge bg-warning text-dark"
                         style="z-index:2; font-size: 0.75rem; padding: 6px 12px;">
                      <i class="bi bi-tools me-1"></i> Under Maintenance
                    </div>

                    <div class="col-md-4 overflow-hidden" style="min-height: 200px;">
                      <img [src]="room.imageUrl" [alt]="room.roomType" class="w-100 h-100 object-fit-cover">
                    </div>
                    <div class="col-md-8 p-4 d-flex flex-column">
                      <div class="d-flex justify-content-between align-items-start mb-2">
                        <div>
                          <h4 class="serif-font mb-0 fs-5" style="color: var(--text-dark);">{{ room.roomType | titlecase }}</h4>
                          <small class="text-muted">
                            <i class="bi bi-people me-1" style="color: var(--primary-color);"></i>
                            Up to {{ room.capacity }} guests
                          </small>
                        </div>
                        <span class="fw-bold serif-font text-dark fs-5">
                          ₹{{ room.pricePerNight | number }}<span class="text-muted small fw-normal" style="font-size: 0.8rem;">/night</span>
                        </span>
                      </div>

                      <p class="text-muted small card-text mb-3" style="line-height: 1.7;">{{ room.description }}</p>

                      <!-- Room Amenities Badges -->
                      <div class="d-flex flex-wrap gap-2 mb-3">
                        <span class="badge badge-gold" style="font-size: 0.72rem;"><i class="bi bi-tv me-1"></i>TV</span>
                        <span class="badge badge-gold" style="font-size: 0.72rem;" *ngIf="room.roomType?.toLowerCase().includes('ac') || room.roomType?.toLowerCase().includes('deluxe')">
                          <i class="bi bi-wind me-1"></i>AC
                        </span>
                        <span class="badge badge-gold" style="font-size: 0.72rem;"><i class="bi bi-shield-check me-1"></i>Safe</span>
                        <span class="badge badge-gold" style="font-size: 0.72rem;" *ngIf="room.roomType?.toLowerCase().includes('deluxe')">
                          <i class="bi bi-cup-hot me-1"></i>Mini Bar
                        </span>
                        <span class="badge bg-light text-dark border" style="font-size: 0.72rem;">
                          <i class="bi bi-info-circle me-1" style="color: var(--primary-color);"></i>Status: {{ room.status }}
                        </span>
                      </div>

                      <!-- Availability Alert: Booked From-To Warning -->
                      <div *ngIf="getAvailability(room.id) as avail">
                        <div *ngIf="avail.checked && !avail.isAvailable && room.status !== 'Maintenance'"
                             class="alert alert-warning border-0 rounded-0 py-2 px-3 mb-3 d-flex align-items-start gap-2"
                             style="background: rgba(255,193,7,0.12); border-left: 3px solid #ffc107 !important; font-size: 0.85rem;">
                          <i class="bi bi-exclamation-triangle-fill text-warning mt-1"></i>
                          <div>
                            <strong>Not Available for Selected Dates</strong>
                            <div class="text-muted small mt-1">
                              These rooms are fully booked from
                              <strong>{{ formatDate(dates.checkIn) }}</strong> to
                              <strong>{{ formatDate(dates.checkOut) }}</strong>.
                              Please choose different dates or another room type.
                            </div>
                          </div>
                        </div>
                        <div *ngIf="avail.checked && avail.isAvailable"
                             class="alert alert-success border-0 rounded-0 py-2 px-3 mb-3 d-flex align-items-center gap-2"
                             style="background: rgba(25,135,84,0.08); border-left: 3px solid #198754 !important; font-size: 0.85rem;">
                          <i class="bi bi-check-circle-fill text-success"></i>
                          <strong>Rooms Available for Your Dates!</strong>
                        </div>
                      </div>

                      <!-- Rooms Count Selector + Book CTA -->
                      <div class="mt-auto pt-3 border-top">
                        <div class="row align-items-center g-3">
                          <div class="col-md-5">
                            <label class="form-label small text-muted text-uppercase fw-semibold mb-1" style="font-size: 0.68rem; letter-spacing: 0.05em;">
                              Number of Rooms (max 10)
                            </label>
                            <select class="form-select form-select-sm form-control-premium"
                                    [(ngModel)]="roomsCountMap[room.id]"
                                    (ngModelChange)="onRoomsCountChange(room.id)"
                                    [disabled]="room.status === 'Maintenance'">
                              <option *ngFor="let n of roomCountOptions" [value]="n">{{ n }} {{ n === 1 ? 'Room' : 'Rooms' }}</option>
                            </select>
                          </div>
                          <div class="col-md-7 d-flex gap-2 justify-content-md-end">
                            <button class="btn btn-premium-outline btn-sm py-2 px-3"
                                    (click)="checkAvailability(room.id)"
                                    [disabled]="room.status === 'Maintenance' || getAvailability(room.id)?.checking">
                              <span *ngIf="getAvailability(room.id)?.checking" class="spinner-border spinner-border-sm me-1" role="status"></span>
                              <i *ngIf="!getAvailability(room.id)?.checking" class="bi bi-calendar-check me-1"></i>
                              Check Availability
                            </button>
                            <button class="btn btn-premium btn-sm py-2 px-3"
                                    (click)="onBookRoom(room.id)"
                                    [disabled]="room.status === 'Maintenance' || isRoomUnavailable(room.id)">
                              <i class="bi bi-bookmark-check me-1"></i>Book Now
                            </button>
                          </div>
                        </div>
                        <!-- Price Preview -->
                        <div *ngIf="totalNights > 0 && roomsCountMap[room.id]" class="mt-2 small text-muted">
                          <i class="bi bi-calculator me-1" style="color: var(--primary-color);"></i>
                          Est. Total: <strong class="text-dark">₹{{ (room.pricePerNight * totalNights * roomsCountMap[room.id]) | number }}</strong>
                          ({{ roomsCountMap[room.id] }} room{{ roomsCountMap[room.id] > 1 ? 's' : '' }} × {{ totalNights }} night{{ totalNights > 1 ? 's' : '' }})
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Reviews Section (Airbnb-style) -->
            <div class="mt-5 pt-4 border-top">
              <div class="d-flex justify-content-between align-items-center mb-4">
                <div>
                  <h3 class="serif-font mb-1">Guest Reviews</h3>
                  <div class="d-flex align-items-center gap-2">
                    <div class="d-flex gap-1">
                      <i class="bi bi-star-fill" style="color: var(--primary-color); font-size: 0.9rem;" *ngFor="let s of [1,2,3,4]"></i>
                      <i class="bi bi-star-half" style="color: var(--primary-color); font-size: 0.9rem;"></i>
                    </div>
                    <span class="fw-bold text-dark">{{ hotel.rating }}</span>
                    <span class="text-muted small">({{ reviews.length }} reviews)</span>
                  </div>
                </div>
                <div class="text-end">
                  <!-- Review Summary Bars -->
                  <div class="d-flex flex-column gap-1" style="min-width: 160px;">
                    <div class="d-flex align-items-center gap-2 small">
                      <span class="text-muted" style="min-width:60px; text-align:right;">Cleanliness</span>
                      <div class="progress flex-grow-1" style="height:5px; background:#eee; border-radius:3px; min-width:80px;">
                        <div class="progress-bar" style="width:92%; background:var(--primary-color); border-radius:3px;"></div>
                      </div>
                      <span class="fw-semibold text-dark small">4.6</span>
                    </div>
                    <div class="d-flex align-items-center gap-2 small">
                      <span class="text-muted" style="min-width:60px; text-align:right;">Location</span>
                      <div class="progress flex-grow-1" style="height:5px; background:#eee; border-radius:3px; min-width:80px;">
                        <div class="progress-bar" style="width:88%; background:var(--primary-color); border-radius:3px;"></div>
                      </div>
                      <span class="fw-semibold text-dark small">4.4</span>
                    </div>
                    <div class="d-flex align-items-center gap-2 small">
                      <span class="text-muted" style="min-width:60px; text-align:right;">Value</span>
                      <div class="progress flex-grow-1" style="height:5px; background:#eee; border-radius:3px; min-width:80px;">
                        <div class="progress-bar" style="width:84%; background:var(--primary-color); border-radius:3px;"></div>
                      </div>
                      <span class="fw-semibold text-dark small">4.2</span>
                    </div>
                    <div class="d-flex align-items-center gap-2 small">
                      <span class="text-muted" style="min-width:60px; text-align:right;">Service</span>
                      <div class="progress flex-grow-1" style="height:5px; background:#eee; border-radius:3px; min-width:80px;">
                        <div class="progress-bar" style="width:90%; background:var(--primary-color); border-radius:3px;"></div>
                      </div>
                      <span class="fw-semibold text-dark small">4.5</span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Review Cards -->
              <div class="row g-4">
                <div class="col-md-6" *ngFor="let review of reviews">
                  <div class="border p-4 h-100" style="border-radius: 0; background: #fafafa;">
                    <div class="d-flex align-items-center gap-3 mb-3">
                      <div class="rounded-circle d-flex align-items-center justify-content-center fw-bold text-white"
                           [style.background]="review.avatar"
                           style="width:44px; height:44px; font-size:1.1rem; flex-shrink:0;">
                        {{ review.name[0] }}
                      </div>
                      <div>
                        <div class="fw-bold text-dark small">{{ review.name }}</div>
                        <div class="text-muted" style="font-size: 0.75rem;">
                          <i class="bi bi-geo-alt me-1"></i>{{ review.location }}
                        </div>
                      </div>
                      <div class="ms-auto text-end">
                        <div class="d-flex gap-1">
                          <i class="bi" [ngClass]="s <= review.rating ? 'bi-star-fill' : 'bi-star'"
                             style="color: var(--primary-color); font-size: 0.75rem;"
                             *ngFor="let s of [1,2,3,4,5]"></i>
                        </div>
                        <div class="text-muted" style="font-size: 0.7rem;">{{ review.date }}</div>
                      </div>
                    </div>
                    <p class="text-muted small mb-0" style="line-height: 1.7; font-style: italic;">"{{ review.text }}"</p>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Right Booking Sidebar -->
          <div class="col-lg-4">
            <div class="bg-white border p-4 shadow-sm position-sticky" style="top: 100px;">
              <h5 class="serif-font mb-4 border-bottom pb-2">Select Your Stay</h5>

              <div class="mb-3">
                <label class="form-label small text-muted text-uppercase fw-semibold" style="font-size: 0.7rem; letter-spacing: 0.05em;">Check-In Date</label>
                <input type="date" class="form-control form-control-premium" [(ngModel)]="dates.checkIn" [min]="minDate" (ngModelChange)="onDatesChange()">
              </div>

              <div class="mb-4">
                <label class="form-label small text-muted text-uppercase fw-semibold" style="font-size: 0.7rem; letter-spacing: 0.05em;">Check-Out Date</label>
                <input type="date" class="form-control form-control-premium" [(ngModel)]="dates.checkOut" [min]="minDate" (ngModelChange)="onDatesChange()">
              </div>

              <!-- Stay Duration Preview -->
              <div *ngIf="totalNights > 0" class="p-3 mb-4 text-center" style="background: var(--bg-cream); border-left: 3px solid var(--primary-color);">
                <span class="small text-muted text-uppercase fw-semibold d-block mb-1" style="font-size: 0.7rem; letter-spacing: 0.06em;">Stay Duration</span>
                <span class="serif-font fw-bold fs-5 text-dark">{{ totalNights }} Night{{ totalNights > 1 ? 's' : '' }}</span>
                <div class="small text-muted mt-1">{{ formatDate(dates.checkIn) }} → {{ formatDate(dates.checkOut) }}</div>
              </div>

              <div class="p-3 bg-light text-center small text-muted border-start border-3 border-dark">
                <i class="bi bi-info-circle me-1"></i>
                Standard check-in: <strong>12:00 PM</strong><br>
                Standard check-out: <strong>11:00 AM</strong>
              </div>

              <!-- Quick Check All Availability -->
              <button class="btn btn-premium-outline w-100 mt-3 py-2" (click)="checkAllRoomsAvailability()" [disabled]="!dates.checkIn || !dates.checkOut || checkingAll">
                <span *ngIf="checkingAll" class="spinner-border spinner-border-sm me-2"></span>
                <i *ngIf="!checkingAll" class="bi bi-calendar3 me-2"></i>
                {{ checkingAll ? 'Checking...' : 'Check All Room Availability' }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .tracking-wider { letter-spacing: 0.15em; }
    .progress { overflow: hidden; }
  `]
})
export class HotelDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private hotelService = inject(HotelService);
  private bookingService = inject(BookingService);
  private authService = inject(AuthService);

  hotel: any = null;
  loading = true;
  minDate = '';
  roomQuery = '';
  checkingAll = false;

  dates = { checkIn: '', checkOut: '' };
  totalNights = 0;

  roomCountOptions = [1,2,3,4,5,6,7,8,9,10];
  roomsCountMap: { [roomId: number]: number } = {};

  // Availability info per room
  availabilityMap: { [roomId: number]: RoomAvailabilityInfo } = {};

  // Reviews
  reviews: Review[] = [
    {
      name: 'Priya Sharma',
      avatar: 'linear-gradient(135deg, #c5a880, #a0836a)',
      date: 'April 2026',
      rating: 5,
      text: 'Absolutely loved my stay! The rooms were immaculate and the staff went above and beyond. The location in Bangalore is perfect for both business and leisure.',
      location: 'Mumbai, Maharashtra'
    },
    {
      name: 'Rahul Verma',
      avatar: 'linear-gradient(135deg, #6c8ebf, #4a6fa5)',
      date: 'March 2026',
      rating: 4,
      text: 'Great value for money. The Super Deluxe room was spacious and well-appointed. The complimentary breakfast was delicious. Would definitely return!',
      location: 'Delhi, NCR'
    },
    {
      name: 'Ananya Krishnan',
      avatar: 'linear-gradient(135deg, #82b366, #5a8c3e)',
      date: 'March 2026',
      rating: 5,
      text: 'The AC room was superb — perfectly cooled, extremely clean, and the king bed was the most comfortable I\'ve slept on in any hotel in Bangalore.',
      location: 'Chennai, Tamil Nadu'
    },
    {
      name: 'Vikram Patel',
      avatar: 'linear-gradient(135deg, #d94f3d, #b03829)',
      date: 'February 2026',
      rating: 4,
      text: 'Ideal for corporate trips. High-speed Wi-Fi, excellent workstation setup in the room, and quick room service. The gym was also well-maintained.',
      location: 'Pune, Maharashtra'
    },
    {
      name: 'Meera Nair',
      avatar: 'linear-gradient(135deg, #9b59b6, #7d3f98)',
      date: 'February 2026',
      rating: 5,
      text: 'One of the best hotel experiences I\'ve had! The loyalty points system is a fantastic touch — redeemed mine on this visit and got a great discount.',
      location: 'Hyderabad, Telangana'
    },
    {
      name: 'Aditya Bose',
      avatar: 'linear-gradient(135deg, #e67e22, #d35400)',
      date: 'January 2026',
      rating: 4,
      text: 'Smooth booking process via AuraStay. The confirmation email was detailed and professional. Check-in was seamless — staff recognized our reservation immediately.',
      location: 'Kolkata, West Bengal'
    }
  ];

  getAvailability(roomId: number): RoomAvailabilityInfo | undefined {
    return this.availabilityMap[roomId];
  }

  getFilteredRooms(): any[] {
    if (!this.hotel || !this.hotel.rooms) return [];
    if (!this.roomQuery.trim()) return this.hotel.rooms;
    const q = this.roomQuery.toLowerCase().trim();
    return this.hotel.rooms.filter((r: any) => {
      return r.roomType.toLowerCase().includes(q) ||
             r.description.toLowerCase().includes(q) ||
             (q === 'normal' && r.roomType.toLowerCase().includes('normal')) ||
             (q === 'ac' && r.roomType.toLowerCase().includes('ac')) ||
             (q === 'deluxe' && r.roomType.toLowerCase().includes('deluxe'));
    });
  }

  onRoomQueryChange(): void {
    // Reset availability checks when filter changes
  }

  isRoomUnavailable(roomId: number): boolean {
    const avail = this.availabilityMap[roomId];
    if (!avail) return false;
    return avail.checked && !avail.isAvailable;
  }

  ngOnInit(): void {
    const hotelId = Number(this.route.snapshot.paramMap.get('id'));

    this.route.queryParams.subscribe(params => {
      const today = new Date();
      this.minDate = today.toISOString().split('T')[0];
      const tomorrow = new Date(today);
      tomorrow.setDate(today.getDate() + 1);
      const dayAfter = new Date(tomorrow);
      dayAfter.setDate(tomorrow.getDate() + 1);

      this.dates.checkIn = params['checkIn'] || tomorrow.toISOString().split('T')[0];
      this.dates.checkOut = params['checkOut'] || dayAfter.toISOString().split('T')[0];
      this.calculateNights();
    });

    this.loadHotelDetails(hotelId);
  }

  calculateNights(): void {
    if (!this.dates.checkIn || !this.dates.checkOut) { this.totalNights = 0; return; }
    const start = new Date(this.dates.checkIn);
    const end = new Date(this.dates.checkOut);
    this.totalNights = Math.max(0, Math.ceil((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24)));
  }

  loadHotelDetails(id: number): void {
    this.loading = true;
    this.hotelService.getById(id).subscribe({
      next: (data: any) => {
        this.hotel = data;
        // Initialize roomsCount to 1 for each room
        if (data.rooms) {
          data.rooms.forEach((r: any) => {
            if (!this.roomsCountMap[r.id]) this.roomsCountMap[r.id] = 1;
          });
        }
        this.loading = false;
        // Auto-check availability after loading hotel
        setTimeout(() => this.checkAllRoomsAvailability(), 400);
      },
      error: () => {
        this.loading = false;
        this.router.navigate(['/hotels']);
      }
    });
  }

  onDatesChange(): void {
    this.calculateNights();
    // Reset all availability checks when dates change
    this.availabilityMap = {};
    // Auto re-check
    if (this.dates.checkIn && this.dates.checkOut && this.totalNights > 0) {
      setTimeout(() => this.checkAllRoomsAvailability(), 300);
    }
  }

  onRoomsCountChange(roomId: number): void {
    // Reset availability when count changes — must re-check
    if (this.availabilityMap[roomId]) {
      this.availabilityMap[roomId].checked = false;
      this.availabilityMap[roomId].isAvailable = false;
    }
  }

  checkAvailability(roomId: number): void {
    if (!this.dates.checkIn || !this.dates.checkOut) return;
    const count = this.roomsCountMap[roomId] || 1;

    this.availabilityMap[roomId] = {
      roomId,
      isAvailable: false,
      checked: false,
      checking: true
    };

    this.bookingService.checkAvailability(roomId, this.dates.checkIn, this.dates.checkOut, count).subscribe({
      next: (res) => {
        this.availabilityMap[roomId] = {
          roomId,
          isAvailable: res.isAvailable,
          checked: true,
          checking: false
        };
      },
      error: () => {
        this.availabilityMap[roomId] = {
          roomId,
          isAvailable: false,
          checked: true,
          checking: false
        };
      }
    });
  }

  checkAllRoomsAvailability(): void {
    if (!this.hotel?.rooms || !this.dates.checkIn || !this.dates.checkOut || this.totalNights <= 0) return;
    this.checkingAll = true;
    const rooms = this.hotel.rooms.filter((r: any) => r.status !== 'Unavailable' && r.status !== 'Maintenance');
    let remaining = rooms.length;
    if (remaining === 0) { this.checkingAll = false; return; }

    rooms.forEach((room: any) => {
      const count = this.roomsCountMap[room.id] || 1;
      this.availabilityMap[room.id] = { roomId: room.id, isAvailable: false, checked: false, checking: true };
      this.bookingService.checkAvailability(room.id, this.dates.checkIn, this.dates.checkOut, count).subscribe({
        next: (res) => {
          this.availabilityMap[room.id] = { roomId: room.id, isAvailable: res.isAvailable, checked: true, checking: false };
          remaining--;
          if (remaining === 0) this.checkingAll = false;
        },
        error: () => {
          this.availabilityMap[room.id] = { roomId: room.id, isAvailable: false, checked: true, checking: false };
          remaining--;
          if (remaining === 0) this.checkingAll = false;
        }
      });
    });
  }

  onBookRoom(roomId: number): void {
    if (!this.authService.isLoggedIn) {
      this.router.navigate(['/login'], {
        queryParams: {
          returnUrl: `/booking?roomId=${roomId}&checkIn=${this.dates.checkIn}&checkOut=${this.dates.checkOut}&roomsCount=${this.roomsCountMap[roomId] || 1}`
        }
      });
      return;
    }

    this.router.navigate(['/booking'], {
      queryParams: {
        roomId,
        checkIn: this.dates.checkIn,
        checkOut: this.dates.checkOut,
        roomsCount: this.roomsCountMap[roomId] || 1
      }
    });
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '';
    return new Date(dateStr).toLocaleDateString('en-IN', { day: 'numeric', month: 'short', year: 'numeric', timeZone: 'UTC' });
  }
}
