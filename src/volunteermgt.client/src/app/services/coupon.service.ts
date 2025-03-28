import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CouponService {

  constructor(private http: HttpClient) { }
  private baseUrl = `/api`;

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
