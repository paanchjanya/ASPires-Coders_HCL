import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { BookingService } from '../../services/booking.service';
import { BookingResponse } from '../../models/booking.model';

@Component({
  selector: 'app-my-bookings',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="container my-5 fade-in">
      <div class="row mb-5">
        <div class="col-12">
          <span class="text-muted text-uppercase fw-semibold tracking-wider font-monospace" style="font-size: 0.8rem; letter-spacing: 0.15em;">Stays Ledger</span>
          <h1 class="serif-font display-6 mt-1 mb-0">My Bookings</h1>
          <p class="text-muted mt-2">Manage your active, completed, or cancelled staying reservations.</p>
        </div>
      </div>

      <div *ngIf="loading" class="text-center py-5">
        <div class="spinner-border text-dark" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
      </div>

      <div *ngIf="!loading && bookings.length === 0" class="text-center py-5 bg-white border border-light shadow-sm">
        <i class="bi bi-calendar-x text-muted fs-1 mb-3 d-block"></i>
        <h4 class="serif-font">No Reservations Found</h4>
        <p class="text-muted small">You haven't made any bookings yet.</p>
        <a routerLink="/" class="btn btn-premium btn-sm mt-3">Browse Hotels</a>
      </div>

      <div *ngIf="!loading && bookings.length > 0" class="row g-4">
        <div class="col-12" *ngFor="let booking of bookings">
          <div class="card bg-white border border-light p-4 p-md-5 shadow-sm">
            <div class="row align-items-center">
              <div class="col-lg-8">
                <!-- Status Badge -->
                <div class="d-flex align-items-center mb-3">
                  <span class="badge text-uppercase tracking-wider" [ngClass]="booking.status === 'Confirmed' ? 'bg-success-subtle text-success border border-success' : 'bg-danger-subtle text-danger border border-danger'" style="border-radius: 0; padding: 0.35em 0.8em; font-weight: 600; font-size: 0.72rem;">
                    {{ booking.status }}
                  </span>
                  <span class="text-muted small ms-3">Reservation Number: <strong class="text-dark font-monospace">{{ booking.reservationNumber }}</strong></span>
                </div>

                <h3 class="serif-font mb-2 fs-4" style="color: var(--text-dark); font-weight: 400;">{{ booking.hotelName }}</h3>
                <p class="text-muted small mb-3">{{ booking.roomType }} &bull; {{ booking.hotelLocation }}, Bangalore</p>

                <div class="row text-muted small">
                  <div class="col-md-6 mb-2 mb-md-0">
                    <span class="d-block text-uppercase fw-semibold" style="font-size: 0.7rem; letter-spacing: 0.05em;">Check In</span>
                    <span class="fw-semibold text-dark fs-6">{{ formatDate(booking.checkInDate) }}</span>
                  </div>
                  <div class="col-md-6">
                    <span class="d-block text-uppercase fw-semibold" style="font-size: 0.7rem; letter-spacing: 0.05em;">Check Out</span>
                    <span class="fw-semibold text-dark fs-6">{{ formatDate(booking.checkOutDate) }}</span>
                  </div>
                </div>
              </div>

              <div class="col-lg-4 text-lg-end mt-4 mt-lg-0 pt-3 pt-lg-0 border-top border-lg-top-0 d-flex flex-column align-items-lg-end justify-content-between h-100">
                <div class="mb-3">
                  <span class="text-muted small d-block" style="font-size: 0.75rem;">Total Paid</span>
                  <span class="fs-4 fw-bold serif-font text-dark">₹{{ booking.totalPrice | number }}</span>
                </div>

                <div class="d-flex gap-2 w-100 justify-content-lg-end flex-wrap">
                  <button class="btn btn-premium-outline btn-sm py-2 px-3" (click)="viewEmailLog(booking.id)">
                    <i class="bi bi-envelope-open me-1"></i> Read Email
                  </button>
                  <button *ngIf="booking.status === 'Confirmed'" class="btn btn-outline-danger btn-sm py-2 px-3 rounded-0" (click)="cancelBooking(booking.id)" [disabled]="cancellingId === booking.id">
                    <span *ngIf="cancellingId === booking.id" class="spinner-border spinner-border-sm me-1" role="status"></span>
                    Cancel Stay
                  </button>
                  <button class="btn btn-premium btn-sm py-2 px-3" (click)="quickRebook(booking)">
                    Quick Rebook
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Email Log Modal Overlay -->
    <div class="modal fade show d-block" *ngIf="selectedEmailLog" tabindex="-1" style="background: rgba(0,0,0,0.55); backdrop-filter: blur(5px); z-index: 1050;">
      <div class="modal-dialog modal-dialog-centered modal-lg">
        <div class="modal-content border-0 shadow-lg" style="border-radius: 0;">
          <div class="modal-header bg-dark text-white border-0 py-3" style="border-radius: 0;">
            <h5 class="modal-title serif-font"><i class="bi bi-envelope-open me-2 text-warning"></i> Simulated Confirmation Email</h5>
            <button type="button" class="btn-close btn-close-white" (click)="selectedEmailLog = null"></button>
          </div>
          <div class="modal-body p-4" style="background: #fdfdfd; max-height: 70vh; overflow-y: auto;">
            <div class="mb-3 border-bottom pb-2" style="font-size: 0.85rem;">
              <p class="mb-1 text-muted"><strong>From:</strong> concierge&#64;aurastay.com (Concierge AuraStay)</p>
              <p class="mb-1 text-muted"><strong>To:</strong> {{ selectedEmailLog.recipientEmail }}</p>
              <p class="mb-1 text-muted"><strong>Subject:</strong> {{ selectedEmailLog.subject }}</p>
              <p class="mb-0 text-muted"><strong>Sent At:</strong> {{ formatDateLong(selectedEmailLog.sentAt) }}</p>
            </div>
            <!-- Sanitize and render HTML body -->
            <div class="email-container border p-4 bg-white shadow-sm" [innerHTML]="getSafeHtml(selectedEmailLog.body)"></div>
          </div>
          <div class="modal-footer border-0 bg-light py-2" style="border-radius: 0;">
            <button type="button" class="btn btn-premium btn-sm px-4" (click)="selectedEmailLog = null">Close Inbox</button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [
    `
      .bg-success-subtle {
        background-color: rgba(25, 135, 84, 0.08) !important;
        color: #198754 !important;
        border: 1px solid rgba(25, 135, 84, 0.2) !important;
      }
      .bg-danger-subtle {
        background-color: rgba(220, 53, 69, 0.08) !important;
        color: #dc3545 !important;
        border: 1px solid rgba(220, 53, 69, 0.2) !important;
      }
    `
  ]
})
export class MyBookingsComponent implements OnInit {
  private bookingService = inject(BookingService);
  private router = inject(Router);
  private sanitizer = inject(DomSanitizer);

