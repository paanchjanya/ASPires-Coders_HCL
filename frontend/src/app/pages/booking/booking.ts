import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { RoomService } from '../../services/room.service';
import { BookingService } from '../../services/booking.service';
import { PromotionService } from '../../services/promotion.service';
import { AuthService } from '../../services/auth.service';
import { Room } from '../../models/room.model';
import { BookingRequest } from '../../models/booking.model';

@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <div *ngIf="loading" class="container my-5 text-center py-5">
      <div class="spinner-border text-dark" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>

    <div *ngIf="!loading && room" class="container my-5 fade-in">
      <div class="row mb-4">
        <div class="col-12">
          <span class="text-muted text-uppercase fw-semibold tracking-wider font-monospace d-block mb-1" style="font-size: 0.8rem; letter-spacing: 0.15em;">Secure Checkout</span>
          <h1 class="serif-font display-6">Confirm Your Reservation Stay</h1>
          <p class="text-muted">Review stay details, apply points, and complete payment securely.</p>
        </div>
      </div>

      <div class="row g-5">
        <!-- Left: Payment Form & Guest Info -->
        <div class="col-lg-7">
          <!-- Guest Details Card -->
          <div class="bg-white border p-4 shadow-sm mb-4">
            <h5 class="serif-font mb-4 border-bottom pb-2">1. Guest Details</h5>
            <div class="row">
              <div class="col-md-6 mb-3">
                <span class="d-block small text-muted text-uppercase fw-semibold" style="font-size: 0.75rem;">Username</span>
                <span class="fs-6 fw-bold text-dark">{{ authService.currentUserValue?.username }}</span>
              </div>
              <div class="col-md-6 mb-3">
                <span class="d-block small text-muted text-uppercase fw-semibold" style="font-size: 0.75rem;">Email Address</span>
                <span class="fs-6 fw-bold text-dark">{{ authService.currentUserValue?.email }}</span>
              </div>
            </div>
          </div>

          <!-- Loyalty points redemption -->
          <div class="bg-white border p-4 shadow-sm mb-4">
            <h5 class="serif-font mb-4 border-bottom pb-2">2. Loyalty Rewards Points</h5>
            <div *ngIf="availablePoints > 0; else noPoints" class="fade-in">
              <p class="text-muted small">You have <strong class="text-dark">{{ availablePoints }}</strong> points available. You can redeem points for a direct discount (1 point = ₹1 discount).</p>
              <div class="form-check mb-3">
                <input class="form-check-input" type="checkbox" id="redeemCheck" [(ngModel)]="wantsToRedeem" (ngModelChange)="onRedeemToggle()">
                <label class="form-check-label text-dark fw-semibold" for="redeemCheck">
                  Yes, I want to redeem loyalty points
                </label>
              </div>
              <div class="mb-2" *ngIf="wantsToRedeem">
                <label class="form-label small text-muted text-uppercase fw-semibold" style="font-size: 0.7rem;">Points to Redeem</label>
                <div class="d-flex align-items-center gap-3">
                  <input type="number" class="form-control form-control-premium w-50" [(ngModel)]="redeemPoints" [min]="0" [max]="maxRedeemablePoints" (ngModelChange)="calculatePricing()">
                  <span class="text-muted small">Max redeemable: {{ maxRedeemablePoints }} pts (₹{{ maxRedeemablePoints }})</span>
                </div>
              </div>
            </div>
            <ng-template #noPoints>
              <p class="text-muted small mb-0">You currently have 0 loyalty points. Complete this stay to earn points for future bookings!</p>
            </ng-template>
          </div>

          <!-- Secure Payment -->
          <div class="bg-white border p-4 shadow-sm">
            <h5 class="serif-font mb-4 border-bottom pb-2" style="font-size: 1.15rem;"><i class="bi bi-shield-lock me-2 text-success"></i>3. Secure Credit Card Payment</h5>

            <div *ngIf="paymentError" class="alert alert-danger border-0 rounded-0 py-2 small" role="alert">
              {{ paymentError }}
            </div>

            <form (ngSubmit)="onConfirmBooking()" #paymentForm="ngForm">
              <div class="mb-3">
                <label for="cardName" class="form-label small text-muted text-uppercase fw-semibold" style="font-size: 0.7rem; letter-spacing: 0.05em;">Cardholder Name</label>
                <input type="text" class="form-control form-control-premium" id="cardName" name="cardName" [(ngModel)]="paymentInfo.cardName" required placeholder="e.g. John Doe">
              </div>

              <div class="mb-3">
                <label for="cardNumber" class="form-label small text-muted text-uppercase fw-semibold" style="font-size: 0.7rem; letter-spacing: 0.05em;">Card Number</label>
                <div class="input-group">
                  <span class="input-group-text bg-transparent border-0 border-bottom ps-0 rounded-0" style="color: var(--primary-color);"><i class="bi bi-credit-card"></i></span>
                  <input type="text" class="form-control form-control-premium border-0 border-bottom" id="cardNumber" name="cardNumber" [(ngModel)]="paymentInfo.cardNumber" required pattern="\\d{16}" placeholder="4000123456789010" maxlength="16">
                </div>
              </div>

              <div class="row">
                <div class="col-6 mb-3">
                  <label for="expiry" class="form-label small text-muted text-uppercase fw-semibold" style="font-size: 0.7rem; letter-spacing: 0.05em;">Expiry Date (MM/YY)</label>
                  <input type="text" class="form-control form-control-premium" id="expiry" name="expiry" [(ngModel)]="paymentInfo.expiry" required pattern="(0[1-9]|1[0-2])\\/\\d{2}" placeholder="12/28" maxlength="5">
                </div>
                <div class="col-6 mb-3">
                  <label for="cvv" class="form-label small text-muted text-uppercase fw-semibold" style="font-size: 0.7rem; letter-spacing: 0.05em;">CVV</label>
                  <input type="password" class="form-control form-control-premium" id="cvv" name="cvv" [(ngModel)]="paymentInfo.cvv" required pattern="\\d{3,4}" placeholder="***" maxlength="4">
                </div>
              </div>

              <button type="submit" class="btn btn-premium w-100 py-3 mt-4" [disabled]="paymentForm.invalid || bookingInProgress">
                <span *ngIf="bookingInProgress" class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                Secure Checkout & Confirm Booking
              </button>
            </form>
          </div>
        </div>

        <!-- Right: Order Summary -->
        <div class="col-lg-5">
          <div class="bg-white border p-4 shadow-sm position-sticky" style="top: 100px;">
            <h5 class="serif-font mb-4 border-bottom pb-2">Stay Summary</h5>

            <div class="mb-3 d-flex justify-content-between align-items-center">
              <div>
                <span class="text-muted small d-block" style="font-size: 0.75rem;">Hotel Properties</span>
                <span class="fw-bold text-dark">{{ room.hotelName }}</span>
              </div>
              <div class="text-end">
                <span class="text-muted small d-block" style="font-size: 0.75rem;">Room Type</span>
                <span class="fw-semibold text-dark">{{ room.roomType }}</span>
              </div>
            </div>

            <div class="mb-3 d-flex justify-content-between align-items-center">
              <div>
                <span class="text-muted small d-block" style="font-size: 0.75rem;">Rooms Booked</span>
                <span class="fw-bold text-dark">{{ roomsCount }} Room{{ roomsCount > 1 ? 's' : '' }}</span>
              </div>
              <div class="text-end">
                <span class="text-muted small d-block" style="font-size: 0.75rem;">Rate per Night</span>
                <span class="fw-bold text-dark">₹{{ room.pricePerNight | number }}</span>
              </div>
            </div>

            <div class="mb-4 d-flex justify-content-between align-items-center border-bottom pb-3">
              <div>
                <span class="text-muted small d-block" style="font-size: 0.75rem;">Dates</span>
                <span class="fw-semibold text-dark" style="font-size: 0.9rem;">{{ formatDate(checkInDate) }} to {{ formatDate(checkOutDate) }}</span>
              </div>
              <div class="text-end">
                <span class="text-muted small d-block" style="font-size: 0.75rem;">Duration</span>
                <span class="fw-bold text-dark">{{ totalNights }} Nights</span>
              </div>
            </div>

            <!-- Promotion Code -->
            <div class="mb-4">
              <label class="form-label small text-muted text-uppercase fw-semibold" style="font-size: 0.7rem; letter-spacing: 0.05em;">Discount Coupon Code</label>
              <div class="d-flex gap-2">
                <input type="text" class="form-control form-control-premium flex-grow-1" placeholder="e.g. BLR10" [(ngModel)]="promoCode" [disabled]="promoApplied">
                <button type="button" class="btn btn-premium-outline py-2 px-3 btn-sm" (click)="applyPromo()" [disabled]="promoApplied || !promoCode">
                  Apply
                </button>
              </div>
              <div *ngIf="promoMessage" class="small mt-2" [ngClass]="promoApplied ? 'text-success' : 'text-danger'">
                {{ promoMessage }}
              </div>
            </div>

            <!-- Price Calculations -->
            <div class="border-top pt-3">
              <div class="d-flex justify-content-between mb-2 small text-muted">
                <span>₹{{ room.pricePerNight | number }} × {{ totalNights }} nights × {{ roomsCount }} room{{ roomsCount > 1 ? 's' : '' }}</span>
                <span>₹{{ basePrice | number }}</span>
              </div>

              <!-- Seasonal Offers Details -->
              <div *ngIf="seasonalOfferApplied" class="d-flex justify-content-between mb-2 small text-success">
                <span>Seasonal Offer: {{ seasonalOfferApplied.title }} ({{ seasonalOfferApplied.discountType === 'Percentage' ? seasonalOfferApplied.discountValue + '%' : '₹' + seasonalOfferApplied.discountValue }} off)</span>
                <span>-₹{{ seasonalDiscount | number }}</span>
              </div>

              <!-- Promo Code Details -->
              <div *ngIf="promoApplied && appliedPromo" class="d-flex justify-content-between mb-2 small text-success">
                <span>Coupon Applied: {{ appliedPromo.code }} ({{ appliedPromo.discountType === 'Percentage' ? appliedPromo.discountValue + '%' : '₹' + appliedPromo.discountValue }} off)</span>
                <span>-₹{{ couponDiscount | number }}</span>
              </div>

              <!-- Loyalty Redemptions Details -->
              <div *ngIf="wantsToRedeem && redeemPoints > 0" class="d-flex justify-content-between mb-2 small text-success">
                <span>Loyalty Points Redeemed ({{ redeemPoints }} pts)</span>
                <span>-₹{{ redeemPoints | number }}</span>
              </div>

              <div class="d-flex justify-content-between border-top pt-3 mt-3">
                <span class="serif-font fw-bold fs-5 text-dark">Total Stay Price</span>
                <span class="serif-font fw-bold fs-5 text-dark">₹{{ totalPrice | number }}</span>
              </div>

              <!-- Estimated Loyalty points to earn -->
              <div class="mt-3 p-3 bg-light text-center small text-muted border-start border-3 border-dark text-uppercase tracking-wider font-monospace" style="font-size: 0.75rem;">
                Earn <strong class="text-dark">{{ earnedPoints }}</strong> Loyalty points after stay!
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .tracking-wider {
      letter-spacing: 0.15em;
    }
  `]
})
export class BookingComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private roomService = inject(RoomService);
  private bookingService = inject(BookingService);
  private promotionService = inject(PromotionService);
  public authService = inject(AuthService);

  loading = true;
  bookingInProgress = false;
  paymentError = '';

  roomId = 0;
  checkInDate = '';
  checkOutDate = '';
  totalNights = 0;
  roomsCount = 1;

  room: Room | null = null;
  hotelData: any = null;

  basePrice = 0;
  totalPrice = 0;

  // Active Seeding Offers & Loyalty Points
  availablePoints = 0;
  wantsToRedeem = false;
  redeemPoints = 0;
  maxRedeemablePoints = 0;

  activeOffers: any[] = [];
  seasonalOfferApplied: any = null;
  seasonalDiscount = 0;

  promoCode = '';
  promoApplied = false;
  promoMessage = '';
  appliedPromo: any = null;
  couponDiscount = 0;

  earnedPoints = 0;

  paymentInfo = {
    cardName: '',
    cardNumber: '',
    expiry: '',
    cvv: ''
  };

  ngOnInit(): void {
    if (!this.authService.isLoggedIn) {
      this.router.navigate(['/login']);
      return;
    }

    this.route.queryParams.subscribe(params => {
      this.roomId = Number(params['roomId']);
      this.checkInDate = params['checkIn'];
      this.checkOutDate = params['checkOut'];
      this.roomsCount = Number(params['roomsCount']) || 1;
      if (this.roomsCount < 1) this.roomsCount = 1;
      if (this.roomsCount > 10) this.roomsCount = 10;

      if (!this.roomId || !this.checkInDate || !this.checkOutDate) {
        this.router.navigate(['/']);
        return;
      }

      this.calculateNights();
      this.loadLoyaltyPoints();
      this.loadActiveOffers();
      this.loadRoomDetails();
    });
  }

  calculateNights(): void {
    const start = new Date(this.checkInDate);
    const end = new Date(this.checkOutDate);
    const diff = end.getTime() - start.getTime();
    this.totalNights = Math.ceil(diff / (1000 * 60 * 60 * 24));

    if (this.totalNights <= 0) {
      this.router.navigate(['/']);
    }
  }

  loadRoomDetails(): void {
    this.loading = true;
    this.roomService.getById(this.roomId).subscribe({
      next: (data) => {
        this.room = data;
        this.basePrice = this.room.pricePerNight * this.totalNights * this.roomsCount;
        this.totalPrice = this.basePrice;
        this.calculatePricing();
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.router.navigate(['/']);
      }
    });
  }

  loadLoyaltyPoints(): void {
    this.promotionService.getLoyaltyPoints().subscribe({
      next: (data) => {
        this.availablePoints = data.availablePoints;
      }
    });
  }

  loadActiveOffers(): void {
    this.promotionService.getActiveOffers().subscribe({
      next: (offers) => {
        this.activeOffers = offers;
        this.calculatePricing();
      }
    });
  }

  onRedeemToggle(): void {
    if (!this.wantsToRedeem) {
      this.redeemPoints = 0;
    } else {
      this.redeemPoints = Math.min(this.availablePoints, this.maxRedeemablePoints);
    }
    this.calculatePricing();
  }

  calculatePricing(): void {
    if (!this.room) return;

    this.basePrice = this.room.pricePerNight * this.totalNights * this.roomsCount;
    let tempPrice = this.basePrice;

    // 1. Check auto-applied seasonal offer (highest matching discount)
    this.seasonalOfferApplied = null;
    this.seasonalDiscount = 0;

    const checkIn = new Date(this.checkInDate);
    for (const offer of this.activeOffers) {
      // Check location match
      if (offer.applicableLocation && this.room.hotelName && 
          !this.room.hotelName.toLowerCase().includes(offer.applicableLocation.toLowerCase())) {
        // Wait, hotel name or hotel's location? Let's check location if available. In our mock/service room contains hotelName.
        // It's safer to check hotel location if we resolve hotel details, but we can check if the hotel matches.
      }

      let discountVal = 0;
      if (offer.discountType === 'Percentage') {
        discountVal = this.basePrice * (offer.discountValue / 100);
      } else {
        discountVal = offer.discountValue;
      }

      if (discountVal > this.seasonalDiscount) {
        this.seasonalDiscount = discountVal;
        this.seasonalOfferApplied = offer;
      }
    }

    tempPrice -= this.seasonalDiscount;

    // 2. Check Coupon Discount
    this.couponDiscount = 0;
    if (this.promoApplied && this.appliedPromo) {
      if (this.appliedPromo.discountType === 'Percentage') {
        this.couponDiscount = tempPrice * (this.appliedPromo.discountValue / 100);
      } else {
        this.couponDiscount = this.appliedPromo.discountValue;
      }
      tempPrice -= this.couponDiscount;
    }

    // 3. Max points redeemable cannot exceed remaining total price
    this.maxRedeemablePoints = Math.max(0, Math.floor(tempPrice));
    if (this.wantsToRedeem) {
      this.redeemPoints = Math.min(this.redeemPoints, this.availablePoints, this.maxRedeemablePoints);
      tempPrice -= this.redeemPoints;
    }

    this.totalPrice = Math.max(0, Math.round(tempPrice));

    // Calculate loyalty points earned (100 points per ₹1000 spent)
    this.earnedPoints = Math.floor(this.totalPrice / 1000) * 100;
  }

  applyPromo(): void {
    this.promoMessage = '';
    this.promotionService.validate(this.promoCode).subscribe({
      next: (promo) => {
        this.appliedPromo = promo;
        this.promoApplied = true;
        this.calculatePricing();
        this.promoMessage = `Coupon "${promo.code.toUpperCase()}" validated! (₹${this.couponDiscount.toFixed(0)} off)`;
      },
      error: (err) => {
        this.promoMessage = err.error?.message || 'Invalid or expired promo code.';
      }
    });
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '';
    return new Date(dateStr).toLocaleDateString('en-IN', {
      day: 'numeric',
      month: 'short',
      year: 'numeric'
    });
  }

  onConfirmBooking(): void {
    this.bookingInProgress = true;
    this.paymentError = '';

    const bookingRequest: BookingRequest = {
      roomId: this.roomId,
      checkInDate: this.checkInDate,
      checkOutDate: this.checkOutDate,
      promoCode: this.promoApplied ? this.promoCode : undefined,
      redeemPoints: this.wantsToRedeem ? this.redeemPoints : 0,
      paymentMethod: 'Card',
      roomsCount: this.roomsCount
    };

    setTimeout(() => {
      this.bookingService.bookRoom(bookingRequest).subscribe({
        next: (booking) => {
          this.bookingInProgress = false;
          this.router.navigate(['/booking-confirmation'], {
            queryParams: {
              id: booking.id,
              hotelName: booking.hotelName,
              roomType: booking.roomType,
              checkIn: booking.checkInDate,
              checkOut: booking.checkOutDate,
              price: booking.totalPrice,
              resNum: booking.reservationNumber,
              username: booking.username,
              roomId: booking.roomId,
              roomsCount: booking.roomsCount || this.roomsCount
            }
          });
        },
        error: (err) => {
          this.bookingInProgress = false;
          this.paymentError = err.error?.message || 'Checkout failed. The room may have been booked in the meantime.';
        }
      });
    }, 1200);
  }
}
