import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';

@Component({
  selector: 'app-booking-confirmation',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="container my-5 fade-in text-center">
      <div class="row justify-content-center">
        <div class="col-md-6 col-lg-5">
          <div class="bg-white border p-4 p-md-5 shadow-sm">
            <div class="mb-4">
              <div class="success-icon mx-auto d-flex align-items-center justify-content-center bg-light rounded-circle" style="width: 80px; height: 80px; color: var(--primary-color);">
                <i class="bi bi-patch-check-fill display-4"></i>
              </div>
            </div>

            <h2 class="serif-font mb-2">Booking Confirmed</h2>
            <p class="text-muted small">Thank you for choosing AuraStay. Your reservation is complete.</p>

            <!-- System Confirmation Message Banner -->
            <div class="alert alert-success border-0 rounded-0 text-start my-4 p-3 shadow-sm" style="background-color: rgba(40, 167, 69, 0.08); border-left: 4px solid #28a745 !important; font-size: 0.92rem; line-height: 1.6;">
              <div class="d-flex gap-3">
                <div class="fs-5" style="color: #28a745;"><i class="bi bi-check-circle-fill"></i></div>
                <div>
                  Room <strong>#{{ 100 + roomId }}</strong> ({{ roomsCount > 1 ? roomsCount + ' rooms' : '1 room' }}) has been reserved for
                  <strong>{{ username }}</strong>.<br>
                  <strong>Check-In:</strong> {{ formatDate(checkIn) }} at <strong>12:00 PM</strong><br>
                  <strong>Check-Out:</strong> {{ formatDate(checkOut) }} at <strong>11:00 AM</strong>
                </div>
              </div>
            </div>

            <!-- Details Panel -->
            <div class="border my-4 p-4 text-start bg-light">
              <div class="mb-3 text-center border-bottom pb-3">
                <span class="text-muted small text-uppercase fw-semibold d-block mb-1" style="font-size: 0.7rem; letter-spacing: 0.1em;">Reservation Number</span>
                <span class="fs-4 fw-bold text-dark serif-font" style="letter-spacing: 0.05em; color: var(--primary-color);">{{ resNum }}</span>
              </div>

              <div class="mb-2">
                <span class="text-muted small d-block" style="font-size: 0.75rem;">Property</span>
                <span class="fw-bold text-dark">{{ hotelName }}</span>
              </div>

              <div class="mb-2">
                <span class="text-muted small d-block" style="font-size: 0.75rem;">Room Type</span>
                <span class="fw-semibold text-dark">{{ roomType }}</span>
              </div>

              <div class="mb-2">
                <span class="text-muted small d-block" style="font-size: 0.75rem;">Rooms Booked</span>
                <span class="fw-semibold text-dark">{{ roomsCount }} Room{{ roomsCount > 1 ? 's' : '' }}</span>
              </div>

              <div class="mb-2 row">
                <div class="col-6">
                  <span class="text-muted small d-block" style="font-size: 0.75rem;">Check In</span>
                  <span class="fw-semibold text-dark" style="font-size: 0.9rem;">{{ formatDate(checkIn) }}</span>
                </div>
                <div class="col-6">
                  <span class="text-muted small d-block" style="font-size: 0.75rem;">Check Out</span>
                  <span class="fw-semibold text-dark" style="font-size: 0.9rem;">{{ formatDate(checkOut) }}</span>
                </div>
              </div>

              <div class="mt-3 pt-2 border-top d-flex justify-content-between align-items-center">
                <span class="fw-semibold text-muted small">Amount Paid</span>
                <span class="fs-5 fw-bold serif-font text-dark">₹{{ price }}</span>
              </div>
            </div>

            <!-- Action buttons -->
            <div class="d-flex gap-3">
              <a routerLink="/my-bookings" class="btn btn-premium w-100 py-3 btn-sm">My Bookings</a>
              <a routerLink="/" class="btn btn-premium-outline w-100 py-3 btn-sm">Back Home</a>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class BookingConfirmationComponent implements OnInit {
  private route = inject(ActivatedRoute);

  hotelName = '';
  roomType = '';
  checkIn = '';
  checkOut = '';
  price = '';
  resNum = '';
  username = '';
  roomId = 0;
  roomsCount = 1;

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.hotelName = params['hotelName'] || '';
      this.roomType = params['roomType'] || '';
      this.checkIn = params['checkIn'] || '';
      this.checkOut = params['checkOut'] || '';
      this.price = params['price'] || '';
      this.resNum = params['resNum'] || '';
      this.username = params['username'] || '';
      this.roomId = Number(params['roomId']) || 0;
      this.roomsCount = Number(params['roomsCount']) || 1;
    });
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric', timeZone: 'UTC' });
  }
}
