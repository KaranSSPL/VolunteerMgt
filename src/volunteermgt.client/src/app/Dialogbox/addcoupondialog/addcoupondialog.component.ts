import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarRef } from '@angular/material/snack-bar';
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
      this.showSnackbar('Please enter a valid coupon value!', 'error');
      return;
    }
    const couponData = {
      id: 0,
      date: new Date().toISOString(),
      couponValue: this.couponValue
    };
    this.couponService.addCoupon(couponData).subscribe(
      (response) => {
        this.showSnackbar('Coupon added successfully!', 'success');
        this.dialogRef.close(response);
        window.location.reload();
      },
      (error) => {
        this.showSnackbar('Error adding coupon. Try again.', 'error');
      }
    );
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
