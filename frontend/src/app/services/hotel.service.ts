import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Hotel, Location } from '../models/hotel.model';

@Injectable({
  providedIn: 'root'
})
export class HotelService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5106/api/hotels';
  private locationUrl = 'http://localhost:5106/api/locations';

  getAll(): Observable<Hotel[]> {
    return this.http.get<Hotel[]>(this.apiUrl);
  }

  getById(id: number): Observable<Hotel> {
    return this.http.get<Hotel>(`${this.apiUrl}/${id}`);
  }

  getByLocationId(locationId: number): Observable<Hotel[]> {
    return this.http.get<Hotel[]>(`${this.apiUrl}/location/${locationId}`);
  }

  search(params: any): Observable<Hotel[]> {
    let query = '';
    const keys = Object.keys(params);
    if (keys.length > 0) {
      query = '?' + keys
        .filter(k => params[k] !== undefined && params[k] !== null && params[k] !== '')
        .map(k => `${k}=${encodeURIComponent(params[k])}`)
        .join('&');
    }
    return this.http.get<Hotel[]>(`${this.apiUrl}/search${query}`);
  }

  create(hotel: any): Observable<Hotel> {
    return this.http.post<Hotel>(this.apiUrl, hotel);
  }

  update(id: number, hotel: any): Observable<Hotel> {
    return this.http.put<Hotel>(`${this.apiUrl}/${id}`, hotel);
  }

  delete(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

  // Location management APIs
  getLocations(): Observable<Location[]> {
    return this.http.get<Location[]>(this.locationUrl);
  }

  createLocation(location: any): Observable<Location> {
    return this.http.post<Location>(this.locationUrl, location);
  }

  updateLocation(id: number, location: any): Observable<Location> {
    return this.http.put<Location>(`${this.locationUrl}/${id}`, location);
  }

  deleteLocation(id: number): Observable<any> {
    return this.http.delete<any>(`${this.locationUrl}/${id}`);
  }
}
