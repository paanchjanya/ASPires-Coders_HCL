import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BookingRequest, BookingResponse } from '../models/booking.model';

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5106/api/bookings';

  bookRoom(bookingData: BookingRequest): Observable<BookingResponse> {
    return this.http.post<BookingResponse>(this.apiUrl, bookingData);
  }

  getMyBookings(): Observable<BookingResponse[]> {
    return this.http.get<BookingResponse[]>(`${this.apiUrl}/my`);
  }

  getAllBookings(): Observable<BookingResponse[]> {
    return this.http.get<BookingResponse[]>(`${this.apiUrl}/all`);
  }

  cancelBooking(id: number): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}/cancel`, {});
  }

  checkAvailability(roomId: number, checkIn: string, checkOut: string, roomsCount: number = 1): Observable<{ isAvailable: boolean, roomsCount?: number }> {
    const params = new HttpParams()
      .set('roomId', roomId.toString())
      .set('checkIn', checkIn)
      .set('checkOut', checkOut)
      .set('roomsCount', roomsCount.toString());
    return this.http.get<{ isAvailable: boolean, roomsCount?: number }>(`${this.apiUrl}/check-availability`, { params });
  }

  getEmailLog(bookingId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${bookingId}/email-log`);
  }
}
