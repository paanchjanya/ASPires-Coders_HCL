import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { AdminService } from '../../../services/admin.service';
import { HotelService } from '../../../services/hotel.service';
import { RoomService } from '../../../services/room.service';
import { BookingService } from '../../../services/booking.service';
import { PromotionService } from '../../../services/promotion.service';
import { Hotel, Location, Amenity } from '../../../models/hotel.model';
import { Room } from '../../../models/room.model';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})
export class AdminDashboardComponent implements OnInit {
  public authService = inject(AuthService);
  private adminService = inject(AdminService);
  private hotelService = inject(HotelService);
  private roomService = inject(RoomService);
  private bookingService = inject(BookingService);
  private promotionService = inject(PromotionService);
  private router = inject(Router);

  // General State
  activeTab = 'dashboard'; // dashboard, locations, hotels, admins, bookings, rooms, scheduler, coupon
  loading = false;
  submitting = false;

  // Static/Config Data
  locationsList: Location[] = [];
  hotelsList: any[] = [];
  allAmenities: Amenity[] = [
    { id: 1, name: 'Free High-Speed Wi-Fi', icon: 'bi-wifi' },
    { id: 2, name: 'Swimming Pool', icon: 'bi-water' },
    { id: 3, name: 'Fitness Center / Gym', icon: 'bi-activity' },
    { id: 4, name: 'Luxury Spa', icon: 'bi-heart-pulse' },
    { id: 5, name: 'Complimentary Breakfast', icon: 'bi-egg-fried' }
  ];

  // ----------------------------------------------------
  // SUPER ADMIN DATA
  // ----------------------------------------------------
  superStats: any = null;
  usersList: any[] = [];
  couponsList: any[] = [];
  systemBookings: any[] = [];

  // Super Admin CRUD Forms
  locationForm = { id: 0, name: '' };
  isEditingLocation = false;

  hotelForm = {
    id: 0,
    name: '',
    locationId: 0,
    description: '',
    imageUrl: '',
    rating: 4.5,
    selectedAmenityIds: [] as number[]
  };
  isEditingHotel = false;

  adminForm = {
    username: '',
    email: '',
    password: '',
    assignedHotelId: 0
  };

  couponForm = {
    id: 0,
    code: '',
    discountType: 'Percentage', // Flat, Percentage
    discountValue: 10,
    active: true,
    expiryDate: ''
  };
  isEditingCoupon = false;

  // ----------------------------------------------------
  // HOTEL ADMIN DATA
  // ----------------------------------------------------
  myHotel: Hotel | null = null;
  myRooms: Room[] = [];
  myBookings: any[] = [];

  // Hotel Admin Form State
  myHotelForm = {
    description: '',
    imageUrl: '',
    rating: 4.5,
    selectedAmenityIds: [] as number[]
  };

  roomForm = {
    id: 0,
    roomType: 'normal room',
    pricePerNight: 1500,
    capacity: 2,
    description: '',
    imageUrl: '',
    status: 'Available'
  };
  isEditingRoom = false;

  // Availability Scheduler State
  schedulerForm = {
    roomId: 0,
    date: '',
    status: 'Available'
  };

  ngOnInit(): void {
    if (!this.authService.isLoggedIn) {
      this.router.navigate(['/login']);
      return;
    }

    this.loadCommonData();

    if (this.authService.isSuperAdmin) {
      this.activeTab = 'dashboard';
      this.loadSuperAdminData();
    } else if (this.authService.isAdmin) {
      this.activeTab = 'dashboard';
      this.loadHotelAdminData();
    } else {
      this.router.navigate(['/']);
    }
  }

  loadCommonData(): void {
    this.hotelService.getLocations().subscribe(locs => this.locationsList = locs);
    this.hotelService.getAll().subscribe(hotels => this.hotelsList = hotels);
  }

  // ----------------------------------------------------
  // SUPER ADMIN OPERATIONS
  // ----------------------------------------------------
  loadSuperAdminData(): void {
    this.loading = true;
    this.adminService.getDashboardStats().subscribe({
      next: (stats) => {
        this.superStats = stats;
      }
    });

    this.adminService.getAllBookings().subscribe(b => this.systemBookings = b);
    this.adminService.getAllUsers().subscribe(u => this.usersList = u);
    this.promotionService.getAll().subscribe(p => this.couponsList = p);
    this.loading = false;
  }

  // Locations CRUD
  submitLocation(): void {
    this.submitting = true;
    if (this.isEditingLocation) {
      this.hotelService.updateLocation(this.locationForm.id, { name: this.locationForm.name }).subscribe({
        next: () => {
          this.resetLocationForm();
          this.loadCommonData();
        }
      });
    } else {
      this.hotelService.createLocation({ name: this.locationForm.name }).subscribe({
        next: () => {
          this.resetLocationForm();
          this.loadCommonData();
        }
      });
    }
  }

  editLocation(loc: Location): void {
    this.locationForm = { id: loc.id, name: loc.name };
    this.isEditingLocation = true;
  }

