import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Room } from '../models/room.model';

@Injectable({
  providedIn: 'root'
})
export class RoomService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5106/api/rooms';

  getAll(): Observable<Room[]> {
    return this.http.get<Room[]>(this.apiUrl);
  }

  getByHotelId(hotelId: number): Observable<Room[]> {
    return this.http.get<Room[]>(`${this.apiUrl}/hotel/${hotelId}`);
  }

  getById(id: number): Observable<Room> {
    return this.http.get<Room>(`${this.apiUrl}/${id}`);
  }

  create(room: any): Observable<Room> {
    return this.http.post<Room>(this.apiUrl, room);
  }

  update(id: number, room: any): Observable<Room> {
    return this.http.put<Room>(`${this.apiUrl}/${id}`, room);
  }

  updateStatus(roomId: number, status: string): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/status/${roomId}`, status, {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  getAvailabilitySchedule(hotelId: number, checkIn: string, checkOut: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/availability?hotelId=${hotelId}&checkIn=${checkIn}&checkOut=${checkOut}`);
  }

  updateAvailabilityByDate(roomId: number, date: string, status: string): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${roomId}/availability-by-date?date=${date}&status=${status}`, {});
  }
}
