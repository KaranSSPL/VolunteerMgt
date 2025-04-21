import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CouponService } from '../../services/coupon.service';
import { MatSnackBar, MatSnackBarRef } from '@angular/material/snack-bar';

@Component({
  selector: 'app-coupondialog',
  standalone: false,
  templateUrl: './coupondialog.component.html',
  styleUrl: './coupondialog.component.css'
})
export class CoupondialogComponent {
 
  couponValue: number = 0; 
  createdDate: string = new Date().toISOString().split('T')[0];
  constructor(
    public dialogRef: MatDialogRef<CoupondialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { coupons: number },
    private couponService: CouponService,
    private snackBar: MatSnackBar,
  ) {
    this.couponValue = data.coupons || 0;
  }

  closeDialog() {
    this.dialogRef.close();
  }

  saveCoupons() {
    this.couponService.getCoupons().subscribe((coupons) => {
      const currentDate = new Date().toISOString().split('T')[0];
      const currentCoupon = coupons.find(coupon => {
        const couponDate = new Date(coupon.date);
        const formattedCouponDate = couponDate.getFullYear() + '-' +
          ('0' + (couponDate.getMonth() + 1)).slice(-2) + '-' +
          ('0' + couponDate.getDate()).slice(-2);
        return formattedCouponDate === currentDate;
      });
      if (currentCoupon) {
        const additionalCouponData = {
          id: 0,
          additionalCouponValue: this.couponValue,
          couponId: currentCoupon.id,
          createdDate: new Date(this.createdDate).toISOString()
        };
        this.couponService.addAdditionalCoupon(currentCoupon.id, additionalCouponData)
          .subscribe(response => {
            this.dialogRef.close(this.couponValue);
            window.location.reload();
            this.showSnackbar("Additional Coupon Added Successfully", "success");
          }, error => {
            console.error('Error updating coupon', error);
          });
      } else {
        this.showSnackbar("First Add Coupon then only you're able to add additional coupon", "error");
      }
    });
  }

  showSnackbar(message: string, type: "success" | "error") {
    const snackbarRef: MatSnackBarRef<any> = this.snackBar.open(message, "close", {
      duration: 3000,
      verticalPosition: "top",
      horizontalPosition: "center",
    });
    snackbarRef.afterOpened().subscribe(() => {
      const snackbarElement = document.querySelector('.mat-mdc-snack-bar-container');
      if (snackbarElement) {
        snackbarElement.classList.add(type === "success" ? "snackbar-success" : "snackbar-error");
      }
    });
  }
}
