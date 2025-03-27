import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CouponService {

  constructor(private http: HttpClient) { }
  private baseUrl = `${environment.apiUrl}/api`;

  addCoupon(coupon: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/coupons/add`, coupon);
  }

  getCoupons(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/coupons`);
  }

  addAdditionalCoupon(couponId: number, additionalCoupon: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/coupons/${couponId}/additional`, additionalCoupon);
  }

  getAdditionalCoupons(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/coupons/additional`);
  }
}
