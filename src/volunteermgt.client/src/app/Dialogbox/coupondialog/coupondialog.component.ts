import { Component, HostListener, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CouponService } from '../../services/coupon.service';
import { Service } from '../../Models/voluteerService.model';
import { MatSnackBar, MatSnackBarRef } from '@angular/material/snack-bar';
import { VolunteerService } from '../../services/volunteer.service';

@Component({
  selector: 'app-coupondialog',
  standalone: false,
  templateUrl: './coupondialog.component.html',
  styleUrl: './coupondialog.component.css'
})
export class CoupondialogComponent {
  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event): void {
    const targetElement = event.target as HTMLElement;
    if (!targetElement.closest('.service-container')) {
      this.serviceSuggestions = [];
    }
  }
  couponValue: number = 0; 
  services: Service[] = [];
  serviceQuery: string = '';
  serviceSuggestions: Service[] = [];
  selectedService: Service | null = null;
  assignedServices: number[] = [];
  selectedVolunteerIndex: number = -1;
  selectedServiceIndex: number = -1;
  createdDate: string = new Date().toISOString().split('T')[0];
  constructor(
    public dialogRef: MatDialogRef<CoupondialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { coupons: number },
    private couponService: CouponService,
    private snackBar: MatSnackBar,
    private volunteerService: VolunteerService,
  ) {
    this.couponValue = data.coupons || 0;
    this.volunteerService.getAllServices().subscribe((data) => {
      this.services = data;
    });
  }

  closeDialog() {
    this.dialogRef.close();
  }

  searchServices(): void {
    if (this.serviceQuery.trim() === '') {
      this.serviceSuggestions = [];
      return;
    }
    this.serviceSuggestions = this.services.filter(service =>
      service.serviceName.toLowerCase().includes(this.serviceQuery.toLowerCase()) &&
      !this.assignedServices.includes(service.id)
    );
  }

  selectService(service: Service): void {
    if (this.assignedServices.includes(service.id)) {
      this.showSnackbar("This service is already assigned to the selected volunteer.", "error");
      return;
    }
    this.serviceQuery = service.serviceName;
    this.serviceSuggestions = [];
    this.selectedService = service;
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
          serviceName: this.selectedService?.serviceName,
          createdDate: new Date(this.createdDate).toISOString()
        };
        this.couponService.addAdditionalCoupon(currentCoupon.id, additionalCouponData)
          .subscribe(response => {
            this.dialogRef.close(this.couponValue);
            window.location.reload();
          }, error => {
            console.error('Error updating coupon', error);
          });
      } else {
        console.error('No coupon found for the current date');
      }
    });
  }

  handleKeydown(event: KeyboardEvent, type: 'service') {
    if (type === 'service') {
      if (this.serviceSuggestions.length === 0) return;
      if (event.key === 'ArrowDown') {
        event.preventDefault();
        this.selectedServiceIndex = (this.selectedServiceIndex + 1) % this.serviceSuggestions.length;
        this.scrollIntoView('service');
      } else if (event.key === 'ArrowUp') {
        event.preventDefault();
        this.selectedServiceIndex = (this.selectedServiceIndex - 1 + this.serviceSuggestions.length) % this.serviceSuggestions.length;
        this.scrollIntoView('service');
      } else if (event.key === 'Enter' && this.selectedServiceIndex >= 0) {
        event.preventDefault();
        this.selectService(this.serviceSuggestions[this.selectedServiceIndex]);
      }
    }
  }

  scrollIntoView(type: 'volunteer' | 'service') {
    setTimeout(() => {
      let index = type === 'volunteer' ? this.selectedVolunteerIndex : this.selectedServiceIndex;
      let list = document.querySelectorAll(`.${type}-suggestions li`);
      if (list.length > 0 && index >= 0 && list[index]) {
        list[index].scrollIntoView({ behavior: 'smooth', block: 'nearest' });
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