  deleteLocation(id: number): void {
    if (!confirm('Are you sure you want to delete this location? This will delete all hotels in this location!')) return;
    this.hotelService.deleteLocation(id).subscribe(() => {
      this.loadCommonData();
    });
  }

  resetLocationForm(): void {
    this.locationForm = { id: 0, name: '' };
    this.isEditingLocation = false;
    this.submitting = false;
  }

  // Hotels CRUD
  toggleHotelAmenity(amenityId: number): void {
    const idx = this.hotelForm.selectedAmenityIds.indexOf(amenityId);
    if (idx > -1) {
      this.hotelForm.selectedAmenityIds.splice(idx, 1);
    } else {
      this.hotelForm.selectedAmenityIds.push(amenityId);
    }
  }

  submitHotel(): void {
    this.submitting = true;
    const payload = {
      name: this.hotelForm.name,
      locationId: Number(this.hotelForm.locationId),
      description: this.hotelForm.description,
      imageUrl: this.hotelForm.imageUrl,
      rating: this.hotelForm.rating,
      amenityIds: this.hotelForm.selectedAmenityIds
    };

    if (this.isEditingHotel) {
      this.hotelService.update(this.hotelForm.id, payload).subscribe({
        next: () => {
          this.resetHotelForm();
          this.loadCommonData();
          this.loadSuperAdminData();
        }
      });
    } else {
      this.hotelService.create(payload).subscribe({
        next: () => {
          this.resetHotelForm();
          this.loadCommonData();
          this.loadSuperAdminData();
        }
      });
    }
  }

  editHotel(hotel: any): void {
    this.hotelForm = {
      id: hotel.id,
      name: hotel.name,
      locationId: hotel.locationId,
      description: hotel.description,
      imageUrl: hotel.imageUrl,
      rating: hotel.rating,
      selectedAmenityIds: hotel.hotelAmenities?.map((ha: any) => ha.amenityId) || []
    };
    this.isEditingHotel = true;
  }

  deleteHotel(id: number): void {
    if (!confirm('Are you sure you want to delete this hotel?')) return;
    this.hotelService.delete(id).subscribe(() => {
      this.loadCommonData();
      this.loadSuperAdminData();
    });
  }

  resetHotelForm(): void {
    this.hotelForm = {
      id: 0,
      name: '',
      locationId: this.locationsList[0]?.id || 0,
      description: '',
      imageUrl: '',
      rating: 4.5,
      selectedAmenityIds: []
    };
    this.isEditingHotel = false;
    this.submitting = false;
  }

  // Create Hotel Admin
  submitAdmin(): void {
    this.submitting = true;
    this.adminForm.assignedHotelId = Number(this.adminForm.assignedHotelId);
    this.adminService.createHotelAdmin(this.adminForm).subscribe({
      next: () => {
        alert('Hotel Admin account created and assigned successfully.');
        this.adminForm = { username: '', email: '', password: '', assignedHotelId: 0 };
        this.loadSuperAdminData();
        this.submitting = false;
      },
      error: (err) => {
        alert(err.error?.message || 'Failed to create admin.');
        this.submitting = false;
      }
    });
  }

  // Coupon CRUD
  submitCoupon(): void {
    this.submitting = true;
    const payload = {
      code: this.couponForm.code.toUpperCase(),
      discountType: this.couponForm.discountType,
      discountValue: this.couponForm.discountValue,
      active: this.couponForm.active,
      expiryDate: new Date(this.couponForm.expiryDate)
    };

    if (this.isEditingCoupon) {
      this.promotionService.update(this.couponForm.id, payload).subscribe({
        next: () => {
          this.resetCouponForm();
          this.loadSuperAdminData();
        }
      });
    } else {
      this.promotionService.create(payload).subscribe({
        next: () => {
          this.resetCouponForm();
          this.loadSuperAdminData();
        }
      });
    }
  }

  editCoupon(coupon: any): void {
    const formattedDate = coupon.expiryDate ? new Date(coupon.expiryDate).toISOString().split('T')[0] : '';
    this.couponForm = {
      id: coupon.id,
      code: coupon.code,
      discountType: coupon.discountType,
      discountValue: coupon.discountValue,
      active: coupon.active,
      expiryDate: formattedDate
    };
    this.isEditingCoupon = true;
  }

  deleteCoupon(id: number): void {
    if (!confirm('Are you sure you want to delete this coupon?')) return;
    this.promotionService.delete(id).subscribe(() => {
      this.loadSuperAdminData();
    });
  }

  resetCouponForm(): void {
    this.couponForm = {
      id: 0,
      code: '',
      discountType: 'Percentage',
      discountValue: 10,
      active: true,
      expiryDate: ''
    };
    this.isEditingCoupon = false;
    this.submitting = false;
  }

