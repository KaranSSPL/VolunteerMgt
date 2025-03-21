import { Component } from '@angular/core';
import { CouponService } from '../../../services/coupon.service';
import { Coupons } from '../../../Models/coupon.model';

@Component({
  selector: 'app-coupon',
  standalone: false,
  templateUrl: './coupon.component.html',
  styleUrl: './coupon.component.css'
})
export class CouponComponent {
  coupons: Coupons[] = [];

  constructor(private couponService: CouponService) { }

  ngOnInit(): void {
    this.fetchCoupon();
  }

  private fetchCoupon(): void {
    this.couponService.getAdditionalCoupons().subscribe(data => {
      this.coupons = data;
    });
  }
}
