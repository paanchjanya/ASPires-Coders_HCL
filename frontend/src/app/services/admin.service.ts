import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Hotel } from '../models/hotel.model';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private http = inject(HttpClient);
  private adminApiUrl = 'http://localhost:5106/api/admin';
  private superAdminApiUrl = 'http://localhost:5106/api/superadmin';

  // Hotel Admin APIs
  getMyHotel(): Observable<Hotel> {
    return this.http.get<Hotel>(`${this.adminApiUrl}/my-hotel`);
  }

  getMyHotelBookings(): Observable<any[]> {
    return this.http.get<any[]>(`${this.adminApiUrl}/my-hotel/bookings`);
  }

  getMyHotelRooms(): Observable<any[]> {
    return this.http.get<any[]>(`${this.adminApiUrl}/my-hotel/rooms`);
  }

  // Super Admin APIs
  getDashboardStats(): Observable<any> {
    return this.http.get<any>(`${this.superAdminApiUrl}/dashboard`);
  }

  createHotelAdmin(adminData: any): Observable<any> {
    return this.http.post<any>(`${this.superAdminApiUrl}/create-admin`, adminData);
  }

  getAllUsers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.superAdminApiUrl}/users`);
  }

  getAllBookings(): Observable<any[]> {
    return this.http.get<any[]>(`${this.superAdminApiUrl}/bookings`);
  }

  getAllHotels(): Observable<any[]> {
    return this.http.get<any[]>(`${this.superAdminApiUrl}/hotels`);
  }
}