  bookings: BookingResponse[] = [];
  loading = true;
  cancellingId: number | null = null;
  selectedEmailLog: any = null;

  ngOnInit(): void {
    this.loadBookings();
  }

  loadBookings(): void {
    this.loading = true;
    this.bookingService.getMyBookings().subscribe({
      next: (data) => {
        this.bookings = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  cancelBooking(id: number): void {
    if (!confirm('Are you sure you want to cancel this reservation?')) return;
    this.cancellingId = id;

    this.bookingService.cancelBooking(id).subscribe({
      next: () => {
        this.cancellingId = null;
        this.loadBookings();
      },
      error: (err) => {
        this.cancellingId = null;
        alert(err.error?.message || 'Failed to cancel booking.');
      }
    });
  }

  quickRebook(booking: BookingResponse): void {
    const today = new Date();
    const tomorrow = new Date(today);
    tomorrow.setDate(today.getDate() + 1);
    const dayAfter = new Date(tomorrow);
    dayAfter.setDate(tomorrow.getDate() + 1);

    const checkIn = tomorrow.toISOString().split('T')[0];
    const checkOut = dayAfter.toISOString().split('T')[0];

    this.router.navigate(['/booking'], {
      queryParams: {
        roomId: booking.roomId,
        checkIn,
        checkOut
      }
    });
  }

  viewEmailLog(bookingId: number): void {
    this.bookingService.getEmailLog(bookingId).subscribe({
      next: (log) => {
        this.selectedEmailLog = log;
      },
      error: (err) => {
        alert(err.error?.message || 'Email log not found for this reservation.');
      }
    });
  }

  getSafeHtml(htmlStr: string): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(htmlStr);
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-IN', {
      day: 'numeric',
      month: 'short',
      year: 'numeric'
    });
  }

  formatDateLong(dateStr: string): string {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    return date.toLocaleString('en-IN', {
      day: 'numeric',
      month: 'long',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
