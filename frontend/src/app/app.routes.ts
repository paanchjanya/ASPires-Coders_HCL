import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home';
import { LoginComponent } from './pages/login/login';
import { RegisterComponent } from './pages/register/register';
import { HotelListComponent } from './pages/hotel-list/hotel-list';
import { HotelDetailComponent } from './pages/hotel-detail/hotel-detail';
import { BookingComponent } from './pages/booking/booking';
import { BookingConfirmationComponent } from './pages/booking-confirmation/booking-confirmation';
import { MyBookingsComponent } from './pages/my-bookings/my-bookings';
import { AdminDashboardComponent } from './pages/admin/dashboard/dashboard';
import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'hotels', component: HotelListComponent },
  { path: 'hotels/:id', component: HotelDetailComponent },
  { path: 'booking', component: BookingComponent, canActivate: [authGuard] },
  { path: 'booking-confirmation', component: BookingConfirmationComponent, canActivate: [authGuard] },
  { path: 'my-bookings', component: MyBookingsComponent, canActivate: [authGuard] },
  { path: 'admin', component: AdminDashboardComponent, canActivate: [authGuard, adminGuard] },
  { path: '**', redirectTo: '' }
];
