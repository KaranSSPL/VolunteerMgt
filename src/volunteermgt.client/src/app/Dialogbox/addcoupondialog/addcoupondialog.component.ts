import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CouponService } from '../../services/coupon.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-addcoupondialog',
  standalone: false,
  templateUrl: './addcoupondialog.component.html',
  styleUrl: './addcoupondialog.component.css'
})
export class AddcoupondialogComponent {
  couponValue!: number;
  currentDate: string;

  constructor(
    private couponService: CouponService,
    private snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<AddcoupondialogComponent>,
    private datePipe: DatePipe,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.currentDate = this.datePipe.transform(new Date(), 'dd-MM-yyyy') || '';
  }

  addCoupon(): void {
    if (this.couponValue <= 0) {
      this.snackBar.open('Please enter a valid coupon value!', 'Close', { duration: 3000, panelClass: 'snackbar-error' });
      return;
    }

    const couponData = {
      id: 0,
      date: new Date().toISOString(),
      couponValue: this.couponValue
    };

    this.couponService.addCoupon(couponData).subscribe(
      (response) => {
        this.snackBar.open('Coupon added successfully!', 'Close', { duration: 3000, panelClass: 'snackbar-success' });
        this.dialogRef.close(response);
        window.location.reload();
      },
      (error) => {
        this.snackBar.open('Error adding coupon. Try again.', 'Close', { duration: 3000, panelClass: 'snackbar-error' });
      }
    );
  }
}
