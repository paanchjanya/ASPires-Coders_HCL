import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Promotion } from '../models/booking.model';

@Injectable({
  providedIn: 'root'
})
export class PromotionService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5106/api/promotions';
  private rewardsUrl = 'http://localhost:5106/api/rewards';
  private offersUrl = 'http://localhost:5106/api/offers';

  getAll(): Observable<Promotion[]> {
    return this.http.get<Promotion[]>(this.apiUrl);
  }

  validate(code: string): Observable<Promotion> {
    return this.http.get<Promotion>(`${this.apiUrl}/validate/${code}`);
  }

  create(promotion: any): Observable<Promotion> {
    return this.http.post<Promotion>(this.apiUrl, promotion);
  }

  update(id: number, promotion: any): Observable<Promotion> {
    return this.http.put<Promotion>(`${this.apiUrl}/${id}`, promotion);
  }

  delete(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

  // Loyalty Rewards
  getLoyaltyPoints(): Observable<any> {
    return this.http.get<any>(`${this.rewardsUrl}/my-points`);
  }

  // Seasonal Offers
  getActiveOffers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.offersUrl}/active`);
  }

  getAllOffers(): Observable<any[]> {
    return this.http.get<any[]>(this.offersUrl);
  }

  createOffer(offer: any): Observable<any> {
    return this.http.post<any>(this.offersUrl, offer);
  }

  updateOffer(id: number, offer: any): Observable<any> {
    return this.http.put<any>(`${this.offersUrl}/${id}`, offer);
  }

  deleteOffer(id: number): Observable<any> {
    return this.http.delete<any>(`${this.offersUrl}/${id}`);
  }
}