  // Cancel Bookings
  cancelBooking(id: number): void {
    if (!confirm('Cancel this booking reservation?')) return;
    this.bookingService.cancelBooking(id).subscribe({
      next: () => {
        if (this.authService.isSuperAdmin) {
          this.loadSuperAdminData();
        } else {
          this.loadHotelAdminData();
        }
      },
      error: (err) => {
        alert(err.error?.message || 'Failed to cancel staying.');
      }
    });
  }

  // ----------------------------------------------------
  // HOTEL ADMIN OPERATIONS
  // ----------------------------------------------------
  loadHotelAdminData(): void {
    this.loading = true;
    this.adminService.getMyHotel().subscribe({
      next: (hotel) => {
        this.myHotel = hotel;
        this.myHotelForm = {
          description: hotel.description,
          imageUrl: hotel.imageUrl,
          rating: hotel.rating,
          selectedAmenityIds: hotel.hotelAmenities?.map((ha: any) => ha.amenityId) || []
        };

        // Fetch bookings for this hotel
        this.adminService.getMyHotelBookings().subscribe(b => this.myBookings = b);
        // Fetch rooms for this hotel
        this.adminService.getMyHotelRooms().subscribe(r => {
          this.myRooms = r;
          if (r.length > 0) {
            this.schedulerForm.roomId = r[0].id;
          }
        });
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  // Update Hotel Info
  toggleMyHotelAmenity(amenityId: number): void {
    const idx = this.myHotelForm.selectedAmenityIds.indexOf(amenityId);
    if (idx > -1) {
      this.myHotelForm.selectedAmenityIds.splice(idx, 1);
    } else {
      this.myHotelForm.selectedAmenityIds.push(amenityId);
    }
  }

  submitMyHotelDetails(): void {
    if (!this.myHotel) return;
    this.submitting = true;
    const payload = {
      name: this.myHotel.name,
      locationId: this.myHotel.locationId,
      description: this.myHotelForm.description,
      imageUrl: this.myHotelForm.imageUrl,
      rating: this.myHotelForm.rating,
      amenityIds: this.myHotelForm.selectedAmenityIds
    };

    this.hotelService.update(this.myHotel.id, payload).subscribe({
      next: () => {
        alert('Assigned Hotel details and facilities updated successfully.');
        this.submitting = false;
        this.loadHotelAdminData();
      },
      error: () => {
        this.submitting = false;
      }
    });
  }

  // Rooms Management (No Deletion Allowed)
  submitRoom(): void {
    if (!this.myHotel) return;
    this.submitting = true;
    const payload = {
      hotelId: this.myHotel.id,
      roomType: this.roomForm.roomType,
      pricePerNight: this.roomForm.pricePerNight,
      capacity: this.roomForm.capacity,
      description: this.roomForm.description,
      imageUrl: this.roomForm.imageUrl,
      status: this.roomForm.status
    };

    if (this.isEditingRoom) {
      this.roomService.update(this.roomForm.id, payload).subscribe({
        next: () => {
          this.resetRoomForm();
          this.loadHotelAdminData();
        }
      });
    } else {
      this.roomService.create(payload).subscribe({
        next: () => {
          this.resetRoomForm();
          this.loadHotelAdminData();
        }
      });
    }
  }

  editRoom(room: Room): void {
    this.roomForm = {
      id: room.id,
      roomType: room.roomType,
      pricePerNight: room.pricePerNight,
      capacity: room.capacity,
      description: room.description,
      imageUrl: room.imageUrl,
      status: room.status
    };
    this.isEditingRoom = true;
  }

  resetRoomForm(): void {
    this.roomForm = {
      id: 0,
      roomType: 'normal room',
      pricePerNight: 1500,
      capacity: 2,
      description: '',
      imageUrl: '',
      status: 'Available'
    };
    this.isEditingRoom = false;
    this.submitting = false;
  }

  // Update Status directly on grid
  toggleRoomStatus(room: Room, nextStatus: string): void {
    this.roomService.updateStatus(room.id, nextStatus).subscribe({
      next: () => {
        this.loadHotelAdminData();
      }
    });
  }

  // Availability Scheduler Update
  submitScheduler(): void {
    if (this.schedulerForm.roomId <= 0 || !this.schedulerForm.date) {
      alert('Please select a valid Room and Date.');
      return;
    }
    this.submitting = true;
    this.roomService.updateAvailabilityByDate(
      this.schedulerForm.roomId,
      this.schedulerForm.date,
      this.schedulerForm.status
    ).subscribe({
      next: () => {
        alert(`Room availability schedule successfully updated to ${this.schedulerForm.status} for date: ${this.schedulerForm.date}`);
        this.submitting = false;
      },
      error: (err) => {
        alert(err.error?.message || 'Failed to update schedule.');
        this.submitting = false;
      }
    });
  }

  // Helpers
  formatDate(dateStr: string): string {
    if (!dateStr) return '';
    return new Date(dateStr).toLocaleDateString('en-IN', {
      day: 'numeric',
      month: 'short',
      year: 'numeric'
    });
  }
}
