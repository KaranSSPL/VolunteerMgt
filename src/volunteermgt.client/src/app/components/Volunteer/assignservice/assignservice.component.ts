import { Component, HostListener, ViewEncapsulation } from '@angular/core';
import { Volunteer } from '../../../Models/volunteer.model';
import { VolunteerService } from '../../../services/volunteer.service';
import { Service } from '../../../Models/voluteerService.model';
import { AssignedVolunteer } from '../../../Models/assignVolunteer.model';
import { MatSnackBar, MatSnackBarRef } from '@angular/material/snack-bar';
import { AddcoupondialogComponent } from '../../../Dialogbox/addcoupondialog/addcoupondialog.component';
import { MatDialog } from '@angular/material/dialog';
import { CouponService } from '../../../services/coupon.service';
import { CoupondialogComponent } from '../../../Dialogbox/coupondialog/coupondialog.component';
import { map, of, switchMap } from 'rxjs';

@Component({
  selector: 'app-assignservice',
  standalone: false,
  templateUrl: './assignservice.component.html',
  styleUrl: './assignservice.component.css',
  encapsulation: ViewEncapsulation.None,
})
export class AssignserviceComponent {
  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event): void {
    const targetElement = event.target as HTMLElement;

    if (!targetElement.closest('.volunteer-container')) {
      this.searchSuggestions = [];
    }

    if (!targetElement.closest('.service-container')) {
      this.serviceSuggestions = [];
    }
  }
  timeSlot: string = '';
  serviceVolunteerCounts: AssignedVolunteer[] = [];
  volunteers: Volunteer[] = [];
  searchQuery: string = '';
  searchSuggestions: Volunteer[] = [];
  selectedVolunteer: Volunteer | null = null;
  assignedServices: number[] = []; 

  services: Service[] = [];
  serviceQuery: string = '';
  serviceSuggestions: Service[] = [];
  selectedService: Service | null = null;
  todayDate: Date = new Date();
  totalCoupons: number = 0;
  remainingCoupons: number = 0;
  additionalCoupons: number = 0;
  selectedVolunteerIndex: number = -1;
  selectedServiceIndex: number = -1;
  totalValue: number = 0;

  constructor(private volunteerService: VolunteerService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    private couponService: CouponService ) {
    this.volunteerService.getVolunteers().subscribe((data) => {
      this.volunteers = data;
    });

    this.setCurrentTime();
    this.fetchServiceVolunteerCounts();
    this.fetchCoupons();
    this.volunteerService.getAllServices().subscribe((data) => {
      this.services = data;
    });
  }

  fetchCoupons() {
    this.couponService.getCoupons().pipe(
      switchMap((coupons) => {
        const today = new Date().toISOString().split("T")[0];
        const todayCoupon = coupons.find(coupon => coupon.date.startsWith(today));
        this.totalCoupons = todayCoupon ? todayCoupon.couponValue : 0;
        return todayCoupon ? this.couponService.getAdditionalCoupons().pipe(
          map((additionalCoupons) => {
            const matchingCoupon = additionalCoupons.find(c => c.couponId === todayCoupon.id);
            return matchingCoupon ? matchingCoupon.totalValue : 0;
          })
        ) : of(0);
      })
    ).subscribe((additionalCouponValue) => {
      this.additionalCoupons = additionalCouponValue; 
      this.fetchServiceVolunteerCounts();
    });
  }

  fetchServiceVolunteerCounts() {
    this.volunteerService.getServiceVolunteerCounts().subscribe((data) => {
      this.serviceVolunteerCounts = data;
      this.calculateRemainingCoupons(); 
    });
  }

  calculateRemainingCoupons() {
    const totalVolunteers = this.getTotal('volunteerCount');
    const remaining = (this.totalCoupons || 0) - totalVolunteers - (this.additionalCoupons || 0);
    this.remainingCoupons = remaining > 0 ? remaining : 0;
  }

  searchVolunteers(): void {
    if (this.searchQuery.trim() === '') {
      this.searchSuggestions = [];
      return;
    }
    this.searchSuggestions = this.volunteers.filter(volunteer =>
      volunteer.name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
      volunteer.code.toLowerCase().includes(this.searchQuery.toLowerCase())
    );
  }

  selectSuggestion(volunteer: Volunteer): void {
    this.searchQuery = volunteer.name;
    this.searchSuggestions = [];
    this.selectedVolunteer = volunteer;
    this.selectedVolunteerIndex = -1;
    this.volunteerService.getServiceVolunteerById(volunteer.id).subscribe((assignedServices) => {
      this.assignedServices = assignedServices.map((service: { serviceId: any; }) => service.serviceId);
    });
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
    this.selectedServiceIndex = -1;
    this.selectedService = service;
  }

  private setCurrentTime(): void {
    const now = new Date();
    const hours = now.getHours().toString().padStart(2, '0');
    const minutes = now.getMinutes().toString().padStart(2, '0');
    this.timeSlot = `${hours}:${minutes}`;
  }

  onAssign() {
    if (!this.selectedVolunteer || !this.selectedService || !this.timeSlot) {
      this.showSnackbar("Please fill all fields before assigning.", "error");
      return;
    }
    if (this.assignedServices.includes(this.selectedService.id)) {
      this.showSnackbar("This service is already assigned to the selected volunteer.", "error");
      return;
    }
    const [hours, minutes] = this.timeSlot.split(":").map(Number);
    const currentDate = new Date();
    currentDate.setHours(hours, minutes, 0, 0);
    const istTime = new Date(currentDate.getTime() - currentDate.getTimezoneOffset() * 60000);
    const assignedData = {
      volunteerId: this.selectedVolunteer.id,
      serviceId: this.selectedService.id,
      timeSlot: istTime.toISOString()
    };
    this.volunteerService.assignService(assignedData).subscribe({
      next: (response) => {
        this.showSnackbar("Service assigned successfully!", "success");
        this.assignedServices.push(this.selectedService!.id);
      },
      error: (error) => {
        this.showSnackbar("Failed to assign service. Please try again.", "error");
      }
    });
    window.location.reload();
  }

  handleKeydown(event: KeyboardEvent, type: 'volunteer' | 'service') {
    if (type === 'volunteer') {
      if (this.searchSuggestions.length === 0) return;
      if (event.key === 'ArrowDown') {
        event.preventDefault();
        this.selectedVolunteerIndex = (this.selectedVolunteerIndex + 1) % this.searchSuggestions.length;
        this.scrollIntoView('volunteer');
      } else if (event.key === 'ArrowUp') {
        event.preventDefault();
        this.selectedVolunteerIndex = (this.selectedVolunteerIndex - 1 + this.searchSuggestions.length) % this.searchSuggestions.length;
        this.scrollIntoView('volunteer');
      } else if (event.key === 'Enter' && this.selectedVolunteerIndex >= 0) {
        event.preventDefault();
        this.selectSuggestion(this.searchSuggestions[this.selectedVolunteerIndex]);
      }
    } else if (type === 'service') {
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
 
  onAddCoupons() {
    const dialogRef = this.dialog.open(AddcoupondialogComponent, {
      width: '400px'
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        console.log(`Coupons Added: ${result}`);     
      }
    });
  }

  openCouponDialog() {
    const dialogRef = this.dialog.open(CoupondialogComponent, {
      width: '600px',
      data: { coupons: this.additionalCoupons }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result !== undefined) {
        this.additionalCoupons = result;
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

  getTotal(field: keyof AssignedVolunteer): number {
    return this.serviceVolunteerCounts.reduce((total, service) => total + Number(service[field]), 0);
  }
}
